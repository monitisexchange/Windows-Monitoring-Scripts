'************************************************************************
' VBSCRIPT: funcInternalAgents
'
' Required include file(s): 
' classAgents.vbs
'************************************************************************

Function GetInternalAgents(aObjHttp, aObjAgents, aShowMonitors, aShowProcesses)
	Dim objAgents, xmlAgents
	Set objAgents = CreateObject("Scripting.Dictionary")
	
	WScript.Echo "Acquiring internal agents..."
	
	'Retrieve the list of agents
	url = "http://www.monitis.com/api?action=agents&apikey=" + apiKey + "&output=xml"
	aObjHttp.open "GET", url, False
	aObjHttp.send
	
	'Parse response
	Set xmlAgents = CreateObject("Microsoft.XMLDOM")
	xmlAgents.async = False
	xmlAgents.LoadXML(aObjHttp.responseText)
	If xmlAgents.parseError.errorCode <> 0 Then    
		wscript.Echo xmlAgents.parseError.errorCode    
		wscript.Echo xmlAgents.parseError.reason    
		wscript.Echo xmlAgents.parseError.line
	End If  	
	
	'Retrieve the agent information for each agent
	For Each agent in xmlAgents.documentElement.childnodes

		'Create an InternalAgent Object	
		Set IntAgent = New class_InternalAgent
		IntAgent.Id = agent.selectSingleNode("id").text
		IntAgent.Name = agent.selectSingleNode("key").text

		url = "http://www.monitis.com/api?action=agentInfo&apikey=" + apiKey + "&output=xml&agentId=" + IntAgent.Id + "&loadTests=true"
		aObjHttp.open "GET", url, False
		aObjHttp.send
		
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
	
Function GetGlobalMonitors(aObjHttp, oNames, aObjAgent, aShowMonitors)
	Dim oName
  
	For Each oName in oNames
	
    	If oName.NodeName <> "#text" And _
		  SupportedMonitors.Exists(LCase(oName.NodeName)) And _
		  aShowMonitors.Exists(LCase(oName.NodeName)) Then
    
    		' Create new monitor object
			Set Monitor = New class_Monitor
			Monitor.Name = GetMonitorName(oName.selectSingleNode("name").text)
		    strMonitorName = LCase(GetMonitorBaseName(Monitor.Name))
			Monitor.Id = oName.selectSingleNode("id").text
			Monitor.DisplayName = Monitor.Name
			
			'Get the monitor data 
			dt = DateSerial(year(now), month(now), day(now))
			url = "http://www.monitis.com/api?apikey=" & apikey & "&output=xml&action=" & strMonitorName & "Result&monitorId=" + Monitor.Id + "&timezone=" & timezone & "&day=" & day(dt) & "&month=" & month(dt) & "&year=" & year(dt)

			aObjHttp.open "GET", url, False
			aObjHttp.send
			Set oRes = CreateObject("Microsoft.XMLDOM")
			oRes.async = False
			oRes.LoadXML(aObjHttp.responseText)
			
			If Not oRes.firstchild.lastchild Is Nothing Then
				Set oNode = oRes.firstChild.lastChild
				GetResult oNode, Monitor, SupportedMonitors(strMonitorName)
				aObjAgent.MonitorList.Add Monitor.Name, Monitor
			End If
		End If 
		
	Next  
End Function
	
'-----------------------------------------------------------------------------------------------
	
Function GetProcessMonitors(aObjHttp, oNames, aObjAgent, aShowProcesses)
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

Function GetResult(aNode, aMonitor, aFields)
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
	
Sub ShowInternalAgents
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
