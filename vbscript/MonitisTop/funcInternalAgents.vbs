'************************************************************************
' VBSCRIPT: funcInternalAgents
'
' Required include file(s): 
' classAgents.vbs
'************************************************************************

Function GetInternalAgents(aObjHttp, aObjAgents, aShowMonitors)
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
			GetAgentMonitors aObjHttp, info.childnodes, IntAgent, aShowMonitors
		Next
		
		'Add the agent object to the list of agents
		aObjAgents.Add IntAgent.Id, IntAgent
	Next
	
'	Set GetInternalAgents = objAgents
End Function
	
'-----------------------------------------------------------------------------------------------
	
Function GetAgentMonitors(aObjHttp, oNames, aObjAgent, aShowMonitors)
	Dim oName, IP, name, str, pos, pos1, computer
  
	For Each oName in oNames
	
    	If oName.NodeName <> "#text" And _
    	   SupportedMonitors.Exists(LCase(oName.NodeName)) And _
    	   aShowMonitors.Exists(LCase(oName.NodeName)) Then
    
			Set Monitor = New class_Monitor
			
			If Not oName.selectSingleNode("ip") Is Nothing Then 
				Monitor.IP = oName.selectSingleNode("ip").text 
			End If
		  
			Monitor.Id = oName.selectSingleNode("id").text
			Monitor.Name = GetMonitorName(oName.selectSingleNode("name").text)
			Monitor.Width = Len(Monitor.Name) + 4
			Monitor.ComputerName = GetMonitorComputerName(oName.selectSingleNode("name").text)
		  
		    strMonitorName = LCase(GetMonitorBaseName(Monitor.Name))
			dt = DateSerial(year(now), month(now), day(now))
			url = "http://www.monitis.com/api?apikey=" & apikey & "&output=xml&action=" & strMonitorName & "Result&monitorId=" + Monitor.Id + "&timezone=" & timezone & "&day=" & day(dt) & "&month=" & month(dt) & "&year=" & year(dt)

			aObjHttp.open "GET", url, False
			aObjHttp.send
			Set oRes = CreateObject("Microsoft.XMLDOM")
			oRes.async = False
			oRes.LoadXML(aObjHttp.responseText)
			
			If Not oRes.firstchild Is Nothing Then
				 For Each oNode in oRes.firstchild.childnodes
				 
				 	Select Case strMonitorName
				 		Case "memory"	: Monitor.Result = GetMemoryResult(oNode, SupportedMonitors(strMonitorName))
				 		Case "cpu"		: Monitor.Result = GetCpuResult(oNode, SupportedMonitors(strMonitorName))
				 		Case "drive"	: Monitor.Result = GetDriveResult(oNode, SupportedMonitors(strMonitorName))
				 	
				 	End Select
				 	 
				 Next
			End If

			'Add the Monitor to the agent's monitor list
			aObjAgent.MonitorList.Add Monitor.Name, Monitor
		End If 
	Next  
End Function

'-----------------------------------------------------------------------------------------------

Function GetMemoryResult(aNode, aField)
	avgFactor = 0
	totalMemory = 0
	resultValue = 0
	
	For Each oCell In aNode.ChildNodes
	
		If oCell.nodename = "totalMemory" Then
			totalMemory = totalMemory + CDbl(oCell.text)
		End If

		If LCase(oCell.nodename) = LCase(aField) Then
			resultValue = resultValue + CDbl(oCell.text)
		End If
		
		avgFactor = avgFactor + 1
	Next

	resultValue = resultValue / avgFactor
	totalMemory = totalMemory / avgFactor
	
	GetMemoryResult = CStr(Round(resultValue))
	'GetMemoryResult = Round((freeMemory / totalMemory) * 100) & "%"
End Function

'-----------------------------------------------------------------------------------------------

Function GetCpuResult(aNode, aField)
	avgFactor = 0
	resultValue = 0
	
	For Each oCell In aNode.ChildNodes
	
		If LCase(oCell.nodename) = LCase(aField) Then
			resultValue = resultValue + CDbl(oCell.text)
		End If

		avgFactor = avgFactor + 1
	Next

	resultValue = resultValue / avgFactor
	
	GetCpuResult = CStr(Round(resultValue)) & "%"
End Function

'-----------------------------------------------------------------------------------------------

Function GetDriveResult(aNode, aField)
	avgFactor = 0
	resultValue = 0
	
	For Each oCell In aNode.ChildNodes
	
		If LCase(oCell.nodename) = LCase(aField) Then
			resultValue = CLng(oCell.text)
		End If

		avgFactor = avgFactor + 1
	Next

	'resultValue = resultValue / avgFactor
	'GetDriveResult = Round(totalFreeSpace) & "MB"
	GetDriveResult = CStr(resultValue) & "GB"
End Function


