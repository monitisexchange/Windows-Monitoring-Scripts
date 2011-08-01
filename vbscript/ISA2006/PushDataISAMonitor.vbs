'****************************************************************************************
' PushDataISAMonitor.vbs
'
' Post ISA data to the custom ISA monitor on the Monitis dashboard. 
'****************************************************************************************
Dim apiKey				'apiKey for Monitis. Replace with your assigned API key
Dim secretKey			'secretKey for Monitis API. Replace with your assigned Secret key
Dim token				'Monitis authToken
Dim Computer			'Local or remote computer name to connect to
Dim dtGMT				'GMT Time variable
Dim UnixDate			'Unix date format
Dim objHTTP				'HTTP object to submit POST/GET to Monitis
Dim objWMIService		'WMI object
Dim objResponse			'Response object
Dim objTimeStamp		'WMI Locate Date and Time object
Dim PageTitle			'Title to appear on the page tabs
Dim MonitorID			'Monitor identifier
Dim MonitorName			'Name the monitor
Dim MonitorTag			'Monitor identifier tag
Dim Results				'Values to be pushed to the monitor
Dim ISAversion			'Supported version of ISA server
Dim objDiskData			'WMI Disk counters
Dim objCPUData			'WMI CPU counters
Dim objNetworkData		'WMI Network Adapter counters
Dim objMemoryData		'WMI Memory counters


'Initialize the API and Secret Key
apiKey = "AVUU86JKUKERMF4LUF15RAAPS" 
secretKey = "1S29IOT3MOQNJUCL2MOEO76SN4" 

'Initialize monitor variables
Computer = "."
ISAversion = "2006"
MonitorTag = "isaServer"

'Initialize WMI
Set objWMIService = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + computer + "\root\cimv2")

'Create the performance counter objects
Set objRefresher = CreateObject("WbemScripting.SWbemRefresher")

WScript.Echo "Adding Disk Performance Object..."
Set objDiskData = objRefresher.AddEnum(objWMIService, "Win32_PerfFormattedData_PerfDisk_PhysicalDisk").objectSet
objRefresher.Refresh

WScript.Echo "Adding Processot Performance Object..."
Set objCPUData = objRefresher.AddEnum(objWMIService, "Win32_PerfFormattedData_PerfOS_Processor").objectSet
objRefresher.Refresh

WScript.Echo "Adding Memory Performance Object..."
Set objMemoryData = objRefresher.AddEnum(objWMIService, "Win32_PerfFormattedData_PerfOS_Memory").ObjectSet
objRefresher.Refresh

WScript.Echo "Adding Network Performance Object..."
Set objNetworkData = objRefresher.AddEnum(objWMIService, "Win32_PerfFormattedData_Tcpip_NetworkInterface").objectSet
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
	'GetISAServicesStatus
	'GetISASubsystems
	GetNetworkUtilization

	'Wait 30 seconds
	WScript.Sleep(3000)
Loop


'Cleanup
Set objHTTP = Nothing
Set objWMIService = Nothing
Set objRes = Nothing
Set objTimeStamp = Nothing
Set objResponse = Nothing
Set objRefresher = Nothing
Set objDiskData = Nothing
Set objMemoryData = Nothing
Set objCPUData = Nothing
Set objNetworkData = Nothing

'---------------------------------------------------------------------
' FUNCTIONS
'---------------------------------------------------------------------

Function GetISAServicesStatus
	On Error Resume Next
    Dim objServices, intResult

    If(ISAVersion <> 2006) Then
    	WScript.Echo "Unsupported Version"
        Exit Function
    End If

     ' Get the services list
    Set objServices = objWMIService.ExecQuery("Select * from Win32_Service")
    If (Err.Number <> 0) Then
        WScript.Echo "Unable to Query WMI"
        Exit Function
    End If
    
    If (objServices.Count <= 0) Then
        WScript.Echo "Win32_Service class does not exist"
        Exit Function
    End If 
     
	MonitorName = "ISA Services"
	MonitorID = FindMonitorID(MonitorName)
	WScript.Echo "Monitor ID: " & MonitorID
	
	If Trim(MonitorID) <> "" Then
		Results = "isaSrvCtrl:" & ISAServiceState(objServices, "isactrl", "Microsoft ISA Server Control") & _
		 	      ";isaFirewall:" & ISAServiceState(objServices, "fwsrv", "Microsoft Firewall") & _
				  ";isaSrvStorage:" & ISAServiceState(objServices, "isastg", "Microsoft ISA Server Storage") & _
				  ";isaJobSched:" & ISAServiceState(objServices, "isasched", "Microsoft ISA Server Job Scheduler")
	
		AddResult
	End If
	
    On Error Goto 0
End Function

'---------------------------------------------------------------------

