Imports System.IO

Public Class LevelLoader
    Public Shared Function LoadLevel(path As String) As String()
        Return File.ReadAllLines(path)
    End Function
End Class
