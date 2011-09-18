'Global variables
Dim cmd, agentType
Dim objHttp
Dim SupportedMonitors
Dim InternalAgents

'Setup dictionaries
Set InternalAgents = CreateObject("Scripting.Dictionary")
Set SupportedMonitors = CreateObject("Scripting.Dictionary")
Set ShowMonitors = CreateObject("Scripting.Dictionary")

'Include section
Call Include("classes.vbs")
Call Include("funcString.vbs")
Call Include("funcDates.vbs")
Call Include("funcMonitisKeys.vbs")
Call Include("funcInternalAgents.vbs")

'Initialize supported monitors. Key specifies monitor and value Item specifies monitor field to return
SupportedMonitors.Add "cpu", "userValue"
SupportedMonitors.Add "memory", "freeMemory"
SupportedMonitors.Add "drive", "freeSpace"

'Command line arguments
argCmd = WScript.Arguments.Named.Item("cmd")
argAgentType = WScript.Arguments.Named.Item("type")
argMonitors = WScript.Arguments.Named.Item("monitors")
argApiKey = WScript.arguments.named.item("apiKey")
argSecretKey = WScript.arguments.named.item("secretKey")


Select Case LCase(argCmd)
	Case "help"
		ShowUsage("Command Line Help")
		
	Case "listagents"
		ListAgents argAgentType, argMonitors
		
	Case "setkeys"
		If Len(argApiKey) = 0 Then
			ShowUsage "Missing APIKey"
		ElseIf Len(secretKey) = 0 Then
			ShowUsage "Missing SecretKey"
		Else
			SetAuthenticationKeys argApiKey, argSecretKey
		End If
		
	Case "getkeys"
		GetAuthenticationKeys(True)
	
	Case Else
		ShowUsage "Missing command"
End Select

'-------------------------------------------------------------------------
'Cleanup
Set InternalAgents = Nothing
Set SupportedMonitors = Nothing
Set ShowMonitors = Nothing

'-------------------------------------------------------------------------

Sub ListAgents(agentType, monitors)
	'Initialize HTTP connection object
	Set objHttp = CreateObject("Microsoft.XMLHTTP")
	
	'Setup the keys and authentication token
	GetAuthenticationKeys False
	authToken = GetAuthToken(objHttp, apiKey, secretKey)
	
	'Process monitor(s) command line parameter
	If Len(monitors) = 0 Or LCase(monitors) = "all" Then
		For Each monitor In SupportedMonitors
			ShowMonitors.Add monitor, monitor
		Next
	Else
		tempList = Split(monitors, ",")
		For i = 0 To UBound(tempList)
			If SupportedMonitors.Exists(LCase(tempList(i))) And Not ShowMonitors.Exists(LCase(tempList(i))) Then
				ShowMonitors.Add tempList(i), tempList(i)
			End If
		Next
	End If	
	
	Select Case LCase(agentType)
	
		Case "internal"
			GetInternalAgents objHttp, InternalAgents, ShowMonitors
		
		Case "external"
			WScript.Echo "Not yet implemented"
		
		Case "all"
			GetInternalAgents objHttp, InternalAgents, ShowMonitors
		
		Case Else
			GetInternalAgents objHttp, InternalAgents, ShowMonitors

	End Select
	
	
	'Write the list of agents to the screen	
	For Each agent In InternalAgents.Items
		WScript.Echo "------------------------------------------------------"
		WScript.Echo " AGENT: " & agent.Name
		WScript.Echo "------------------------------------------------------"
		
		strHeader = ""
		For Each monitor In agent.MonitorList.Items
			strHeader = strHeader & format(monitor.Name, monitor.Width)
		Next
		WScript.Echo strHeader
		
		strRow = ""
		For Each monitor In agent.MonitorList.Items
			strRow = strRow & format(monitor.Result, monitor.Width)
		Next
		
		WScript.Echo strRow
		WScript.Echo ""
	Next
End Sub

'-------------------------------------------------------------------------

Sub ShowUsage(strError)
	WScript.Echo "Error: " & strError
	WScript.Echo "See usage examples below"
	WScript.Echo ""
	WScript.Echo "Show command line help:"
	WScript.Echo "/cmd:help"
	WScript.Echo ""
	WScript.Echo "View APIKey and SecretKey values:"
	WScript.Echo "/cmd:getkeys"
	WScript.Echo ""
	WScript.Echo "Set APIKey and SecretKey:"
	WScript.Echo "/cmd:setkeys /apikey:<apikey> /secretkey:<secretKey>"
	WScript.Echo ""
	WScript.Echo "Show monitor results for defined agents:"
	WScript.Echo "/cmd:listagents /type:<int>|<ext>|<all> [/monitors:<name>,<name>,..."
	WScript.Echo ""
	WScript.Echo "Example:"
	WScript.Echo "Show cpu and memory monitors for all internal agents"
	WScript.Echo "/cmd:listagents /type:int /monitors:cpu,memory"
End Sub

'-------------------------------------------------------------------------

Sub Include(strFilename)
	On Error Resume Next
	Dim oFSO, f, s

	Set oFSO = CreateObject("Scripting.FileSystemObject")
	If oFSO.FileExists(strFilename) Then
		Set f = oFSO.OpenTextFile(strFilename)
		s = f.ReadAll
		f.Close
		ExecuteGlobal s
	End If

	Set oFSO = Nothing
	Set f = Nothing
	On Error Goto 0
End Sub

