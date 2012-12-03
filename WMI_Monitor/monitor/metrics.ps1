[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.ComponentModel") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Collections.Generic") 
clear-host
$path = Get-Location

#get OS name
$OS  = Systeminfo | find "OS Name"
$d = $OS.ToLower()

#load XML file
[xml]$xml0 = (Get-Content $path"\AppList.xml")
[xml]$xml = (Get-Content $newFile)
[xml]$description_xml = (Get-Content $path"\description.xml")

#show the previous window
function btnBack
{
	$Form.Visible=$false
	$Form.Close()
	& $path"\propwin.ps1"
}


function Form_Load
{	
	#add metrics in checklistbox 
	for($i = 0; $i -le $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		$xmlcontentmetrics = $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).Name
		$row = New-Object System.Windows.Forms.DataGridViewRow
		$dataGrid.Rows.Add($row);
		$dataGrid.Rows[$i].Cells[0].value = $xmlcontentmetrics
		$MainPanel.Controls.Add($dataGrid)
		$MainPanel.Controls.Add($description)
		$MainPanel.Controls.add($lbl_Description)
	}
	#show the last monitored  metrics as marked
	for($i = 0; $i -le $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText -eq "true")
		{
			if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).Name -eq $dataGrid.Rows[$i].cells[0].Value)
			{		
				$dataGrid.Rows[$i].Cells[1].Value = [System.Windows.Forms.CheckState]::checked
			}
		}
		elseif($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText -eq "false")
		{
			$dataGrid.Rows[$i].Cells[1].Value = [System.Windows.Forms.CheckState]::Unchecked
		}
	}
	#show last calculation method as selected
	for($i = 0; $i -le $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).Name -eq $dataGrid.Rows[$i].cells[0].Value)
		{	
			$dataGrid.Rows[$i].cells[2].Value = $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).GetAttribute("isdinamic")
		}
	}
	#count of marked metrics
	for($i = 0; $i -le $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText -eq "true")
		{
			$checkedCount = $checkedCount+1
		}
	}
	#set state of "select all" checkbox 
	if ($checkedCount -eq $dataGrid.rowCount)
	{
		$SelectAll_chkbx.checked = $true
	}
	elseif($checkedCount -le $dataGrid.rowCount)
	{
		$SelectAll_chkbx.checked = $false
	}
	$SelectAll_chkbx.Location = New-Object System.Drawing.Point(($NameCol.Width+18),3)

}
#Get application name from configuration file
for ($k = 0; $k -lt $xml0.Application.ChildNodes.count; $k++) 
{
	if($xml0.Application.ChildNodes.item($k).Innertext -eq "true")
	{
		$AppName = $xml0.Application.ChildNodes.item($k).Tostring()
	}
}

function OnCheck
{
	#write true in XML if metric is checked or false if it isn't checked
	if ($dataGrid.CurrentCell.ColumnIndex -eq 1)
	{
		if(($dataGrid.CurrentCell.Value -eq [System.Windows.Forms.CheckState]::checked))
		{
			$dataGrid.CurrentCell.Value = [System.Windows.Forms.CheckState]::Unchecked
			$xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($dataGrid.CurrentCell.rowindex).InnerText = "false"
			$xml.Save($newFile)
			if ($dataGrid.SelectedCells[0].ColumnIndex -eq 1)
			{
				$dataGrid.CurrentCell = $dataGrid.Rows[$dataGrid.SelectedCells[0].RowIndex].Cells[0]
			}
		}
		else
		{
			$dataGrid.CurrentCell.Value = [System.Windows.Forms.CheckState]::checked
			$xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($dataGrid.CurrentCell.rowindex).InnerText = "true"
			$xml.Save($newFile)
			if ($dataGrid.SelectedCells[0].ColumnIndex -eq 1)
			{
				$dataGrid.CurrentCell = $dataGrid.Rows[$dataGrid.SelectedCells[0].RowIndex].Cells[0]
			}
		}				
	}
	#count of marked metrics
	for($i = 0; $i -le $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText -eq "true")
		{
			$checkedCount = $checkedCount+1
		}
	}
	#set state of "select all" checkbox 
	if ($checkedCount -eq $dataGrid.RowCount)
	{
		$SelectAll_chkbx.checked = $true
	}
	elseif($checkedCount -le $dataGrid.RowCount)
	{
		$SelectAll_chkbx.checked = $false
	}
	#show selected metric's description in description box
	if($dataGrid.CurrentCell.ColumnIndex -eq 0)
	{
		for($i=0; $i -le $description_xml.MonitorPropertiesList.SelectSingleNode($AppName).ChildNodes.Count;$i++)
		{
			if ($dataGrid.CurrentCell.value.tostring() -eq  $description_xml.MonitorPropertiesList.SelectSingleNode($AppName).ChildNodes.Item($i).innertext)
			{	
				$description.text =  $description_xml.MonitorPropertiesList.SelectSingleNode($AppName).ChildNodes.Item($i).GetAttribute("description").Tostring()
			}
		}
	}
}

