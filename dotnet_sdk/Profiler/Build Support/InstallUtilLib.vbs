Option Explicit

Const msiOpenDatabaseModeTransact     = 1
Const msiViewModifyAssign         = 3

Dim installer : Set installer = Nothing
Set installer = Wscript.CreateObject("WindowsInstaller.Installer")

Dim sqlQuery : sqlQuery = "SELECT `Name`,`Data` FROM Binary"

Dim database : Set database = installer.OpenDatabase("..\MonitisProfilerSetup_x64\Release\MonitisProfilerSetup_x64.msi", msiOpenDatabaseModeTransact)
Dim view     : Set view = database.OpenView(sqlQuery)
Dim record

Set record = installer.CreateRecord(2)
record.StringData(1) = "InstallUtil"
view.Execute record

record.SetStream 2, "InstallUtilLib.dll"

view.Modify msiViewModifyAssign, record 
database.Commit 
Set view = Nothing
Set database = Nothing