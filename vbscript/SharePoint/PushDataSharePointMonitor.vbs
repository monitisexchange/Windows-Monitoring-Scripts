'****************************************************************************************
' PushDataSharePointMonitor.vbs
'
' Post SharePoint data to the custom SharePoint monitor on the Monitis dashboard. 
'****************************************************************************************
Dim apiKey				'apiKey for Monitis. Replace with your assigned API key
Dim secretKey			'secretKey for Monitis API. Replace with your assigned Secret key
Dim token				'Monitis authToken
Dim strSPServer			'Name of the SharePoint server
Dim strSQLServer		'Name of the SQL Server for SharePoint's databases
Dim dtGMT				'GMT Time variable
Dim UnixDate			'Unix date format
Dim objHTTP				'HTTP object to submit POST/GET to Monitis
Dim objSPWMI			'WMI object for SharePoint Server
Dim objSQLWMI			'WMI object for SQL Server
Dim objResponse			'Response object
Dim objTimeStamp		'WMI Locate Date and Time object
Dim PageTitle			'Title to appear on the page tabs
Dim MonitorID			'Monitor identifier
Dim MonitorName			'Name the monitor
Dim MonitorTag			'Monitor identifier tag
Dim Results				'Values to be pushed to the monitor
Dim objDiskData			'WMI Disk counters
Dim objCPUData			'WMI CPU counters
Dim objNetworkData		'WMI Network Adapter counters
Dim objMemoryData		'WMI Memory counters
Dim objASPNETData		'WMI ASPNET counters


'Initialize the API and Secret Key
apiKey = "AVUU86JKUKERMF4LUF15RAAPS"		'REPLACE WITH YOUR API KEY 
secretKey = "1S29IOT3MOQNJUCL2MOEO76SN4" 	'REPLACE WITH YOUR SECRET KEY

'Specify the monitor tag we're working with
strSPServer = "."							'REPLACE WITH YOUR SHAREPOINT SERVERNAME
strSQLServer = "."							'REPLACE WITH YOUR SQL SERVERNAME
MonitorTag = "SharePoint"

'Initialize WMI. Use "." for local computer, otherwise specify the remote computer name
Set objSPWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + strSPServer + "\root\cimv2")
Set objSQLWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + strSQLServer + "\root\cimv2")

'Create the performance counter objects
Set objRefresher = CreateObject("WbemScripting.SWbemRefresher")

WScript.Echo "Adding Disk Performance Object..."
Set objDiskData = objRefresher.AddEnum(objSPWMI, "Win32_PerfFormattedData_PerfDisk_PhysicalDisk").objectSet
objRefresher.Refresh

WScript.Echo "Adding Processot Performance Object..."
Set objCPUData = objRefresher.AddEnum(objSPWMI, "Win32_PerfFormattedData_PerfOS_Processor").objectSet
objRefresher.Refresh

WScript.Echo "Adding Memory Performance Object..."
Set objMemoryData = objRefresher.AddEnum(objSPWMI, "Win32_PerfFormattedData_PerfOS_Memory").ObjectSet
objRefresher.Refresh

WScript.Echo "Adding Network Performance Object..."
Set objNetworkData = objRefresher.AddEnum(objSPWMI, "Win32_PerfFormattedData_Tcpip_NetworkInterface").objectSet
objRefresher.Refresh

WScript.Echo "Adding ASP.NET Performance Object..."
Set objASPNETData = objRefresher.AddEnum(objSPWMI, "Win32_PerfFormattedData_ASPNET4030319_ASPNETAppsv4030319").ObjectSet
objRefresher.Refresh

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Get current local date and time using the timezone from the obtained GMT date
'dtGMT = GMTDate()
'unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"

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

'Start monitoring
Monitoring = True
Do While Monitoring = True
	'Get current local date and time using the timezone from the obtained GMT date
	dtGMT = GMTDate()
	unixDate = CStr(DateDiff("s", "01/01/1970 00:00:00", DateSerial(Year(dtGMT), Month(dtGMT), Day(dtGMT)) + TimeSerial(Hour(dtGMT), Minute(dtGMT), Second(dtGMT)))) + "000"
	WScript.Echo UnixDate

	'Refresh the performance counters
	objRefresher.Refresh
    
	'Get performance data
	GetSharePointCounters
	GetSQLCounters

	'Wait 30 seconds
	WScript.Sleep(3000)
Loop


'Cleanup
Set objHTTP = Nothing
Set objSQLWMI = Nothing
Set objSPWMI = Nothing
Set objRes = Nothing
Set objTimeStamp = Nothing
Set objResponse = Nothing
Set objRefresher = Nothing
Set objDiskData = Nothing
Set objMemoryData = Nothing
Set objCPUData = Nothing
Set objNetworkData = Nothing
Set objASPNETData = Nothing

'---------------------------------------------------------------------
' FUNCTIONS
'---------------------------------------------------------------------

Function GetSharePointCounters
	On Error Resume Next
        
    'Add the CPU subsystem counters
	MonitorName = "SP CPUSubsystem"
	MonitorID = FindMonitorID(MonitorName)
	WScript.Echo "Monitor: " & MonitorName & " - " & MonitorID
	
	If Trim(MonitorID) <> "" Then
		Results = GetProcessorUtilization() & ";"
		AddResult
	End If
	
	'Add the memory subsystem counters
	MonitorName = "SP MemorySubsystem"
	MonitorID = FindMonitorID(MonitorName)
	WScript.Echo "Monitor: " & MonitorName & " - " & MonitorID
	
	If Trim(MonitorID) <> "" Then
		Results = GetMemoryUtilization() & ";"
		AddResult
	End If
		
	'Add the disk subsystem counters
	MonitorName = "SP Disk Monitor"
	MonitorID = FindMonitorID(MonitorName)
	WScript.Echo "Monitor: " & MonitorName & " - " & MonitorID
	
	If Trim(MonitorID) <> "" Then
		Results = GetDiskUtilization() & ";"
		AddResult
	End If
		

	'Add the ASPNET subsystem counters
	MonitorName = "SP ASPNET Monitor"
	MonitorID = FindMonitorID(MonitorName)
	WScript.Echo "Monitor: " & MonitorName & " - " & MonitorID
	
	If Trim(MonitorID) <> "" Then
		Results = GetASPNETUtilization() & ";"
		AddResult
	End If
		
		
    On Error Goto 0
