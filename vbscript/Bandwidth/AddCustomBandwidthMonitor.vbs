'****************************************************************************************
' AddCustomBandwidthMonitor.vbs
'
' Ard-Jan Barnas
'****************************************************************************************

Dim apiKey				'apiKey for Monitis. Replace with your assigned API key
Dim secretKey			'secretKey for Monitis API. Replace with your assigned Secret key
Dim token				'Monitis authToken
Dim strComputer			'Local or remote computer name to connect to
Dim dtGMT				'GMT Time variable
Dim objHTTP				'HTTP object to submit POST/GET to Monitis
Dim objWMI				'WMI object
Dim objTimeStamp		'WMI Locate Date and Time object
Dim MonitorPage			'Name of the page we're adding to the dashboard
Dim MonitorPageID		'PageID returned by the API used for futher calls 
Dim PageTitle			'Title to appear on the page tabs
Dim MonitorGroup		'Group identifier
Dim MonitorName			'Name the monitor
Dim MonitorTag			'Monitor identifier tag
Dim Row					'Current row in the dashboard page
Dim Column				'Current column in the dashboard page

'Initialize the API and Secret Key
apiKey = "M054V2B2R3PUTOPUQP0ETLCOV" 		'Replace with your apiKey
secretKey = "2SHGFRICE40OS7TEC4V44443KO" 	'Replace with your secretKey
apiUrl = "sandbox.monitis.com"				'Replace with "www.monitos.com"

'Specify the monitor tag we're working with
strComputer = "."
MonitorPage = "NetworkBandwidth"
PageTitle = "Bandwidth"
MonitorGroup = "NetworkBandwidth"
PageColumns = "2"
MonitorTag = "NetworkBandwidth"

'Initialize WMI. Use "." for local computer, otherwise specify the remote computer name
Set objWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + strComputer + "\root\cimv2")
 
'Get current local date and time using the timezone from the obtained GMT date
dtGMT = GMTDate()

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")
 
'Request a token to use in following calls
url = "http://" & apiUrl & "/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey + "&version=2"
objHTTP.open "GET", url, False
objHTTP.send
resp = objHTTP.responseText
token = DissectStr(resp, "authToken"":""", """")

'Add a page to the dashboard
wscript.echo "Adding dashboard page"
url = "http://" & apiUrl & "/api"
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"

postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addPage&title=" + PageTitle + "&columnCount=" + PageColumns
objHTTP.send postData
resp = objHTTP.responseText

MonitorPageID = DissectStr(resp, "pageId"":", "}")
wscript.echo "PageID: " + MonitorPageID

'----------------------------------------------------------------------
' Build the monitor
'----------------------------------------------------------------------

'Total Bytes Troughput
Row = 1 : Column = 1
MonitorName = "Total+Bytes+Throughput"
monitorParams = "Total+Bytes+Throughput:Bandwidth:Bandwidth:3:false;"
resultParams = "TotalBytesReceivedPerSec:Received/Sec:N%2FA:2;TotalBytesSentPerSec:Sent/Sec:N%2FA:2;TotalBytesTotalPerSec:Total/Sec:N%2FA:2;"
AddCustMon

'Total Packets/Sec
Row = 1 : Column = 2
MonitorName = "Total+Packets"
monitorParams = "Total+PacketsPerSec:Packets:Packets/Sec:3:false;"
resultParams = "TotalPacketsReceived:Received/Sec:N%2FA:2;TotalPacketsSent:Sent/Sec:N%2FA:2;TotalPacketsPerSec:Packets/Sec:N%2FA:2;TotalPacketsReceivedDiscarded:Discarded:N%2FA:2;TotalPacketsReceivedError:Error:N%2FA:2;"
AddCustMon

'Network Monitor
Row = 2 : Column = 1
AddNetworkAdapterMonitors

'----------------------------------------------------------------------
'Cleanup
'----------------------------------------------------------------------
Set objHTTP = Nothing
Set objWMI = Nothing
Set objRes = Nothing
Set objTimeStamp = Nothing

'---------------------------------------------------------------------
' FUNCTIONS
'---------------------------------------------------------------------

Sub AddNetworkAdapterMonitors
	Set colConfs = objWMI.ExecQuery("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = True")
	For Each objConf in colConfs 
	
		strAdapterName = GetAdapterName(objConf.Caption)
		strIPAddress = GetMultiString_FromArray(objConf.IPAddress, ", ")

		Set colNetAdapters = objWMI.ExecQuery("Select * From Win32_NetworkAdapter WHERE Caption = '" & objConf.Caption & "'")
		For Each objAdapter In colNetAdapters
		
			WScript.Echo "Adding monitor for network connection: " & objAdapter.NetConnectionID

			'Bandwidth and Bytes
			Column = 1
			MonitorName = "Bytes+Throughput:+" & Replace(objAdapter.NetConnectionID, " ", "+")
			monitorParams = "Bandwidth:Bytes+Throughput:Bytes+Throughput:3:false;"
			resultParams = "BytesReceivedPerSec:Received/Sec:N%2FA:2;BytesSentPerSec:Sent/Sec:N%2FA:2;BytesTotalPerSec:Total/Sec:N%2FA:2;"
			AddCustMon
			
			'Packets/Sec
			Column = 2
			MonitorName = "Packets:+" & Replace(objAdapter.NetConnectionID, " ", "+")
			monitorParams = "PacketsPerSec:Packets:Packets/Sec:3:false;"
			resultParams = "PacketsReceivedPerSec:Received/Sec:N%2FA:2;PacketsSentPerSec:Sent/Sec:N%2FA:2;TotalPacketsPerSec:Packets/Sec:N%2FA:2;PacketsReceivedDiscarded:Discarded:N%2FA:2;PacketsReceivedError:Error:N%2FA:2;"
			AddCustMon
			
			'Make sure we add the monitor on the correct row and column
			Row = Row + 1
		Next
	Next
End Sub

'---------------------------------------------------------------------

Sub AddCustMon
	Dim postData
	
	WScript.Echo "MonitorName: " & MonitorName
	WScript.Echo "MonitorTag: " & MonitorTag
	
	url = "http://" & apiUrl & "/customMonitorApi"
	postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addMonitor" + "&monitorParams=" + monitorParams + "&resultParams=" + resultParams + "&name=" + MonitorName + "&tag=" + MonitorTag
	wscript.echo "Adding custom monitor"
	wscript.echo "POST: " + url
	wscript.echo "Data: " + postData
	
	objHTTP.open "POST", url, False
	objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	objHTTP.send postData
	resp = objHTTP.responseText
	DataModuleID = DissectStr(resp, "data"":", "}")

	wscript.echo "Result: " + resp
	wscript.echo "DataModuleID: " + DataModuleID
	wscript.echo "Adding " + MonitorName + " to the page"
	
	url = "http://" & apiUrl & "/api"
	postData = "apikey=" & apiKey & "&validation=token&authToken=" & token & "&timestamp=" & FmtDate(dtGMT) & "&action=addPageModule&moduleName=CustomMonitor&pageId=" & MonitorPageID & "&column=" & Column & "&row=" & Row & "&dataModuleId=" & DataModuleID
	objHTTP.open "POST", url, False
	objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	objHTTP.send postData
	resp = objHTTP.responseText
	wscript.echo "Result: " + resp
End Sub

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

'------------------------------------------------------------------------------------

Function GetAdapterName(strName)
	Dim strTemp
	strTemp = strName

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
