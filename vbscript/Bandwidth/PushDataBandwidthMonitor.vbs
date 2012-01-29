'****************************************************************************************
' PushDataBandwidthMonitor.vbs
'
' Post network data to the custom monitor on the Monitis dashboard. 
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
Dim objNetworkData		'WMI Network Adapter counters

Dim TotalBytesReceivedPerSec
Dim TotalBytesSentPerSec
Dim TotalBytesTotalPerSec
Dim TotalPacketsReceivedPerSec
Dim TotalPacketsSentPerSec
Dim TotalPacketsReceivedDiscarded
Dim TotalPacketsReceivedError
Dim TotalPacketsSentDiscarded

'Initialize the API and Secret Key
apiKey = "M054V2B2R3PUTOPUQP0ETLCOV" 		'Replace with your apiKey
secretKey = "2SHGFRICE40OS7TEC4V44443KO" 	'Replace with your secretKey
apiUrl = "sandbox.monitis.com"				'Replace with "www.monitos.com"

'Initialize monitor variables
Computer = "."
MonitorTag = "NetworkBandwidth"

'Initialize WMI
Set objWMIService = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + computer + "\root\cimv2")

'Create the performance counter objects
Set objRefresher = CreateObject("WbemScripting.SWbemRefresher")

WScript.Echo "Adding Network Performance Object..."
Set objNetworkData = objRefresher.AddEnum(objWMIService, "Win32_PerfFormattedData_Tcpip_NetworkInterface").objectSet
objRefresher.Refresh

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")

