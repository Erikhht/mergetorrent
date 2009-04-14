Public Class MultiFileWithBackupStream
    Inherits MultiFileStream
    'This reads from the root and uses the backup root when there is no file in the main root
    Dim root_backup As String

    Sub New(ByVal root As String, ByVal files As List(Of Dictionary(Of String, Object)), ByVal root_backup As String)
        MyBase.New(root, files)
        Me.root_backup = root_backup
    End Sub

    Protected Overrides Function GetStream(ByVal current_file As Integer) As System.IO.Stream
        GetStream = MyBase.GetStream(current_file)
        If GetStream Is System.IO.Stream.Null Then
            Dim filename As String = root_backup
            For Each Dir() As Byte In DirectCast(files(current_file)("path"), List(Of Object))
                filename = My.Computer.FileSystem.CombinePath(filename, System.Text.Encoding.UTF8.GetString(Dir))
            Next
            If Not My.Computer.FileSystem.GetFileInfo(filename).Exists Then
                GetStream = System.IO.Stream.Null 'the empty stream?
            Else
                GetStream = System.IO.File.OpenRead(filename)
            End If
        End If
    End Function
End Class
