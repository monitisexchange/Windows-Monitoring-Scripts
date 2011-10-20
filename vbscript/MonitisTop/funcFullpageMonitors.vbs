'************************************************************************
' VBSCRIPT: funcFullpageMonitors
'
' Required include file(s): 
' classAgents.vbs
'************************************************************************

Function GetFullpageMonitors(aObjHttp, aObjAgent, aShowMonitors)
	Dim objAgents, xmlAgents
	Set objAgents = CreateObject("Scripting.Dictionary")
	
	'Create the dummy Agent object
	Set Agent = New class_InternalAgent
	Agent.Id = ""
	Agent.Name = "FULLPAGE"
	
	url = "http://www.monitis.com/api?version=2&apikey=" & apikey & "&output=xml&action=topFullpage&limit=50&detailedResults=true"
	
	aObjHttp.open "GET", url, False
	aObjHttp.send
	Set oRes = CreateObject("Microsoft.XMLDOM")
	oRes.async = False
	oRes.LoadXML(aObjHttp.responseText)
	
	Set oNode = oRes.selectSingleNode("data/tests")
	For Each t In oNode.childnodes
		
		Set oTest = t.selectSingleNode("testName")
		If Not oTest Is Nothing Then
		
			If ShowThisMonitor(oTest.text, aShowMonitors) Then

				'Create a new monitor object
				Set Monitor = New class_Monitor
				Monitor.Id = t.selectSingleNode("id").text
				Monitor.Name = t.selectSingleNode("testName").text
				Monitor.DisplayName = t.selectSingleNode("testName").text

				'Retrieve the results for this monitor
				GetResult t, Monitor, SupportedMonitors.Item("fullpage")
				
				'Add the monitor to the list of agents
				Agent.MonitorList.Add Monitor.Id, Monitor
			End If
			
		End If
	Next
	
	aObjAgent.Add Agent.Id, Agent			
End Function

'-----------------------------------------------------------------------------------------------

Function ShowThisMonitor(aMonitor, aShowMonitors)
	ShowThisMonitor = False
	For Each sm In aShowMonitors
		If (InStr(LCase(aMonitor), LCase(sm)) > 0) Or (sm = "all") Then
			ShowThisMonitor = True
		End If
	Next
End Function
