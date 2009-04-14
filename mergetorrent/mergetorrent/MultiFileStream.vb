Public Class MultiFileStream
    Inherits System.IO.Stream

    Dim root As String
    Dim files As List(Of Dictionary(Of String, Object))
    Dim current_file As Integer
    Dim current_stream As System.IO.Stream
    Dim current_pos As Long
    Dim current_filepos As Long

    Public Sub New(ByVal root As String, ByVal files As List(Of Dictionary(Of String, Object)))
        MyBase.New()
        Me.root = root
        Me.files = files

        current_file = 0
        current_stream = Nothing
        current_pos = 0
        current_filepos = 0
    End Sub

    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Sub Flush()
        current_stream.Flush()
    End Sub

    Public Overrides ReadOnly Property Length() As Long
        Get
            Length = 0
            For Each file As Dictionary(Of String, Object) In files
                Length += DirectCast(file("length"), Long)
            Next
        End Get
    End Property

    Public Overrides Property Position() As Long
        Get
            Return current_pos
        End Get
        Set(ByVal value As Long)
            If current_stream IsNot Nothing Then
                current_stream.Flush()
                current_stream.Close()
                current_stream.Dispose()
                current_stream = Nothing
            End If

            current_pos = value
            current_file = 0
            Do
                value -= DirectCast(files(current_file)("length"), Long)
                current_file += 1
            Loop While (value >= 0)
            current_file -= 1 'now the current_file is the fie that needs to be opened for this position
            current_filepos = DirectCast(files(current_file)("length"), Long) + value 'calculate it now because we already know
        End Set
    End Property

    Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
        Dim buffer_used As Integer = 0

        If current_stream IsNot Nothing AndAlso current_stream.CanRead = False Then
            current_stream.Flush()
            current_stream.Close()
            current_stream.Dispose()
            current_stream = Nothing
        End If

        Do While buffer_used < count
            If current_stream Is Nothing Then
                Dim filename As String = root
                For Each dir() As Byte In DirectCast(files(current_file)("path"), List(Of Object))
                    filename = My.Computer.FileSystem.CombinePath(filename, System.Text.Encoding.UTF8.GetString(dir))
                Next
                current_stream = System.IO.File.OpenRead(filename)
                current_stream.Position = current_filepos
            End If

            Dim read_len As Integer

            read_len = count - buffer_used 'ideally, we want to fill the buffer
            If read_len > current_stream.Length - current_stream.Position Then
                'the current file is too small to fill the buffer as needed
                read_len = CInt(current_stream.Length - current_stream.Position)
            End If
            current_stream.Read(buffer, offset + buffer_used, read_len) 'read in as much as possible from this file
            buffer_used += read_len
            current_filepos += read_len
            current_pos += read_len
            If current_stream.Position = current_stream.Length Then
                'we are at the end and done with this stream
                current_stream.Flush()
                current_stream.Close()
                current_stream.Dispose()
                current_stream = Nothing
                current_file += 1
                current_filepos = 0
            End If
        Loop
    End Function

    Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
        Throw New NotSupportedException
    End Function

    Public Overrides Sub SetLength(ByVal value As Long)
        Throw New NotSupportedException
    End Sub

    Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        Dim buffer_used As Integer = 0

        If current_stream IsNot Nothing AndAlso current_stream.CanWrite = False Then
            current_stream.Flush()
            current_stream.Close()
            current_stream.Dispose()
            current_stream = Nothing
        End If

        Do While buffer_used < count
            If current_stream Is Nothing Then
                Dim filename As String = root
                For Each Dir() As Byte In DirectCast(files(current_file)("path"), List(Of Object))
                    filename = My.Computer.FileSystem.CombinePath(filename, System.Text.Encoding.UTF8.GetString(Dir))
                Next
                current_stream = System.IO.File.Open(filename, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                current_stream.Position = current_filepos
            End If

            Dim write_len As Integer

            write_len = count - buffer_used 'ideally, we want to use all the buffer
            If write_len > DirectCast(files(current_file)("length"), Long) - current_stream.Position Then
                'the current file is too small to empty the buffer as needed
                write_len = CInt(DirectCast(files(current_file)("length"), Long) - current_stream.Position)
            End If
            current_stream.Write(buffer, offset + buffer_used, write_len) 'write out as much as possible to this file
            buffer_used += write_len
            current_filepos += write_len
            current_pos += write_len
            If current_stream.Position = DirectCast(files(current_file)("length"), Long) Then
                'we are at the end and done with this stream
                current_stream.Flush()
                current_stream.Close()
                current_stream = Nothing
                current_file += 1
                current_filepos = 0
            End If
        Loop
    End Sub
End Class
