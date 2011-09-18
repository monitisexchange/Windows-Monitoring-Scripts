'************************************************************************
' VBSCRIPT: funcMonitisKeys
'
' Include file to be used in Monitis scripts that require apiKey and 
' authentication tokens
'************************************************************************

Const ConfigFile = "config.xml"

' Global variables
Dim apiKey
Dim secretKey
Dim authToken

'-----------------------------------------------------------------------------------------------
' Retrieves the apiKey and secretKey from a configuration file. Use
' include(funcMonitisKeys.vbs) to include this snippet in a VBScript 
' requiring apiKey and secretKey.
'-----------------------------------------------------------------------------------------------

Sub GetAuthenticationKeys(aVerbose)
	Dim oConf
	Dim oFso
	Dim strFilename
	
	strFilename = left(Wscript.ScriptFullName,InStrRev(Wscript.ScriptFullName, "\")) + ConfigFile
	Set oConf = CreateObject("Microsoft.XMLDOM")
	oConf.async = False
	
	Set oFso = CreateObject("Scripting.FileSystemObject")
	If oFso.FileExists(strFilename) Then
		oConf.Load(strFilename)
	Else
	  oConf.LoadXML("<monitis><APIKey/><SecretKey/></monitis>")
	End If
	
	'Get the apiKey
	apiKey = oConf.documentElement.selectSingleNode("APIKey").text
	If Trim(apiKey) = "" then
	  wscript.echo "Error: apiKey value missing from Config.xml"
	  wscript.quit
	End if
	
	'Get the secretKey
	secretKey = oConf.documentElement.selectSingleNode("SecretKey").text
	If Trim(secretKey) = "" Then
	  wscript.echo "Error: secretKey value missing from Config.xml"
	  wscript.quit
	End If
	
	'Display keys
	If aVerbose = True Then
		WScript.Echo "APIKey: " & apiKey
		WScript.Echo "SecretKey: " & secretKey
	End If
	
	'Cleanup
	Set oConf = Nothing
	Set oFso = Nothing
End Sub

'-----------------------------------------------------------------------------------------------
' Save APIKEY and SECRETKEY in config.xml
'-----------------------------------------------------------------------------------------------

Sub SetAuthenticionKeys(apiKey, secretKey)
	Dim oConf
	Dim oFso
	Dim strFilename
	
	strFilename = left(Wscript.ScriptFullName,InStrRev(Wscript.ScriptFullName, "\")) + ConfigFile
	Set oConf = CreateObject("Microsoft.XMLDOM")
	oConf.async = False
	
	Set oFso = CreateObject("Scripting.FileSystemObject")
	If oFso.FileExists(strFilename) Then
		oConf.Load(strFilename)
	Else
	  oConf.LoadXML("<monitis><APIKey/><SecretKey/></monitis>")
	End If

    If Len(apiKey) > 0 Then
		oConf.documentElement.selectSingleNode("APIKey").text = apiKey
	    Wscript.Echo "apiKey set: " + oConf.documentElement.selectSingleNode("APIKey").text
	Else
		WScript.Echo "Missing apiKey"
    End If
    
    If Len(secretKey) > 0 then
		oConf.documentElement.selectSingleNode("SecretKey").text = secretKey
	    Wscript.Echo "secretKey set: " + oConf.documentElement.selectSingleNode("SecretKey").text
	Else
		WScript.Echo "Missing secretKey"
    End If
    
	oConf.Save(cfgfile)
End Sub
	
'-----------------------------------------------------------------------------------------------
' Request a token 
'-----------------------------------------------------------------------------------------------

Function GetAuthToken(objHttp, apiKey, secretKey)
	Dim url

	url = "http://www.monitis.com/api?action=authToken&apikey=" + apiKey + "&secretkey=" + secretKey + "&version=2"

	objHttp.open "GET", url, False
	objHttp.send
	resp = objHttp.responseText
	
	If Trim(resp) <> "" And InStr(LCase(resp), "authentication failure") = 0 Then
		pos = InStr(resp, ":") + 2
		GetAuthToken = mid(resp, pos, len(resp) - pos - 1)
	Else
		GetAuthToken = ""
		WScript.Echo "Error getting authentication token"
		WScript.Echo "APIKey: " & apiKey
		WScript.Echo "SecretKey: " & secretKey
		WScript.Quit
	End If
End Function

