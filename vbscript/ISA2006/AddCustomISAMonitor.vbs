'****************************************************************************************
' AddCustomIsaMonitor.vbs
'
' NOTE: This script supports ISA Server 2006 only
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
apiKey = "AVUU86JKUKERMF4LUF15RAAPS" 
secretKey = "1S29IOT3MOQNJUCL2MOEO76SN4" 

'Specify the monitor tag we're working with
strComputer = "."
MonitorPage = "ISA+Server"
PageTitle = "ISA+Server"
MonitorGroup = "isaServer"
PageColumns = "1"
MonitorName = "isaServer"
MonitorTag = "IsaServer"

'Initialize WMI. Use "." for local computer, otherwise specify the remote computer name
Set objWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + strComputer + "\root\cimv2")
 
'Get current local date and time using the timezone from the obtained GMT date
dtGMT = GMTDate()

'Initialize HTTP connection object
Set objHTTP = CreateObject("Microsoft.XMLHTTP")
 
'Request a token to use in following calls
url = "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey + "&version=2"
objHTTP.open "GET", url, False
objHTTP.send
resp = objHTTP.responseText
token = DissectStr(resp, "authToken"":""", """")

'Add a page to the dashboard
wscript.echo "Adding dashboard page"
url = "http://www.monitis.com/api"
objHTTP.open "POST", url, False
objHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"

postData = "apikey=" + apiKey + "&validation=token&authToken=" + token + "&timestamp=" + FmtDate(dtGMT) + "&action=addPage&title=" + PageTitle + "&columnCount=" + PageColumns
objHTTP.send postData
resp = objHTTP.responseText

MonitorPageID = DissectStr(resp, "pageId"":", "}")
wscript.echo "PageID: " + MonitorPageID

'----------------------------------------------------------------------
' Build the actual monitor
'----------------------------------------------------------------------

'ISA Services to monitor
MonitorName = "ISA+Services" : Row = 1 : Column = 1
monitorParams = "isa:ISAServices:ISA+Service+Status:3:false;"
resultParams = "isaFirewall:Firewall:N%2FA:2;isaSrvCtrl:Server+Control:N%2FA:2;isaJobSched:Job+Scheduler:N%2FA:2;isaSrvStorage:Server+Storage:N%2FA:2;"
AddCustMon

'Packets
MonitorName = "ISA+Packets+Monitor" : Row = 2 : Column = 1
monitorParams = "packets:Packets:Packets:3:false;"
resultParams = "totalPackets:Total Packets:N%2FA:2;passedPerSec:Packets/Sec:N%2FA:2;" & _
		       "allowedPackets:Allowed:N%2FA:2;allowedPerSec:Allowed/Sec:N%2FA:2;" & _
		       "droppedPackets:Dropped:N%2FA:2;droppedPerSec:Dropped/Sec:N%2FA:2;" & _
		       "backloggedPackets:BackLogged:N%2FA:2;"
AddCustMon

'Active Connections
MonitorName = "ISA+TCP+Connections+Monitor" : Row = 3 : Column = 1
monitorParams = "TCPConnections:TCP Connections:TCP Connections:3:false;"
resultParams = "active:Total Active:N%2FA:2;activePerSec:Active/Sec:N%2FA:2;established:Established/Sec:N%2FA:2;"
AddCustMon

'Bytes passed through ISA Server
MonitorName = "ISA+Throughput+in+Bytes+Monitor" : Row = 4 : Column = 1
monitorParams = "Bytes:BytesPassedThrough:Bytes Passed Through:3:false;"
resultParams = "totalPassed:Total Bytes Passed:N%2FA:2;passedPerSec:Passed/Sec:N%2FA:2;"
AddCustMon



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

Sub AddCustMon
	Dim postData
	
	url = "http://www.monitis.com/customMonitorApi"
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
	
	url = "http://www.monitis.com/api"
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

