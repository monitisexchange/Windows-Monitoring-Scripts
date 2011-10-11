'Global variables
Dim cmd, agentType
Dim objHttp
Dim SupportedMonitors
Dim InternalAgents
Dim ShowMonitors
Dim ShowProcesses

'Setup dictionaries
Set InternalAgents = CreateObject("Scripting.Dictionary")
Set SupportedMonitors = CreateObject("Scripting.Dictionary")
Set ShowMonitors = CreateObject("Scripting.Dictionary")
Set ShowProcesses = CreateObject("Scripting.Dictionary")

'Include section
Call Include("classes.vbs")
Call Include("funcString.vbs")
Call Include("funcDates.vbs")
Call Include("funcMonitisKeys.vbs")
Call Include("funcInternalAgents.vbs")

'Initialize supported monitors. Key specifies monitor and value Item specifies monitor field to return
SupportedMonitors.Add "cpu", "userValue,%|kernelValue,%"
SupportedMonitors.Add "memory", "freeMemory,MB|totalMemory,MB"
SupportedMonitors.Add "drive", "usedSpace,MB|freeSpace,MB"
SupportedMonitors.Add "process", "memUsage,MB|cpuUsage,%"
SupportedMonitors.Add "http", "memUsage,MB|cpuUsage,%"

'Command line arguments
argCmd = WScript.Arguments.Named.Item("cmd")
argAgentType = WScript.Arguments.Named.Item("type")
argMonitors = WScript.Arguments.Named.Item("monitors")
argApiKey = WScript.arguments.named.item("apiKey")
argSecretKey = WScript.arguments.named.item("secretKey")
argProcesses = WScript.arguments.named.item("process")

'Main routine
Select Case LCase(argCmd)
	Case "help"
		ShowUsage("Command Line Help")
		
	Case "listagents"
		ListAgents argAgentType, argMonitors, argProcesses
		ShowInternalAgents
		
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
Set ShowProcesses = Nothing

'-------------------------------------------------------------------------

Sub ListAgents(agentType, argMonitors, argProcesses)
	'Initialize HTTP connection object
	Set objHttp = CreateObject("Microsoft.XMLHTTP")
	
	'Setup the keys and authentication token
	GetAuthenticationKeys False
	authToken = GetAuthToken(objHttp, apiKey, secretKey)
	
	'Process monitor(s) command line parameter
	If LCase(argMonitors) = "all" Then
		For Each monitor In SupportedMonitors
			ShowMonitors.Add monitor, monitor
		Next
	ElseIf Len(argMonitors) > 0 Then
		tempList = Split(argMonitors, ",")
		For i = 0 To UBound(tempList)
			If SupportedMonitors.Exists(LCase(tempList(i))) And Not ShowMonitors.Exists(LCase(tempList(i))) Then
				ShowMonitors.Add tempList(i), tempList(i)
			End If
		Next
	ElseIf Len(argProcesses) > 0 Then
		tempList = Split(argProcesses, ",")
		For i = 0 To UBound(tempList)
			If Not ShowProcesses.Exists(LCase(tempList(i))) Then
				ShowProcesses.Add tempList(i), tempList(i)
			End If
		Next
	Else
		ShowUsage "Error: you must specify a monitor or process"
		WScript.Quit
	End If	
	
	
	Select Case LCase(agentType)
		Case "int"
			GetInternalAgents objHttp, InternalAgents, ShowMonitors, ShowProcesses
		
		Case "ext"
			GetExternalMonitors objHttp, InternalAgents, ShowMonitors, ShowProcesses
		
		Case "all"
			GetInternalAgents objHttp, InternalAgents, ShowMonitors, ShowProcesses
		
		Case Else
			GetInternalAgents objHttp, InternalAgents, ShowMonitors, ShowProcesses
	End Select
	
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
	WScript.Echo "Show global monitor results for defined agents:"
	WScript.Echo "/cmd:listagents /type:<int>|<ext>|<all> [/monitors:<all><name>,<name>,...]"
	WScript.Echo ""
	WScript.Echo "Show monitor results for one or more specific processes:"
	WScript.Echo "/cmd:listagents /type:<int>|<ext>|<all> [/process:<name>,<name>,...]"
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



'************************************************************************
' VBSCRIPT: funcExternalMonitors
'
' Required include file(s): 
' classAgents.vbs
'************************************************************************

