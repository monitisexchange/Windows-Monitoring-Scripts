[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Globalization")

$path = Get-Location
Set-Location "$path"

$shortFormat = (Get-Culture).DateTimeFormat.ShortDatePattern -replace "'ב'" -replace '/s' -replace " 'г.'"
$shortFormatArr = @([regex]::Split($shortFormat,'[ ./-]'))
$FormatArr = New-Object string[] $shortFormatArr.Length
for ($i=0; $i -lt $shortFormatArr.Length; $i++) 
{
	if (($shortFormatArr[$i].ToLower() -match "d"))
	{
		$FormatArr[$i] = "dd/"
	}
	elseif(($shortFormatArr[$i].ToLower() -match "m"))
	{
		$FormatArr[$i] = "MM/"
	}
	elseif(($shortFormatArr[$i].ToLower() -match "y"))
	{
		$FormatArr[$i] = "yyyy/"
	}
	$FormatStr =  $FormatStr + $FormatArr[$i]
}
$Format = $FormatStr.TrimEnd("/")
#get OS name
$OS  = Systeminfo | find "OS Name"
$d = $OS.ToLower()

#read username and password from XML file
if(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
{	[xml]$xml = (Get-Content $path"\UserInfo.xml")
	$username = $xml.Key.Login
	$password = $xml.Key.Password
}

#Show daily task as default
function Form_Load
{
	rdBtn_Daily_CheckedChanged
}

#Close the current window
function ExitB
{
	$res = [System.Windows.Forms.MessageBox]::Show("Are you sure?","Warning",[System.Windows.Forms.MessageBoxButtons]::YesNo)
	if($res-eq[System.Windows.Forms.DialogResult]::Yes)
	{
		$Form.Close()
	}
}

#set default value at mouse leave if repetation or recuration values are null
Function mouse_leave
{
	if($txtBx_repeat.Text -eq [System.String]::Empty)
	{
		$txtBx_repeat.Text = $txtBx_repeat.Minimum
	}
	if($txtBx_recur.Text -eq [System.String]::Empty)
	{
		$txtBx_recur.Text = $txtBx_recur.Minimum
	}
}

#show the previous window
function btnBack
{
	if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
	{
		$Form.Visible=$false
		$Form.Close()
		& $path"\metrics.ps1"
	}
	elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
	{
		$Form.Visible=$false
		$Form.Close()
		& $path"\user.ps1"
	}
}

#works when weekly is checked
function rdBtn_Weekly_CheckedChanged
{   
	#set recuration and repeatation values for current task
	$txtBx_recur.add_leave({mouse_leave})
	$txtBx_repeat.add_leave({mouse_leave})
	$txtBx_recur.Value = 1
	$txtBx_repeat.Value = 1
	$txtBx_recur.Maximum =  52
	
	#add controls to panel
	$splitContainer1.Panel2.Controls.Add($txtBx_recur)
	$splitContainer1.Panel2.Controls.Add($lbl_recur)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startTime)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startDate)
	$splitContainer1.Panel2.Controls.Add($ckBox_weekDays)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_expire)
	$splitContainer1.Panel2.Controls.Add($lbl_expire)
	$splitContainer1.Panel2.Controls.Add($lbl_start)
	$splitContainer1.Panel2.Controls.Add($lbl_weekson)
	
	#remove controls from panel
	$splitContainer1.Panel2.Controls.Remove($lbl_days)
	$splitContainer1.Panel2.Controls.Remove($ckBox_Month)
	$splitContainer1.Panel2.Controls.Remove($lbl_Months)
	$splitContainer1.Panel2.Controls.Remove($lbl_on)
	$splitContainer1.Panel2.Controls.Remove($WhichDay)
	$splitContainer1.Panel2.Controls.Remove($DayOfWeek)
	
	#add or remove controls depend of OS
	if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
	{
		$splitContainer1.Panel2.Controls.Add($lbl_min)
		$splitContainer1.Panel2.Controls.Add($txtBx_repeat)
		$splitContainer1.Panel2.Controls.Add($lbl_repeat)
		$splitContainer1.Panel2.Controls.Add($DurationdateTimePicker)
		$splitContainer1.Panel2.Controls.Add($lbl_Duration)
	
	}
	else
	{	
		$splitContainer1.Panel2.Controls.Remove($DurationdateTimePicker)
		$splitContainer1.Panel2.Controls.Remove($lbl_min)
		$splitContainer1.Panel2.Controls.Remove($lbl_Duration)
		$splitContainer1.Panel2.Controls.Remove($txtBx_repeat)
		$splitContainer1.Panel2.Controls.Remove($lbl_repeat)
	}
}
	
#works when Monthly is checked	
function  rdBtn_Monthly_CheckedChanged
{  
	#set recuration and repeatation values for current task
	$txtBx_repeat.add_leave({mouse_leave})
	$txtBx_recur.Value = 1
	$txtBx_repeat.Value = 1
	
	#add controls to panel
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startTime)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startDate)
	$splitContainer1.Panel2.Controls.Add($lbl_start)
	$splitContainer1.Panel2.Controls.Add($ckBox_Month)
	$splitContainer1.Panel2.Controls.Add($lbl_on) 
	$splitContainer1.Panel2.Controls.Add($lbl_Months) 
	$splitContainer1.Panel2.Controls.Add($DayOfWeek);
	$splitContainer1.Panel2.Controls.Add($WhichDay)

	#remove controls from panel
	$splitContainer1.Panel2.Controls.Remove($txtBx_recur)
	$splitContainer1.Panel2.Controls.Remove($lbl_recur)
	$splitContainer1.Panel2.Controls.Remove($lbl_days) 
	$splitContainer1.Panel2.Controls.Remove($ckBox_weekDays);
	$splitContainer1.Panel2.Controls.Remove($lbl_weekson)
	$splitContainer1.Panel2.Controls.Remove($dateTimePicker_expire);
	$splitContainer1.Panel2.Controls.Remove($lbl_expire);
	
	#add or remove controls depend of OS
	if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
	{
		$splitContainer1.Panel2.Controls.Add($lbl_min);
		$splitContainer1.Panel2.Controls.Add($txtBx_repeat);
		$splitContainer1.Panel2.Controls.Add($lbl_repeat); 
		$splitContainer1.Panel2.Controls.Add($DurationdateTimePicker)
		$splitContainer1.Panel2.Controls.Add($lbl_Duration)		
	}
	else
	{
		$splitContainer1.Panel2.Controls.Remove($txtBx_repeat);
		$splitContainer1.Panel2.Controls.Remove($lbl_repeat);
		$splitContainer1.Panel2.Controls.Remove($DurationdateTimePicker)
		$splitContainer1.Panel2.Controls.Remove($lbl_Duration)
		$splitContainer1.Panel2.Controls.Remove($lbl_min)
	}
}
		
