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
           ValueExists = ""
            set objHost = xmlDoc.documentElement.selectSingleNode("//" & app.nodename & "/properties/HostName")
            computer = objHost.text  
	results = ""
'Finding checked applications from XML document
	set isSelect = xmlDoc.documentElement.selectSingleNode("//"& app.nodename & "/IsSelect")
                  if isSelect.text = "true" then
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
                            checkedCnt = checkedCnt + 1
							'Get instances of WMI classes
	                           Set oRes = oWMI.ExecQuery ("select * from " & metName.item(j).getAttribute("WMIclass"),,32)
			                        'Enumerate instances
									For each oEntry in oRes
				                        on error resume next
										'Get the metrics' values
				                        value = oEntry.Properties_(metName.item(j).getAttribute("methodName"))
				                        if  Err.Number <> 0  then   
					                       ValueExists  = ValueExists +"  " + metName.item(j).getAttribute("methodName") 
						                    value = ""
                                            nullCount = nullCount + 1
					                        Err.Clear
				                        end if
					                    results = results +  s & space(30 - len(s)) & chr(9) & value & chr(13)
			                        next
                            
                             end if
                         next
                       ' Output result
                        if ValueExists = "" then
                            MsgBox results,,app.nodename & " Test result"
                        
                        end if
                        if  nullCount = checkedCnt then
                            MsgBox results & chr(13) & "Make sure that monitored application is avaliable in your computer",,app.nodename & " Test result"
                        elseif nullCount <> 0 then 
                        MsgBox  results & chr(13) & "Values of  " &  ValueExists & "  don't exist",,app.nodename & " Test result"
                        end if
	                end if
                end if
                 
   next
End Function
GetNetworkData