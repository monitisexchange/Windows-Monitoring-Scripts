[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
clear-host
$path = Get-Location

#go to the next window
function btn_Next
{
	$Form.Visible=$false
	$Form.Close()
	& $path"\existMon.ps1"
}

#-------------------------------------------------------------------------------------------------------------
# declaration of variables
$btn_Next = New-Object System.Windows.Forms.Button
$WindowText = New-Object System.Windows.Forms.Label
$btn_Exit = New-Object System.Windows.Forms.Button
$TopPanel = New-Object System.Windows.Forms.Panel
$MainPanel = New-Object System.Windows.Forms.Panel
$pictureBox = New-Object System.Windows.Forms.PictureBox
$lbl = New-Object System.Windows.Forms.Label
$File = "$path\monitis_logo-jpg.png"
$image = [System.Drawing.Image]::FromFile($File) 

#create read me window
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(375, 370)
$Form.Cursor = [System.Windows.Forms.Cursors]::Default
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.MaximizeBox = $false
$Form.MinimizeBox = $false
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$Form.ResumeLayout($false)
$Form.PerformLayout
$Form.text = "Read me"

#Main Panel
$MainPanel.AutoScroll  = $true
$MainPanel.BackColor = [System.Drawing.Color]::GhostWhite
$MainPanel.BackgroundImageLayout = [System.Windows.Forms.ImageLayout]::None
$MainPanel.Cursor = [System.Windows.Forms.Cursors]::Default;
$MainPanel.Location = New-Object System.Drawing.Point(101,1);
$MainPanel.Size = New-Object System.Drawing.Size(272, 334);
$MainPanel.ResumeLayout($false);
$MainPanel.PerformLayout()

#button next
$btn_Next.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Next.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Next.Location =  New-Object System.Drawing.Point(296, 342);
$btn_Next.Size = New-Object System.Drawing.Size(75, 23)
$btn_Next.Text = "Next>";
$btn_Next.UseVisualStyleBackColor = $false;

#read me text 
$lbl.Location = New-Object System.Drawing.Point(3, 3)
$lbl.autoSize = $true
$lbl.BackColor = [System.Drawing.Color]::GhostWhite
$lbl.Font = New-Object System.Drawing.Font([System.Drawing.FontFamily]::GenericSansSerif, 10, [System.Drawing.FontStyle]::Regular)
$lbl.Text = "
The WMI monitor is configured to monitor
    for Microsoft generic applications
    health status.
At first please  make sure that you have
    needed applications in your system.
    Then run your Comand Promt as
    administrator and type ' Powershell.exe 
    Set-ExecutionPolicy Unrestricted '
After authorisation choose application for
    monitoring.
By marking  the application you activate 
    the monitor.
If you want to stop any monitor process
	unmark it from application list"
#$pictureBox
$pictureBox.Image = $image
$pictureBox.Location = New-Object System.Drawing.Point(0, 0)
$pictureBox.Size = New-Object System.Drawing.Size(100, 335)
$pictureBox.SizeMode = [System.Windows.Forms.PictureBoxSizeMode]::StretchImage
$pictureBox.TabStop = $false
#-------------------------------------------------------------------------------------------------------------		
#add controls to form
$Form.Controls.Add($btn_Back)
$Form.Controls.Add($btn_Next)
$Form.Controls.Add($pictureBox)
$MainPanel.Controls.Add($lbl)
$Form.Controls.Add($MainPanel)

#events
$btn_Next.add_click({btn_Next})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()
