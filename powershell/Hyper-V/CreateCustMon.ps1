function AddCustMon([string] $name, [string] $monitorParams, [string] $resultParams, [string] $row, [string] $column) {
  Write-Host "Adding Custom Monitor " $name
  $nvc = new-object System.Collections.Specialized.NameValueCollection
  $nvc.Add('apikey', $apikey)
  $nvc.Add('validation', 'token')
  $nvc.Add('authToken', $token)
  $nvc.Add('timestamp', (get-date).touniversaltime().ToString("yyyy-MM-dd HH:mm:ss"))
  $nvc.Add('action', 'addMonitor')
  $nvc.Add('monitorParams', $monitorParams)
  $nvc.Add('resultParams', $resultParams)
  $nvc.Add('name', $name )
  $nvc.Add('tag', 'Hyper-V' )

  $wc = new-object net.webclient
  $wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
  $resp = $wc.UploadValues('http://www.monitis.com/customMonitorApi', $nvc)
  $resp = [text.encoding]::ascii.getstring($resp)
  $pos = $resp.IndexOf("data") + 6
  $testID = $resp.Substring($pos, $resp.Length - $pos - 1)
  $testID
  
  write-host "Adding test " $name " to the page"
  $nvc = new-object System.Collections.Specialized.NameValueCollection
  $nvc.Add('apikey', $apikey)
  $nvc.Add('validation', 'token')
  $nvc.Add('authToken', $token)
  $nvc.Add('timestamp', (get-date).touniversaltime().ToString("yyyy-MM-dd HH:mm:ss"))
  $nvc.Add('action', 'addPageModule')
  $nvc.Add('moduleName', 'CustomMonitor')
  $nvc.Add('pageId', $pageID)
  $nvc.Add('column', $column )
  $nvc.Add('row', $row )
  $nvc.Add('dataModuleId', $testID)

  $wc = new-object net.webclient
  $wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
  $resp = $wc.UploadValues('http://www.monitis.com/api', $nvc)
  [text.encoding]::ascii.getstring($resp)
}

$apiKey = "your API key here"
$secretKey = "your secret key here"

write-host "Requesting token"
$url = "http://www.monitis.com/api?action=authToken&apikey=" + $apiKey + "&secretkey=" + $secretKey
$wc = new-object net.webclient
$resp = $wc.DownloadString($url).ToString()
$pos = $resp.IndexOf(":") + 2
$token = $resp.Substring($pos, $resp.Length - $pos - 2)
write-host "Token: " $token

write-host "Adding a page"
$nvc = new-object System.Collections.Specialized.NameValueCollection
$nvc.Add('apikey', $apikey)
$nvc.Add('validation', 'token')
$nvc.Add('authToken', $token)
$nvc.Add('timestamp', (get-date).touniversaltime().ToString("yyyy-MM-dd HH:mm:ss"))
$nvc.Add('action', 'addPage')
$nvc.Add('title', 'Hyper-V')
$nvc.Add('columnCount', '2')

$wc = new-object net.webclient
$wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
$resp = $wc.UploadValues('http://www.monitis.com/api', $nvc)
$resp = [text.encoding]::ascii.getstring($resp)
$resp
$pos = $resp.IndexOf("pageId") + 8
$pageID = $resp.Substring($pos, $resp.Length - $pos - 2)
$pageID 

AddCustMon 'Percent processor utilization' 'Processor:Root Processor utilization:Percent processor time:3:false;;' 'total:Total:N%2FA:2;privileged:Privileged:N%2FA:2;' '1' '1'
AddCustMon 'VM1 percent processor utilization' 'Processor:VM1 Processor utilization:Percent processor time:3:false;;' 'total:Total:N%2FA:2;guest:Guest:N%2FA:2;' '1' '2'

AddCustMon 'Disk transferred bytes per second' 'Disk:Disk utilization:Bytes/sec:3:false;;' 'read:Read:N%2FA:2;write:Write:N%2FA:2;' '2' '1'
AddCustMon 'VM1 disk transferred bytes per second' 'Disk:VM1 disk utilization:Bytes/sec:3:false;;' 'read:Read:N%2FA:2;write:Write:N%2FA:2;' '2' '2'

AddCustMon 'Network transferred bytes per second' 'Network:Network utilization:Bytes/sec:3:false;;' 'sent:Sent:N%2FA:2;received:Received:N%2FA:2;' '3' '1'
AddCustMon 'VM1 network transferred bytes per second' 'Network:Network utilization:Bytes/sec:3:false;;' 'sent:Sent:N%2FA:2;received:Received:N%2FA:2;' '3' '2'

AddCustMon 'Available memory' 'memavail:Available memory:bytes:3:false;;' 'bytes:Bytes:N%2FA:2;' '4' '1'
AddCustMon 'Page faults' 'pagefaults:Page faults:pagespersec:3:false;;' 'pagespersec:Pages/sec:N%2FA:2;' '4' '2'

AddCustMon 'VM1 Dynamic memory pressure' 'avgpressure:Agerage pressure:pagespersec:3:false;;' 'pressure:Pressure:N%2FA:2;' '5' '1'
AddCustMon 'Virtual machines count' 'vmcount:Virtual machines count:number:3:false;;' 'running:Running:N%2FA:2;turnedoff:Turned off:N%2FA:2;' '5' '1'

