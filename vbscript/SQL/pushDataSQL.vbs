Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, action, tag, monitorID, computer, value, unixDate, results, dtGMT
dim instance, database, connection_string, recordset, connection, query, ordProcessing, ordShipping

Set connection = WScript.CreateObject("ADODB.Connection")
Set recordset = CreateObject("ADODB.Recordset")

'You API key and Secret Key
apiKey = "your API key here"
secretKey = "your secret key here"

dtGMT = GMTDate()

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
tag = "[sql+sample]"
url = "http://www.monitis.com/customMonitorApi?action=getMonitors&apikey=" + apiKey + "&tag=" + tag
objHTTP.open "GET", url, False
wscript.echo "Requesting monitors list"
wscript.echo "GET: " + url
objHTTP.send
resp = objHTTP.responseText
wscript.echo "Result: " + resp
monitorID = DissectStr(resp, "id"":""", """,""tag")
wscript.echo "Monitor ID: " + monitorID

'Queries SQL Server to obtain data to upload
instance = "localhost\SQLEXPRESS"
database = "Customers"

'Connection string for Trusted Authentication
'connection_string = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + database + ";Data Source=" + instance

'Connection string for Standard Authentication
connection_string = "Provider=SQLOLEDB.1;Data Source=" + instance + ";Initial Catalog=" + database + ";User Id=user1;Password=password1;"

Set connection = WScript.CreateObject("ADODB.Connection")
Set recordset = CreateObject("ADODB.Recordset")
connection.Open connection_string

query = "select count(*) from Orders where Status = 'Processing'"
recordset.Open query, connection
ordProcessing = recordset(0)
recordSet.Close

query = "select count(*) from Orders where Status = 'Shipping'"
recordset.Open query, connection
ordShipping = recordset(0)

'Posts data
url = "http://www.monitis.com/customMonitorApi"
action = "addResult"
results = "processing:" & ordProcessing & ";shipping:" & ordShipping & ";"
unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=" + action + "&monitorId=" + monitorID + "&checktime=" + unixDate + "&results=" + results
wscript.echo "Add monitor result"
wscript.echo "POST: " + url
wscript.echo "Data: " + postData
objHTTP.send postData
resp = objHTTP.responseText
wscript.echo "Result: " + resp

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
  dim oWMI, oRes, oEntry
  Set oWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\.\root\cimv2")
  GMTDate = now
  Set oRes = oWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
  For each oEntry in oRes
    GMTDate = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), GMTDate)
  next
End function