#select and unselect all metrics
function SelectAll_changed
{
	for($i = 0; $i -le $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count-1;$i++)
	{
		if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText -eq "true")
		{
			$checkedCount1 = $checkedCount1+1
		}
	}
	
	if($SelectAll_chkbx.checked -eq $true)
	{
		for ( $i = 0; $i -lt $dataGrid.RowCount;$i++)
		{
			if($dataGrid.Rows[$i].Cells[1].value -eq [System.Windows.Forms.CheckState]::Unchecked)
			{
				$dataGrid.Rows[$i].Cells[1].value = [System.Windows.Forms.CheckState]::Checked
				$xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText = "true"
				$xml.Save($newFile);
				if ($dataGrid.SelectedCells[0].ColumnIndex -eq 1)
				{
					$dataGrid.CurrentCell = $dataGrid.Rows[$dataGrid.SelectedCells[0].RowIndex].Cells[0]
				}	
			}
		}
	}
	elseif(($SelectAll_chkbx.checked -eq $false) -and ($checkedCount1 -eq $dataGrid.RowCount) )
	{
		for ( $i = 0; $i -lt $dataGrid.RowCount;$i++)
		{
			if ($dataGrid.Rows[$i].Cells[1].value -eq [System.Windows.Forms.CheckState]::Checked)
			{
				$dataGrid.Rows[$i].Cells[1].value = [System.Windows.Forms.CheckState]::Unchecked
				$xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($i).InnerText = "false"
				$xml.Save($newFile);
				if ($dataGrid.SelectedCells[0].ColumnIndex -eq 1)
				{
					$dataGrid.CurrentCell = $dataGrid.Rows[$dataGrid.SelectedCells[0].RowIndex].Cells[0]
				}	
			}
		}
	}
}

#go to the next window
[int[]]$IsCheckedArr = @()
function Next_btn
{
	#count of marked metrics
	$IsChecked = 0
	for ($j = 0; $j -lt $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count; $j++)
	{
		if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($j).InnerText -eq "true")
		{
			$IsChecked = $IsChecked + 1
		}
	}
		
	#if any metric  is checked go to the next window
	if ($IsChecked -eq 0)
	{
		[System.Windows.Forms.MessageBox]::Show("Please choose metrics")
	}
	else
	{
		if (($d.Contains("windows 7")) -or ($d.Contains("server 2008"))) 
		{
			$Form.Visible=$false
			$Form.Close()
			& $path"\scheduler.ps1"
		}
		elseif(($d.Contains("windows xp")) -or ($d.Contains("server 2003")))
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
	$IsChecked = 0
	#count of checked metrics
	for ($j = 0; $j -lt $xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Count; $j++)
	{
		if($xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($j).InnerText -eq "true")
		{
			$IsChecked = $IsChecked + 1
		}
	}
	#if any metric is checked show test result
	if ($IsChecked -eq 0)
	{
		[System.Windows.Forms.MessageBox]::Show("Please choose metrics")
	}
	else
	{
		& $path"\TestData.vbs" "$newFile"
	}
}
#save calculation method in configuration file
function formulated
{
	if ($dataGrid.CurrentCell.ColumnIndex -eq 2)
    {
	   	$xml.Monitor.Childnodes.Item(0).SelectSingleNode("metrics").ChildNodes.Item($dataGrid.CurrentCell.rowindex).setattribute("isdinamic",$dataGrid.currentCell.value.toString())
		$xml.Save($newFile)
	}
}

#-------------------------------------------------------------------------------------------------------------
# declaration of variables 
$btn_Next = New-Object System.Windows.Forms.Button
$btn_back = New-Object System.Windows.Forms.Button
$btn_Test = New-Object System.Windows.Forms.Button
$SelectAll_chkbx = New-Object System.Windows.Forms.CheckBox
$MainPanel = New-Object System.Windows.Forms.Panel
$NameCol = New-Object System.Windows.Forms.DataGridViewTextBoxColumn
$IDCol = New-Object System.Windows.Forms.DataGridViewCheckBoxColumn
$DinCol = New-Object System.Windows.Forms.DataGridViewComboBoxColumn
$description = New-Object System.Windows.Forms.RichTextBox
$lbl_Description = New-Object System.Windows.Forms.Label
$toolTip = New-Object System.Windows.Forms.ToolTip

