'************************************************************************
' VBSCRIPT: Agent and Monitor datatypes
'************************************************************************

Class class_Monitor
	Public Id
	Public Name
	Public ComputerName
	Public IP
	Public Result
	Public Width
	
	Private Sub Class_Initialize
		Id = ""
		Name = ""
		ComputerName = ""
		IP = ""
		Result = ""
	End Sub
End Class

'-----------------------------------------------------------------------------------------------

Class class_InternalAgent
	Public Id
	Public Name
	Public MonitorList
	
	Private Sub Class_Initialize
		Set MonitorList = CreateObject("Scripting.Dictionary")
	End Sub
	
	Private Sub Class_Terminate
		Set MonitorList = Nothing
	End Sub
End Class