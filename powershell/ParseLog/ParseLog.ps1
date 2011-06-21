param([string]$LogName = ".", [string]$DateFormat, [string]$MatchText, [Boolean]$ShouldMatch = $true, [int]$MinutesBack, [string]$Tag, [string]$Name)

function FindMonitor([string] $name) {
  foreach ($node in $monitors.monitors.childnodes) {
    if ($node.name -eq $name) {
	  return $node.id
	}
  }
}

function AddResult() {
  $nvc = new-object System.Collections.Specialized.NameValueCollection
  $nvc.Add('apikey', $apikey)
  $nvc.Add('validation', 'token')
  $nvc.Add('authToken', $token)
  $nvc.Add('timestamp', ((get-date).touniversaltime().ToString("yyyy-MM-dd HH:mm:ss").ToString()))
  $nvc.Add('action', 'addResult')
  $nvc.Add('monitorId', $monitorID)
  $nvc.Add('checktime', ([int][double]::Parse((get-date((get-date).touniversaltime()) -UFormat %s))).ToString() + '000')
  $nvc.Add('results', $results)

  $wc = new-object net.webclient
  $wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
  $resp = $wc.UploadValues('http://www.monitis.com/customMonitorApi', $nvc)
  [text.encoding]::ascii.getstring($resp)
}

$files = @(Get-ChildItem $LogName | Sort-Object LastWriteTime -descending)
$LogName = $files[0].FullName
Write-host "Parsing file:" $LogName
$dfre = [regex]("(?<date>" +  ($DateFormat -replace "\w", "\d") +")")
$matchRes = 0;
foreach ($line in Get-Content $LogName) {
  try {
    $dtLine = [datetime]::ParseExact($dfre.matches($line)[0].Value, $DateFormat, $null)
    $diff = New-TimeSpan -Start $dtLine -End (Get-Date)
  
    if ($diff.TotalMinutes -le $MinutesBack) {
      if ($line -match $MatchText) {
        $matchRes = 1;
        Write-Host $line
        if ($ShouldMatch -eq $true) {
		  break;
		}
	  }
    }
  }
  catch {
    Write-Host "Unk"
  }
}
if ($ShouldMatch -eq $false) {
  if ($matchRes -eq 0) {
    $matchRes = 1;
  } else {
    $matchRes = 0;
  }
}

Write-Host $matchRes

$apiKey = "Your API key here"
$secretKey = "Your Secret key here"

#Requesting token
$url = "http://www.monitis.com/api?action=authToken&apikey=" + $apiKey + "&secretkey=" + $secretKey
$wc = new-object net.webclient
$resp = $wc.DownloadString($url).ToString()
$pos = $resp.IndexOf(":") + 2
$token = $resp.Substring($pos, $resp.Length - $pos - 2)

#Requests the monitor list in order to find the MonitorID
$url = 'http://www.monitis.com/customMonitorApi?action=getMonitors&apikey=' + $apiKey + "&tag=" + $Tag + "&output=xml"
$wc = new-object net.webclient
$resp = $wc.DownloadString($url).ToString()

$monitors = new-object "System.Xml.XmlDocument"
$monitors.LoadXml($resp)

$monitorID = FindMonitor $Name
$results = "match:" + $matchRes + ";"
AddResult