#create metrics window
$Form = New-Object System.Windows.Forms.Form 
$Form.AutoScaleDimensions = New-Object System.Drawing.SizeF(6, 13)
$Form.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$Form.BackColor = [System.Drawing.Color]::LightSteelBlue
$Form.ClientSize = New-Object System.Drawing.Size(554, 370)
$Form.Cursor = [System.Windows.Forms.Cursors]::Default
$Form.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::FixedToolWindow
$Form.MaximizeBox = $false
$Form.MinimizeBox = $false
$Form.StartPosition = [System.Windows.Forms.FormStartPosition]::CenterScreen
$Form.ResumeLayout($false)
$Form.PerformLayout
$Form.text = "Select $AppName Metrics"

#MainPanel
$MainPanel.Location = New-Object System.Drawing.Point(0,0);
$MainPanel.Size = New-Object System.Drawing.Size(554, 337);
$MainPanel.BackColor = [System.Drawing.Color]::GhostWhite

#metrics datagrid
$dataGrid = New-Object System.Windows.Forms.DataGridView
$dataGrid.AllowUserToAddRows = $false;
$dataGrid.AllowUserToDeleteRows = $false
$dataGrid.AllowUsertoResizeRows = $false
$dataGrid.AllowUsertoResizeColumns = $false
$dataGrid.AllowUserToOrderColumns = $false
$dataGrid.ColumnHeadersHeightSizeMode = [System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode]::AutoSize
$dataGrid.Location = New-Object System.Drawing.Point(0, 0);
$dataGrid.Size = New-Object System.Drawing.Size(350, 312)
$dataGrid.BackGroundColor = [System.Drawing.Color]::White
$NameCol.HeaderText = "Metrics"
$NameCol.ReadOnly = $true
$NameCol.Width = 180
$NameCol.SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::NotSortable
$DinCol.HeaderText = "Calc Method"
$formulas = @("Raw Value","Per time Unit","Difference","Percent")
$IDCol.trueValue = [System.Windows.Forms.CheckState]::checked
$DinCol.DisplayStyle = [System.Windows.Forms.DataGridViewComboBoxDisplayStyle]::ComboBox
$DinCol.items.addRange($formulas)
$IDCol.falseValue = [System.Windows.Forms.CheckState]::Unchecked
$IDCol.Width = 50
$dataGrid.Columns.Add($NameCol)
$dataGrid.Columns.Add($IDCol)
$dataGrid.Columns.Add($DinCol)
$dataGrid.Cellborderstyle =[System.Windows.Forms.DataGridViewCellBorderStyle]::None
$dataGrid.borderstyle = [System.Windows.Forms.BorderStyle]::FixedSingle
$dataGrid.RowHeadersVisible = $false

#description
$description.Size = New-Object System.Drawing.Size(180,200)
$description.Location = New-Object System.Drawing.Point(355,15)
$description.BorderStyle = [System.Windows.Forms.BorderStyle]::FixedSingle
$description.readonly = $true

$lbl_Description.text = "Description"
$lbl_Description.Location = New-Object System.Drawing.Point(355,1)

#button next
$btn_Next.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_Next.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_Next.Location = New-Object System.Drawing.Point(475, 342);
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
$toolTip.SetToolTip($btn_Test, "Check data availability.");

#back button
$btn_back.BackColor = [System.Drawing.Color]::LightSteelBlue;
$btn_back.Cursor = [System.Windows.Forms.Cursors]::Default;
$btn_back.Location = New-Object System.Drawing.Point(397, 342);
$btn_back.Size = New-Object System.Drawing.Size(75, 23)
$btn_back.Text = "<Back"
$btn_back.UseVisualStyleBackColor = $false;

# SelectAll_rdBtn
$SelectAll_chkbx.AutoSize = $true;
$SelectAll_chkbx.Size = New-Object System.Drawing.Size(68, 17);
$SelectAll_chkbx.TabStop = $true;
$SelectAll_chkbx.UseVisualStyleBackColor = $true;

#add controls to form
$Form.controls.add($MainPanel)
$Form.Controls.Add($btn_Test)
$Form.Controls.Add($btn_back)
$Form.Controls.Add($btn_Next)
$dataGrid.Controls.Add($SelectAll_chkbx)

#events
$dataGrid.add_DataError({})
$btn_Test.add_click({TestBtn_Click})
$dataGrid.add_CellEndEdit({formulated})
$Form.add_Load({Form_Load})
$dataGrid.Add_CellContentClick({OnCheck})
$btn_Next.add_click({Next_btn})
$btn_back.add_Click({btnBack})
$SelectAll_chkbx.Add_CheckStateChanged({SelectAll_changed})
$Form.Add_Shown({$Form.Activate()})
[void] $Form.ShowDialog()
