[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")


$path = Get-Location

#Close the current window
function btnExit
{
	$res = [System.Windows.Forms.MessageBox]::Show("Are you sure?","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
	if($res-eq[System.Windows.Forms.DialogResult]::Yes)
	{
		$Form.Close()
	}
}

#load XML file
[xml]$xml = (Get-Content $path"\metrics.xml")

$count = 0
$selectedApp = @()
$tab = @()
$a = @()

#get selected applications from XML
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

#add the required number of tabs,labels,textboxes to window
function Form_Load
{
	try
	{
		for ( $j = 0; $j -lt $Count; $j++)
		{	
			$lblText = @()
			$lbl = new-object System.Windows.Forms.Label[] $a[$j]

			[xml]$xml = (Get-Content $path"\metrics.xml")
			foreach ($node in $xml.Monitor.SelectSingleNode($selectedApp[$j]).SelectSingleNode("properties").ChildNodes)
			{
				[string[]]$lblText += $node.ToString()
			}
			
			for ( $i = 0 ; $i -lt $a[$j]; $i++)
			{	
				TabSelect
				$Boxes[$i].Text = $xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("properties").ChildNodes.Item($i).InnerText
				$b = $i*38+20
				$lbl[$i] = New-Object System.Windows.Forms.Label
				$lbl[$i].Location = New-Object System.Drawing.Point(20,$b)
				$lbl[$i].Text = $lblText[$i]
				$tab[$j].Controls.Add($lbl[$i])
			}
		}
	}
	catch
	{
	}
}

#write each application properties in XML document
function TabDeselect
{
	try
	{
		for ($i=0;$i -lt $a[$AppTab.SelectedIndex]; $i++)
		{ 
			$xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("properties").ChildNodes.Item($i).InnerText = $Boxes[$i].text
			$xml.Save("metrics.xml");
			$tab[$AppTab.SelectedIndex].Controls.Remove($Boxes[$i])
			$Boxes[$i].text = ""
		}
	}
	catch{}
}

#read properties' values from XML for each application
function TabSelect
{
	for ($i=0; $i -lt $a[$AppTab.SelectedIndex]; $i++)
	{	
		$tab[$AppTab.SelectedIndex].Controls.Add($Boxes[$i])
		$Boxes[$i].Text = $xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("properties").ChildNodes.Item($i).InnerText
	}
}

#show the previous window
function btnBack
{
	$Form.Visible=$false
	$Form.Close()
	& $path"\selwin.ps1"
}


#save each application properties values in xml document
#validation checking for fields
#if all fields are filled go to the next window
function btnNext
{
	try
	{
		for ($i=0;$i -lt $a[$AppTab.SelectedIndex]; $i++)
		{	
			$xml.Monitor.SelectSingleNode($selectedApp[$AppTab.SelectedIndex]).SelectSingleNode("properties").ChildNodes.Item($i).InnerText = $Boxes[$i].text
			$xml.Save("metrics.xml");
		}
	}
	catch{}

	#count of filled fillds
	$IsChecked = 0
	for ( $k = 0; $k -lt $Count; $k++)
	{
		for ($j = 0; $j -lt $xml.Monitor.SelectSingleNode($selectedApp[$k]).SelectSingleNode("properties").ChildNodes.Count; $j++)
		{
			if($xml.Monitor.SelectSingleNode($selectedApp[$k]).SelectSingleNode("properties").ChildNodes.Item($j).InnerText -eq "")
			{
				$IsChecked = $IsChecked + 1
				
			}
		}
	}
	# fields filled checking
	if ($IsChecked -gt 0)
	{
		[System.Windows.Forms.MessageBox]::Show("Fill all fields")
	}
	
	else
	{
		$Form.Visible=$false
		$Form.Close()
		& $path"\metrics.ps1"
	}
	
}
#-------------------------------------------------------------------------------------------------------------
# declaration of variables 
$btn_Next = New-Object System.Windows.Forms.Button
$btn_Back = New-Object System.Windows.Forms.Button
$AppTab = New-Object System.Windows.Forms.TabControl
$tab  = new-object System.Windows.Forms.TabPage[] $Count
$txtBx1 = New-Object System.Windows.Forms.TextBox
$txtBx2 = New-Object System.Windows.Forms.TextBox
$txtBx3 = New-Object System.Windows.Forms.TextBox
$txtBx4 = New-Object System.Windows.Forms.TextBox
$txtBx5 = New-Object System.Windows.Forms.TextBox
$txtBx6 = New-Object System.Windows.Forms.TextBox
$txtBx7 = New-Object System.Windows.Forms.TextBox
$txtBx8 = New-Object System.Windows.Forms.TextBox
$Boxes = @($txtBx1,$txtBx2,$txtBx3,$txtBx4,$txtBx5,$txtBx6,$txtBx7,$txtBx8)

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
$Form.TEXT = "Monitor Properties"

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
$AppTab.Location = New-Object System.Drawing.Point(0, 0);
$AppTab.Size = New-Object System.Drawing.Size(370, 334);
$Apptab.SelectedIndex = 0;
$Apptab.BackColor = [System.Drawing.Color]::GhostWhite

#adding tabs to window
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
#-------------------------------------------------------------------------------------------------------------

#add controls to form
$Form.Controls.Add($btn_Back)
$Form.Controls.Add($btn_Next)
$Form.Controls.Add($AppTab)
$Form.Controls.Add($TopPanel)

#events
$Form.add_Load({Form_Load})
$btn_Back.add_Click({btnBack})
$btn_Next.add_Click({btnNext})
$AppTab.Add_click({TabSelect})
$AppTab.Add_Deselected({TabDeselect})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()