End Function

'---------------------------------------------------------------------

Function GetSQLCounters
	On Error Resume Next
        
    'Add the CPU subsystem counters
	MonitorName = "SPSQL Monitor"
	MonitorID = FindMonitorID(MonitorName)
	WScript.Echo "Monitor: " & MonitorName & " - " & MonitorID
	
	If Trim(MonitorID) <> "" Then
	
		Set oRes = objSQLWMI.ExecQuery ("select Buffercachehitratio from Win32_PerfFormattedData_MSSQLSQLEXPRESS_MSSQLSQLEXPRESSBufferManager")
		For each oEntry in oRes
		  hr = oEntry.Buffercachehitratio
		Next

		Set oRes = objSQLWMI.ExecQuery ("select UserConnections from Win32_PerfFormattedData_MSSQLSQLEXPRESS_MSSQLSQLEXPRESSGeneralStatistics")
		For each oEntry in oRes
		  users = oEntry.UserConnections
		Next
		
		Results = "hitratio:" & CStr(hr) & ";" & "users:" & CStr(users) & ";"
		AddResult
	End If
	
		
    On Error Goto 0
End Function

'---------------------------------------------------------------------

Function GetDiskUtilization
	For Each objItem in objDiskData
		If objItem.Name = "_Total" Then
			WScript.Echo "Name: " & objItem.Name
			Wscript.Echo "Avg. Disk Queue Length: " & objItem.AvgDiskQueueLength
			Wscript.Echo "Avg. Read Queue Length: " & objItem.AvgReadQueueLength
			Wscript.Echo "Avg. Write Queue Length: " & objItem.AvgWriteQueueLength
			Wscript.Echo "Disk Reads/sec: " & objItem.DiskReadsPersec
			Wscript.Echo "Disk Writes/sec: " & objItem.DiskWritesPersec
			
			GetDiskUtilization = "spAvgQueueLen:" & CStr(objItem.AvgDiskQueueLength) & ";" & _
								 "spAvgReadQueueLen:" & CStr(objItem.AvgReadQueueLength) & ";" & _
								 "spAvgWriteQueueLen:" & CStr(objItem.AvgWriteQueueLength) & ";" & _
								 "spDiskReadPerSec:" & CStr(objItem.DiskReadsPersec) & ";" & _
								 "spDiskWritePerSec:" & CStr(objItem.DiskWritesPersec)
		End If
	Next
End Function

'---------------------------------------------------------------------

Function GetProcessorUtilization
	For Each objItem in objCPUData
		If objItem.Name = "_Total" Then
			Wscript.Echo "Percent Processor Time: " & objItem.PercentProcessorTime
			
			GetProcessorUtilization = "spCPU:" & CStr(objItem.PercentProcessorTime)
		End If
	Next
End Function

'---------------------------------------------------------------------

Function GetMemoryUtilization
	For Each objItem In objMemoryData
		WScript.Echo "Available Memory: " & objItem.AvailableMBytes
		
		GetMemoryUtilization = "spMemory:" & CStr(objItem.AvailableMBytes) & "MB" & ";" & _
						     "spCacheFaultPerSec:" & CStr(objItem.CacheFaultsPerSec) & ";" & _		
						     "spPagesPerSec:" & CStr(objItem.PagesPerSec)
	Next
End Function

'---------------------------------------------------------------------

Function GetASPNETUtilization
	For Each objItem in objASPNETData
		Wscript.Echo "Total Requests: " & objItem.RequestsTotal
		Wscript.Echo "Requests in Queue: " & objItem.RequestsInApplicationQueue
		Wscript.Echo "Requests Wait Time: " & objItem.RequestsWaitTime
		Wscript.Echo "Requests Rejected: " & objItem.RequestsRejected
		
		GetASPNETUtilization = "spRequestsTotal:" & CStr(objItem.RequestsTotal) & ";" & _
							 "spRequestsQueued:" & CStr(objItem.RequestsInApplicationQueue) & ";" & _
							 "spRequestWaitTime:" & CStr(objItem.RequestsWaitTime) & ";" & _
							 "spRequestsRejected:" & CStr(objItem.RequestsRejected)
	Next
End Function

'---------------------------------------------------------------------

Sub AddResult
  url = "http://www.monitis.com/customMonitorApi"
  action = "addResult"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=" + action + "&monitorId=" + MonitorID + "&checktime=" + UnixDate + "&results=" + Results
 ' wscript.echo "Add monitor result"
 ' wscript.echo "POST: " + url
 ' wscript.echo "Data: " + postData
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
  Set oRes = objSPWMI.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
  For each oEntry in oRes
    GMTDate = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), GMTDate)
  Next
End function

'------------------------------------------------------------------------------------

Function GetMultiString_FromArray( ArrayString, Seprator)
     If IsNull ( ArrayString ) Then
         StrMultiArray = ArrayString
     else
         StrMultiArray = Join( ArrayString, Seprator )
    end if
    GetMultiString_FromArray = StrMultiArray
    
End Function
