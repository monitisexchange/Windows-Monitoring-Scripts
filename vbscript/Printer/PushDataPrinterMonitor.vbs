'****************************************************************************************
' PushDataPrinterMonitor.vbs
'
' Post printer data to the custom printer monitor to the Monitis dashboard. The monitor
' supports multiple printers connected to a single computer.
'****************************************************************************************

Dim apiKey				'apiKey for Monitis. Replace with your assigned API key
Dim secretKey			'secretKey for Monitis API. Replace with your assigned Secret key
Dim token				'Monitis authToken
Dim Computer			'Local or remote computer name to connect to
Dim dtGMT				'GMT Time variable
Dim UnixDate			'Unix date format
Dim objHTTP				'HTTP object to submit POST/GET to Monitis
Dim objWMI				'WMI object
Dim objResponse			'Response object
Dim objTimeStamp		'WMI Locate Date and Time object
Dim PageTitle			'Title to appear on the page tabs
Dim MonitorID			'Monitor identifier
Dim MonitorName			'Name the monitor
Dim MonitorTag			'Monitor identifier tag
Dim Results				'Values to be pushed to the monitor

'Initialize the API and Secret Key
apiKey = "AVUU86JKUKERMF4LUF15RAAPS" 
secretKey = "1S29IOT3MOQNJUCL2MOEO76SN4" 

'Initialize monitor variables
Computer = "."
MonitorName = "printers"
MonitorTag = "Printers"

'Initialize WMI
Set objWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + computer + "\root\cimv2")

'Get current local date and time using the timezone from the obtained GMT date
dtGMT = GMTDate()
unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Request a token to use in following calls
url = "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey + "&version=2"
objHTTP.open "GET", url, False
objHTTP.send
resp = objHTTP.responseText
token = DissectStr(resp, "authToken"":""", """")

'Requests the monitor list so we can find the monitorId of each printer monitor on the dashboard page
url = "http://www.monitis.com/customMonitorApi?action=getMonitors&apikey=" + apiKey + "&tag=" + MonitorTag + "&output=xml"
objHTTP.open "GET", url, False
wscript.echo "Requesting monitors list"
wscript.echo "GET: " + url
objHTTP.send
resp = objHTTP.responseText

Set objResponse = CreateObject("Microsoft.XMLDOM")
objResponse.async = False
objResponse.LoadXML(resp)

'Loop through all printers and post the printer values to the custom printer monitor for each printer
Set colInstalledPrinters = objWMI.ExecQuery("Select * from Win32_Printer")
For Each objPrinter in colInstalledPrinters
	
    Select Case objPrinter.PrinterStatus
        Case 1 : strPrinterStatus = "Not responding"
        Case 2 : strPrinterStatus = "Not responding"
        Case 3 : strPrinterStatus = "Idle"
        Case 4 : strPrinterStatus = "Printing"
        Case 5 : strPrinterStatus = "Warmup"
        Case 6 : strPrinterStatus = "Stopped Printing"
        Case 7 : strPrinterStatus = "Offline"
        Case Else 
        	strPrinterStatus = "Unknown"
    End Select
    
	Select Case objPrinter.DetectedErrorState
		Case 0 : strErrorState = "No Error"    
		Case 1 : strErrorState = "Other"    
		Case 2 : strErrorState = "No Error"    
		Case 3 : strErrorState = "Low Paper"    
		Case 4 : strErrorState = "No Paper"    
		Case 5 : strErrorState = "Low Toner"    
		Case 6 : strErrorState = "No Toner"    
		Case 7 : strErrorState = "Door Open"    
		Case 8 : strErrorState = "Jammed"    
		Case 9 : strErrorState = "Offline"    
		Case 10 : strErrorState = "Service Requested"    
		Case 11 : strErrorState = "Output Bin Full"    
        Case Else 
        	strErrorState = "Unknown"
	End Select
    
    'Find the monitorId for the current printer
	MonitorID = FindMonitorID(objPrinter.Name)
	If Trim(MonitorID) <> "" Then
		WScript.Echo "Printer: " & objPrinter.Name & " - " & strPrinterStatus & " - " & strErrorState
		WScript.Echo "MonitorID: " & MonitorID
		
		Results = "status:" & strPrinterStatus & ";detectedError:" & strErrorState
		AddResult
	End If
Next	

'Cleanup
Set objHTTP = Nothing
Set objWMI = Nothing
Set objRes = Nothing
Set objTimeStamp = Nothing
Set objResponse = Nothing

'---------------------------------------------------------------------
' FUNCTIONS
'---------------------------------------------------------------------

Sub AddResult
  url = "http://www.monitis.com/customMonitorApi"
  action = "addResult"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=" + action + "&monitorId=" + MonitorID + "&checktime=" + UnixDate + "&results=" + Results
  wscript.echo "Add monitor result"
  wscript.echo "POST: " + url
  wscript.echo "Data: " + postData
  objHTTP.send postData
  resp = objHTTP.responseText
  wscript.echo "Result: " + resp
End Sub

'---------------------------------------------------------------------

Function FindMonitorID(monName)
  For Each objNode in objResponse.documentElement.childnodes
    If objNode.selectSingleNode("name").text = monName Then
      FindMonitorID = objNode.selectSingleNode("id").text
	  Exit For
    End If
  Next
End Function

'---------------------------------------------------------------------

Function DissectStr(cString, cStart, cEnd)
	Dim nStart, nEnd
	nStart = InStr(cString, cStart)
	
	If nStart = 0 then
		DissectStr = ""
	Else
		nStart = nStart + len(cStart)
		If cEnd = "" then
			nEnd = len(cString)
		Else
			nEnd = InStr(nStart, cString, cEnd)
			If nEnd = 0 Then 
				nEnd = nStart 
			Else 
				nEnd = nEnd - nStart
			End If
		End if
		DissectStr = Mid(cString, nStart, nEnd)
	End if
End Function
 
'---------------------------------------------------------------------

Function FmtDate(dt)
	FmtDate = CStr(Datepart("yyyy", dt)) + _
			  "-" + Right("0"+CStr(Datepart("m", dt)),2) + _
			  "-" + Right("0"+CStr(Datepart("d", dt)),2) + _
			  " " + Right("0"+CStr(Datepart("h", dt)),2) + _
			  ":" + Right("0"+CStr(Datepart("n", dt)),2) + _
			  ":" + Right("0" + CStr(Datepart("S", dt)),2)
End Function

'---------------------------------------------------------------------

Function GMTDate()
  GMTDate = Now
  Set oRes = objWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
  For each oEntry in oRes
    GMTDate = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), GMTDate)
  Next
End function

