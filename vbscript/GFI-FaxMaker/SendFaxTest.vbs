'Sends a test fax through the FaxMaker API and uploads result to Monitis
'August 28th, 2012
'Adam Alford

Dim faxNumber, attachment, faxBody, apiPath, sendTimeout, timerName, fso, writer, errFound, objFile, strLine, sendStatus, description, i

'Assign values to the variables that will be used to create the API document
faxNumber = "::5551212"
attachment = "::a=test.jpg"
faxBody = "This is a test fax"
apiPath = "Your FaxMaker API directory here"

'how long to wait in seconds for the fax to send
sendTimeout = 120

' Use Timer function to retrieve number of seconds since midnight for unique filename
timerName = Round(Timer())

' write the API document using the variables and the rounded number from the Timer function as the filename
Set fso = CreateObject("Scripting.FileSystemObject")
Set writer = fso.OpenTextFile(apiPath & timerName & ".txt", 8, True)
writer.WriteLine(faxNumber)
writer.WriteLine(attachment)
writer.WriteLine(faxBody)
writer.Close
Set writer = NOTHING

'Copy attachment to FAXmaker API folder
fso.CopyFile "test.jpg", apiPath

' Look for .ok or .err file until timeout expires
i = 0
Do Until i = sendTimeout
	if fso.FileExists(apiPath & timerName & ".ok") then
		errFound = False
		Exit Do
	elseif fso.FileExists(apiPath & timerName & ".err") then
		errFound = True
		Exit Do
	end if
	wscript.sleep 1000
	i=i+1
Loop

'read error file for description and replase : with , for monitis upload
if errFound = True then
	Set objFile = fso.OpenTextFile(apiPath & timerName & ".err", 1)
	Do Until objFile.AtEndOfStream
		strLine = objFile.ReadLine
		if InStr(strLine, "Description: ") then
			sendStatus = "Failure"
			strLine = Replace(strLine, "Description: ", "")
			description = Replace(strLine, ":", ",")
			Set objFile = NOTHING
			fso.DeleteFile apiPath & timerName & ".err"			
			Exit Do
		end if
	Loop	
elseif i = sendTimeout then
	sendStatus = "Failure"
	description = "No response was received before timeout of " & sendTimeout & " seconds expired"
else
	sendStatus = "Success"
	description = " "
	fso.DeleteFile apiPath & timerName & ".ok"
end if

'cleanup
if (fso.FileExists(apiPath & timerName & ".txt")) then
	fso.DeleteFile apiPath & timerName & ".txt"
	fso.DeleteFile apiPath & "test.jpg"
end if
Set fso = NOTHING

'begin Monitis Upload
Dim apiKey, secretKey, objHTTP, tag, monitorID, computer, oWMI, oRes, oEntry, unixDate, results, dtGMT, oResp, oNode

'Your API key and Secret Key
apiKey = "Your API key here"
secretKey = "Your Secret key here"

'Connecting to WMI, "." is for local computer, specify computer name for another computer
computer = "."
Set oWMI = GetObject("winmgmts:{impersonationLevel=impersonate,(Security)}!\\" + computer + "\root\cimv2")

'Finds current timezone to obtain GMT date
dtGMT = now
Set oRes = oWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
for each oEntry in oRes
	dtGMT = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), dtGMT)
next

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Request a token to use in following calls
objHTTP.open "GET", "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey, False
objHTTP.send
token = DissectStr(objHTTP.responseText, "authToken"":""", """")

'Requests the monitor list in order to find the MonitorID
tag = "[FaxMaker]"
objHTTP.open "GET", "http://www.monitis.com/customMonitorApi?action=getMonitors&apikey=" + apiKey + "&tag=" + tag + "&output=xml", False
objHTTP.send

Set oResp = CreateObject("Microsoft.XMLDOM")
oResp.async = False
oResp.LoadXML(objHTTP.responseText)

for each oNode in oResp.documentElement.childnodes
	if oNode.selectSingleNode("name").text = "FaxMaker Send Fax" then 
		monitorID = oNode.selectSingleNode("id").text
	exit for
	end if
next

'Posts data
results = "sendStatus:" + sendStatus + ";description:" + description + ";"
unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"
objHTTP.open "POST", "http://www.monitis.com/customMonitorApi", False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
objHTTP.send "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addResult&monitorId=" + monitorID + "&checktime=" + unixDate + "&results=" + results

'Exit Script
WScript.Quit()

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

Function FmtDateEvt(dt)
	FmtDateEvt = cstr(Datepart("yyyy", dt)) + right("0" + cstr(Datepart("m", dt)),2) +  right("0" + cstr(Datepart ("d", dt)),2) + right("0" + cstr(Datepart("h", dt)),2) + right("0" + cstr(Datepart("n", dt)),2) + right("0" + cstr(Datepart("S", dt)),2) + ".000000-000"
end function