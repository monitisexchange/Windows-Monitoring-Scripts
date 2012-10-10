'Getting checked metrics' values from WMI 
Function GetNetworkData
rootfolder = left(WScript.ScriptFullName,(Len(WScript.ScriptFullName))-(len(WScript.ScriptName)))
dim objHost
set xmlDoc = CreateObject("Microsoft.XMLDOM")
xmlDoc.async="false"

set objxmlDoc = CreateObject("Microsoft.XMLDOM")
objxmlDoc.async="false"

xmlDoc.load(rootfolder & "metrics.xml")

set appList = xmlDoc.documentElement.selectSingleNode("/Monitor")
for each app in appList.childnodes
	nullCount = 0
	checkedCnt = 0
	
	set objHost = xmlDoc.documentElement.selectSingleNode("//" & app.nodename & "/properties/HostName")
	computer = objHost.text  
	set monitorID = xmlDoc.documentElement.selectSingleNode("//" & app.nodename & "/monitorID")
	results = ""
	'Finding checked applications from XML document
		'Connecting to WMI
		on error resume next
		Set oWMI = GetObject("WINMGMTS:\\" & computer & "\ROOT\cimv2")
		'Checking WMI status 
		if  Err.Number <> 0  then
			MsgBox  "WMI connection to host failed",,app.nodename & " Test result"
			Err.Clear
		else
			WScript.Sleep(1000)
			node = "//"& app.nodename &"/metrics/metric"
			xmlDoc.load(rootfolder & "metrics.xml")
			set metName = xmlDoc.documentElement.selectNodes(node)
			results = ""
			dim s   
			'Finding checked metrics from XML document
			for j = 0 to (metName.Length)-1 
				s = metName.item(j).getAttribute("name")
				if metName.item(j).text = "true" then
					'Get instances of WMI classes
					Set oRes = oWMI.ExecQuery ("select * from " & metName.item(j).getAttribute("WMIclass"))
					if oRes.Count <> 0 then
						For each oEntry in oRes
							value = oEntry.Properties_(metName.item(j).getAttribute("methodName"))
						next 
					else
						value = "Absent"
					end if
						results = results +  s & space(70 - len(s)) & chr(9) & value & chr(13)
				end if
			next
			'Output result
			wscript.echo results
		end if
	
next
End Function
GetNetworkData