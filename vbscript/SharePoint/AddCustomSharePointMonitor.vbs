'****************************************************************************************
' AddCustomSharePointMonitor.vbs
'****************************************************************************************

Dim apiKey				'apiKey for Monitis. Replace with your assigned API key
Dim secretKey			'secretKey for Monitis API. Replace with your assigned Secret key
Dim token				'Monitis authToken
Dim strSPServer			'Name of the SharePoint server
Dim strSQLServer		'Name of the SQL Server for SharePoint's databases
Dim dtGMT				'GMT Time variable
Dim objHTTP				'HTTP object to submit POST/GET to Monitis
Dim objSPWMI			'WMI object for SharePoint Server
Dim objSQLWMI			'WMI object for SQL Server
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
apiKey = "AVUU86JKUKERMF4LUF15RAAPS"		'REPLACE WITH YOUR API KEY 
secretKey = "1S29IOT3MOQNJUCL2MOEO76SN4" 	'REPLACE WITH YOUR SECRET KEY

'Specify the monitor tag we're working with
strSPServer = "."							'REPLACE WITH YOUR SHAREPOINT SERVERNAME
strSQLServer = "."							'REPLACE WITH YOUR SQL SERVERNAME
MonitorPage = "SharePoint"
PageTitle = "SharePoint"
MonitorGroup = "SharePoint"
PageColumns = "2"
MonitorName = "SharePoint"
MonitorTag = "SharePoint"

'Initialize WMI. Use "." for local computer, otherwise specify the remote computer name
Set objSPWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + strSPServer + "\root\cimv2")
Set objSQLWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" + strSQLServer + "\root\cimv2")
 
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

'SharePoint CPU Subsystem monitor
Row = 1 : Column = 1
MonitorName = "SP+CPUSubsystem"
monitorParams = "spCPUSubsystem:SPCPUSubsystem:SP+CPU+Subsystem:3:false;"
resultParams = "spCPU:Processor+Time:N%2FA:2;"
AddCustMon

'SharePoint Memory Subsystem monitor
Row = 1 : Column = 2
MonitorName = "SP+MemorySubsystem"
monitorParams = "spMemorySubsystem:SPMemorySubsystems:SP+Memory+Subsystem:3:false;"
resultParams = "spMemory:Available+Memory:N%2FA:2;spCacheFaultPerSec:Cache+Fault+PerSec:N%2FA:2;spPagesPerSec:Pages+PerSec:N%2FA:2;"
AddCustMon

'SharePoint Disk subsystem monitor
Row = 2 : Column = 1
MonitorName = "SP+Disk+Monitor"
monitorParams = "spAvgQueueLen:spAVGQueueLen:Avg+Queue+Length:3:false;"
resultParams = "spAvgQueueLen:AvgQueueLen:N%2FA:2;spAvgReadQueueLen:ReadQueueLen:N%2FA:2;spAvgWriteQueueLen:WriteQueueLen:N%2FA:2;spDiskReadPerSec:Reads/Sec:N%2FA:2;spDiskWritePerSec:Writes/Sec:N%2FA:2;"
AddCustMon

'SharePoint ASP.NET monitor
Row = 2 : Column = 2
MonitorName = "SP+ASPNET+Monitor"
monitorParams = "spAspNet:SPAspNet:ASP.NET:3:false;"
resultParams = "spRequestsTotal:RequestsTotal:N%2FA:2;spRequestsQueued:RequestsQueued:N%2FA:2;spRequestWaitTime:RequestWaitTime:N%2FA:2;spRequestsRejected:RequestsRejected:N%2FA:2;"
AddCustMon

'SQL Monitor
Row = 3 : Column = 1
MonitorName = "SPSQL+Monitor"
monitorParams = "spSQL:SPSQL:SP+SQL:3:false;"
resultParams = "users:Users:N%2FA:2;hitratio:HitRatio:N%2FA:2;"
AddCustMon

'----------------------------------------------------------------------
'Cleanup
'----------------------------------------------------------------------
Set objHTTP = Nothing
Set objSQLWMI = Nothing
Set objSPWMI = Nothing
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