#works when One time is checked	
function rdBtn_Onetime_CheckedChanged
{
	#set recuration and repeatation values for current task
	$txtBx_repeat.add_leave({mouse_leave})
	$txtBx_recur.Value = 1
	$txtBx_repeat.Value = 1
	
	#add controls to panel
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startTime)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startDate)
	$splitContainer1.Panel2.Controls.Add($lbl_start)
	
	#remove controls from panel
	$splitContainer1.Panel2.Controls.Remove($lbl_recur)
	$splitContainer1.Panel2.Controls.Remove($lbl_days)
	$splitContainer1.Panel2.Controls.Remove($lbl_weekson)
	$splitContainer1.Panel2.Controls.Remove($txtBx_recur)
	$splitContainer1.Panel2.Controls.Remove($ckBox_weekDays)
	$splitContainer1.Panel2.Controls.Remove($ckBox_Month)
	$splitContainer1.Panel2.Controls.Remove($lbl_Months)
	$splitContainer1.Panel2.Controls.Remove($lbl_on)
	$splitContainer1.Panel2.Controls.Remove($WhichDay)
	$splitContainer1.Panel2.Controls.Remove($DayOfWeek)
	$splitContainer1.Panel2.Controls.Remove($dateTimePicker_expire)
	$splitContainer1.Panel2.Controls.Remove($lbl_expire)
	
	#add or remove controls depend of OS
	if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
	{	
		$splitContainer1.Panel2.Controls.Add($lbl_min)
		$splitContainer1.Panel2.Controls.Add($txtBx_repeat)
		$splitContainer1.Panel2.Controls.Add($lbl_repeat)
		$splitContainer1.Panel2.Controls.Add($DurationdateTimePicker)
		$splitContainer1.Panel2.Controls.Add($lbl_Duration)
	}
	else
	{
		$splitContainer1.Panel2.Controls.Remove($lbl_min)
		$splitContainer1.Panel2.Controls.Remove($txtBx_repeat)
		$splitContainer1.Panel2.Controls.Remove($lbl_repeat)
		$splitContainer1.Panel2.Controls.Remove($DurationdateTimePicker)
		$splitContainer1.Panel2.Controls.Remove($lbl_Duration)
	
	}
}

#works when Daily is checked	
function  rdBtn_Daily_CheckedChanged
{
	#set recuration and repeatation values for current task
	$txtBx_recur.add_leave({mouse_leave})
	$txtBx_repeat.add_leave({mouse_leave})
	$txtBx_recur.Value = 1
	$txtBx_repeat.Value = 1
	$txtBx_recur.Maximum =  365
	
	#add controls to panel
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_expire);
	$splitContainer1.Panel2.Controls.Add($lbl_expire);
	$splitContainer1.Panel2.Controls.Add($lbl_days);
	$splitContainer1.Panel2.Controls.Add($txtBx_recur);
	$splitContainer1.Panel2.Controls.Add($lbl_recur);
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startTime)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startDate)
	$splitContainer1.Panel2.Controls.Add($lbl_start);
	
	#remove controls from panel
	$splitContainer1.Panel2.Controls.Remove($lbl_weekson)
	$splitContainer1.Panel2.Controls.Remove($ckBox_weekDays)
	$splitContainer1.Panel2.Controls.Remove($ckBox_Month)
	$splitContainer1.Panel2.Controls.Remove($lbl_Months)
	$splitContainer1.Panel2.Controls.Remove($lbl_on)
	$splitContainer1.Panel2.Controls.Remove($WhichDay)
	$splitContainer1.Panel2.Controls.Remove($DayOfWeek)
	
	#add or remove controls depend of OS
	if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
	{	
		$splitContainer1.Panel2.Controls.Add($lbl_Duration)
		$splitContainer1.Panel2.Controls.Add($lbl_repeat);
		$splitContainer1.Panel2.Controls.Add($lbl_min);
		$splitContainer1.Panel2.Controls.Add($txtBx_repeat);
		$splitContainer1.Panel2.Controls.Add($DurationdateTimePicker)

	}
	else
	{
		$splitContainer1.Panel2.Controls.Remove($DurationdateTimePicker)
		$splitContainer1.Panel2.Controls.Remove($txtBx_repeat)
		$splitContainer1.Panel2.Controls.Remove($lbl_min)
		$splitContainer1.Panel2.Controls.Remove($lbl_repeat);
		$splitContainer1.Panel2.Controls.Remove($lbl_Duration)
	}
}
	
