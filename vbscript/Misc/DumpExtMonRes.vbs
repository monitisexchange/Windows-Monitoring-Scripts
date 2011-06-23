Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, tag, computer, monitorName
dim oResp, oNode, oWMI, oRes, oEntry, IDs, pos, name, dtStart, dtEnd, locations, oFso, aLocations, oLocations, aLocIDs, outdir, timezone

Set oFso = CreateObject("Scripting.FileSystemObject")
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

apiKey = WScript.arguments.named.item("apiKey")
secretKey = WScript.arguments.named.item("secretKey")
tag = WScript.arguments.named.item("tag")
name = WScript.arguments.named.item("name")
locations = WScript.arguments.named.item("locations")
dtStart = WScript.arguments.named.item("dtStart")
dtEnd = WScript.arguments.named.item("dtEnd")
outdir = WScript.arguments.named.item("outdir")

if apiKey = "" then
  wscript.echo "apiKey parameter not specified"
  ShowUsage
end if

if secretKey = "" then
  wscript.echo "secretKey parameter not specified"
  ShowUsage
end if

if dtStart = "" then
  wscript.echo "dtStart parameter not specified"
  ShowUsage
else
  dtStart = DateSerial(CInt(left(dtStart,4)), CInt(mid(dtStart,5,2)), CInt(mid(dtStart,7,2)))
end if

if outdir > "" then
  if right(outdir,1) <> "\" then outdir = outdir + "\"
  if not oFso.FolderExists(outdir) then
    wscript.echo "Output folder " + outdir + " does not exists"
	ShowUsage
  end if
end if

if locations > "" then
  'Gets the current location list to decode locations to corresponding IDs
  url = "http://www.monitis.com/api?action=locations&apikey=" + apiKey + "&output=xml"
  wscript.echo "Requesting location IDs"
  objHTTP.open "GET", url, False
  objHTTP.send
  Set oLocations = CreateObject("Microsoft.XMLDOM")
  oLocations.async = False
  oLocations.LoadXML(objHTTP.responseText)

  'Populates the locations arrays
  aLocations = split(locations, ",")
  redim aLocIDs(UBound(aLocations))
  for pos = 0 to UBound(aLocations)
	aLocIDs(pos) = GetLocID(aLocations(pos))
  next
end if 

if dtEnd = "" then
  dtEnd = DateSerial(year(now), month(now), day(now))
else
  dtEnd = DateSerial(CInt(left(dtEnd,4)), CInt(mid(dtEnd,5,2)), CInt(mid(dtEnd,7,2)))
end if

wscript.echo "Dumping data from", dtStart, "to", dtEnd

'Connecting to WMI, "." is for local computer, specify computer name for another computer
computer = "."
Set oWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + computer + "\root\cimv2")

'Finds current timezone
Set oRes = oWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
For each oEntry in oRes
  timezone = CInt(right(oEntry.LocalDateTime, 4))
next

'Request a token to use in following calls
url = "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey
wscript.echo "Requesting token"
objHTTP.open "GET", url, False
objHTTP.send
resp = objHTTP.responseText
pos = InStr(resp, ":") + 2
token = mid(resp, pos, len(resp) - pos - 1)

IDs = ""

'Get monitor list
if tag > "" then
  Set oResp = GetExtMonByTag(tag)
else
  Set oResp = GetAllExtMon()
end if

'Process each monitor
for each oNode in oResp.documentElement.childnodes
  monitorName = oNode.text
  if name > "" then 'if a name is specified dumps only the corresponding monitor
    if monitorName = name then
      DumpMonRes oNode.Attributes.getNamedItem("id").text
    end if
  else
    DumpMonRes oNode.Attributes.getNamedItem("id").text
  end if
next

Function GetAllExtMon
  url = "http://www.monitis.com/api?action=tests&apikey=" + apiKey + "&output=xml"
  objHTTP.open "GET", url, False
  wscript.echo "Requesting monitors list"
  objHTTP.send

  Set GetAllExtMon = CreateObject("Microsoft.XMLDOM")
  GetAllExtMon.async = False
  GetAllExtMon.LoadXML(objHTTP.responseText)
End Function

