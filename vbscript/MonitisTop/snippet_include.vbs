
Sub Include(strFilename)
	On Error Resume Next
	Dim oFSO, f, s

	Set oFSO = CreateObject("Scripting.FileSystemObject")
	If oFSO.FileExists(strFilename) Then
		Set f = oFSO.OpenTextFile(strFilename)
		s = f.ReadAll
		f.Close
		ExecuteGlobal s
	End If

	Set oFSO = Nothing
	Set f = Nothing
	On Error Goto 0
End Sub
