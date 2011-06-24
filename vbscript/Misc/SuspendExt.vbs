Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, tag, computer, unixDate, dtGMT
dim oResp, oNode, oWMI, oRes, oEntry, IDs, pos, cfgfile, oConf, oFso

tag = WScript.arguments.named.item("tag")

cfgfile = left(Wscript.ScriptFullName,InStrRev(Wscript.ScriptFullName, "\")) + "config.xml"
Set oConf = CreateObject("Microsoft.XMLDOM")
oConf.async = False
Set oFso = CreateObject("Scripting.FileSystemObject")
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

'Connecting to WMI, "." is for local computer, specify computer name for another computer
computer = "."
Set oWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + computer + "\root\cimv2")

'Finds current timezone to obtain GMT date
dtGMT = now
Set oRes = oWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
For each oEntry in oRes
  dtGMT = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), dtGMT)
next

dtGMT = GMTDate()
unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Request a token to use in following calls
url = "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey
wscript.echo "Requesting token"
wscript.echo "GET: " + url
objHTTP.open "GET", url, False
objHTTP.send
resp = objHTTP.responseText
wscript.echo "Result: " + resp
pos = InStr(resp, ":") + 2
token = mid(resp, pos, len(resp) - pos - 1)

IDs = ""

if tag > "" then
  Set oResp = GetExtMonByTag(tag)
else
  Set oResp = GetAllExtMon()
end if
for each oNode in oResp.documentElement.childnodes
  if oNode.Attributes.getNamedItem("isSuspended").text = "0" then
    IDs = AddList(IDs, oNode.Attributes.getNamedItem("id").text)
  end if
next
wscript.echo IDs
if IDs > "" then SuspendExt IDs

Function AddList(list, add)
  if list > "" then 
    AddList = list + "," + add
  else
    AddList = add
  end if
End Function

Function GetAllExtMon
  url = "http://www.monitis.com/api?action=tests&apikey=" + apiKey + "&output=xml"
  objHTTP.open "GET", url, False
  wscript.echo "Requesting monitors list"
  wscript.echo "GET: " + url
  objHTTP.send

  Set GetAllExtMon = CreateObject("Microsoft.XMLDOM")
  GetAllExtMon.async = False
  GetAllExtMon.LoadXML(objHTTP.responseText)
End Function

Function GetExtMonByTag(tag)
  url = "http://www.monitis.com/api?action=tagtests&apikey=" + apiKey + "&output=xml&tag=" + tag
  objHTTP.open "GET", url, False
  wscript.echo "Requesting monitors list"
  wscript.echo "GET: " + url
  objHTTP.send

  Set GetExtMonByTag = CreateObject("Microsoft.XMLDOM")
  GetExtMonByTag.async = False
  GetExtMonByTag.LoadXML(objHTTP.responseText)
End Function

Sub SuspendExt(IDs)
  url = "http://www.monitis.com/api"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=suspendExternalMonitor&monitorIds=" + IDs
  wscript.echo "Suspend monitors " + IDs
  wscript.echo "POST: " + url
  wscript.echo "Data: " + postData
  objHTTP.send postData
  resp = objHTTP.responseText
  wscript.echo "Result: " + resp
End sub

Function FmtDate(dt)
  FmtDate = cstr(Datepart("yyyy", dt)) + "-" + right("0" + cstr(Datepart("m", dt)),2) + "-" +  right("0" + cstr(Datepart ("d", dt)),2) + " " + right("0" + cstr(Datepart("h", dt)),2) + ":" + right("0" + cstr(Datepart("n", dt)),2) + ":" + right("0" + cstr(Datepart("S", dt)),2)
end function

Function GMTDate()
  GMTDate = now
  Set oRes = oWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
  For each oEntry in oRes
    GMTDate = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), GMTDate)
  next
End function
