Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, action, tag, monitorID, computer, value, unixDate, results, dtGMT
dim instance, database, connection_string, recordset, connection, query, value1, value2, oResp, oNode, oWMI, oRes, oEntry, SQLInstance

Set connection = WScript.CreateObject("ADODB.Connection")
Set recordset = CreateObject("ADODB.Recordset")

'You API key and Secret Key
apiKey = "your API key here"
secretKey = "your secret key here"

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
token = DissectStr(resp, "authToken"":""", """")

'Requests the monitor list in order to find the MonitorID
tag = "SQL Server"
url = "http://www.monitis.com/customMonitorApi?action=getMonitors&apikey=" + apiKey + "&tag=" + tag + "&output=xml"
objHTTP.open "GET", url, False
wscript.echo "Requesting monitors list"
wscript.echo "GET: " + url
objHTTP.send
resp = objHTTP.responseText

Set oResp = CreateObject("Microsoft.XMLDOM")
oResp.async = False
oResp.LoadXML(resp)

monitorID = FindMonitorID("Percent Processor time")
Set oRes = oWMI.ExecQuery ("select PercentProcessorTime, PercentPrivilegedTime from Win32_PerfFormattedData_PerfOS_Processor where Name = '_Total'")
For each oEntry in oRes
  value1 = oEntry.PercentProcessorTime
  value2 = oEntry.PercentPrivilegedTime
next
results = "privileged:" & value2 & ";" & "total:" & value1
AddResult

monitorID = FindMonitorID("Percent Disk time")
Set oRes = oWMI.ExecQuery ("select PercentDiskTime, PercentDiskReadTime from Win32_PerfFormattedData_PerfDisk_PhysicalDisk where Name = '0 C:'")
For each oEntry in oRes
  value1 = oEntry.PercentDiskTime
  value2 = oEntry.PercentDiskReadTime
next
results = "read:" & value2 & ";total:" & value1
AddResult

monitorID = FindMonitorID("Batch Requests per second")
Set oRes = oWMI.ExecQuery ("select BatchRequestsPersec from Win32_PerfFormattedData_MSSQLSQLEXPRESS_MSSQLSQLEXPRESSSQLStatistics")
For each oEntry in oRes
  value1 = oEntry.BatchRequestsPersec
next
results = "requests:" & value1 & ";"
AddResult

monitorID = FindMonitorID("Buffer cache hit ratio")
Set oRes = oWMI.ExecQuery ("select Buffercachehitratio, PagelookupsPersec from Win32_PerfFormattedData_MSSQLSQLEXPRESS_MSSQLSQLEXPRESSBufferManager")
For each oEntry in oRes
  value1 = oEntry.Buffercachehitratio
  value2 = oEntry.PagelookupsPersec
next
results = "hitratio:" & value1 & ";"
AddResult

monitorID = FindMonitorID("Page lookups")
results = "lookups:" & value2 & ";"
AddResult

monitorID = FindMonitorID("Physical pages")
Set oRes = oWMI.ExecQuery ("select PagereadsPersec, PagewritesPersec from Win32_PerfFormattedData_MSSQLSQLEXPRESS_MSSQLSQLEXPRESSBufferManager")
For each oEntry in oRes
  value1 = oEntry.PagereadsPersec
  value2 = oEntry.PagewritesPersec
next
results = "write:" & value2 & ";read:" & value1
AddResult

monitorID = FindMonitorID("User connections")
Set oRes = oWMI.ExecQuery ("select UserConnections from Win32_PerfFormattedData_MSSQLSQLEXPRESS_MSSQLSQLEXPRESSGeneralStatistics")
For each oEntry in oRes
  value1 = oEntry.UserConnections
next
results = "users:" & value1
AddResult

monitorID = FindMonitorID("SQL Server memory")
Set oRes = oWMI.ExecQuery ("select TargetServerMemoryKB, TotalServerMemoryKB from Win32_PerfFormattedData_MSSQLSQLEXPRESS_MSSQLSQLEXPRESSMemoryManager")
For each oEntry in oRes
  value1 = oEntry.TotalServerMemoryKB
  value2 = oEntry.TargetServerMemoryKB
next
results = "target:" & value2 & ";total:" & value1
AddResult

instance = "localhost\SQLEXPRESS"
database = "Test"

'Connection string for Trusted Authentication
connection_string = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + database + ";Data Source=" + instance

'Connection string for Standard Authentication
'connection_string = "Provider=SQLOLEDB.1;Data Source=" + instance + ";Initial Catalog=" + database + ";User Id=user1;Password=password1;"

Set connection = WScript.CreateObject("ADODB.Connection")
Set recordset = CreateObject("ADODB.Recordset")
connection.Open connection_string

monitorID = FindMonitorID("Test database size")
query = "select sum(cast(size as bigint)) * 8000000 / 1024 size, (sum(cast(size as bigint)) * 8000000 / 1024) - (sum(cast(fileproperty(mf.name, 'SpaceUsed') as bigint)) * 8000000 / 1024) free  from sys.master_files mf where database_id = db_id() and type_desc = 'ROWS'"
recordset.Open query, connection
value1 = recordset(0)
value2 = recordset(1)
recordSet.Close
results = "used:" & value2 & ";allocated:" & value1
AddResult

monitorID = FindMonitorID("Test log size")
query = "select sum(cast(size as bigint)) * 8000000 / 1024 size, (sum(cast(size as bigint)) * 8000000 / 1024) - (sum(cast(fileproperty(mf.name, 'SpaceUsed') as bigint)) * 8000000 / 1024) free  from sys.master_files mf where database_id = db_id() and type_desc = 'LOG'"
recordset.Open query, connection
value1 = recordset(0)
value2 = recordset(1)
recordSet.Close
results = "used:" & value2 & ";allocated:" & value1
AddResult

Function FindMonitorID(monitorName)
  for each oNode in oResp.documentElement.childnodes
    if oNode.selectSingleNode("name").text = monitorName then 
      FindMonitorID = oNode.selectSingleNode("id").text
	  exit for
    end if
  next
End Function

Sub AddResult
  url = "http://www.monitis.com/customMonitorApi"
  action = "addResult"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=" + action + "&monitorId=" + monitorID + "&checktime=" + unixDate + "&results=" + results
  wscript.echo "Add monitor result"
  wscript.echo "POST: " + url
  wscript.echo "Data: " + postData
  objHTTP.send postData
  resp = objHTTP.responseText
  wscript.echo "Result: " + resp
End sub

Function DissectStr(cString, cStart, cEnd)
  'Generic string manipulation function to extract value from JSON output
  dim nStart, nEnd
  nStart = InStr(cString, cStart)
  if nStart = 0 then 
    DissectStr = ""
  else
    nStart = nStart + len(cStart)
    if cEnd = "" then
      nEnd = len(cString)
    else
      nEnd = InStr(nStart, cString, cEnd)
      if nEnd = 0 then nEnd = nStart else nEnd = nEnd - nStart
    end if
    DissectStr = mid(cString, nStart, nEnd)
  end if
End Function

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
