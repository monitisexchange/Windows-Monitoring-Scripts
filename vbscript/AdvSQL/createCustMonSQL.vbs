Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, monitorParams, resultParams, name, dtGMT, computer, tag, page_title, group, columns, pageID, testID, colunm, row

'You API key and Secret Key
apiKey = "your API key here"
secretKey = "your secret key here"

dtGMT = GMTDate()

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Request a token to use in following calls
url = "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey
objHTTP.open "GET", url, False
wscript.echo "Requesting token"
wscript.echo "GET: " + url
objHTTP.send
resp = objHTTP.responseText
wscript.echo "Result: " + resp
token = DissectStr(resp, "authToken"":""", """")

'Add a page
wscript.echo "Adding a page"
url = "http://www.monitis.com/api"
page_title = "SQL Server"
group = "sqlserver"
columns = "2"
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addPage&title=" + page_title + "&columnCount=" + columns
objHTTP.send postData
resp = objHTTP.responseText
wscript.echo "Result: " + resp
pageID = DissectStr(resp, "pageId"":", "}")
wscript.echo "PageID: " + pageID

tag = "SQL Server"

'% Processor Time
monitorParams = "Processor:Processor utilization:% processor time:3:false;"
resultParams = "total:Total:N%2FA:2;privileged:Privileged:N%2FA:2;"
name = "Percent Processor time" : row = 1 : colunm = 1
AddCustMon

'% Disk time
monitorParams = "Disk:Disk utilization:% disk time:3:false;"
resultParams = "total:Total:N%2FA:2;read:Read:N%2FA:2;"
name = "Percent Disk time" : row = 1 : colunm = 2
AddCustMon

'Batch requests per second
monitorParams = "BatchReq:Batch requests per second:req/sec:3:false;"
resultParams = "requests:Requests/sec:N%2FA:2;"
name = "Batch Requests per second" : row = 2 : colunm = 1
AddCustMon

'Buffer cache hit ratio
monitorParams = "hitratio:Hit ratio:hitratio:3:false;"
resultParams = "hitratio:Hit ratio:N%2FA:2;"
name = "Buffer cache hit ratio" : row = 2 : colunm = 2
AddCustMon

'Page lookups
monitorParams = "pagelookups:Page lookups:pagelookups:3:false;"
resultParams = "lookups:lookups:N%2FA:2;"
name = "Page lookups" : row = 3 : colunm = 1
AddCustMon

'Page read and Page writes
monitorParams = "physicalPages:Physical pages:physicalPages:3:false;"
resultParams = "read:Read:N%2FA:2;write:Write:N%2FA:2;"
name = "Physical pages" : row = 3 : colunm = 2
AddCustMon

'User connections
monitorParams = "users:User connections:users:3:false;"
resultParams = "users:users:N%2FA:2;"
name = "User connections" : row = 4 : colunm = 1
AddCustMon

'SQL Server Memory
monitorParams = "sqlmemory:SQL Server memory:sqlmemory:3:false;"
resultParams = "total:Total KB:N%2FA:2;target:Target KB:N%2FA:2;"
name = "SQL Server memory" : row = 4 : colunm = 2
AddCustMon

'Database size and usage
monitorParams = "TestDBSize:Test database size:TestDBSize:3:false;"
resultParams = "allocated:Allocated bytes:N%2FA:2;used:Used bytes:N%2FA:2;"
name = "Test database size" : row = 5 : colunm = 1
AddCustMon

'Log size and usage
monitorParams = "TestLogSize:Test log size:TestLogSize:3:false;"
resultParams = "allocated:Allocated bytes:N%2FA:2;used:Used bytes:N%2FA:2;"
name = "Test log size" : row = 5 : colunm = 2
AddCustMon

Sub AddCustMon
  url = "http://www.monitis.com/customMonitorApi"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addMonitor&monitorParams=" + monitorParams + "&resultParams=" + resultParams + "&name=" + name + "&tag=" + tag
  wscript.echo "Adding custom monitor"
  wscript.echo "POST: " + url
  wscript.echo "Data: " + postData
  objHTTP.send postData
  resp = objHTTP.responseText
  wscript.echo "Result: " + resp
  testID = DissectStr(resp, "data"":", "}")
  wscript.echo "TestID: " + testID
  
  wscript.echo "Adding test " + name + " to the page"
  url = "http://www.monitis.com/api"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" & apiKey & "&validation=token&authToken=" & token & "&timestamp=" & FmtDate(dtGMT) & "&action=addPageModule&moduleName=CustomMonitor&pageId=" & pageID & "&column=" & colunm & "&row=" & row & "&dataModuleId=" & testID
  objHTTP.send postData
  resp = objHTTP.responseText
  wscript.echo "Result: " + resp
End Sub

Function DissectStr(cString, cStart, cEnd)
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
  dim oWMI, oRes, oEntry
  Set oWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\.\root\cimv2")
  GMTDate = now
  Set oRes = oWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
  For each oEntry in oRes
    GMTDate = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), GMTDate)
  next
End function
