' Declare variables to be used throughout script
Dim  outBoundNum, inBoundNum, numQueues, fso, folderArray(2,1), objFolder, files, folderCount, faxCount

numQueues = 2
'folderArray(0,0) = "C:\Program Files (x86)\GFI\Faxmaker\out\"
'folderArray(0,1) = "C:\Program Files (x86)\GFI\Faxmaker\in\"
folderArray(0,0) = "C:\Users\aalford\Desktop\API\out\"
folderArray(0,1) = "C:\Users\aalford\Desktop\API\in\"
'folderArray(0,2) = "Put a third queue folder here"

folderArray(1,0) = "outbound:"
folderArray(1,1) = "inbound:"
'folderArray(1,2) = 3rd_QueueCol

Set fso = CreateObject("Scripting.FileSystemObject")

For folderCount = 0 to numQueues - 1
	Set objFolder = fso.GetFolder(folderArray(0,folderCount))
	Set files = objFolder.Files
	faxCount = 0
	For Each objFile in files
		If UCase(fso.GetExtensionName(objFile.name)) = "SFM" Then
			faxCount = faxCount + 1
    End If
	Next
	folderArray(2, folderCount) = faxCount
Next


'begin Monitis Upload
Dim apiKey, secretKey, objHTTP, tag, monitorID, computer, oWMI, oRes, oEntry, unixDate, results, dtGMT, oResp, oNode

'Your API key and Secret Key
apiKey = "Your API Key here"
secretKey = "Your Secret Key here"

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
objHTTP.open "GET", "http://sandbox.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey, False
objHTTP.send
token = DissectStr(objHTTP.responseText, "authToken"":""", """")

'Requests the monitor list in order to find the MonitorID
tag = "[FaxMaker]"
objHTTP.open "GET", "http://sandbox.monitis.com/customMonitorApi?action=getMonitors&apikey=" + apiKey + "&tag=" + tag + "&output=xml", False
objHTTP.send

Set oResp = CreateObject("Microsoft.XMLDOM")
oResp.async = False
oResp.LoadXML(objHTTP.responseText)

for each oNode in oResp.documentElement.childnodes
	if oNode.selectSingleNode("name").text = "FaxMaker Queue Size" then 
		monitorID = oNode.selectSingleNode("id").text
	exit for
	end if
next

'Posts data
For folderCount = 0 to numQueues - 1
	results = results & folderArray(1,folderCount) & folderArray(2,folderCount) & ";"
Next

unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"
objHTTP.open "POST", "http://sandbox.monitis.com/customMonitorApi", False
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