#works when onstart is checked			
function rdBtn_onStart_CheckedChanged
{
	#set recuration and repeatation values for current task
	$txtBx_recur.Value = 1
	$txtBx_repeat.Value = 1
	
	#remove controls from panel
	$splitContainer1.Panel2.Controls.Remove($lbl_weekson)
	$splitContainer1.Panel2.Controls.Remove($ckBox_weekDays)
	$splitContainer1.Panel2.Controls.Remove($ckBox_Month)
	$splitContainer1.Panel2.Controls.Remove($lbl_Months)
	$splitContainer1.Panel2.Controls.Remove($lbl_on)
	$splitContainer1.Panel2.Controls.Remove($WhichDay)
	$splitContainer1.Panel2.Controls.Remove($DayOfWeek)
	$splitContainer1.Panel2.Controls.Remove($dateTimePicker_expire)
	$splitContainer1.Panel2.Controls.Remove($lbl_expire)
	$splitContainer1.Panel2.Controls.Remove($lbl_min)
	$splitContainer1.Panel2.Controls.Remove($txtBx_repeat)
	$splitContainer1.Panel2.Controls.Remove($lbl_repeat)
	$splitContainer1.Panel2.Controls.Remove($lbl_days)
	$splitContainer1.Panel2.Controls.Remove($txtBx_recur)
	$splitContainer1.Panel2.Controls.Remove($lbl_recur)
	$splitContainer1.Panel2.Controls.Remove($dateTimePicker_startTime)
	$splitContainer1.Panel2.Controls.Remove($dateTimePicker_startDate)
	$splitContainer1.Panel2.Controls.Remove($lbl_start)
	$splitContainer1.Panel2.Controls.Remove($DurationdateTimePicker)
	$splitContainer1.Panel2.Controls.Remove($lbl_Duration)
}

function rdBtn_Minute_CheckedChanged
{
	#set recuration and repeatation values for current task
	$txtBx_recur.add_leave({mouse_leave})
	$txtBx_recur.Value = 1
	$txtBx_repeat.Value = 1
	$lbl_min.Location = New-Object System.Drawing.Point(153, 101);
	$txtBx_recur.Maximum =  59
	#remove controls from panel
	$splitContainer1.Panel2.Controls.Remove($lbl_weekson)
	$splitContainer1.Panel2.Controls.Remove($ckBox_weekDays)
	$splitContainer1.Panel2.Controls.Remove($ckBox_Month)
	$splitContainer1.Panel2.Controls.Remove($lbl_Months)
	$splitContainer1.Panel2.Controls.Remove($lbl_on)
	$splitContainer1.Panel2.Controls.Remove($WhichDay)
	$splitContainer1.Panel2.Controls.Remove($DayOfWeek)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_expire)
	$splitContainer1.Panel2.Controls.Add($lbl_expire)
	$splitContainer1.Panel2.Controls.Add($lbl_min)
	$splitContainer1.Panel2.Controls.Remove($txtBx_repeat)
	$splitContainer1.Panel2.Controls.Remove($lbl_repeat)
	$splitContainer1.Panel2.Controls.Remove($lbl_days)
	$splitContainer1.Panel2.Controls.Add($txtBx_recur)
	$splitContainer1.Panel2.Controls.Add($lbl_recur)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startTime)
	$splitContainer1.Panel2.Controls.Add($dateTimePicker_startDate)
	$splitContainer1.Panel2.Controls.Add($lbl_start)
	$splitContainer1.Panel2.Controls.Remove($DurationdateTimePicker)
	$splitContainer1.Panel2.Controls.Remove($lbl_Duration)
}



