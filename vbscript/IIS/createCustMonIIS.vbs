Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, monitorParams, resultParams, name, dtGMT, computer, tag, page_title, group, columns, pageID, testID, colunm, row, site

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
page_title = "IIS"
group = "IIS"
columns = "2"
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addPage&title=" + page_title + "&columnCount=" + columns
objHTTP.send postData
resp = objHTTP.responseText
wscript.echo "Result: " + resp
pageID = DissectStr(resp, "pageId"":", "}")
wscript.echo "PageID: " + pageID

tag = "IIS"

url = "http://www.monitis.com/api"
objHTTP.open "POST", url, False
site = "www.yoursite.com"
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addExternalMonitor&type=http&name=Availability"
postData = postData + "&tag=" + tag  + "&url=" + site + "&locationIds=1,2&interval=5&timeout=10000&overSSL=false&detailedTestType=1&fromDashboard=true"
wscript.echo postData
objHTTP.send postData
resp = objHTTP.responseText
wscript.echo "Result: " + resp  
testID = DissectStr(resp, "testId"":", ",")
wscript.echo "TestID: " + testID
  
wscript.echo "Adding test Availability to the page"

url = "http://www.monitis.com/api"
row = 1 : colunm = 1
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addPageModule&moduleName=External&pageId=" + pageID + "&column=" & colunm & "&row=" & row & "&dataModuleId=" + testID

objHTTP.send postData
resp = objHTTP.responseText
wscript.echo "Result: " + resp
  
'Current connections
monitorParams = "Connections:Current connections:Connections:3:false;"
resultParams = "connections:Connections:N%2FA:2;"
name = "Current connections" : row = 2 : colunm = 1
AddCustMon

'% Bytes transmitted
monitorParams = "Bytes:Bytes transmitted per minute:Bytes:3:false;"
resultParams = "sent:Sent:N%2FA:2;received:Received:N%2FA:2;"
name = "Bytes transmitted per minute" : row = 3 : colunm = 1
AddCustMon

'GET requests per minute
monitorParams = "GET Requests:GET requests per minute:req/min:3:false;"
resultParams = "requests:Requests/min:N%2FA:2;"
name = "GET requests per minute" : row = 4 : colunm = 1
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
