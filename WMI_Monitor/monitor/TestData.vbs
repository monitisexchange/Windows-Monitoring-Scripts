args = WScript.Arguments(0)
information="Please note that this test is aimed only for checking" & chr(13) & "data availability,so only raw data is displayed."&chr(13)&chr(13)

'Getting checked metrics' values from WMI 
Function GetNetworkData
	rootfolder = left(WScript.ScriptFullName,(Len(WScript.ScriptFullName))-(len(WScript.ScriptName)))
	dim objHost
	dim instanceName
	set xmlDoc = CreateObject("Microsoft.XMLDOM")
	xmlDoc.async="false"
	set objxmlDoc = CreateObject("Microsoft.XMLDOM")
	objxmlDoc.async="false"
	xmlDoc.load(args)
	set appList = xmlDoc.documentElement.selectSingleNode("/Monitor")
	for each app in appList.childnodes
		nullCount = 0
		checkedCnt = 0
		ValueExists = ""
		set objHost = xmlDoc.documentElement.selectSingleNode("//" & app.nodename & "/properties/HostName")
		computer = objHost.text  
		results = ""
		
		'Finding checked applications from XML document
		set isSelect = xmlDoc.documentElement.selectSingleNode("//"& app.nodename & "/Create")
		set instanceName = xmlDoc.documentElement.selectSingleNode("//" & app.nodename & "/InstanceName")
		set monitorID = xmlDoc.documentElement.selectSingleNode("//" & app.nodeName & "/monitorID")
		
		'Connecting to WMI
		on error resume next
		Set oWMI = GetObject("WINMGMTS:\\" & computer & "\ROOT\cimv2")
		'Checking WMI status 
		if  Err.Number <> 0  then
			MsgBox  "WMI connection failed",,app.nodename & "Test result"
			Err.Clear
		else
			set inst = xmlDoc.documentElement.selectsinglenode("//"& app.nodename &"/InstanceName")
			instanceName = inst.text					
			node = "//"& app.nodename &"/metrics/metric"
			xmlDoc.load(args)
			set metName = xmlDoc.documentElement.selectNodes(node)
			results = ""
			'Finding checked metrics from XML document
			for j = 0 to (metName.Length)-1 
				value = 0
				s = metName.item(j).getAttribute("name")
				if metName.item(j).text = "true" then
					checkedCnt = checkedCnt + 1
					'Get instances of WMI classes
					instance = metName.item(j).getAttribute("instance")
					if instance = "true" then
						Set oRes = oWMI.ExecQuery ("select * from " & metName.item(j).getAttribute("WMIclass")  & " where Name= "& chr(34) & instanceName & chr(34))
					else
						Set oRes = oWMI.ExecQuery ("select * from " & metName.item(j).getAttribute("WMIclass"))
					end if	
					'Enumerate instances
					For each oEntry in oRes
						on error resume next
						'Get metrics' values
						 Value = oEntry.Properties_(metName.item(j).getAttribute("methodName"))
						if  Err.Number <> 0  then   
							ValueExists  = ValueExists + metName.item(j).getAttribute("methodName") 
							Value = ""
							nullCount = nullCount + 1
							Err.Clear
						end if
					next
					results = results +  s & space(30 - len(s)) & chr(9) & Abs(Value) & chr(13)
				end if
			next
			' Output result
			if ValueExists = "" then
				MsgBox information & results,,app.nodename & "Test result"
			end if
			if  nullCount = checkedCnt and checkedcount<>0 then
				MsgBox results & chr(13) & "Please make sure that monitored application is avaliable in your computer",,app.nodename & "Test result"
			elseif nullCount <> 0 then 
				MsgBox  information & results & chr(13) &  "'" & ValueExists & "'"& " isn't applicable for current" & chr(13) & "environment",,app.nodename & "Test result"
			end if
		end if
	next
End Function
GetNetworkData