'************************************************************************
' VBSCRIPT: funcString
'
' Generic string functions
'************************************************************************

' Return the monitor name given "obj.selectSingleNode("name").text" 
Function GetMonitorName(strInput)
	Dim strTemp
	strTemp = Left(strInput, InStr(strInput, "@")-1)
	GetMonitorName = Replace(strTemp, "processes_", "")
End Function

'-----------------------------------------------------------------------------------------------

' Return the computer name given "obj.selectSingleNode("name").text" 
Function GetMonitorComputerName(strInput)
	strTemp = Mid(strInput, InStr(strInput, "@")+1)
	GetMonitorComputerName = Left(strTemp, InStr(strTemp, "@")-1)
End Function

'-----------------------------------------------------------------------------------------------
' Return the 'base' name of a monitor. For example: 'drive_C' will be returned as 'drive' so
' we know what monitor type we're dealing with

Function GetMonitorBaseName(inString)

	GetMonitorBaseName = LCase(inString)
	If InStr(inString, "_") Then
		GetMonitorBaseName = LCase(Left(inString, InStr(inString, "_")-1))
	End If
	
End Function

'-----------------------------------------------------------------------------------------------
' Return a formatted/padded string. For now this is a very simple function replacing a blank
' input string with a number of spaces

Function Format(inString, padding)
	Dim tmp
	tmp = inString
	
	For i = 1 To padding - Len(inString)
		tmp = tmp + " "
	Next
	
	Format = tmp
End Function