'Request a token to use in following calls
url = "http://" & apiUrl & "/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey + "&version=2"
objHTTP.open "GET", url, False
objHTTP.send
resp = objHTTP.responseText
token = DissectStr(resp, "authToken"":""", """")

'Requests the monitor list so we can find the monitorId of each printer monitor on the dashboard page
url = "http://" & apiUrl & "/customMonitorApi?action=getMonitors&apikey=" + apiKey + "&tag=" + MonitorTag + "&output=xml"
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

	'Refresh the performance counters
	objRefresher.Refresh
    
	'Get performance data
	GetNetworkData

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
Set objNetworkData = Nothing

'---------------------------------------------------------------------
' FUNCTIONS
'---------------------------------------------------------------------

Function GetNetworkData

	'Clear total counters
	TotalBytesReceivedPerSec = 0
	TotalBytesSentPerSec = 0
	TotalBytesTotalPerSec = 0
	TotalPacketsReceivedPerSec = 0
	TotalPacketsSentPerSec = 0
	TotalPacketsReceivedDiscarded = 0
	TotalPacketsReceivedError = 0
	TotalPacketsSentDiscarded = 0

	Set colNetAdapters = objWMIService.ExecQuery("Select * From Win32_NetworkAdapter") ' WHERE Caption = '" & objConf.Caption & "'")
	For Each objAdapter In colNetAdapters

		strAdapterName = GetAdapterName(objAdapter.Name)
		
		'Handle the bandwidth and bytes counters
		MonitorName = "Bytes Throughput: " + objAdapter.NetConnectionID
		MonitorID = FindMonitorID(MonitorName)
		
		If Trim(MonitorID) <> "" Then
			For Each objItem in objNetworkData
				strPerfDataName = GetAdapterName(objItem.Name)
				
				WScript.Echo strPerfDataName & " - " & GetAdapterName(objItem.Name)
				
				If strPerfDataName = strAdapterName Then
					Wscript.echo "Name: " & strPerfDataName
					Wscript.Echo "Bytes Received/Sec: " & objItem.BytesReceivedPersec
					Wscript.Echo "Bytes Sent/Sec: " & objItem.BytesSentPersec
					Wscript.Echo "Bytes Total/Sec: " & objItem.BytesTotalPersec
					
					Results = "BytesTotalPerSec:" & CStr(objItem.BytesTotalPerSec) & _
							  ";BytesReceivedPerSec:" & CStr(objItem.BytesReceivedPerSec) & _		
							  ";BytesSentPerSec:" & CStr(objItem.BytesSentPerSec)		
					AddResult
				
					'Accumulate Totals
					TotalBytesReceivedPerSec = TotalBytesReceivedPerSec + objItem.BytesReceivedPerSec
					TotalBytesSentPerSec = TotalBytesSentPerSec + objItem.BytesSentPerSec
					TotalBytesTotalPerSec = TotalBytesTotalPerSec + objItem.BytesTotalPerSec
				End If
			Next
		End If
		
		'Handle the packet counters
		MonitorName = "Packets: " & objAdapter.NetConnectionID
		MonitorID = FindMonitorID(MonitorName)
		If Trim(MonitorID) <> "" Then
			For Each objItem in objNetworkData
				strPerfDataName = GetAdapterName(objItem.Name)
				
				WScript.Echo strPerfDataName & " - " & GetAdapterName(objItem.Name)
				
				If strPerfDataName = strAdapterName Then
				
					Wscript.echo "Name: " & strPerfDataName
					Wscript.Echo "Packets Received/Sec: " & objItem.PacketsReceivedPersec
					Wscript.Echo "Packets Sent/Sec : " & objItem.PacketsSentPersec
					
					Results = "PacketsReceivedPerSec:" & CStr(objItem.PacketsReceivedPersec) & _
							  ";PacketsSentPerSec:" & CStr(objItem.PacketsSentPersec) & _
							  ";PacketsReceivedDiscarded:" & CStr(objItem.PacketsReceivedDiscarded) & _
							  ";PacketsReceivedError:" & CStr(objItem.PacketsReceivedErrors) & _
							  ";TotalPacketsPerSec:" & CStr(objItem.PacketsPerSec)
					AddResult
					
					'Accumulate Totals
					TotalPacketsReceivedPerSec = TotalPacketsReceivedPerSec + objItem.PacketsReceivedPersec
					TotalPacketsSentPerSec = TotalPacketsSentPerSec + objItem.PacketsSentPersec
					TotalPacketsReceivedDiscarded = TotalPacketsReceivedDiscarded + objItem.PacketsReceivedDiscarded
					TotalPacketsReceivedError = TotalPacketsReceivedError + objItem.PacketsReceivedErrors
					TotalPacketsPerSec = TotalPacketsPerSec + objItem.PacketsPerSec
					
				End If
			Next
		End If
		
	Next


	'Add Total results to the monitor
	MonitorName = "Total Bytes Throughput"
	MonitorID = FindMonitorID(MonitorName)
	If Trim(MonitorID) <> "" Then
		Results = "TotalBytesTotalPerSec:" & CStr(TotalBytesTotalPerSec) & _
				  ";TotalBytesReceivedPerSec:" & CStr(TotalBytesReceivedPerSec) & _		
				  ";TotalBytesSentPerSec:" & CStr(TotalBytesSentPerSec)	
		AddResult
	End If

	MonitorName = "Total Packets"
	MonitorID = FindMonitorID(MonitorName)
	If Trim(MonitorID) <> "" Then
		Results = "TotalPacketsReceived:" & CStr(TotalPacketsReceivedPerSec) & _
				  ";TotalPacketsSent:" & CStr(TotalPacketsSentPerSec) & _
				  ";TotalPacketsReceivedDiscarded:" & CStr(TotalPacketsReceivedDiscarded) & _
				  ";TotalPacketsReceivedErrors:" & CStr(TotalPacketsReceivedError) & _
				  ";TotalPacketsPerSec:" & CStr(TotalPacketsPerSec)
		AddResult
	End If		

End Function

'---------------------------------------------------------------------

Sub AddResult
  url = "http://" & apiUrl & "/customMonitorApi"
  action = "addResult"
  objHTTP.open "POST", url, False
  objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
  postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=" + action + "&monitorId=" + MonitorID + "&checktime=" + UnixDate + "&results=" + Results
  'wscript.echo "Add monitor result"
  'wscript.echo "POST: " + url
  'wscript.echo "Data: " + postData
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
