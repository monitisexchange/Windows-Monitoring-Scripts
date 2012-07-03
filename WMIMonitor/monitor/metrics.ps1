[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")

$path = Get-Location

#get OS name
$OS  = Systeminfo | find "OS Name"
$d = $OS.ToLower()

#Close the current window
function ExitB
{
	$res = [System.Windows.Forms.MessageBox]::Show("Are you sure?","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
	if($res-eq[System.Windows.Forms.DialogResult]::Yes)
	{
		$Form.Close()
	}
}


#load XML file
[xml]$xml = (Get-Content $path"\metrics.xml")

#get selected applications from XML
$count = 0
foreach($al in $xml.Monitor.ChildNodes )
{
	if($al.SelectSingleNode("IsSelect").InnerText -eq "true")
	{
		#selected application count
		$count++
		[string[]] $selectedApp +=  $al.ToString()
	}
}
for ( $j = 0; $j -lt $Count; $j++)
{
	#count of each application properties
	[int[]]$a +=  $xml.Monitor.SelectSingleNode($selectedApp[$j]).SelectSingleNode("properties").ChildNodes.Count
}

#show the previous window
function btnBack
{
	$Form.Visible=$false
	$Form.Close()
	& $path"\propwin.ps1"
}

#Add the required number of metrics  to selected tab
function Form_Load
{
	TabSelect	
}

#remove metrics from deselected tabs
function TabDeselect
{	
	$checkListBx.Items.Clear()
}

#add metrics in checklistbox and  show the last monitored  metrics
function TabSelect
{
	TabDeselect
	for($i = 0; $i -le $xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		$xmlcontentmetrics = $xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Item($i).Name		
		$checkListBx.Items.AddRange($xmlcontentmetrics)
		$tab[$AppTab.SelectedIndex].Controls.Add($checkListBx)
	}	

	for($i = 0; $i -le $xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		if($xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText -eq "true")
		{
			if($xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Item($i).Name -eq $checkListBx.Items[$i])
			{	
				$checkListBx.SetItemChecked($checkListBx.Items.IndexOf($xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Item($i).Name),$true)
			}
		}
	}
}

#write true in XML if metric is checked or false if it isn't checked
function OnCheck
{
	try
	{
		for ($j = 0; $j -lt $xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Count; $j++)
		{
			if ($checkListBx.GetItemCheckState($j) -eq [System.Windows.Forms.CheckState]::Unchecked)
			{
				$xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Item($j).InnerText = "false"
				$xml.Save("metrics.xml");
			}
			else
			{	
				$xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("metrics").ChildNodes.Item($j).InnerText = "true"
				$xml.Save("metrics.xml");
			}
		}
	}
	catch{}
}


#go to the next window
[int[]]$IsCheckedArr = @()
function Next_btn
{
	for ( $k = 0; $k -lt $Count; $k++)
	{
		#count of cheked metrics
		$IsChecked = 0
		for ($j = 0; $j -lt $xml.Monitor.SelectSingleNode($selectedApp[$k]).SelectSingleNode("metrics").ChildNodes.Count; $j++)
		{
			if($xml.Monitor.SelectSingleNode($selectedApp[$k]).SelectSingleNode("metrics").ChildNodes.Item($j).InnerText -eq "true")
			{
				$IsChecked = $IsChecked + 1
			}
		}
		[int[]]$IsCheckedArr += $IsChecked
	}
	
	for ( $k = 0; $k -lt $Count; $k++)
	{
		if($IsCheckedArr[$k] -eq 0)
		{
			$cnt = $cnt+1
		}
	}
	
	#if any metric from each application is checked go to the next window
	if ($cnt -gt 0)
	{
		[System.Windows.Forms.MessageBox]::Show("Choose metrics")
	}
	
	else
	{
		if ($d.Contains("windows 7"))
		{
			$Form.Visible=$false
			$Form.Close()
			& $path"\scheduler.ps1"
		}
		elseif($d.Contains("windows xp"))
		{
			$Form.Visible=$false
			$Form.Close()
			& $path"\user.ps1"
		}
	}
}

#Getting data for test
function TestBtn_Click
{
	for ( $k = 0; $k -lt $Count; $k++)
	{
		#count of cheked metrics
		$IsChecked = 0
		for ($j = 0; $j -lt $xml.Monitor.SelectSingleNode($selectedApp[$k]).SelectSingleNode("metrics").ChildNodes.Count; $j++)
		{
			if($xml.Monitor.SelectSingleNode($selectedApp[$k]).SelectSingleNode("metrics").ChildNodes.Item($j).InnerText -eq "true")
			{
				$IsChecked = $IsChecked + 1
			}
		}
		[int[]]$IsCheckedArr += $IsChecked
	}
	
	for ( $k = 0; $k -lt $Count; $k++)
	{
		if($IsCheckedArr[$k] -eq 0)
		{
			$cnt = $cnt+1
		}
	}
	#if any metric from each application is checked show test result
	if ($cnt -gt 0)
	{
		[System.Windows.Forms.MessageBox]::Show("Choose metrics")
	}
	else
	{
		  & $path"\TestData.vbs"
	}
}

#-------------------------------------------------------------------------------------------------------------
# declaration of variables 
$btn_Next = New-Object System.Windows.Forms.Button
$btn_back = New-Object System.Windows.Forms.Button
$checkListBx = New-Object System.Windows.Forms.CheckedListBox
$AppTab = New-Object System.Windows.Forms.TabControl
$tab  = new-object System.Windows.Forms.TabPage[] $Count
$btn_Exit = New-Object System.Windows.Forms.Button
$btn_Test = New-Object System.Windows.Forms.Button

#create metrics window
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
$Form.text = "Select Metrics"

	
#button next
$btn_Next.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Next.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Next.Location = New-Object System.Drawing.Point(291, 342);
$btn_Next.Size = New-Object System.Drawing.Size(75, 23)
$btn_Next.Text = "Next>";
$btn_Next.UseVisualStyleBackColor = $false;

#button Test
$btn_Test.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Test.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Test.Location = New-Object System.Drawing.Point(5, 342);
$btn_Test.Size = New-Object System.Drawing.Size(75, 23)
$btn_Test.Text = "Test";
$btn_Test.UseVisualStyleBackColor = $false;


#back button
$btn_back.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_back.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_back.Location = New-Object System.Drawing.Point(213, 342);
$btn_back.Size = New-Object System.Drawing.Size(75, 23)
$btn_back.Text = "<Back";
$btn_back.UseVisualStyleBackColor = $false;

#tab Control
$AppTab.Location = New-Object System.Drawing.Point(0,0);
$AppTab.Size = New-Object System.Drawing.Size(370, 334);
$Apptab.SelectedIndex = 0;
$Apptab.BackColor = [System.Drawing.Color]::GhostWhite



#adding the required number of tabs to window
for ( $j = 0; $j -lt $Count; $j++)
{	
	$tab[$j] = New-Object System.Windows.Forms.TabPage 
	$tab[$j].Location = New-Object System.Drawing.Point(4, 22);
	$tab[$j].Padding = New-Object System.Windows.Forms.Padding(3);
	$tab[$j].Size = New-Object System.Drawing.Size(348, 320);
	$tab[$j].BackColor = [System.Drawing.Color]::GhostWhite
	$tab[$j].Text = $selectedApp[$j]
	$AppTab.Controls.Add($tab[$j])
}

#checkListBox
$checkListBx.BackColor = [System.Drawing.Color]::GhostWhite
$checkListBx.BorderStyle = [System.Windows.Forms.BorderStyle]::None
$checkListBx.CheckOnClick = $true;
$checkListBx.FormattingEnabled = $true;
$checkListBx.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 10, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Point, 0);
$checkListBx.Location = New-Object System.Drawing.Point(0, 2);
$checkListBx.Size = New-Object System.Drawing.Size(360, 320);
#-------------------------------------------------------------------------------------------------------------

#add controls
$Form.Controls.Add($btn_back)
$Form.Controls.Add($btn_Next)
$Form.Controls.Add($AppTab)
$Form.Controls.Add($btn_Test)

#events
$btn_back.add_Click({btnBack})
$AppTab.Add_click({TabSelect})
$AppTab.Add_Deselected({TabDeselect})
$btn_Test.add_Click({TestBtn_Click})
$Form.add_Load({Form_Load})
$checkListBx.add_MouseUp({OnCheck})
$btn_Next.add_click({Next_btn})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()

