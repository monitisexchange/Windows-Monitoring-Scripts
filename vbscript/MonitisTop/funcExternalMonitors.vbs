'************************************************************************
' VBSCRIPT: funcExternalMonitors
'
' Required include file(s): 
' classAgents.vbs
'************************************************************************

Function GetExternalMonitors(aObjHttp, aObjAgents, aShowMonitors)
	Dim objAgents, xmlAgents
	Set objAgents = CreateObject("Scripting.Dictionary")
	
	'Create the dummy Agent object
	Set Agent = New class_InternalAgent
	Agent.Id = ""
	Agent.Name = "EXTERNAL"
	
	
	'Retrieve the list of agents
	url = "http://www.monitis.com/api?apikey=" + apiKey + "&output=xml&version=2&action=tests"
	aObjHttp.open "GET", url, False
	aObjHttp.send
	
	
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
	For Each mon in xmlMonitors.documentElement.childnodes

		'Create a Monitor Object	
		Set Monitor = New class_Monitor
		Monitor.Id = mon.Attributes.getNamedItem("id").text
		Monitor.Name = mon.text
		Monitor.DisplayName = mon.text
		
		dt = DateSerial(year(now), month(now), day(now))
		url = "http://www.monitis.com/api?action=testresult&apikey=" + apiKey + "&output=xml&testId=" & Monitor.Id + "&timezone=" & timezone & "&day=" & day(dt) & "&month=" & month(dt) & "&year=" & year(dt)
		aObjHttp.open "GET", url, False
		aObjHttp.send

		Set oRes = CreateObject("Microsoft.XMLDOM")
		oRes.async = False
		oRes.LoadXML(aObjHttp.responseText)
		
		'Loop throught the locations
		For Each t In oRes.firstchild.childnodes
			If t.nodeName = "location" Then

				Set Metric = New class_Metric
				Metric.Name = t.Attributes.GetNamedItem("name").text

				'Loop through the results for the current location
				For Each cell In t.firstChild.childnodes
					If LCase(cell.nodename) = "cell" Then
						Metric.Result = cell.text	
					End If
				Next

				'Add the Metric object to the Monitor List
				Monitor.MetricList.Add Metric.Name, Metric
			End If
		Next
		
		'Add the agent object to the list of agents
		Agent.MonitorList.Add Monitor.Id, Monitor
	Next
	
	
	aObjAgents.Add Agent.Id, Agent
End Function
