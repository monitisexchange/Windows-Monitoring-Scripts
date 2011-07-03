Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, tag, computer, monitorName, cfgfile, oConf
dim oResp, oNode, oWMI, oRes, oEntry, IDs, pos, name, dtStart, dtEnd, locations, oFso, aLocations, oLocations, aLocIDs, outdir, timezone

Set oFso = CreateObject("Scripting.FileSystemObject")
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

tag = WScript.arguments.named.item("tag")
name = WScript.arguments.named.item("name")
dtStart = WScript.arguments.named.item("dtStart")
dtEnd = WScript.arguments.named.item("dtEnd")
outdir = WScript.arguments.named.item("outdir")

cfgfile = left(Wscript.ScriptFullName,InStrRev(Wscript.ScriptFullName, "\")) + "config.xml"
Set oConf = CreateObject("Microsoft.XMLDOM")
oConf.async = False
if oFso.FileExists(cfgfile) then
  oConf.Load(cfgfile)
else
  oConf.LoadXML("<monitis><APIKey/><SecretKey/></monitis>")
end if

apiKey = oConf.documentElement.selectSingleNode("APIKey").text
secretKey = oConf.documentElement.selectSingleNode("SecretKey").text

if apiKey = "" then
  wscript.echo "APIKey not configured"
  wscript.quit
end if

if secretKey = "" then
  wscript.echo "SecretKey not configured"
  wscript.quit
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

url = "http://www.monitis.com/customMonitorApi?apikey=" + apiKey + "&action=getMonitors&&output=xml"
if tag > "" then url = url + "&tag=" + tag
objHTTP.open "GET", url, False
wscript.echo "Requesting custom monitors list"
objHTTP.send

Set oResp = CreateObject("Microsoft.XMLDOM")
oResp.async = False
oResp.LoadXML(objHTTP.responseText)

'Process each monitor
for each oNode in oResp.documentElement.childnodes
  monitorName = oNode.SelectSingleNode("name").text
  if name > "" then 'if a name is specified dumps only the corresponding monitor
    if monitorName = name then
      WriteMonRes oNode.SelectSingleNode("id").text
    end if
  else
    WriteMonRes oNode.SelectSingleNode("id").text
  end if
next

Sub WriteMonRes(ID)
  dim dt, oRes, filename, col, oNode, row, oFile, header, aCols
  
  'Set the output file name
  filename = outdir + monitorName + "-" + ID & "-" & year(dtStart) & right("0" & month(dtStart),2) & right("0" & day(dtStart),2) & "-" & year(dtEnd) & right("0" & month(dtEnd),2) & right("0" & day(dtEnd),2) & ".csv"
  Set oFile = oFso.CreateTextFile(filename, True)
  
  url = "http://www.monitis.com/customMonitorApi?action=getMonitorInfo&apikey=" + apiKey + "&monitorId=" + ID + "&output=xml"
  objHTTP.open "GET", url, False
  objHTTP.send

  Set oRes = CreateObject("Microsoft.XMLDOM")
  oRes.async = False
  oRes.LoadXML(objHTTP.responseText)
  
  header = "checktime"
  Set oRes = oRes.documentelement.selectsinglenode("resultParams")
  for each oNode in oRes.childnodes
    header = header + ";" + oNode.selectsingleNode("name").text
  next
  aCols = split(header, ";")
  
  dt = dtStart
  do while dt <= dtEnd
    wscript.echo "Dumping results of", monitorName, "(", ID, ") on", dt
	
	'Requesting the monitor results
    url = "http://www.monitis.com/customMonitorApi?action=getMonitorResults&apikey=" + apiKey + "&monitorId=" + ID + "&day=" & day(dt) & "&month=" & month(dt) & "&year=" & year(dt) & "&output=xml&timezone=" & timezone
	objHTTP.open "GET", url, False
    objHTTP.send

    Set oRes = CreateObject("Microsoft.XMLDOM")
    oRes.async = False
    oRes.LoadXML(objHTTP.responseText)
	if not oRes.firstchild is nothing then
      for each oNode in oRes.firstchild.childnodes
        if oNode.nodename = "result" then
          row = """" & year(dt) & "-" & right("0" & month(dt),2) & "-" & right("0" & day(dt),2) + " " + right(oNode.SelectSingleNode("checkTime").text, 5) + """"
		  for col = 1 to UBound(aCols)
            row = row + ";""" + oNode.SelectSingleNode(aCols(col)).text + """"
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

Sub ShowUsage
  wscript.echo "DumpCustMonRes.vbs parameters:"
  wscript.echo " [/tag:<tag>] the tag of the monitors to dump results for"
  wscript.echo " [/name:<name>] the name of the monitor to dump results for"
  wscript.echo " /dtStart:<YYYYMMMDD> the starting date for the period to export"
  wscript.echo " [/dtEnd:<YYYYMMMDD>] the ending date for the period to export"
  wscript.echo " [/outdir:<directryname>] the directory where result files will be stored, defaults to current folder"
  wscript.quit
End Sub