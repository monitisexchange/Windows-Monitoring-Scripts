[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Xml")
clear-host
if($newFile -eq $null)
{
	New-Variable -Name newFile  -Value 0 -Visibility Public -Option AllScope
}
#get OS name
$OS  = Systeminfo | find "OS Name"
$d = $OS.ToLower()
$path = Get-Location
#load XML file
[xml]$xml = (Get-Content $path"\AppList.xml")

#Close the current window
function btnExit
{
	$res = [System.Windows.Forms.MessageBox]::Show("Are you sure?","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
	if($res -eq [System.Windows.Forms.DialogResult]::Yes)
	{
		$Form.Close()
	}
}		

#if any application is checked go to the next window
$IsChecked = 0
function btn_Next
{	
	for ($i = 0; $i -lt $AppCount; $i++) 
	{
		if($radiobtn[$i].Checked -eq $true)
		{
			$xml.Application.ChildNodes.Item($i).InnerText = "true"
			$xml.Save("AppList.xml");
		}
		else
		{
			$xml.Application.ChildNodes.Item($i).InnerText = "false"
			$xml.Save("AppList.xml");
		}
	}
	foreach($item in $xml.Application.ChildNodes)
	{
		if($item.InnerText -eq "false")
		{
			$IsChecked = $IsChecked+1
		}
	}
	if($IsChecked -eq $xml.Application.ChildNodes.Count)
	{
		[System.Windows.Forms.MessageBox]::Show("Please select any application")
	}
	else
	{
		[xml]$xml = (Get-Content $path"\AppList.xml")
		foreach($al in $xml.Application.ChildNodes)
		{
			if($al.InnerText -eq "true")
			{
				$bbb = $al.GetAttribute("config")
				$newFile = "$path\metrics$(Get-Random).xml"
				Copy-Item $path"\$bbb" $newFile
				$new = '"'+"C:\Program Files\WMIMonitor"+ '"'
				#read username and password from XML file
				if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
				{
					$command ="ICacls $new /grant Users:F /T"
					cmd.exe /c $command
				}
				[xml]$xml1 = (Get-Content $newFile)
			}
		}
		$Form.Close();
		$Form.Visible=$false
		& $path"\propwin.ps1"
	}
}
#-------------------------------------------------------------------------------------------------------------
# declaration of variables 
$btn_Next = New-Object System.Windows.Forms.Button
$btn_Cancel = New-Object System.Windows.Forms.Button
$btn_Exit = New-Object System.Windows.Forms.Button
$detailPanel = New-Object System.Windows.Forms.Panel
$Panel = New-Object System.Windows.Forms.Panel
$btn_oldmonitors = New-Object System.Windows.Forms.Button
$lbl = New-Object System.Windows.Forms.Label

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
$Form.Text = "Select Application"

#checkListBox
[xml] $xml1 = (Get-Content $path"\AppList.xml")
$AppCount = $xml1.Application.Childnodes.Count
$radiobtn = new-object system.Windows.Forms.RadioButton[] $AppCount

for ($i = 0; $i -lt $AppCount; $i++) 
{
	$radiobtn[$i] = New-Object System.Windows.Forms.RadioButton
	$b = $i*38+10
	$radiobtn[$i].Location = New-Object System.Drawing.Point(20,$b);
	$radiobtn[$i].Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 13, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Point, 0);
	$radiobtn[$i].Text = $xml1.Application.ChildNodes.Item($i).ToString()
	$radiobtn[$i].Size = New-Object System.Drawing.Size(300,25)
	$Panel.Controls.Add($radiobtn[$i])
}

#Next button
$btn_Next.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Next.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Next.Location = New-Object System.Drawing.Point(222, 342);
$btn_Next.Size = New-Object System.Drawing.Size(75, 23)
$btn_Next.Text = "Next>";
$btn_Next.UseVisualStyleBackColor = $false;



#cancel button
$btn_Cancel.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Cancel.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Cancel.Location = New-Object System.Drawing.Point(144, 342);
$btn_Cancel.Size = New-Object System.Drawing.Size(75, 23)
$btn_Cancel.Text = "Cancel";
$btn_Cancel.UseVisualStyleBackColor = $false;

#detail button
$btn_oldmonitors.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_oldmonitors.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_oldmonitors.Location = New-Object System.Drawing.Point(3, 342);
$btn_oldmonitors.Size = New-Object System.Drawing.Size(80,23)
$btn_oldmonitors.Text = "Old Monitors";
$btn_oldmonitors.UseVisualStyleBackColor = $false;


#Panel
$Panel.BackColor = [System.Drawing.Color]::GhostWhite
$Panel.BorderStyle = [System.Windows.Forms.BorderStyle]::Fixed3D
$Panel.Location = New-Object System.Drawing.Point(0,0);
$Panel.Size = New-Object System.Drawing.Size(300, 334);


#-------------------------------------------------------------------------------------------------------------

#add controls to form
$Form.Controls.Add($btn_Cancel)
$Form.Controls.Add($btn_Next)
$Form.Controls.Add($Panel)
#events
$btn_Next.Add_Click({btn_Next})
$btn_Cancel.add_Click({btnExit})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()


