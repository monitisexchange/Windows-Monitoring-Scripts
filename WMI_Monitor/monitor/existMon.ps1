[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Xml")
clear-host
$path = Get-Location
if($FilePath -eq $null)
{
	New-Variable -Name FilePath  -Value "" -Visibility Public -Option AllScope
}
#load XML file 
[xml]$xml = (Get-Content $path"\AppList.xml")

$path = Get-Location
[xml]$xml1 = (Get-Content $path"\ApiKey.xml")
$apikey = $xml1.Key.ApiKey
$secretkey = $xml1.Key.SecretKey
$connect_to = $xml1.Key.ConnectTo
if ($connect_to -eq "Monitis")
{
	$PortalUrl = "www.monitis.com"
}
elseif($connect_to -eq "Monitor.us")
{
	$PortalUrl = "www.monitor.us"
}
$wc = new-object net.webclient
$url = "http://$PortalUrl/api?action=authToken&apikey=$apikey&secretkey=$secretkey"
$resp = $wc.DownloadString($url).ToString()
$pos = $resp.IndexOf(":") + 2
$token = $resp.Substring($pos, $resp.Length - $pos - 2)
$time=((get-date).touniversaltime().ToString("yyyy-MM-dd HH:mm:ss").ToString())
$xmlHttp = New-Object -ComObject Microsoft.XMLHTTP

function Form_load
{
	$i = -1
	$j = 1
	#Get monitors information from Monitis
	$xmlHttp.Open("Get", "http://$PortalUrl/customMonitorApi?apikey=$apikey&validation=token&authToken=$token&output=xml&timestamp=$time&version=2&action=getMonitors", $false)
	$xmlHttp.Send()
	$response1 = $xmlHttp.ResponseText
	$responseXml1 = $response1 -as [xml]
	
	#Get all configuration files of existing monitors
	foreach($a in [system.IO.directory]::GetFiles($path,"metrics*.xml"))
	{
		[xml]$allxmls = (Get-Content $a)
		foreach($item in  $allxmls.Monitor.ChildNodes)
		{
			if($item.SelectSingleNode("monitorID").InnerText.tostring() -ne "0")
			{
				#Show existing monitors in window 
				for($b=0;$b -lt $responseXml1.monitors.childnodes.count; $b++)
				{
					if($item.SelectSingleNode("monitorID").InnerText.tostring()-eq $responseXml1.monitors.ChildNodes.Item($b).SelectSingleNode("id").InnerText)
					{	
						$i++
						$rows = New-Object System.Windows.Forms.DataGridViewRow
						$oldMonitors_dataGrid.Rows.Add($rows);
						$oldMonitors_dataGrid.Rows[$i].Cells[2].value = $item.name+$j
						$oldMonitors_dataGrid.Rows[$i].Cells[2].Tag = $item.name
						$oldMonitors_dataGrid.Rows[$i].Cells[1].value = $a
					}
				}
			}
		}
		$j++
	}
	if ($oldMonitors_dataGrid.RowCount -eq 0)
	{
		$lbl = New-Object System.Windows.Forms.Label
		$lbl.Location = New-Object System.Drawing.Point(2,0)
		$lbl.Size =  New-Object System.Drawing.Size(296, 50)
		$lbl.BackColor = [System.Drawing.Color]::GhostWhite
		$lbl.Text = "You do not have any monitors.
Press <Create New> button to create new monitor"
		$oldMonitors_dataGrid.Controls.Add($lbl)
	}
	$oldMonitors_dataGrid.Columns[1].visible = $false
}
#go to the next window
Function NewMonitor_Click
{
	$Form.Close();
	$Form.Visible=$false
	& $path"\selwin.ps1"
}

function  detail_click
{
	for($i=0; $i -lt $oldMonitors_dataGrid.RowCount; $i++)
	{
		$findxml=""
		$monitorID=""
		$response=""
		if(($oldMonitors_dataGrid.Rows[$i].cells[0].Value -eq [System.Windows.Forms.CheckState]::checked))
		{	
			$tag = $oldMonitors_dataGrid.Rows[$i].cells[2].Tag
			$findxml = $oldMonitors_dataGrid.Rows[$i].cells[1].Value
			[xml]$xml2 = (Get-Content $findxml)
			$monitorID = $xml2.Monitor.SelectSingleNode("$tag").SelectSingleNode("monitorID").Innertext
			#Show monitor details:monitor tag, monitor name,monitored metrics
			$xmlHttp.Open("GET", "http://$PortalUrl/customMonitorApi?apikey=$apikey&output=xml&version=2&timestamp=$time&action=getMonitorInfo&monitorId=$monitorID&excludeHidden=true", $false)
			$xmlHttp.Send()
			$response = $xmlHttp.ResponseText
			$responseXml = $response -as [xml]
			$text = "Tag = " + $responseXml.monitor.SelectSingleNode("tag").InnerText + [char]13 +"Name = "+$responseXml.monitor.SelectSingleNode("name").InnerText + [char]13 + "Metrics = {"	
			for ($j=0;$j -lt $responseXml.monitor.resultParams.ChildNodes.count;$j++)
			{
				$text =  $text + [char]13 + $responseXml.monitor.resultParams.ChildNodes.Item($j).SelectSingleNode("displayName").InnerText 
			}
			$text = $text + [char]13 + "}"
			[System.Windows.Forms.MessageBox]::Show($text,"Monitor Info",[System.Windows.Forms.MessageBoxButtons]::OK)
		}
	}
}

function  delete_click
{ 
	#delete monitor and all corresponding data from Monitis
	for($i=0; $i -lt $oldMonitors_dataGrid.RowCount; $i++)
	{
		if(($oldMonitors_dataGrid.Rows[$i].cells[0].Value -eq [System.Windows.Forms.CheckState]::checked))
		{
			$res = [System.Windows.Forms.MessageBox]::Show("Are you sure?","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
			if($res-eq[System.Windows.Forms.DialogResult]::Yes)
			{
				try
				{
					$xmlHttp.Open("POST", "http://$PortalUrl/customMonitorApi?apikey=$apikey&validation=token&authToken=$token&timestamp=$time&version=2&action=deleteMonitor&monitorId=$monitorID", $false)
					$xmlHttp.Send()
					$file =  $oldMonitors_dataGrid.Rows[$oldMonitors_dataGrid.CurrentCell.RowIndex].Cells[1].value
					[xml]$xml3 = (Get-Content $file)
					$taskName = $xml3.Monitor.ChildNodes.Item(0).SelectSingleNode("task").InnerText
					$Command = "schtasks.exe /delete /tn $taskName /f >a.txt"
					cmd.exe /c $Command
					clear-host 
					[xml]$xml4 = (Get-Content $oldMonitors_dataGrid.Rows[$oldMonitors_dataGrid.CurrentCell.RowIndex].Cells[1].value)
					$xmlremove = $xml4.Monitor.Childnodes.Item(0).SelectSingleNode("filepath").InnerText
					$prevaluefile = "$path\$xmlremove"
					Remove-Item $prevaluefile
					Remove-Item $oldMonitors_dataGrid.Rows[$oldMonitors_dataGrid.CurrentCell.RowIndex].Cells[1].value
					$oldMonitors_dataGrid.Rows.Remove($oldMonitors_dataGrid.Rows[$oldMonitors_dataGrid.CurrentCell.RowIndex]);
					$Form.refresh
				}
				catch{}
			}
		}
	}
}
#-------------------------------------------------------------------------------------------------------------
#declaration of variables 
$detailPanel = New-Object System.Windows.Forms.Panel
$btn_NewMonitor = New-Object System.Windows.Forms.Button

#Create Select application form
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(300, 370)
$Form.Cursor = [System.Windows.Forms.Cursors]::Default
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$Form.ResumeLayout($false)
$Form.PerformLayout
$Form.Text = "Existed Monitor List"

#old monitor datagrid 
$oldMonitors_dataGrid = New-Object System.Windows.Forms.DataGridView
$NameCol = New-Object System.Windows.Forms.DataGridViewTextBoxColumn
$root = New-Object System.Windows.Forms.DataGridViewTextBoxColumn
$toolTip = New-Object System.Windows.Forms.ToolTip
$IDCol = New-Object System.Windows.Forms.DataGridViewCheckBoxColumn
$btn_detail = New-Object System.Windows.Forms.Button
$btn_delete = New-Object System.Windows.Forms.Button

$NameCol.name = "Name"
$NameCol.ReadOnly = $true
$NameCol.width = 245
$IDCol.width = 50
$oldMonitors_dataGrid.AllowUserToAddRows = $false;
$oldMonitors_dataGrid.AllowUserToDeleteRows = $false
$oldMonitors_dataGrid.AllowUsertoResizeRows = $false
$oldMonitors_dataGrid.AllowUsertoResizeColumns = $false
$oldMonitors_dataGrid.AllowUserToOrderColumns = $false
$oldMonitors_dataGrid.ColumnHeadersHeightSizeMode = [System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode]::AutoSize
$oldMonitors_dataGrid.Location = New-Object System.Drawing.Point(2, 0);
$oldMonitors_dataGrid.Size = New-Object System.Drawing.Size(300, 340)
$oldMonitors_dataGrid.BackGroundColor = [System.Drawing.Color]::GhostWhite
$oldMonitors_dataGrid.Cellborderstyle =[System.Windows.Forms.DataGridViewCellBorderStyle]::Single
$oldMonitors_dataGrid.borderstyle = [System.Windows.Forms.BorderStyle]::Fixed3D
$oldMonitors_dataGrid.RowHeadersVisible = $false
$oldMonitors_dataGrid.ColumnHeadersVisible = $false
$oldMonitors_dataGrid.Columns.add($IDCol)
$oldMonitors_dataGrid.Columns.add($root)
$oldMonitors_dataGrid.Columns.add($NameCol)
	

#Create button
$btn_NewMonitor.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_NewMonitor.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_NewMonitor.Location = New-Object System.Drawing.Point(218, 343);
$btn_NewMonitor.Size = New-Object System.Drawing.Size(80,23)
$btn_NewMonitor.Text = "Create New";
$btn_NewMonitor.UseVisualStyleBackColor = $false;
$toolTip.SetToolTip($btn_NewMonitor, "Create new custom monitor.");


#detail button
$btn_detail.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_detail.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_detail.Location = New-Object System.Drawing.Point(115, 343);
$btn_detail.Size = New-Object System.Drawing.Size(80,23)
$btn_detail.Text = "Detail";
$btn_detail.UseVisualStyleBackColor = $false;
$toolTip.SetToolTip($btn_detail, "View custom monitor details.");

#delete button
$btn_delete.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_delete.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_delete.Location = New-Object System.Drawing.Point(5, 343);
$btn_delete.Size = New-Object System.Drawing.Size(80,23)
$btn_delete.Text = "Delete";
$btn_delete.UseVisualStyleBackColor = $false;
$toolTip.SetToolTip($btn_delete, "Remove custom monitor.");

#-------------------------------------------------------------------------------------------------------------
#add controls to form
$Form.Controls.Add($btn_NewMonitor)
$Form.Controls.Add($btn_detail)
$Form.Controls.Add($btn_delete)
$Form.Controls.add($oldMonitors_dataGrid)

#events
$Form.add_load({Form_load})
$oldMonitors_dataGrid.add_DataError({})
#$oldMonitors_dataGrid.Add_CellContentClick({cell_click})
$btn_NewMonitor.Add_Click({NewMonitor_Click})
$btn_detail.add_click({detail_click})
$btn_delete.add_click({delete_click})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()