Function GetExtMonByTag(tag)
  url = "http://www.monitis.com/api?action=tagtests&apikey=" + apiKey + "&output=xml&tag=" + tag
  objHTTP.open "GET", url, False
  wscript.echo "Requesting monitors list for tag", tag
  objHTTP.send

  Set GetExtMonByTag = CreateObject("Microsoft.XMLDOM")
  GetExtMonByTag.async = False
  GetExtMonByTag.LoadXML(objHTTP.responseText)
End Function

Sub DumpMonRes(ID)
  dim oInfo, oNode, pos
  if locations = "" then 'if no locations list is specified gets the list from the monitor itself
    url = "http://www.monitis.com/api?action=testinfo&apikey=" + apiKey + "&testId=" + ID + "&output=xml"
	objHTTP.open "GET", url, False
    objHTTP.send

    Set oInfo = CreateObject("Microsoft.XMLDOM")
    oInfo.async = False
    oInfo.LoadXML(objHTTP.responseText)
	Set oInfo = oInfo.DocumentElement.SelectSingleNode("locations")
	for each oNode in oInfo.childnodes
	  WriteMonRes ID, oNode.selectSingleNode("id").text, oNode.selectSingleNode("name").text
	next
  else 'if a locations list is specified gets results for those locations
    for pos = 0 to UBound(aLocations)
      WriteMonRes ID, aLocIDs(pos), aLocations(pos)
	next
  end if
End sub

Sub WriteMonRes(ID, location, locname)
  dim dt, oRes, filename, oCell, oNode, row, oFile, oLocRes
  
  'Set the output file name
  filename = outdir + ID & "-" & locname & "-" & year(dtStart) & right("0" & month(dtStart),2) & right("0" & day(dtStart),2) & "-" & year(dtEnd) & right("0" & month(dtEnd),2) & right("0" & day(dtEnd),2) & ".csv"
  Set oFile = oFso.CreateTextFile(filename, True)
  
  dt = dtStart
  do while dt <= dtEnd
    wscript.echo "Dumping results of", monitorName, "(", ID, ") from", locname, "on", dt
	
	'Requesting the monitor results
    url = "http://www.monitis.com/api?action=testresult&apikey=" + apiKey + "&testId=" + ID + "&day=" & day(dt) & "&month=" & month(dt) & "&year=" & year(dt) & "&locationIds=" & location & "&output=xml&timezone=" & timezone
	objHTTP.open "GET", url, False
    objHTTP.send

    Set oRes = CreateObject("Microsoft.XMLDOM")
    oRes.async = False
    oRes.LoadXML(objHTTP.responseText)
	if not oRes.firstchild.firstchild is nothing then
      for each oNode in oRes.firstchild.firstchild.childnodes
        if oNode.nodename = "row" then
          row = ""
          for each oCell in oNode.ChildNodes
            if row = "" then 
              row = """" & year(dt) & "-" & right("0" & month(dt),2) & "-" & right("0" & day(dt),2) & " " & oCell.text & """"
            else
              row = row + ";""" + oCell.text + """"
            end if
          next
	      oFile.writeline row
	    end if
	  next
	end if
	dt = DateAdd("d", 1, dt)
  loop
  oFile.close
  Set oFile = oFso.GetFile(filename)
  if oFile.Size = 0 then oFso.DeleteFile(filename)
End Sub

Function GetLocID(location)
  dim oNode
  for each oNode in oLocations.firstchild.childnodes
    if oNode.selectsinglenode("name").text = location then
	  GetLocID = oNode.selectsinglenode("id").text
	end if
  next
End Function

Function GMTDate()
  GMTDate = now
  Set oRes = oWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
  For each oEntry in oRes
    GMTDate = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), GMTDate)
  next
End function

Sub ShowUsage
  wscript.echo "DumpExtMonRes.vbs parameters:"
  wscript.echo " /apiKey:<apiKey> Your API key"
  wscript.echo " /secretKey:<secretKey> Your Secret Key"
  wscript.echo " [/tag:<tag>] the tag of the monitors to dump results for"
  wscript.echo " [/name:<name>] the name of the monitor to dump results for"
  wscript.echo " /dtStart:<YYYYMMMDD> the starting date for the period to export"
  wscript.echo " [/dtEnd:<YYYYMMMDD>] the ending date for the period to export"
  wscript.echo " [/locations:<location [,<location>]>] a comma separated list of locations"
  wscript.echo " [/outdir:<directryname>] the directory where result files will be stored, defaults to current folder"
  wscript.quit
End Sub