Function GetNetworkUtilization

	Set colNetAdapters = objWMIService.ExecQuery("Select * From Win32_NetworkAdapter") ' WHERE Caption = '" & objConf.Caption & "'")
	For Each objAdapter In colNetAdapters

		strAdapterName = GetAdapterName(objAdapter.Name)
		MonitorName = "ISA Network Adapter: " & objAdapter.NetConnectionID
		MonitorID = FindMonitorID(MonitorName)
		
		If Trim(MonitorID) <> "" Then
		
			For Each objItem in objNetworkData
	
				strPerfDataName = GetAdapterName(objItem.Name)
				If strPerfDataName = strAdapterName Then
					Wscript.echo "Name: " & strPerfDataName
					Wscript.Echo "Bytes Received/Sec: " & objItem.BytesReceivedPersec
					Wscript.Echo "Bytes Sent/Sec: " & objItem.BytesSentPersec
					Wscript.Echo "Packets Received/Sec: " & objItem.PacketsReceivedPersec
					Wscript.Echo "Packets Sent/Sec : " & objItem.PacketsSentPersec
				
					Results = "isaNetBytesSentPersec:" & CStr(objItem.BytesSentPersec) & _
							  "isaNetBytesReceivedPersec:" & CStr(objItem.BytesReceivedPersec) & _
							  "isaNetPacketsSentPersec:" & CStr(objItem.PacketsSentPersec) & _
							  "isaNetPacketsReceivedPersec:" & CStr(objItem.PacketsReceivedPersec)
				
					AddResult
				End If
			Next
		
		End If
	Next

End Function

'---------------------------------------------------------------------

Function GetISASubsystems

	MonitorName = "ISA Subsystems"
	MonitorID = FindMonitorID(MonitorName)
			 
	If Trim(MonitorID) <> "" Then
		Results = GetProcessorUtilization() & ";" & _
				  GetAvailableMemory() & ";" & _
				  GetDiskUtilization()
				 
		AddResult
	End If
				 
End Function

'---------------------------------------------------------------------

Function GetDiskUtilization
	For Each objItem in objDiskData
		If objItem.Name = "_Total" Then
			WScript.Echo "Name: " & objItem.Name
			Wscript.Echo "Disk Transfers/sec: " & objItem.DiskTransfersPersec
			Wscript.Echo "Disk Reads/sec: " & objItem.DiskReadsPersec
			Wscript.Echo "Disk Writes/sec: " & objItem.DiskWritesPersec
			Wscript.Echo "Disk Bytes/sec: " & objItem.DiskBytesPersec
			Wscript.Echo "Disk Read Bytes/sec: " & objItem.DiskReadBytesPersec
			Wscript.Echo "Disk Avg Bytes/transfer: " & objItem.AvgDiskBytesPerTransfer
			
			GetDiskUtilization = "isaDiskUtil:" & CStr(objItem.DiskTransfersPersec)
		End If
	Next
End Function

'---------------------------------------------------------------------

Function GetProcessorUtilization
	For Each objItem in objCPUData
		If objItem.Name = "_Total" Then
			Wscript.Echo "Percent DPC Time: " & objItem.PercentDPCTime
			Wscript.Echo "Percent Privileged Time: " & objItem.PercentPrivilegedTime
			Wscript.Echo "Percent Processor Time: " & objItem.PercentProcessorTime
			Wscript.Echo "Percent User Time: " & objItem.PercentUserTime
			
			GetProcessorUtilization = "isaCPU:" & CStr(objItem.PercentProcessorTime)
		End If
	Next
End Function

'---------------------------------------------------------------------

Function GetAvailableMemory
	For Each objItem In objMemoryData
		WScript.Echo "Available Memory: " & objItem.AvailableMBytes
		
		GetAvailableMemory = "isaMemory:" & CStr(objItem.AvailableMBytes) & "MB"
	Next
End Function

'---------------------------------------------------------------------

Function ISAServiceState(lstServices, strServiceName, strServiceDescription)
    Dim objService
    ISAServiceState = "Not Installed"

    For Each objService in lstServices			
		
        If(Err.Number <> 0) Then
            WScript.Echo "Error" 
            Exit Function
        End If	 
	   
        ' Check If this is the service we are looking for
        If (LCase(objService.Name) = LCase(strServiceName)) Then	
        	WScript.Echo objService.Name & ": " & objService.State			
            ISAServiceState = objService.State
            Exit Function
        End If
    Next
  
End Function

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
  Set oRes = objWMIService.ExecQuery("Select LocalDateTime from Win32_OperatingSystem")
  For each oEntry in oRes
    GMTDate = DateAdd("n", -CInt(right(oEntry.LocalDateTime, 4)), GMTDate)
  Next
End function

'------------------------------------------------------------------------------------

Function GetAdapterName(strName)
	Dim strTemp
	strTemp = strName
	
	'If InStr(strTemp, "]") Then
	'	strTemp = Trim(Mid(strTemp, InStr(strTemp, "]")+1))
	'End If

	strTemp = Replace(strTemp, "[", "")
	strTemp = Replace(strTemp, "]", "")
	strTemp = Replace(strTemp, "(", "")
	strTemp = Replace(strTemp, ")", "")
	strTemp = Replace(strTemp, "/", " ")
	strTemp = Replace(strTemp, "\", " ")
	strTemp = Replace(strTemp, "_", " ")
	strTemp = Replace(strTemp, "-", " ")
	
	GetAdapterName = strTemp
End Function

'------------------------------------------------------------------------------------

Function GetMultiString_FromArray( ArrayString, Seprator)
     If IsNull ( ArrayString ) Then
         StrMultiArray = ArrayString
     else
         StrMultiArray = Join( ArrayString, Seprator )
    end if
    GetMultiString_FromArray = StrMultiArray
    
End Function