#create new task in windows scheduler tasks 
function startbtn_click
{
	# $country = (Get-culture).Name
# 	if (($country -eq "bg-BG") -or ($country -eq "he-IL"))
# 	{
# 		[System.Windows.Forms.MessageBox]::Show("This scheduler is not supported your datetime format please choose another format")
# 		$Form.Close()
# 	}
# 	else
# 	{
		if($rdBtn_Daily.checked -eq $true)
		{
			$schedul = "DAILY"
			$taskname = "DAILY"+"$(Get-Random)"
			$taskrun = '"\"' +"$path"+"\CreateCustMonitor.vbs" + '\""'
			$starttime = $dateTimePicker_startTime.Value.ToString("HH:mm", [System.Globalization.CultureInfo]::InvariantCulture)
			$duration = $DurationdateTimePicker.Value.ToString("HH:mm", [System.Globalization.CultureInfo]::InvariantCulture)
			$startdate = $dateTimePicker_startDate.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			$enddate = $dateTimePicker_expire.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			$modifier = [System.Convert]::ToInt32($txtBx_recur.Value)
			$repeat = [System.Convert]::ToInt32($txtBx_repeat.Value)
			
			$b = $DurationdateTimePicker.Value.Hour * 60 + $DurationdateTimePicker.Value.Minute	
			
			#compare start date and now date
			#start date cannot be earlier than now date
			
			if($startdate -lt [System.DateTime]::Now.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture))
			{
				[System.Windows.Forms.MessageBox]::Show("The date a task begins cannot be earlier than the current date")
			}
			elseif($startdate -lt [System.DateTime]::Now.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture))
			{
				[System.Windows.Forms.MessageBox]::Show("The date a task begins cannot be earlier than the current date")
			}
			#compare repeatition value and duration value
			#repeatation value can't be smaller than duration value
			elseif($repeat -ge  $b)
			{
				[System.Windows.Forms.MessageBox]::Show("The duration must be  greater then the repetition interval")
			}
			#compare start date and end date
			#end date can't be earlier than start date
		 	elseif ($enddate -le $startdate)
			{	
				[System.Windows.Forms.MessageBox]::Show("The date a task begins cannot be later than the date it expires")
			}
			else
			{
				#create new task  using required parametrs for current OS
				if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
				{
					$Command = "schtasks /create /tn $taskname /tr $taskrun /sc $schedul  /mo $modifier /RI $repeat /du $duration /sd $startdate /st $starttime /ed $enddate >status"
					cmd.exe /c $Command
					$Form.Close()
					$Form.Visible = $false
					$a = Get-Content $path"\status"
					[System.Windows.Forms.MessageBox]::Show($a)
				}
				elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
				{
					if(($password -ne "") -and ($username -ne ""))
					{
						$Command = "schtasks /create /tn $taskname /tr $taskrun /sc $schedul  /mo $modifier /sd $startdate /st $starttime /ed $enddate /ru $username /rp $password >status"
						cmd.exe /c $Command
						$Form.Close()
						$Form.Visible = $false
						$a = Get-Content $path"\status"
						[System.Windows.Forms.MessageBox]::Show($a)
					}
					else
					{
						[System.Windows.Forms.MessageBox]::Show("Please enter the run as password.When the run as password is empty, the scheduled task may not run because of the security policy")
					}
				}
			}
		}
		
		$cnt = 0
		if($rdBtn_Weekly.Checked -eq $true)
		{
			$lbl_min.Location = New-Object System.Drawing.Point(153, 202);
			$schedul = "WEEKLY"
			$repeat = [System.Convert]::ToInt32($txtBx_repeat.Text)
			$modifier = [System.Convert]::ToInt32($txtBx_recur.Text)
			
			try
			{
				Foreach($checkbox in $ckBox_weekDays.CheckedIndices)
				{
					$day = $day +  $ckBox_weekDays.Items[$checkbox].ToString()+","
					#count of checked days
					$cnt = $cnt+1
				}
				#checked days
				$day = $day.substring(0,$day.length-1)
			}
			catch{}
			$duration = $DurationdateTimePicker.Value.ToString("HH:mm", [System.Globalization.CultureInfo]::InvariantCulture)
			#$startdate = $dateTimePicker_startDate.Value.Date.ToString("dd/MM/yyyy", [System.Globalization.CultureInfo]::InvariantCulture)
			
				$startdate = $dateTimePicker_startDate.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			
				$enddate = $dateTimePicker_expire.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			
			$starttime = $dateTimePicker_startTime.Value.ToString("HH:mm:ss", [System.Globalization.CultureInfo]::InvariantCulture)
			#$enddate = $dateTimePicker_expire.Value.Date.ToString("MM/dd/yyyy", [System.Globalization.CultureInfo]::InvariantCulture)
			$taskname = "WEEKLY"+"$(Get-Random)"
			$taskrun = '"\"' +"$path"+"\CreateCustMonitor.vbs" + '\""'
			
			$b = $DurationdateTimePicker.Value.Hour * 60 + $DurationdateTimePicker.Value.Minute	
			#compare repeatition value and duration value
			#repeatation value can't be smaller than duration value
			if($repeat -ge  $b)
			{
				[System.Windows.Forms.MessageBox]::Show("The duration must be  greater then the repetition interval")
			}
			#compare start date and now date
			#start date cannot be earlier than now date
			elseif($startdate -lt [System.DateTime]::Now.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture))
			{
				[System.Windows.Forms.MessageBox]::Show("The date a task begins cannot be earlier than the current date")
			}
			#compare start date and end date
			#end date can't be earlier than start date
		 	elseif($enddate -le $startdate)
			{	
				[System.Windows.Forms.MessageBox]::Show("The date a task begins cannot be later than the date it expires")
			}
			else
			{	
				#default value for week day
				if($cnt -eq 0)
				{$day = "MON"}
				
				#create new task  using required parametrs for current OS
				if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
				{
					$Command = "schtasks /create /tn $taskname /tr $taskrun /sc $schedul  /mo $modifier /d $day /RI $repeat /du $duration /sd $startdate /st $starttime /ed $enddate >status"
					cmd.exe /c $Command
					$Form.Close()
					$Form.Visible = $false
					$a = Get-Content $path"\status"
					[System.Windows.Forms.MessageBox]::Show($a)
				}
				elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
				{
					if(($password -ne "") -and ($username -ne ""))
					{
						$Command = "schtasks /create /tn $taskname /tr $taskrun /sc $schedul  /mo $modifier /d $day /sd $startdate /st $starttime /ed $enddate /ru $username /rp $password >status"
						cmd.exe /c $Command
						$Form.Close()
						$Form.Visible = $false
						$a = Get-Content $path"\status"
						[System.Windows.Forms.MessageBox]::Show($a)
					}
					else
					{
						[System.Windows.Forms.MessageBox]::Show("Please enter the run as password.When the run as password is empty, the scheduled task may not run because of the security policy")
					}
				}
				
			}
		}
		
		if($rdBtn_OneTime.Checked -eq $true)
		{			
			$schedul = "Once"
			$repeat = [System.Convert]::ToInt32($txtBx_repeat.Value)
			#$startdate = $dateTimePicker_startDate.Value.Date.ToString("MM/dd/yyyy", [System.Globalization.CultureInfo]::InvariantCulture)
			
			$startdate = $dateTimePicker_startDate.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			$starttime = $dateTimePicker_startTime.Value.ToString("HH:mm:ss", [System.Globalization.CultureInfo]::InvariantCulture)
			$taskname = "Once"+"$(Get-Random)"
			$duration = $DurationdateTimePicker.Value.ToString("HH:mm", [System.Globalization.CultureInfo]::InvariantCulture)
			$taskrun = '"\"' +"$path"+"\CreateCustMonitor.vbs" + '\""'
			
			$b = $DurationdateTimePicker.Value.Hour * 60 + $DurationdateTimePicker.Value.Minute	
			#compare repeatition value and duration value
			#repeatation value can't be smaller than duration value
			if($repeat -ge  $b)
			{
				[System.Windows.Forms.MessageBox]::Show("The duration must be  greater then the repetition interval")
			}
			#compare start time and start date 
			# start time can't be earlier than now time if start date is not great than now date
			elseif((($starttime -lt [System.DateTime]::Now.ToString("HH:mm:ss", [System.Globalization.CultureInfo]::InvariantCulture)) -and  ($startdate -le [System.DateTime]::Now.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture))) -or (($starttime -gt [System.DateTime]::Now.ToString("HH:mm:ss", [System.Globalization.CultureInfo]::InvariantCulture)) -and  ($startdate -lt [System.DateTime]::Now.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture))))
			{
				[System.Windows.Forms.MessageBox]::Show("Start time or date  is earlier then current time or date")
			}
			else
			{
				#create new task  using required parametrs for current OS
				if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
				{
					$Command = "schtasks /create /tn $taskname /tr $taskrun /sc $schedul /RI $repeat /sd $startdate /st $starttime /du $duration >status"			
					cmd.exe /c $Command
					$Form.Close()
					$Form.Visible = $false
					$a = Get-Content $path"\status"
					[System.Windows.Forms.MessageBox]::Show($a)
				}
				elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
				{
					if(($password -ne "") -and ($username -ne ""))
					{
						$Command = "schtasks /create /tn $taskname /tr $taskrun /sc $schedul /sd $startdate /st $starttime /ru $username /rp $password >status"
						cmd.exe /c $Command
						$Form.Close()
						$Form.Visible = $false
						$a = Get-Content $path"\status"
						[System.Windows.Forms.MessageBox]::Show($a)
					}
					else
					{
						[System.Windows.Forms.MessageBox]::Show("Please enter the run as password.When the run as password is empty, the scheduled task may not run because of the security policy")
					}
					
				}
			}
		}
		$count = 0 #checked days count (default: "Monday")
		$count1 = 0 #which day of week count (default: "FIRST")
		$count2 = 0 #checked months count (default: "January")
		if($rdBtn_Monthly.Checked -eq $true)
		{
			$TaskName = "monthly"+"$(Get-Random)"
			$TaskRun = '"\"' +"$path"+"\CreateCustMonitor.vbs" + '\""'
			$Schedule = "monthly"
			try
			{
				$day = $DayOfWeek.SelectedItem.ToString()
				$count = $count + 1
			}
			catch{}
	
			if($count -eq 0)
			{
				$day = "MON"
			}
			try
			{
				$Modifier = $WhichDay.SelectedItem.ToString()
				$count1 = $count1 + 1
			}
			catch{}
			
			if($count1 -eq 0)
			{
				$Modifier = "FIRST"
			}
			try
			{
				Foreach($checkbox in $ckBox_Month.CheckedIndices)
				{
					$Months = $Months +  $ckBox_Month.Items[$checkbox].ToString()+","
					$count2 = $count2 + 1
				}
				$Months = $Months.substring(0,$Months.length-1)
			}
			catch
			{}
			if($count2 -eq 0)
			{
				$Months = "JAN"
			}
			
			$duration = $DurationdateTimePicker.Value.ToString("HH:mm", [System.Globalization.CultureInfo]::InvariantCulture)
			$StartTime = $dateTimePicker_startTime.Value.ToString("HH:mm:ss", [System.Globalization.CultureInfo]::InvariantCulture)
			$StartDate = $dateTimePicker_startDate.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			$Interval = [System.Convert]::ToInt32($txtBx_repeat.Value) 
			$b = $DurationdateTimePicker.Value.Hour * 60 + $DurationdateTimePicker.Value.Minute	
			#compare repeatition value and duration value
			#repeatation value can't be smaller than duration value
			if($Interval -ge  $b)
			{
				[System.Windows.Forms.MessageBox]::Show("The duration must be  greater then the repetition interval")
			}
			else
			{
				#create new task  using required parametrs for current OS
				if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
				{
					$Command = "schtasks.exe /create /tn $TaskName /tr $TaskRun /sc $Schedule /mo $Modifier /d $day /m $Months /st $StartTime  /ri $Interval /du $duration >status"
					cmd.exe /c $Command
					$Form.Close()
					$Form.Visible = $false
					$a = Get-Content $path"\status"
					[System.Windows.Forms.MessageBox]::Show($a)
				}
				elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
				{
					if(($password -ne "") -and ($username -ne ""))
					{
						$Command = "schtasks.exe /create /tn $TaskName /tr $TaskRun /sc $Schedule /mo $Modifier /d $day /m $Months /st $StartTime /ru $username /rp $password >status"
						cmd.exe /c $Command
						$Form.Close()
						$Form.Visible = $false
						$a = Get-Content $path"\status"
						[System.Windows.Forms.MessageBox]::Show($a)
					}
					else
					{
						[System.Windows.Forms.MessageBox]::Show("Please enter the run as password.When the run as password is empty, the scheduled task may not run because of the security policy")
					}
				}
			}
				
		}
		
		if ($rdBtn_onStart.Checked -eq $true)
		{
			$TaskName = "onstart"+"$(Get-Random)"
			$Schedule = "ONSTART"
			$TaskRun = '"\"' +"$path"+"\CreateCustMonitor.vbs" + '\""'
			
			#create new task  using required parametrs for current OS
			if (($d.Contains("windows 7")) -or ($d.Contains("server 2008")))
			{
				$myCmd = "schtasks.exe /Create /TN $TaskName /SC $Schedule /tr $TaskRun >status"
				cmd.exe /c $myCmd 
				$Form.Close()
				$Form.Visible = $false
				$a = Get-Content $path"\status"
				[System.Windows.Forms.MessageBox]::Show($a)
			}
			elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
			{
				if(($password -ne "") -and ($username -ne ""))
				{
					$myCmd = "schtasks.exe /Create /TN $TaskName /SC $Schedule /tr $TaskRun /ru $username /rp $password >status"
					cmd.exe /c $myCmd 
					$Form.Close()
					$Form.Visible = $false
					$a = Get-Content $path"\status"
					[System.Windows.Forms.MessageBox]::Show($a)
				}
				else
				{
					[System.Windows.Forms.MessageBox]::Show("Please enter the run as password.When the run as password is empty, the scheduled task may not run because of the security policy")
				}
			}
		}
		
		
		if($rdBtn_Minute.checked -eq $true)
		{
			$schedul = "MINUTE"
			$taskname = "MINUTE"+"$(Get-Random)"
			$taskrun = '"\"' +"$path"+"\CreateCustMonitor.vbs" + '\""'
			$starttime = $dateTimePicker_startTime.Value.ToString("HH:mm:ss", [System.Globalization.CultureInfo]::InvariantCulture)
			$startdate = $dateTimePicker_startDate.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			$enddate = $dateTimePicker_expire.Value.Date.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
			$modifier = [System.Convert]::ToInt32($txtBx_recur.Value)
			$b = $DurationdateTimePicker.Value.Hour * 60 + $DurationdateTimePicker.Value.Minute	
			#compare start date and now date
			#start date cannot be earlier than now date
			if($startdate -lt [System.DateTime]::Now.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture))
			{
				[System.Windows.Forms.MessageBox]::Show("The date a task begins cannot be earlier than the current date")
			}
			#compare start date and end date
			#end date can't be earlier than start date
		 	elseif ($enddate -le $startdate)
			{	
				[System.Windows.Forms.MessageBox]::Show("The date a task begins cannot be later than the date it expires")
			}
			#create new task  using required parametrs for current OS
			elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
			{
				if(($password -ne "") -and ($username -ne ""))
				{
					$Command = "schtasks /create /tn $taskname /tr $taskrun /sc $schedul  /mo $modifier /sd $startdate /st $starttime /ed $enddate /ru $username /rp $password >status"
						cmd.exe /c $Command
						$Form.Close()
						$Form.Visible = $false
						$a = Get-Content $path"\status"
						[System.Windows.Forms.MessageBox]::Show($a)
					}
					else
					{
						[System.Windows.Forms.MessageBox]::Show("Please enter the run as password.When the run as password is empty, the scheduled task may not run because of the security policy")
					}
				}
			}
