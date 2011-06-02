$apiKey = "your API key here"
$secretKey = "your secret key here"

#Requesting token
$url = "http://www.monitis.com/api?action=authToken&apikey=" + $apiKey + "&secretkey=" + $secretKey
$wc = new-object net.webclient
$resp = $wc.DownloadString($url).ToString()
$pos = $resp.IndexOf(":") + 2
$token = $resp.Substring($pos, $resp.Length - $pos - 2)
$token

#Adding custom monitor
$nvc = new-object System.Collections.Specialized.NameValueCollection
$nvc.Add('apikey', $apikey)
$nvc.Add('validation', 'token')
$nvc.Add('authToken', $token)
$nvc.Add('timestamp', (get-date).touniversaltime().ToString("yyyy-MM-dd HH:mm:ss"))
$nvc.Add('action', 'addMonitor')
$nvc.Add('monitorParams', 'exchange_queues:Exchange+data:Number of messages:3:false;')
$nvc.Add('resultParams', 'queued:Queued:N%2FA:2;')
$nvc.Add('name', 'Exchange-queues' )
$nvc.Add('tag', '[exchange+sample]' )

$wc = new-object net.webclient
$wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
$resp = $wc.UploadValues('http://www.monitis.com/customMonitorApi', $nvc)
[text.encoding]::ascii.getstring($resp)

