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

$apiKey = "your API key here"
$secretKey = "your secret key here"

#Requesting token
$url = "http://www.monitis.com/api?action=authToken&apikey=" + $apiKey + "&secretkey=" + $secretKey
$wc = new-object net.webclient
$resp = $wc.DownloadString($url).ToString()
$pos = $resp.IndexOf(":") + 2
$token = $resp.Substring($pos, $resp.Length - $pos - 2)

#Requests the monitor list in order to find the MonitorID
$tag = 'Hyper-V'
$url = 'http://www.monitis.com/customMonitorApi?action=getMonitors&apikey=' + $apiKey + "&tag=" + $tag + "&output=xml"
$wc = new-object net.webclient
$resp = $wc.DownloadString($url).ToString()

$monitors = new-object "System.Xml.XmlDocument"
$monitors.LoadXml($resp)

$monitorID = FindMonitor "Percent processor utilization"
$value1 = (get-counter '\Processor(_Total)\% Processor Time').CounterSamples[0].CookedValue
$value2 = (get-counter '\Processor(_Total)\% Privileged Time').CounterSamples[0].CookedValue
$results = "privileged:" + $value2 + ";total:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "VM1 percent processor utilization"
$value1 = (get-counter '\Hyper-V Hypervisor Virtual Processor(XPHV:Hv VP 0)\% Total Run Time').CounterSamples[0].CookedValue
$value2 = (get-counter '\Hyper-V Hypervisor Virtual Processor(XPHV:Hv VP 0)\% Guest Run Time').CounterSamples[0].CookedValue
$results = "guest:" + $value2 + ";total:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "Disk transferred bytes per second"
$value1 = (get-counter '\PhysicalDisk(0 C: V: S:)\Disk Read Bytes/sec').CounterSamples[0].CookedValue
$value2 = (get-counter '\PhysicalDisk(0 C: V: S:)\Disk Write Bytes/sec').CounterSamples[0].CookedValue
$results = "write:" + $value2 + ";read:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "VM1 disk transferred bytes per second"
$value1 = (get-counter '\Hyper-V Virtual IDE Controller(XPHV:Ide controller)\Read Bytes/sec').CounterSamples[0].CookedValue
$value2 = (get-counter '\Hyper-V Virtual IDE Controller(XPHV:Ide controller)\Write Bytes/sec').CounterSamples[0].CookedValue
$results = "write:" + $value2 + ";read:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "Network transferred bytes per second"
$value1 = (get-counter '\Network Interface(D-Link DFE-538TX 10_100 Adapter)\Bytes Received/sec').CounterSamples[0].CookedValue
$value2 = (get-counter '\Network Interface(D-Link DFE-538TX 10_100 Adapter)\Bytes Sent/sec').CounterSamples[0].CookedValue
$results = "sent:" + $value2 + ";received:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "VM1 network transferred bytes per second"
$value1 = (get-counter '\Hyper-V Virtual Network Adapter(D-Link DFE-538TX 10_100 Adapter - VirtualBox Bridged Networking Driver Miniport__DEVICE_{E34F3122-DB37-4745-86BD-456EB0666F0A})\Bytes Received/sec').CounterSamples[0].CookedValue
$value2 = (get-counter '\Hyper-V Virtual Network Adapter(D-Link DFE-538TX 10_100 Adapter - VirtualBox Bridged Networking Driver Miniport__DEVICE_{E34F3122-DB37-4745-86BD-456EB0666F0A})\Bytes Sent/sec').CounterSamples[0].CookedValue
$results = "sent:" + $value2 + ";received:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "Available memory"
$value1 = (get-counter '\Memory\Available Mbytes').CounterSamples[0].CookedValue
$results = "bytes:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "Page faults"
$value1 = (get-counter '\Memory\Pages/sec').CounterSamples[0].CookedValue
$results = "bytes:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "VM1 Dynamic memory pressure"
$value1 = (get-counter '\Hyper-V Dynamic Memory VM(XPHV)\Average Pressure').CounterSamples[0].CookedValue
$results = "pressure:" + $value1 + ";"
$results 
AddResult

$monitorID = FindMonitor "Virtual machines count"
$value1 = (get-counter '\Hyper-V Virtual Machine Summary\Running').CounterSamples[0].CookedValue
$value2 = (get-counter '\Hyper-V Virtual Machine Summary\Turned Off').CounterSamples[0].CookedValue
$results = "turnedoff:" + $value2 + ";running:" + $value1 + ";"
$results 
AddResult