# 		}
}


#-------------------------------------------------------------------------------------------------------------
#create scheduler window
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(557, 348);
$Form.Cursor = [System.Windows.Forms.Cursors]::Default
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$Form.ResumeLayout($false)
$Form.PerformLayout
$title = $OS.Substring(36)
$Form.Text = "Create task for " + $title


# declaration of variables 
$splitContainer1 = New-Object System.Windows.Forms.SplitContainer
$rdBtn_onStart = New-Object System.Windows.Forms.RadioButton
$rdBtn_Onetime = New-Object System.Windows.Forms.RadioButton
$rdBtn_Monthly = New-Object System.Windows.Forms.RadioButton
$rdBtn_Weekly = New-Object System.Windows.Forms.RadioButton
$rdBtn_Daily = New-Object System.Windows.Forms.RadioButton
$rdBtn_Minute = New-Object System.Windows.Forms.RadioButton
$lbl_Duration = New-Object System.Windows.Forms.Label
$lbl_expire = New-Object System.Windows.Forms.Label
$lbl_min = New-Object System.Windows.Forms.Label
$lbl_repeat = New-Object System.Windows.Forms.Label
$lbl_days = New-Object System.Windows.Forms.Label
$lbl_recur = New-Object System.Windows.Forms.Label
$lbl_start = New-Object System.Windows.Forms.Label
$lbl_weekson = New-Object System.Windows.Forms.Label
$lbl_Months = New-Object System.Windows.Forms.Label
$lbl_on = New-Object System.Windows.Forms.Label
$DurationdateTimePicker = New-Object System.Windows.Forms.DateTimePicker
$dateTimePicker_expire = New-Object System.Windows.Forms.DateTimePicker
$dateTimePicker_startDate = New-Object System.Windows.Forms.DateTimePicker
$ckBox_weekDays = New-Object System.Windows.Forms.CheckedListBox
$ckBox_Month = New-Object System.Windows.Forms.CheckedListBox
$WhichDay = New-Object System.Windows.Forms.ComboBox
$DayOfWeek = New-Object System.Windows.Forms.ComboBox
$TopPanel = New-Object System.Windows.Forms.Panel
$btn_Exit = New-Object System.Windows.Forms.Button
$btn_start = New-Object System.Windows.Forms.Button
$btn_Back = New-Object System.Windows.Forms.Button
$dateTimePicker_startTime = New-Object System.Windows.Forms.DateTimePicker
$txtBx_repeat = New-Object System.Windows.Forms.NumericUpDown
$txtBx_recur = New-Object System.Windows.Forms.NumericUpDown

