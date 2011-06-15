Option Explicit
dim apiKey, secretKey, objHTTP, url, postData, resp, token, monitorParams, resultParams, name, oWMI, oRes, oEntry, dtGMT, computer, tag

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

'Adds the custom monitor
url = "http://www.monitis.com/customMonitorApi"
monitorParams = "ActiveDirectory:Group:Members:3:false;"
resultParams = "members:Members:N%2FA:2;"
name = "AD group members"
tag = "[AD+sample]"
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addMonitor&monitorParams=" + monitorParams + "&resultParams=" + resultParams + "&name=" + name + "&tag=" + tag
wscript.echo "Adding custom monitor"
wscript.echo "POST: " + url
wscript.echo "Data: " + postData
objHTTP.send postData
resp = objHTTP.responseText
wscript.echo "Result: " + resp

Set objHTTP = Nothing

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
