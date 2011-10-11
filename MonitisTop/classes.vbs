'************************************************************************
' VBSCRIPT: Agent and Monitor datatypes
'************************************************************************

Class class_Metric
	Public Name
	Public Metric
	Public Result
	Public Suffix
	Public Width
	
	Private Sub Class_Initialize
		Name = ""
		Metric = ""
		Result = ""
		Suffix = ""
		Width = 0
	End Sub
End Class

'-----------------------------------------------------------------------------------------------

Class class_Monitor
	Public Id
	Public Name
	Public DisplayName
	
	Public MetricList
	
	Private Sub Class_Initialize
		Id = ""
		Name = ""
		DisplayName = ""
		Set MetricList = CreateObject("Scripting.Dictionary")
	End Sub
	
	Private Sub Class_Terminate
		Set MetricList = Nothing
	End Sub
End Class

'-----------------------------------------------------------------------------------------------

Class class_ProcessMonitor
	Public Id
	Public Name
	Public DisplayName
	Public Metric
	Public ComputerName
	Public IP
	Public Result
	Public Width
	Public Memory
	Public Cpu
	
	Private Sub Class_Initialize
		Id = ""
		Name = ""
		Metric = ""
		DisplayName = ""
		ComputerName = ""
		IP = ""
		Result = ""
		Memory = ""
		Cpu = ""
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