$splitContainer1.Panel1.SuspendLayout
$splitContainer1.Panel2.SuspendLayout
$splitContainer1.SuspendLayout
$TopPanel.SuspendLayout
$SuspendLayout

#splitContainer1
$splitContainer1.Location = New-Object System.Drawing.Point(0, 0);
$splitContainer1.IsSplitterFixed = $true
$splitContainer1.BackColor = [System.Drawing.Color]::LightSteelBlue
$splitContainer1.BorderStyle = [System.Windows.Forms.BorderStyle]::Fixed3D

#Panel1(radiobuttons)
$splitContainer1.Panel1.BackColor = [System.Drawing.Color]::GhostWhite
$splitContainer1.Panel1.Controls.Add($rdBtn_onStart);
$splitContainer1.Panel1.Controls.Add($rdBtn_Onetime);
$splitContainer1.Panel1.Controls.Add($rdBtn_Monthly);
$splitContainer1.Panel1.Controls.Add($rdBtn_Weekly);
$splitContainer1.Panel1.Controls.Add($rdBtn_Daily);
if(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
{
	$splitContainer1.Panel1.Controls.Add($rdBtn_Minute);
}
           
#Panel2(Controls)
$splitContainer1.Panel2.AllowDrop = $true;
$splitContainer1.Panel2.BackColor = [System.Drawing.Color]::GhostWhite;
$splitContainer1.Size = New-Object System.Drawing.Size(555, 310);
$splitContainer1.SplitterDistance = 154;

# RadioButton Minute
$rdBtn_Minute.AutoSize = $true;
$rdBtn_Minute.Location = New-Object System.Drawing.Point(21, 261);
$rdBtn_Minute.Size = New-Object System.Drawing.Size(129, 17);
$rdBtn_Minute.TabStop = $true;
$rdBtn_Minute.Text = "Minute";
$rdBtn_Minute.UseVisualStyleBackColor = $true;

# RadioButton on Start
$rdBtn_onStart.AutoSize = $true;
$rdBtn_onStart.Location = New-Object System.Drawing.Point(21, 216);
$rdBtn_onStart.Size = New-Object System.Drawing.Size(129, 17);
$rdBtn_onStart.TabStop = $true;
$rdBtn_onStart.Text = "When Windows starts";
$rdBtn_onStart.UseVisualStyleBackColor = $true;

#RadioButton One time
$rdBtn_Onetime.AutoSize = $true;
$rdBtn_Onetime.Location = New-Object System.Drawing.Point(21, 171);
$rdBtn_Onetime.Size = New-Object System.Drawing.Size(67, 17);
$rdBtn_Onetime.TabStop = $true;
$rdBtn_Onetime.Text = "One time";
$rdBtn_Onetime.UseVisualStyleBackColor = $true;
 
# RadioButton Monthly
$rdBtn_Monthly.AutoSize = $true;
$rdBtn_Monthly.Location = New-Object System.Drawing.Point(21, 129);
$rdBtn_Monthly.Size = New-Object System.Drawing.Size(62, 17);
$rdBtn_Monthly.TabStop = $true;
$rdBtn_Monthly.Text = "Monthly";
$rdBtn_Monthly.UseVisualStyleBackColor = $true;

#RadioButton Weekly 
$rdBtn_Weekly.AutoSize = $true;
$rdBtn_Weekly.Location = New-Object System.Drawing.Point(21, 86);
$rdBtn_Weekly.Size = New-Object System.Drawing.Size(61, 17);
$rdBtn_Weekly.TabStop = $true;
$rdBtn_Weekly.Text = "Weekly";
$rdBtn_Weekly.UseVisualStyleBackColor = $true;

#RadioButton Dayily
$rdBtn_Daily.AutoSize = $true;
$rdBtn_Daily.Location = New-Object System.Drawing.Point(21, 43);
$rdBtn_Daily.Size = New-Object System.Drawing.Size(48, 17);
$rdBtn_Daily.TabStop = $true;
$rdBtn_Daily.Text = "Daily";
$rdBtn_Daily.UseVisualStyleBackColor = $true;
$rdBtn_Daily.checked = $true

#end date Time  
$dateTimePicker_expire.Format = [System.Windows.Forms.DateTimePickerFormat]::Short
$dateTimePicker_expire.Location = New-Object System.Drawing.Point(106, 245);
$dateTimePicker_expire.Size = New-Object System.Drawing.Size(95, 20);


#start date
 $dateTimePicker_startDate.Format = [System.Windows.Forms.DateTimePickerFormat]::Short;
$dateTimePicker_startDate.Location = New-Object System.Drawing.Point(201, 44);
$dateTimePicker_startDate.Size = New-Object System.Drawing.Size(85, 20);

# start time
$dateTimePicker_startTime.Format = [System.Windows.Forms.DateTimePickerFormat]::Time;
$dateTimePicker_startTime.Location = New-Object System.Drawing.Point(101, 44);
$dateTimePicker_startTime.Size = New-Object System.Drawing.Size(85, 20);
$dateTimePicker_startTime.Value = [System.DateTime]::Now
$dateTimePicker_startTime.CustomFormat = "HH:mm:ss"
$dateTimePicker_startTime.ShowUpDown = $true

# label expire
$lbl_expire.AutoSize = $true;
$lbl_expire.Location = New-Object System.Drawing.Point(57, 245);
$lbl_expire.Size = New-Object System.Drawing.Size(36, 13);
$lbl_expire.Text = "Expire";

#label Duration
$lbl_Duration.Location = New-Object System.Drawing.Point(250, 200);
$lbl_Duration.Text = "Duration"
$lbl_Duration.Visible = $true
$lbl_Duration.Font = New-Object System.Drawing.Font("Microsoft Sans Serif", 8, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Point, 0);
$lbl_Duration.AutoSize = $true

#$Duration Time
$DurationdateTimePicker.Format = [System.Windows.Forms.DateTimePickerFormat]::Custom
$DurationdateTimePicker.Location = New-Object System.Drawing.Point(($lbl_Duration.Location.X+57),$lbl_Duration.location.y)
$DurationdateTimePicker.Size = New-Object System.Drawing.Size(85, 20);
$DurationdateTimePicker.Value = "01:00:00"
$DurationdateTimePicker.CustomFormat = "HH:mm:ss"
$DurationdateTimePicker.ShowUpDown = $true

# label repeat 
$lbl_repeat.AutoSize = $true;
$lbl_repeat.Location = New-Object System.Drawing.Point(6, 200);
$lbl_repeat.Size = New-Object System.Drawing.Size(94, 13);
$lbl_repeat.Text = "Repeat task every";


# textBox repeat 
$txtBx_repeat.Location = New-Object System.Drawing.Point(($lbl_repeat.Location.X+114),$lbl_repeat.location.y)
$txtBx_repeat.Maximum =  1438
$txtBx_repeat.Minimum =  1
$txtBx_repeat.Size = New-Object System.Drawing.Size(47, 20);
$txtBx_repeat.Value = 1

# label minute
$lbl_min.AutoSize = $true;
$lbl_min.Size = New-Object System.Drawing.Size(23, 13);
$lbl_min.Text = "min";
$lbl_min.Location = New-Object System.Drawing.Point(($txtBx_repeat.Location.X+50),$txtBx_repeat.location.y)

# label recur
$lbl_recur.AutoSize = $true;
$lbl_recur.Location = New-Object System.Drawing.Point(28, 101);
$lbl_recur.Size = New-Object System.Drawing.Size(65, 13);
$lbl_recur.Text = "Recur every";

#textbox recur
$txtBx_recur.Location = New-Object System.Drawing.Point(($lbl_recur.Location.X+80),$lbl_recur.location.y)
$txtBx_recur.Minimum =  1
$txtBx_recur.Size = New-Object System.Drawing.Size(47, 20);
$txtBx_recur.Value = 1
$txtBx_recur.Maximum =  365

# label days
$lbl_days.AutoSize = $true;
$lbl_days.Location = New-Object System.Drawing.Point(($txtBx_recur.Location.X+50),$txtBx_recur.location.y)
$lbl_days.Size = New-Object System.Drawing.Size(29, 13);
$lbl_days.Text = "days";

#label start
$lbl_start.AutoSize = $true;
$lbl_start.Location = New-Object System.Drawing.Point(64, 44);
$lbl_start.Size = New-Object System.Drawing.Size(29, 13);
$lbl_start.Text = "Start";

#label weeks on
$lbl_weekson.AutoSize = $true;
$lbl_weekson.Location = New-Object System.Drawing.Point(153, 101);
$lbl_weekson.Size = New-Object System.Drawing.Size(56, 13);
$lbl_weekson.Text = "weeks on:";

#week Days 
$ckBox_weekDays.FormattingEnabled = $true;
$DayList = @("MON","TUE","WED","THU","FRI","SAT","SUN")
$ckBox_weekDays.Items.AddRange($DayList)
$ckBox_weekDays.CheckOnClick = $true;
$ckBox_weekDays.Location = New-Object System.Drawing.Point(215, 97);
$ckBox_weekDays.Size = New-Object System.Drawing.Size(90, 64);

#Months list
$ckBox_Month.FormattingEnabled = $true;
$MonthList = @("JAN","FEB","MAR","APR","MAY","JUN","JUL","AUG","SEP","OCT","NOV","DEC")
$ckBox_Month.Items.AddRange($MonthList)
$ckBox_Month.Location = New-Object System.Drawing.Point(82, 120);
$ckBox_Month.CheckOnClick = $true;
$ckBox_Month.Size = New-Object System.Drawing.Size(90, 64);

#label Months 
$lbl_Months.AutoSize = $true;
$lbl_Months.Location = New-Object System.Drawing.Point(6, 122);
$lbl_Months.Size = New-Object System.Drawing.Size(42, 13);
$lbl_Months.Text = "Months";

# label on
$lbl_on.AutoSize = $true;
$lbl_on.Location = New-Object System.Drawing.Point(178, 120);
$lbl_on.Size = New-Object System.Drawing.Size(22, 13);
$lbl_on.Text = "on:";

# which day ov week
$WhichDay.AllowDrop = $true;
$WhichDay.FormattingEnabled = $true;
$secLis = @("First","Second","Third","Fourth","Last")
$WhichDay.Items.AddRange($secLis);
$WhichDay.Location = New-Object System.Drawing.Point(200, 120);
$WhichDay.Size = New-Object System.Drawing.Size(90, 21);
$WhichDay.DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDownList

# Day of week in checkeslistbox 
$DayOfWeek.AllowDrop = $true;
$DayOfWeek.FormattingEnabled = $true;
$DayOfWeek.Items.AddRange($DayList);
$DayOfWeek.Location = New-Object System.Drawing.Point(300, 120);
$DayOfWeek.Size = New-Object System.Drawing.Size(96, 21);
$DayOfWeek.DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDownList

#Start button
$btn_start.Location = New-Object System.Drawing.Point(477, 319);
$btn_start.Size = New-Object System.Drawing.Size(75, 23);
$btn_start.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_start.Text = "Start";

#back button
$btn_back.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_back.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_back.Location = New-Object System.Drawing.Point(400, 319);
$btn_back.Size = New-Object System.Drawing.Size(75, 23)
$btn_back.Text = "<Back";
$btn_back.UseVisualStyleBackColor = $false;
#-------------------------------------------------------------------------------------------------------------

#add controls
$Form.Controls.Add($btn_start)
$Form.Controls.Add($btn_Back)
$Form.Controls.Add($splitContainer1)
$TopPanel.Controls.Add($btn_Exit)

#events
$Form.add_Load({Form_Load})
$rdBtn_Weekly.add_CheckedChanged({rdBtn_Weekly_CheckedChanged})
$rdBtn_Monthly.add_CheckedChanged({rdBtn_Monthly_CheckedChanged})
$rdBtn_Onetime.add_CheckedChanged({ rdBtn_Onetime_CheckedChanged})
$rdBtn_Daily.add_CheckedChanged({ rdBtn_Daily_CheckedChanged})
$rdBtn_onStart.add_CheckedChanged({ rdBtn_onStart_CheckedChanged})
$rdBtn_Minute.add_checkedChanged({rdBtn_Minute_CheckedChanged})
$btn_Exit.add_Click({ExitB})
$btn_back.add_Click({btnBack})
$btn_start.add_Click({startbtn_click})
[void]$Form.ShowDialog()
