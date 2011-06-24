Option Explicit
dim apiKey, secretKey, objHTTP, url, resp, token, computer, timezone, agentname, cfgfile, oConf
dim oAgents, oAgent, oWMI, oRes, oEntry, pos, oAgentInfo, oInfo, dtStart, dtEnd, outdir, oFso, agent

agent = WScript.arguments.named.item("agent")
dtStart = WScript.arguments.named.item("dtStart")
dtEnd = WScript.arguments.named.item("dtEnd")
outdir = WScript.arguments.named.item("outdir")

Set oFso = CreateObject("Scripting.FileSystemObject")

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

if dtEnd = "" then
  dtEnd = DateSerial(year(now), month(now), day(now))
else
  dtEnd = DateSerial(CInt(left(dtEnd,4)), CInt(mid(dtEnd,5,2)), CInt(mid(dtEnd,7,2)))
end if

if outdir > "" then
  if right(outdir,1) <> "\" then outdir = outdir + "\"
  if not oFso.FolderExists(outdir) then
    wscript.echo "Output folder " + outdir + " does not exists"
	ShowUsage
  end if
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

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Request a token to use in following calls
url = "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey
wscript.echo "Requesting token"
objHTTP.open "GET", url, False
objHTTP.send
resp = objHTTP.responseText
wscript.echo "Result: " + resp
pos = InStr(resp, ":") + 2
token = mid(resp, pos, len(resp) - pos - 1)

url = "http://www.monitis.com/api?action=agents&apikey=" + apiKey + "&output=xml"
objHTTP.open "GET", url, False
wscript.echo "Requesting agents list"
objHTTP.send

Set oAgents = CreateObject("Microsoft.XMLDOM")
oAgents.async = False
oAgents.LoadXML(objHTTP.responseText)

for each oAgent in oAgents.documentElement.childnodes
  agentname = mid(oAgent.selectSingleNode("key").text, 1, instr(oAgent.selectSingleNode("key").text, "@")-1)

  if agent = "" or (agent > "" and agentname = agent) then
    'Get the list of monitors for the agent  
    url = "http://www.monitis.com/api?action=agentInfo&apikey=" + apiKey + "&output=xml&agentId=" + oAgent.selectSingleNode("id").text + "&loadTests=true"
    objHTTP.open "GET", url, False
    objHTTP.send

    Set oAgentInfo = CreateObject("Microsoft.XMLDOM")
    oAgentInfo.async = False
    oAgentInfo.LoadXML(objHTTP.responseText)
  
    for each oInfo in oAgentInfo.documentElement.childnodes
      ListNames oInfo.childnodes
    next
  end if
next

Sub ListNames(oNames)
  dim oName, name, inst
  for each oName in oNames
    if oName.NodeName <> "#text" then
	  inst = mid(oName.selectSingleNode("name").text, 1, Instr(oName.selectSingleNode("name").text, "@")-1)
      WriteMonRes oName.nodename, oName.selectSingleNode("id").text, inst
	end if 
  next  
End Sub

Sub WriteMonRes(name, id, inst)
  dim dt, oRes, oNode, row, oCell, pos, hdr, oFile, filename
   
  filename = outdir + agentname & "-" &  inst & "-" & year(dtStart) & right("0" & month(dtStart),2) & right("0" & day(dtStart),2) & "-" & year(dtEnd) & right("0" & month(dtEnd),2) & right("0" & day(dtEnd),2) & ".csv"
  Set oFile = oFso.CreateTextFile(filename, True)

  dt = dtStart : pos = 0
  do while dt <= dtEnd
    wscript.echo "Dumping results for " + agentname + " " + inst, "(" + ID + ") on " & dt
	
    url = "http://www.monitis.com/api?apikey=" & apikey & "&output=xml&action=" & name & "Result&monitorId=" + id + "&day=" & day(dt) & "&month=" & month(dt) & "&year=" & year(dt) & "&timezone=" & timezone
    objHTTP.open "GET", url, False
    objHTTP.send  
    Set oRes = CreateObject("Microsoft.XMLDOM")
    oRes.async = False
    oRes.LoadXML(objHTTP.responseText)
	if not oRes.firstchild is nothing then
      for each oNode in oRes.firstchild.childnodes
	    row = """" & year(dt) & "-" & right("0" & month(dt),2) & "-" & right("0" & day(dt),2) & " " & oNode.selectSingleNode("time").text & """"
        for each oCell in oNode.ChildNodes
		  if oCell.nodename <> "time" then
            row = row + ";""" + oCell.text + """"
          end if
        next
		if pos = 0 then
		  hdr = """datetime"""
          for each oCell in oNode.ChildNodes
	        if oCell.nodename <> "time" then
              hdr = hdr + ";""" + oCell.nodename + """"
            end if
          next
		  oFile.writeline hdr
		end if
        oFile.writeline row
		pos = pos + 1
	  next
    end if
	dt = DateAdd("d", 1, dt)
  loop
  oFile.close
  Set oFile = oFso.GetFile(filename)
  if oFile.Size = 0 then oFso.DeleteFile(filename)
End Sub

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

Sub ShowUsage
  wscript.echo "DumpMonRes.vbs parameters:"
  wscript.echo " /apiKey:<apiKey> Your API key"
  wscript.echo " /secretKey:<secretKey> Your Secret Key"
  wscript.echo " /dtStart:<YYYYMMMDD> the starting date for the period to export"
  wscript.echo " /agent:<agentname> export data only for the specified agent"
  wscript.echo " [/dtEnd:<YYYYMMMDD>] the ending date for the period to export"
  wscript.echo " [/outdir:<directryname>] the directory where result files will be stored, defaults to current folder"
  wscript.quit
End Sub