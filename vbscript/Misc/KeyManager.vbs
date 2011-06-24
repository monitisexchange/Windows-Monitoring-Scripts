Option Explicit
dim apiKey, secretKey, objHTTP, url, cmd, oConf, cfgfile, oFso
dim oResp, oNode, oWMI, oRes, oEntry, IDs, pos

Set oFso = CreateObject("Scripting.FileSystemObject")

cmd = WScript.arguments.named.item("cmd")
apiKey = WScript.arguments.named.item("APIKey")
secretKey = WScript.arguments.named.item("SecretKey")

if cmd = "" then
  wscript.echo "Commmand not specified"
  ShowUsage
end if

cfgfile = left(Wscript.ScriptFullName,InStrRev(Wscript.ScriptFullName, "\")) + "config.xml"
Set oConf = CreateObject("Microsoft.XMLDOM")
oConf.async = False
if oFso.FileExists(cfgfile) then
  oConf.Load(cfgfile)
else
  oConf.LoadXML("<monitis><APIKey/><SecretKey/></monitis>")
end if

select case cmd
  case "get"
    wscript.echo "API Key : " + oConf.documentElement.selectSingleNode("APIKey").text
    wscript.echo "Secret Key : " + oConf.documentElement.selectSingleNode("SecretKey").text
  case "set"
    if apiKey > "" then
	  oConf.documentElement.selectSingleNode("APIKey").text = apikey
    end if
    if secretKey > "" then
	  oConf.documentElement.selectSingleNode("SecretKey").text = secretKey
    end if
	oConf.Save(cfgfile)
    wscript.echo "API Key : " + oConf.documentElement.selectSingleNode("APIKey").text
    wscript.echo "Secret Key : " + oConf.documentElement.selectSingleNode("SecretKey").text
end select

Sub ShowUsage
  wscript.echo "KeyManager.vbs parameters:"
  wscript.echo " /cmd:<get|set>"
  wscript.echo " /apiKey:<apiKey> Your API key"
  wscript.echo " /secretKey:<secretKey> Your Secret Key"
  wscript.quit
End Sub