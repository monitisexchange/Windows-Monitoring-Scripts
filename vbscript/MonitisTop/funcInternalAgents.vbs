'************************************************************************
' VBSCRIPT: funcInternalAgents
'
' Required include file(s): 
' classAgents.vbs
'************************************************************************

Function GetInternalAgents(aObjHttp, aObjAgents, aShowMonitors, aShowProcesses)
	Dim objAgents, xmlAgents
	Set objAgents = CreateObject("Scripting.Dictionary")
	
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

		'Retrieve the agent information 
		url = "http://www.monitis.com/api?action=agentInfo&apikey=" + apiKey + "&output=xml&agentId=" + IntAgent.Id + "&loadTests=true"
		aObjHttp.open "GET", url, False
		aObjHttp.send
		
		Set xmlAgentInfo = CreateObject("Microsoft.XMLDOM")
		xmlAgentInfo.async = False
		xmlAgentInfo.LoadXML(aObjHttp.responseText)

		For Each info in xmlAgentInfo.documentElement.childnodes
		
			If aShowProcesses.Count = 0 Then
				GetGlobalMonitors aObjHttp, info.childnodes, IntAgent, aShowMonitors
			Else
				If LCase(info.nodename) = "processes" Then
					GetProcessMonitors aObjHttp, info.childnodes, IntAgent, aShowProcesses
				End If
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
	
    	If oName.NodeName <> "#text" And oName.NodeName <> "process" And _
		  SupportedMonitors.Exists(LCase(oName.NodeName)) And _
		  aShowMonitors.Exists(LCase(oName.NodeName)) Then
    
    		' Create new monitor object
			Set Monitor = New class_Monitor
			Monitor.Name = GetMonitorName(oName.selectSingleNode("name").text)
		    strMonitorName = LCase(GetMonitorBaseName(Monitor.Name))
			Monitor.Id = UCase(oName.selectSingleNode("id").text)
			Monitor.DisplayName = UCase(Monitor.Name)
			
			url = "http://www.monitis.com/api?version=2&apikey=" & apikey & "&output=xml&action=top" & strMonitorName & "&limit=50&detailedResults=true"

			aObjHttp.open "GET", url, False
			aObjHttp.send
			Set oRes = CreateObject("Microsoft.XMLDOM")
			oRes.async = False
			oRes.LoadXML(aObjHttp.responseText)
			
			Set oNode = oRes.selectSingleNode("data/tests")
			If Not oNode Is Nothing Then
				For Each oCell In oNode.childnodes
					GetResult oCell, Monitor, SupportedMonitors(strMonitorName)
				Next
				aObjAgent.MonitorList.Add Monitor.Name, Monitor
			End If
		End If 
		
	Next  
End Function
	
'-----------------------------------------------------------------------------------------------
	
Function GetProcessMonitors(aObjHttp, oNames, aObjAgent, aShowProcesses)
				
	url = "http://www.monitis.com/api?version=2&apikey=" & apikey & "&output=xml&action=topProcessByCPUUsage&limit=50&detailedResults=true"
	
	aObjHttp.open "GET", url, False
	aObjHttp.send
	Set oRes = CreateObject("Microsoft.XMLDOM")
	oRes.async = False
	oRes.LoadXML(aObjHttp.responseText)
	
	Set oNode = oRes.selectSingleNode("data/tests")
	For Each t In oNode.childnodes
		
		Set oTest = t.selectSingleNode("testName")
		If Not oTest Is Nothing Then
		
			If ShowThisProcess(oTest.text, aShowProcesses) Then

				'Create a new monitor object
				Set Monitor = New class_Monitor
				Monitor.Id = t.selectSingleNode("id").text
				Monitor.Name = t.selectSingleNode("testName").text
				Monitor.DisplayName = GetMonitorName(t.selectSingleNode("testName").text)

				'Retrieve the results for this monitor
				GetResult t, Monitor, SupportedMonitors.Item("process")
				
				'Add the monitor to the list of agents
				aObjAgent.MonitorList.Add Monitor.Id, Monitor
			
			End If
			
		End If

	Next
				
End Function

'-----------------------------------------------------------------------------------------------

Function ShowThisProcess(aProcess, aShowProcesses)
	ShowThisProcess = False
	For Each sp In aShowProcesses
		If (InStr(LCase(aProcess), LCase(sp)) > 0) Or (sp = "all") Then
			ShowThisProcess = True
		End If
	Next
End Function

	
	