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
Dim FirstStartUp

'Initialize the API and Secret Key
apiKey = "Replace with your key" 
secretKey = "Replace with your key" 

'Initialize monitor variables
Computer = "."
MonitorName = "printers"
MonitorTag = "Printers"

'Initialize WMI
Set objWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + computer + "\root\cimv2")

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Get current local date and time using the timezone from the obtained GMT date
dtGMT = GMTDate()
unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"

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
	
	strPrinterStatus = GetPrinterStatus(objPrinter.PrinterStatus)
	strErrorState = GetPrinterErrorState(objPrinter.DetectedErrorState)
    
	'Get current local date and time using the timezone from the obtained GMT date
	dtGMT = GMTDate()
	unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"
    
    'Find the monitorId for the current printer
	MonitorID = FindMonitorID(objPrinter.Name)
	If Trim(MonitorID) <> "" Then
		WScript.Echo "Printer: " & objPrinter.Name & " - " & strPrinterStatus & " - " & strErrorState
		WScript.Echo "MonitorID: " & MonitorID

		Results = "status:" & strPrinterStatus & ";detectedError:" & strErrorState & GetPerformanceCounters(objPrinter.Name)
		AddResult
	End If
	
Next	


'Start the continuous monitoring, only recording event data when the status of a printer changes
Set colPrinters = objWMI.ExecNotificationQuery("Select * from __instancemodificationevent within 30 where TargetInstance isa 'Win32_Printer'")

Monitoring = True
Do While Monitoring = True
	
	Set objPrinter = colPrinters.NextEvent
	If objPrinter.TargetInstance.PrinterStatus <> objPrinter.PreviousInstance.PrinterStatus Or _
	   objPrinter.TargetInstance.DetectedErrorState <> objPrinter.PreviousInstance.DetectedErrorState Then
	   
		strPrinterStatus = GetPrinterStatus(objPrinter.TargetInstance.PrinterStatus)
		strErrorState = GetPrinterErrorState(objPrinter.TargetInstance.DetectedErrorState)

		'Get current local date and time using the timezone from the obtained GMT date
		dtGMT = GMTDate()
		unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"
	    
	    'Find the monitorId for the current printer
		MonitorID = FindMonitorID(objPrinter.TargetInstance.Name)
		If Trim(MonitorID) <> "" Then
			WScript.Echo "Printer: " & objPrinter.TargetInstance.Name & " - " & strPrinterStatus & " - " & strErrorState
			WScript.Echo "MonitorID: " & MonitorID

			Results = "status:" & strPrinterStatus & ";detectedError:" & strErrorState & GetPerformanceCounters(objPrinter.TargetInstance.Name)
			AddResult
		End If

	End If

	WScript.Sleep(1000)
Loop


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

'---------------------------------------------------------------------

Function GetPrinterStatus(aStatus)
	Dim strState
	
    Select Case aStatus
        Case 1 : strState = "Other"
        Case 2 : strState = "Unknown"
        Case 3 : strState = "Idle"
        Case 4 : strState = "Printing"
        Case 5 : strState = "Warmup"
        Case 6 : strState = "Stopped Printing"
        Case 7 : strState = "Offline"
        Case Else 
        	strState = "Unknown"
    End Select
    
    GetPrinterStatus = strState
End Function

'---------------------------------------------------------------------

Function GetPrinterErrorState(aState) 
	Dim strState
	 
	Select Case aState
		Case 0 : strState = "No Error"    
		Case 1 : strState = "Other"    
		Case 2 : strState = "No Error"    
		Case 3 : strState = "Low Paper"    
		Case 4 : strState = "No Paper"    
		Case 5 : strState = "Low Toner"    
		Case 6 : strState = "No Toner"    
		Case 7 : strState = "Door Open"    
		Case 8 : strState = "Jammed"    
		Case 9 : strState = "Offline"    
		Case 10 : strState = "Service Requested"    
		Case 11 : strState = "Output Bin Full"    
        Case Else 
        	strState = "Unknown"
	End Select

	GetPrinterErrorState = strState
End Function

'---------------------------------------------------------------------

Function GetPerformanceCounters(aPrinterName)
	Dim objInstances
	Dim strCounters
	
	strCounters = ""
	Set objInstances = objWMI.InstancesOf("Win32_PerfFormattedData_Spooler_PrintQueue",48)
	
	On Error Resume Next
	For Each objInstance in objInstances
		If objInstance.Name = aPrinterName Then
	        WScript.Echo "NotReadyErrors: " & objInstance.NotReadyErrors
	        WScript.Echo "OutOfPaperErrors: " & objInstance.OutofPaperErrors
	        WScript.Echo "JobsPrinted: " & objInstance.TotalJobsPrinted
	        WScript.Echo "PagesPrinted: " & objInstance.TotalPagesPrinted
		    
		    strCounters = ";notReadyErrors:" & CStr(objInstance.NotReadyErrors) & _
		    			  ";outOfPaperErrors:" & CStr(objInstance.OutofPaperErrors) & _
		    			  ";jobsPrinted:" & CStr(objInstance.TotalJobsPrinted) & _
		    			  ";pagesPrinted:" & CStr(objInstance.TotalPagesPrinted)
		    Exit For
		End If
	Next
	On Error Goto 0
	
	GetPerformanceCounters = strCounters
	Set objInstances = Nothing
End Function