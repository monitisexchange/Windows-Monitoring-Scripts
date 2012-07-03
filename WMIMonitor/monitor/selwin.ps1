[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Xml")

$path = Get-Location

#load XML file
[xml]$xml = (Get-Content $path"\metrics.xml")

#Close the current window
function btnExit
{
	$res = [System.Windows.Forms.MessageBox]::Show("Are you sure?","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
	if($res-eq[System.Windows.Forms.DialogResult]::Yes)
	{
		$Form.Close()
	}
}
			
function Form_Load
{
	#Get application list from XML
	foreach($al in $xml.Monitor.ChildNodes )
	{
		$checkListBxApp.Items.AddRange($al.ToString())
	}
	#checked last monitored applicaations
	#and show as checked 
	foreach($item in $xml.Monitor.ChildNodes)
	{ 
		if($item.SelectSingleNode("IsSelect").InnerText-eq "true")
		{
			for($i=0;$i -lt $xml.Monitor.ChildNodes.Count;$i++ )
			{	
				if($checkListBxApp.Items[$i].ToString() -eq $item.toString())
				{	
					$checkListBxApp.SetItemChecked($checkListBxApp.Items.IndexOf($item.ToString()),$true)
				}
			}
		}
	}
}

#write application state in XML
#if it checked write true 
function OnCheck
{
	try
	{
		if ($checkListBxApp.GetItemCheckState($checkListBxApp.Items.IndexOf($checkListBxApp.SelectedItem)) -eq [System.Windows.Forms.CheckState]::UnChecked)
		{
			$res = [System.Windows.Forms.MessageBox]::Show("Are you sure? Your monitor will be stopped","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
			if($res-eq[System.Windows.Forms.DialogResult]::No)
			{				
				$checkListBxApp.SetItemChecked($checkListBxApp.Items.IndexOf($checkListBxApp.SelectedItem),$true)
				$xml.Save("metrics.xml");
				$checkListBxApp.ClearSelected()
			}
			if($res-eq[System.Windows.Forms.DialogResult]::Yes)
			{	
				$xml.Monitor.SelectSingleNode($checkListBxApp.SelectedItem.Tostring()).SelectSingleNode("IsSelect").InnerText = "false"
				$xml.Save("metrics.xml");
			}
			$checkListBxApp.ClearSelected()
		}
		
		else 
		{
			$xml.Monitor.SelectSingleNode($checkListBxApp.SelectedItem.Tostring()).SelectSingleNode("IsSelect").InnerText = "true"
			$xml.Save("metrics.xml");
		}
		
	}
	catch{}
}

#if any application is checked go to the next window
$IsChecked = 0
function btn_Next
{	
	foreach($item in $xml.Monitor.ChildNodes)
	{
		if($item.SelectSingleNode("IsSelect").InnerText -eq "false")
		{
			$IsChecked = $IsChecked+1
		}
	}
	
	if($IsChecked -eq $xml.Monitor.ChildNodes.Count)
	{
		[System.Windows.Forms.MessageBox]::Show("Mark any application")
	}
	else
	{
		$Form.Close();
		$Form.Visible=$false
		& $path"\propwin.ps1"
	}
}

#-------------------------------------------------------------------------------------------------------------
# declaration of variables 
$btn_Next = New-Object System.Windows.Forms.Button
$btn_Cancel = New-Object System.Windows.Forms.Button
$checkListBxApp = New-Object System.Windows.Forms.CheckedListBox
$btn_Exit = New-Object System.Windows.Forms.Button
$TopPanel = New-Object System.Windows.Forms.Panel

#Create Select application form
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(370, 370)
$Form.Cursor = [System.Windows.Forms.Cursors]::Default
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$Form.ResumeLayout($false)
$Form.PerformLayout
$Form.Text = "Select Application"


#checkListBox
$checkListBxApp.BackColor = [System.Drawing.Color]::GhostWhite
$checkListBxApp.BorderStyle = [System.Windows.Forms.BorderStyle]::Fixed3D
$checkListBxApp.CheckOnClick = $true;
$checkListBxApp.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 13, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Point, 0);
$checkListBxApp.FormattingEnabled = $true;
$checkListBxApp.Location = New-Object System.Drawing.Point(0,0);
$checkListBxApp.Size = New-Object System.Drawing.Size(370, 334);

#Next button
$btn_Next.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Next.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Next.Location = New-Object System.Drawing.Point(291, 342);
$btn_Next.Size = New-Object System.Drawing.Size(75, 23)
$btn_Next.Text = "Next>";
$btn_Next.UseVisualStyleBackColor = $false;


#cancel button
$btn_Cancel.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Cancel.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Cancel.Location = New-Object System.Drawing.Point(213, 342);
$btn_Cancel.Size = New-Object System.Drawing.Size(75, 23)
$btn_Cancel.Text = "Cancel";
$btn_Cancel.UseVisualStyleBackColor = $false;
#-------------------------------------------------------------------------------------------------------------

#add controls to form
$Form.Controls.Add($btn_Cancel)
$Form.Controls.Add($checkListBxApp)
$Form.Controls.Add($btn_Next)

#events
$btn_Exit.add_Click({btnExit})
$btn_Next.Add_Click({btn_Next})
$checkListBxApp.add_mouseup({OnCheck})
$Form.add_Load({Form_Load})
$btn_Cancel.add_Click({btnExit})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()


