[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Xml")
clear-host
$path = Get-Location
	
#Login to Monitis server and go to the next window
function LogBtn_Click
{
	$LogBtn.Text = "Loging In.."
	#MD5 coding
	$passcode = $PassTxt.Text
    $data = [Text.Encoding]::UTF8.GetBytes($passcode)
	$hash = [Security.Cryptography.MD5]::Create().ComputeHash($data)
	$pass = ([BitConverter]::ToString($hash) -replace '-').ToLower()
	if ($connect_to.SelectedItem.ToString() -eq "Monitis")
	{
		$PortalUrl = "www.monitis.com"
	}
	elseif($connect_to.SelectedItem.ToString() -eq "Monitor.us")
	{
		$PortalUrl = "www.monitor.us"
	}
	
	$url = "http://$PortalUrl/api?action=apikey&userName="+$LoginTxt.Text+"&password="+$pass+"&output=xml&version=2"
    $wc = New-Object System.Net.WebClient
	
	try
    {
    	$apikey = $wc.DownloadString($url)
    }
    catch [System.Net.WebException]
    {
        #Username, password verification
		if($_.Exception.ToString() -Contains "400" )
		{
			[System.Windows.Forms.MessageBox]::Show("Wrong user name or pass")
		}
		else
		{
			[System.Windows.Forms.MessageBox]::Show("Error, please try again")
		}
		$LogBtn.Text = "Login"
		return
    }  
	
	#request to get secretkey
	$xmldata = [xml]$apikey
	$apikey = $xmldata.result.apikey
	$url = "http://$PortalUrl/api?action=secretkey&apikey="+$apikey+"&output=xml&version=2"
	$secretkey = $wc.DownloadString($url)
	$xmldata = [xml]$secretkey
	$secretkey = $xmldata.result.secretkey	
	
	Connect-Monitis
	SaveApiSecretKeysInXML
	
	$form.Visible = $false
	$form.close()
	& $path"\readme.ps1"
}

#Connect to monitis server
function Connect-Monitis
{
	    # Translate Credentials into APIKey
        if ($pscmdlet.parametersetName -eq 'Credential') {
            $GetapiKey = $apikey
            $GEtSecretKey = $secretkey  
        }                
		
        $webClient = New-Object Net.Webclient        
        $connectionUrl = "http://$PortalUrl/api?action=authToken&apikey=$apiKey&secretkey=$secretKey"        
        $result = $webClient.DownloadString($connectionUrl)
        #Extract Auth Token            
        if ($result -like '{"authtoken":"*"}') {
            $auth = $result -split '[\"\{\}\:\"]'  | 
                Where-Object { $_ } | 
                Select-Object -Skip 1
        }
        
        # Cache Information
        if ($auth) {
            $script:AuthToken = $auth
            $script:SecretKey = $GetSecretKey
            $script:ApiKey = $GetapiKey
        }     
        if ($outputAuthToken )
		{
            $script:AuthToken 
        }
}

#save apikey and secretkey in XML
function SaveApiSecretKeysInXML
{
	[XML]$ApiSecretKeys = "
	<Key>
		<ApiKey>"+$apikey +"</ApiKey>
		<SecretKey>"+$secretkey+"</SecretKey>
		<ConnectTo>"+$connect_to.SelectedItem.ToString()+"</ConnectTo>
	</Key>"

	$ApiSecretKeys.Save("ApiKey.xml")
}

#-------------------------------------------------------------------------------------------------------------
#create Login form
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(325, 225)
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.SizeGripStyle = [System.Windows.Forms.SizeGripStyle]::Hide
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$Form.panel1.PerformLayout
$Form.PerformLayout
$Form.KeyPreview = $True
$form.Text = "Login"



#enter and escape keypress event for login and exit
$Form.Add_KeyDown(
{
	if ($_.KeyCode -eq "Escape") 
    {
		$form.close()
	}
	if ($_.KeyCode -eq "Enter") 
	{
		LogBtn_Click
	}
})

# declaration of variables 
$LoginTxt = New-Object System.Windows.Forms.TextBox
$PassTxt = New-Object System.Windows.Forms.TextBox
$lblLog = New-Object System.Windows.Forms.Label
$lblPass = New-Object System.Windows.Forms.Label
$LogBtn = New-Object System.Windows.Forms.Button
$lblconnect_to = New-Object System.Windows.Forms.Label
$connect_to = New-Object System.Windows.Forms.ComboBox

#Username label
$lblLog.Location = New-Object System.Drawing.Point(12, 43)
$lblLog.Text = "Username"
$lblLog.ForeColor = [System.Drawing.Color]::WhiteSmoke
$lblLog.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 11, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Point,204 )
$lblLog.AutoSize = $true

#Username textbox
$LoginTxt.BackColor = [System.Drawing.Color]::GhostWhite
$LoginTxt.Location = New-Object System.Drawing.Point(($lblLog.Location.X+100),$lblLog.location.y)
$LoginTxt.Size = New-Object System.Drawing.Size(183, 24)
$LoginTxt.text = ""

#Password label
$lblPass.Location = New-Object System.Drawing.Point(12, 94)
$lblPass.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 11, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Point,204 )
$lblPass.Text = "Password"
$lblPass.ForeColor = [System.Drawing.Color]::WhiteSmoke
$lblPass.AutoSize = $true

#Password Textbox
$PassTxt.BackColor = [System.Drawing.Color]::GhostWhite
$PassTxt.Location = New-Object System.Drawing.Point(($lblPass.Location.X+100),$lblPass.location.y)
$PassTxt.Size = New-Object System.Drawing.Size(183, 24)
$PassTxt.UseSystemPasswordChar = $true
$PassTxt.Text = ""


#Portal label
$lblconnect_to.Location = New-Object System.Drawing.Point(12, 145)
$lblconnect_to.Text = "Connect to"
$lblconnect_to.ForeColor = [System.Drawing.Color]::WhiteSmoke
$lblconnect_to.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 11, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Point,204 )
$lblconnect_to.AutoSize = $true

#Portal combobox
$List1 = @("Monitis","Monitor.us")
$connect_to.BackColor = [System.Drawing.Color]::GhostWhite
$connect_to.Location = New-Object System.Drawing.Point(($lblconnect_to.Location.X+100),$lblconnect_to.location.y)
$connect_to.Size = New-Object System.Drawing.Size(183, 24)
$connect_to.Items.AddRange($List1)
$connect_to.DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDownList
$connect_to.AllowDrop = $true;

#load XML file
$exist = Test-Path $path"\ApiKey.xml"
if($exist -eq $true)
{
	[xml]$xml = (Get-Content $path"\ApiKey.xml")
	$connect_to.SelectedItem = $xml.Key.ConnectTo
}
else
{
	$connect_to.SelectedItem =$List1[0]
}

#Login button
$LogBtn.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 8.25, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Point, 204)
$LogBtn.Location = New-Object System.Drawing.Point(220, 191)
$LogBtn.BackColor = [System.Drawing.Color]::LightSteelBlue
$LogBtn.AutoSize = $true
$LogBtn.Text = "Login"
#-------------------------------------------------------------------------------------------------------------

#add controls to form
$Form.Controls.Add($LogTxt) 
$Form.Controls.Add($lblLog)
$Form.Controls.Add($LoginTxt)
$Form.Controls.Add($lblPass)
$Form.Controls.Add($PassTxt)
$Form.Controls.Add($LogBtn)
$form.Controls.Add($lblconnect_to)
$form.Controls.Add($connect_to)
clear-host

#events
$LogBtn.add_click({LogBtn_Click})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()