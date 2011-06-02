option explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, monitorParams, resultParams, name, dtGMT
dim csvfile, oFso, oFile, line, page_title, columns, pageID, group, testID, row, headers

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
page_title = "Page from CSV"
group = "from_csv"
columns = "1"
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addPage&title=" + page_title + "&columnCount=" + columns
objHTTP.send postData
resp = objHTTP.responseText
wscript.echo "Result: " + resp
pageID = DissectStr(resp, "pageId"":", "}")
wscript.echo "PageID: " + pageID

Set oFso = CreateObject("Scripting.FileSystemObject")

csvfile = "BulkAdd.csv"
Set oFile = oFso.OpenTextFile(csvfile, 1, False)
headers = split(oFile.ReadLine, ";")

row = 1
do until oFile.AtEndOfStream
  line = split(oFile.ReadLine, ";")
  wscript.echo "Adding monitor: " + GetVal("Name")

  url = "http://www.monitis.com/api"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addExternalMonitor&type=http&name=" + GetVal("Name") 
  postData = postData + "&tag=" + GetVal("Tag")  + "&url=" + GetVal("URL") + "&locationIds=" + GetVal("Locations") + "&interval=" + GetVal("Interval") +  "&timeout=" + GetVal("Timeout")
  postData = postData + "&overSSL=false&detailedTestType=1"
  if GetVal("Match") > "" then 
    postData = postData + "&contentMatchFlag=" + GetVal("Match") + "&contentMatchString=" + GetVal("Content")
  else 
    postData = postData + "&contentMatchFlag=0"
  end if
  wscript.echo postData
  objHTTP.send postData
  resp = objHTTP.responseText
  wscript.echo "Result: " + resp  
  testID = DissectStr(resp, "testId"":", ",")
  wscript.echo "TestID: " + testID
  
  wscript.echo "Adding test " + line(1) + " to the page"

  url = "http://www.monitis.com/api"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addPageModule&moduleName=External&pageId=" + pageID + "&column=1&row=" + CStr(row) + "&dataModuleId=" + testID

  objHTTP.send postData
  resp = objHTTP.responseText
  wscript.echo "Result: " + resp
  
  row = row + 1
loop

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

Function GetVal(val)
  dim i, l
  l = UBound(headers) : val = lcase(val)
  for i = 0 to l
    if lcase(headers(i)) = val then
	  GetVal = line(i)
	  exit function
	end if
  next
  GetVal = ""
End Function