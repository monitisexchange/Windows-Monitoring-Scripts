[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
clear-host
$tabs_obj = New-Object 'System.Collections.Generic.Dictionary[String,String]'
if($HostNameindex -eq $null)
{
	New-Variable -Name HostNameindex  -Value 0 -Visibility Public -Option AllScope
}
$path = Get-Location
#Close the current window
[xml]$xmlApiKey = (Get-Content $path"\ApiKey.xml")
$apikey = $xmlApiKey.Key.ApiKey


function btnExit
{
	$res = [System.Windows.Forms.MessageBox]::Show("Are you sure?","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
	if($res-eq[System.Windows.Forms.DialogResult]::Yes)
	{
		$Form.Close()
	}
}

#load XML file
[xml]$xml = (Get-Content $path"\AppList.xml")
$inst_list = @()

[xml]$xml1 = (Get-Content $newFile)
$propCount = $xml1.Monitor.ChildNodes.item(0).SelectSingleNode("properties").ChildNodes.count
$windowName = $xml1.Monitor.Childnodes.Item(0).Tostring()
$windowName =  $windowName + " Monitor Properties"
$connect_to = $xml1.Monitor.childnodes.item(0).SelectSingleNode("ConnectTo").InnerText
if ($connect_to -eq "Monitis")
{
	$PortalUrl = "www.monitis.com"
}
elseif($connect_to -eq "Monitor.us")
{
	$PortalUrl = "www.monitor.us"
}
#read selected application properties' values from XML
function Form_Load
{
	$HostNameindex = 0
	$lo = $propCount*38+20
	$instance_lbl.location = New-Object System.Drawing.Point(20,$lo)
	$instance.location = New-Object System.Drawing.Point(144,$lo)
	$tabs.location = New-Object System.Drawing.Point($instance.location.x,($instance.location.y +38))
	$tabs_lbl.location = New-Object system.Drawing.Point($instance_lbl.location.x, ($instance_lbl.location.y + 38))
	$lblText = @()
	$lbl = new-object System.Windows.Forms.Label[] $propCount
	[xml]$xml1 = (Get-Content $newFile)
	#add textboxes to window
	for ($i=0; $i -lt $propCount; $i++)
	{	
		$Panel.Controls.Add($Boxes[$i])
		$Boxes[$i].Text = $xml1.Monitor.ChildNodes.item(0).SelectSingleNode("properties").ChildNodes.Item($i).InnerText
	}
	foreach ($node in $xml1.Monitor.ChildNodes.item(0).SelectSingleNode("properties").ChildNodes)
	{
		[string[]]$lblText += $node.ToString()
	}
	for ( $i = 0 ; $i -lt $propCount; $i++)
	{	
		$b = $i*38+20
		$lbl[$i] = New-Object System.Windows.Forms.Label
		$lbl[$i].Location = New-Object System.Drawing.Point(20,$b)
		$lbl[$i].Text = $lblText[$i]
		$Panel.Controls.Add($lbl[$i])
	}
	for ( $i = 0 ; $i -lt $propCount; $i++)
	{
		if ($lblText[$i] -eq "HostName")
		{
			$HostNameindex = $i
		}
	}
	$Panel.Controls.Add($instance)
	$Panel.Controls.Add($instance_lbl)
	$Panel.Controls.add($tabs)
	$tabs.Text = $xml1.Monitor.ChildNodes.item(0).SelectSingleNode("DashboardTag").InnerText
	$toolTip.SetToolTip($tabs, "Add a new page to user's dash board or
	use existing page(s)");
	$Panel.Controls.add($tabs_lbl)
	$Boxes[$HostNameindex].Text = "localhost"
	GetInstances
	$Boxes[$HostNameindex].add_Leave({GetInstances})
	if ($connect_to -eq "Monitis")
	{
		GetPages
	}
}

#show the previous window
function btnBack
{
	$Form.Visible=$false
	$Form.Close()
	& $path"\selwin.ps1"
}

#save selected application properties values in xml document
#validation checking for fields
#if all fields are filled go to the next window
function btnNext
{
	[xml]$xml1 = (Get-Content $newFile)
	for ($i=0;$i -lt $propCount; $i++)
	{	
		$xml1.Monitor.ChildNodes.item(0).SelectSingleNode("properties").ChildNodes.Item($i).InnerText = $Boxes[$i].text
		if($Boxes[$i].text -ne "")
		{
			$RequiredFildErrorProvider.Clear()
		}
	}
	for ($i=0;$i -lt $propCount; $i++)
	{	
		$xml1.Monitor.ChildNodes.item(0).SelectSingleNode("properties").ChildNodes.Item($i).InnerText = $Boxes[$i].text
		if($Boxes[$i].text -eq "")
		{
			$RequiredFildErrorProvider.SetError($Boxes[$i], "Required field");
		}
	}
	if ($instance.items.count -eq 0)
	{
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("InstanceName").InnerText = ""
	}
	else
	{
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("InstanceName").InnerText = $instance.SelectedItem.Tostring()
	}
	
	$tagExists = 0
	for ($i = 0; $i -lt $tabs.Items.Count;++$i)
	{	
		if($tabs.text -eq $tabs.Items[$i] )			
		{
			$tagExists ++
		}
	}	
	if($tagExists -gt 0)
	{
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").SetAttribute("exists","true") 
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").SetAttribute("ID",$tabs_obj.item($tabs.text))
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").InnerText = $tabs.Text
	}
	else {
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").SetAttribute("exists","false") 
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").SetAttribute("ID","")
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").InnerText = $tabs.Text
	}
	if($tabs.text -eq "")
	{
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").SetAttribute("exists","false") 
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").SetAttribute("ID","")
		$xml1.Monitor.childnodes.item(0).SelectSingleNode("DashboardTag").InnerText = ""
	}
	if ($count -gt 0)
	{
		$xml1.Save($newFile);
	}
	else
	{
		$xml1.Save($newFile);
	}
	#count of filled fillds
	$IsChecked = 0
	if ($instance.SelectedItem -eq $null)
	{
		$IsChecked = 1
		$RequiredFildErrorProvider.SetError($instance, "Required field");
	}
	for ($j = 0; $j -lt $xml1.Monitor.childnodes.item(0).SelectSingleNode("properties").ChildNodes.Count; $j++)
	{
		if($xml1.Monitor.childnodes.item(0).SelectSingleNode("properties").ChildNodes.Item($j).InnerText -eq "")
		{
			$IsChecked = $IsChecked + 1
		}
	}
	
	# fields filled checking
	if($IsChecked -le 0)
	{
		$Form.Visible=$false
		$Form.Close()
		& $path"\metrics.ps1"
	}
}

function GetInstances
{ 	
	try
	{
		$comp =  $Boxes[$HostNameindex].text
		$job = get-wmiobject -class "win32_operatingsystem" -computer $comp -asjob
		Start-Sleep 1
		$status = $job.state
	}
	catch{}
	$instance.items.clear()
	$class = $xml1.Monitor.childnodes.item(0).SelectSingleNode("metrics").childnodes.item(0).getattribute("WMIclass")
	if ($status -eq "Completed")
	{
		$ConnectionErrorProvider.Clear()
		$Boxes[$HostNameindex].ForeColor = [System.Drawing.Color]::Black
		Try
		{
		    $inst = Get-WmiObject  -computer $Boxes[$HostNameindex].text  -Class $class -ErrorAction "Stop" |Select-object name 
		}
		Catch [Exception]
		{}
		foreach($i in $inst)
		{
			$inst_list = $inst_list +$i.name
		}
		try
		{
			For ($p = 0 ; $p -lt $inst_list.length; $p++ )
			{
				$instance.items.addrange($inst_list[$p])
			}
		}
		catch {}
		if ($xml1.Monitor.ChildNodes.item(0).SelectSingleNode("InstanceName").InnerText -ne "")
		{
			$instance.text = $xml1.Monitor.childnodes.item(0).SelectSingleNode("InstanceName").InnerText
		}
		else
		{
			$instance.SelectedItem  = $inst_list[0]
		}
	}
	else
	{
		$ConnectionErrorProvider.SetError($Boxes[$HostNameindex], "Connection Failed!Incorrect host name");
		$Boxes[$HostNameindex].ForeColor = [System.Drawing.Color]::Red
	}
}
function GetPages
{
	$xmlHttp = New-Object -ComObject Microsoft.XMLHTTP
	$xmlHttp.Open("GET", "http://$PortalUrl/api?apikey="+$apikey+"&output=xml&version=2&action=pages", $false)
	$xmlHttp.Send()
	$response = $xmlHttp.ResponseText
	$responseXml = $response -as [xml]
	if ($responseXml.Error) {
		Write-Error -Message $responseXml.Error
	} elseif ($responseXml.Status) {
		Write-Error -Message $responseXml.Status
	} else {
		$responseXml | 
		Select-Xml //page | 
		ForEach-Object { 
			$tabs_obj.add($_.Node.title,$_.Node.Id)
		}
	}
	$tabNames = @()
	foreach($i in $tabs_obj.keys)
	{
		$tabNames = $tabNames +$i
	}
	foreach($tab in $tabNames)
	{
		$tabs.items.add($tab)
	}
}
#-------------------------------------------------------------------------------------------------------------
# declaration of variables 
$btn_Next = New-Object System.Windows.Forms.Button
$btn_Back = New-Object System.Windows.Forms.Button
$Panel = New-Object System.Windows.Forms.Panel
$txtBx1 = New-Object System.Windows.Forms.TextBox
$txtBx2 = New-Object System.Windows.Forms.TextBox
$txtBx3 = New-Object System.Windows.Forms.TextBox
$txtBx4 = New-Object System.Windows.Forms.TextBox
$txtBx5 = New-Object System.Windows.Forms.TextBox
$txtBx6 = New-Object System.Windows.Forms.TextBox
$txtBx7 = New-Object System.Windows.Forms.TextBox
$txtBx8 = New-Object System.Windows.Forms.TextBox
$instance_lbl = New-Object System.Windows.Forms.Label
$instance = New-Object System.Windows.Forms.ComboBox
$tabs = New-Object System.Windows.Forms.ComboBox
$tabs_lbl = New-Object System.Windows.Forms.Label
$Boxes = @($txtBx1,$txtBx2,$txtBx3,$txtBx4,$txtBx5,$txtBx6,$txtBx7,$txtBx8)
$toolTip = New-Object System.Windows.Forms.ToolTip

#create Property window
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(370, 370)
$Form.Cursor = [System.Windows.Forms.Cursors]::Default
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.MaximizeBox = $false
$Form.MinimizeBox = $false
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$Form.ResumeLayout($false)
$Form.PerformLayout
$Form.Text = $windowName 

$RequiredFildErrorProvider = New-Object System.Windows.Forms.ErrorProvider
$ConnectionErrorProvider = New-Object System.Windows.Forms.ErrorProvider
$buttonToolTip = New-Object System.Windows.Forms.ToolTip
$buttonToolTip.UseFading = $true;
$buttonToolTip.UseAnimation = $true;
$buttonToolTip.ShowAlways = $true;
$buttonToolTip.AutoPopDelay = 2000;
$buttonToolTip.InitialDelay = 50;

#button next
$btn_Next.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Next.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Next.Location =  New-Object System.Drawing.Point(291, 342);
$btn_Next.Size = New-Object System.Drawing.Size(75, 23)
$btn_Next.Text = "Next>";
$btn_Next.UseVisualStyleBackColor = $false;


#button Back
$btn_Back.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Back.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Back.Location =New-Object System.Drawing.Point(213, 342);
$btn_Back.Size = New-Object System.Drawing.Size(75, 23);
$btn_Back.Text = "<Back";
$btn_Back.UseVisualStyleBackColor = $false;

#tab Control
$Panel.Location = New-Object System.Drawing.Point(0, 0);
$Panel.Size = New-Object System.Drawing.Size(370, 334);
$Panel.BackColor = [System.Drawing.Color]::GhostWhite

#TextBoxes
$txtBx1.Location = New-Object System.Drawing.Point(144, 22)
$txtBx1.Size = New-Object System.Drawing.Size(155, 21)

$txtBx2.Location = New-Object System.Drawing.Point(144, 59)
$txtBx2.Size = New-Object System.Drawing.Size(155, 21)

$txtBx3.Location = New-Object System.Drawing.Point(144, 96)
$txtBx3.Size = New-Object System.Drawing.Size(155, 21)

$txtBx4.Location = New-Object System.Drawing.Point(144, 133)
$txtBx4.Size = New-Object System.Drawing.Size(155, 21)

$txtBx5.Location = New-Object System.Drawing.Point(144, 170)
$txtBx5.Size = New-Object System.Drawing.Size(155, 21)

$txtBx6.Location = New-Object System.Drawing.Point(144, 207)
$txtBx6.Size = New-Object System.Drawing.Size(155, 21)

$txtBx7.Location = New-Object System.Drawing.Point(144, 244)
$txtBx7.Size = New-Object System.Drawing.Size(155, 21)

$txtBx8.Location = New-Object System.Drawing.Point(144, 281)
$txtBx8.Size = New-Object System.Drawing.Size(155, 21)

#instance
$instance.Size = New-Object System.Drawing.Size(155, 21)
$instance.DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDownList
$instance.AllowDrop = $true;
$instance_lbl.Text = "Instance"


#dashboard tabs
$tabs.Size = New-Object System.Drawing.Size(155, 21)
$tabs.DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDown
$tabs.AllowDrop = $true;
$tabs_lbl.Text = "Dashboard Tab"
if ($connect_to -eq "Monitor.us")
{
	$tabs.Enabled = $false
}
#-------------------------------------------------------------------------------------------------------------
#add controls to form
$Form.Controls.Add($btn_Back)
$Form.Controls.Add($btn_Next)
$Form.Controls.Add($Panel)

#events
$Form.add_Load({Form_Load})
$btn_Back.add_Click({btnBack})
$btn_Next.add_Click({btnNext})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()