Function GetExternalMonitors(aObjHttp, aObjAgents, aShowMonitors, aShowProcesses)
	Dim objAgents, xmlAgents
	Set objAgents = CreateObject("Scripting.Dictionary")
	
	WScript.Echo "Acquiring internal agents..."
	
	'Retrieve the list of agents
	url = "http://www.monitis.com/api?apikey=" + apiKey + "&output=xml&version=2&action=tests"

	aObjHttp.open "GET", url, False
	aObjHttp.send
	
	WScript.Echo aObjHttp.responseText
	
	'Parse response
	Set xmlMonitors = CreateObject("Microsoft.XMLDOM")
	xmlMonitors.async = False
	xmlMonitors.LoadXML(aObjHttp.responseText)
	If xmlMonitors.parseError.errorCode <> 0 Then    
		wscript.Echo xmlMonitors.parseError.errorCode    
		wscript.Echo xmlMonitors.parseError.reason    
		wscript.Echo xmlMonitors.parseError.line
	End If  	
	
	'Retrieve the agent information for each agent
	For Each monitor in xmlMonitors.documentElement.childnodes

		WScript.Echo monitor.text
		WScript.Echo monitor.nodename

		'Create an InternalAgent Object	
		Set IntAgent = New class_InternalAgent
		'IntAgent.Id = monitor.selectSingleNode("id").text
		'IntAgent.Name = monitor.selectSingleNode("key").text

		url = "http://www.monitis.com/api?action=agentInfo&apikey=" + apiKey + "&output=xml&agentId=" + IntAgent.Id + "&loadTests=true"
		aObjHttp.open "GET", url, False
		aObjHttp.send

		WScript.Echo aObjHttp.responseText
	WScript.Quit
		
		'Retrieve the agent information 
		Set xmlAgentInfo = CreateObject("Microsoft.XMLDOM")
		xmlAgentInfo.async = False
		xmlAgentInfo.LoadXML(aObjHttp.responseText)
		
		For Each info in xmlAgentInfo.documentElement.childnodes
			If aShowProcesses.Count = 0 Then
				GetGlobalMonitors aObjHttp, info.childnodes, IntAgent, aShowMonitors
			Else
				GetProcessMonitors aObjHttp, info.childnodes, IntAgent, aShowProcesses
			End If
		Next
		
		'Add the agent object to the list of agents
		aObjAgents.Add IntAgent.Id, IntAgent
	Next
	
End Function

	
'-----------------------------------------------------------------------------------------------
	
Function GetMonitorValues(aObjHttp, oNames, aObjAgent, aShowProcesses)
	Dim oName
  
	For Each oName in oNames
	
		If oName.NodeName <> "#text" And _
    		LCase(oName.NodeName) = "process" And _
			SupportedMonitors.Exists(LCase(oName.NodeName)) Then
			
			'See if the current nodename contains the process we're looking for
			Found = False
			strCurrentProcess = ""
			For Each showProcess In aShowProcesses
				If InStr(LCase(oName.selectSingleNode("name").text), LCase(showProcess)) > 0 Then
					Found = True
					strCurrentProcess = showProcess
				End If
			Next
			
			If Found Then
				Set Monitor = New class_Monitor
				
				Monitor.Id = oName.selectSingleNode("id").text
				Monitor.Name = strCurrentProcess 
				Monitor.DisplayName = GetMonitorName(oName.selectSingleNode("name").text)
				
				dt = DateSerial(year(now), month(now), day(now))
				url = "http://www.monitis.com/api?apikey=" & apikey & "&output=xml&action=processResult&monitorId=" + Monitor.Id + "&timezone=" & timezone & "&day=" & day(dt) & "&month=" & month(dt) & "&year=" & year(dt)
				
				aObjHttp.open "GET", url, False
				aObjHttp.send
				Set oRes = CreateObject("Microsoft.XMLDOM")
				oRes.async = False
				oRes.LoadXML(aObjHttp.responseText)
				
				If Not oRes.firstchild.lastchild Is Nothing Then
					Set oNode = oRes.firstChild.lastChild
					GetResult oNode, Monitor, SupportedMonitors.Item("process")
					aObjAgent.MonitorList.Add Monitor.Name, Monitor
				End If
				
			End If
		End If 
	Next  
End Function

'-----------------------------------------------------------------------------------------------

Function GetExtResult(aNode, aMonitor, aFields)
	arrValues = Split(aFields, "|")
	For Each value In arrValues 
	
		arrDetails = Split(value,",")
		strValue = arrDetails(0)
		strSuffix = arrDetails(1)
	
		Set Metric = New class_Metric
		Metric.Name = strValue
			
		total = 0
		For Each oCell In aNode.ChildNodes
			If LCase(oCell.nodename) = LCase(strValue) Then
				total = CDbl(oCell.text)
			End If
		Next
	
		Metric.Result = CStr(total) & strSuffix
	 	aMonitor.MetricList.Add Metric.Name, Metric
	Next
	
End Function
		
'-----------------------------------------------------------------------------------------------
	
Sub ShowExternalMonitors	
	'Write the list of agents to the screen	
	For Each agent In InternalAgents.Items
		WScript.Echo "" 
		WScript.Echo "-------------------------------------------------------------------------------"
		WScript.Echo " AGENT: " & agent.Name
		WScript.Echo "-------------------------------------------------------------------------------"

		'Determine the width of the column for the display name of the monitor
		maxMonitorWidth = 0
		maxMetricWidth = 0
		For Each monitor In agent.MonitorList.Items
			If Len(Monitor.DisplayName) > maxMonitorWidth Then
				maxMonitorWidth = Len(Monitor.DisplayName)+4
			End If
			
			For Each objMetric In Monitor.MetricList.Items
				If Len(objMetric.Name) > maxMetricWidth Then
					maxMetricWidth = Len(objMetric.Name)+4
				End If
			Next
		Next
		
		'Build the output strings to show the monitor data
		strTemp = "*"
		strHeader = ""
		strRow = ""
		For Each monitor In agent.MonitorList.Items
			strHeader = format(UCase(Monitor.DisplayName), maxMonitorWidth)
			strRow = format(" ", maxMonitorWidth)
			
			For Each objMetric In Monitor.MetricList.Items
				strHeader = strHeader & format(objMetric.Name, maxMetricWidth)
				strRow = strRow & format(objMetric.Result & objMetric.Suffix, maxMetricWidth)
			Next

			WScript.Echo strHeader
			WScript.Echo strRow
			WScript.Echo ""
		Next

	Next
End Sub

