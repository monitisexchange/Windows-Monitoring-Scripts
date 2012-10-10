[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Xml")

$path = Get-Location
	
	
#Login button click 
function LogBtn_Next
{
	SaveInXML
	$Form.Visible=$false
	$Form.Close()
	& $path"\scheduler.ps1"
	
}
#save apikey and secretkey in XML
function SaveInXML
{
	[XML]$UserInfo = "
	<Key>
		<Login>"+$LoginTxt.text +"</Login>
		<Password>"+$PassTxt.text+"</Password>
	</Key>"
	$UserInfo.Save("UserInfo.xml")
}


#-------------------------------------------------------------------------------------------------------------
#Login form
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(325, 180)
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.SizeGripStyle = [System.Windows.Forms.SizeGripStyle]::Hide
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$form.text = "Windows user account"
$Form.panel1.PerformLayout
$Form.PerformLayout
$Form.KeyPreview = $True
$Form.Add_KeyDown(
{
	if ($_.KeyCode -eq "Escape") 
    {
		$form.close()
	}
	if ($_.KeyCode -eq "Enter") 
	{
		
		LogBtn_Next
	}
})

# declaration of variables 
$LoginTxt = New-Object System.Windows.Forms.TextBox
$PassTxt = New-Object System.Windows.Forms.TextBox
$lblLog = New-Object System.Windows.Forms.Label
$lblPass = New-Object System.Windows.Forms.Label
$LogBtn = New-Object System.Windows.Forms.Button

#Login Caption
$lblLog.Location = New-Object System.Drawing.Point(12, 43)
$lblLog.Size = New-Object System.Drawing.Size(100, 15)
$lblLog.Text = "Username"
$lblLog.ForeColor = [System.Drawing.Color]::WhiteSmoke
$lblLog.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 11, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Point,204 )

#Login textbox
$LoginTxt.BackColor = [System.Drawing.Color]::GhostWhite
$LoginTxt.Location = New-Object System.Drawing.Point(112, 40)
$LoginTxt.Size = New-Object System.Drawing.Size(183, 24)
try
{
	$LoginTxt.text = $xml.Key.Login
}
catch{}
#Password label
$lblPass.Location = New-Object System.Drawing.Point(12, 94)
$lblPass.Size = New-Object System.Drawing.Size(100, 15)
$lblPass.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 11, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Point,204 )
$lblPass.Text = "Password"
$lblPass.ForeColor = [System.Drawing.Color]::WhiteSmoke

#Password Textbox
$PassTxt.BackColor = [System.Drawing.Color]::GhostWhite
$PassTxt.Location = New-Object System.Drawing.Point(112, 92)
$PassTxt.Size = New-Object System.Drawing.Size(183, 24)
$PassTxt.UseSystemPasswordChar = $true

#Login button
$LogBtn.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 8.25, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Point, 204)
$LogBtn.Location = New-Object System.Drawing.Point(220, 138)
$LogBtn.BackColor = [System.Drawing.Color]::LightSteelBlue
$LogBtn.Size = New-Object System.Drawing.Size(75, 23)
$LogBtn.Text = "Next>"

#load XML file
$exist = Test-Path $path"\UserInfo.xml"
if($exist -eq $true)
{
	[xml]$xml = (Get-Content $path"\UserInfo.xml")
	$PassTxt.text = $xml.Key.Password
	$LoginTxt.text = $xml.Key.Login
}
else
{
	$PassTxt.text = ""
	$LoginTxt.text = ""
}
#-------------------------------------------------------------------------------------------------------------

#add controls
$Form.Controls.Add($LogTxt) 
$Form.Controls.Add($lblLog)
$Form.Controls.Add($LoginTxt)
$Form.Controls.Add($lblPass)
$Form.Controls.Add($PassTxt)
$Form.Controls.Add($LogBtn)

#events
$LogBtn.add_click({LogBtn_Next})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()