Public Class MultiFileStream
    Inherits System.IO.Stream

    Public Structure FileInfo
        Dim Path As List(Of String)
        Dim Length As Long
    End Structure

    Protected files As List(Of FileInfo)
    Dim current_file As Integer
    Dim current_stream As System.IO.Stream
    Dim current_pos As Long
    Dim current_filepos As Long
    Dim current_permutation As List(Of Integer)

    Dim file_mode As System.IO.FileMode
    Dim file_access As System.IO.FileAccess
    Dim file_share As System.IO.FileShare

    Public Sub New(ByVal files As List(Of FileInfo), ByVal file_mode As System.IO.FileMode, ByVal file_access As System.IO.FileAccess, ByVal file_share As System.IO.FileShare)
        MyBase.New()
        Me.files = files
        Me.file_mode = file_mode
        Me.file_access = file_access
        Me.file_share = file_share
        current_file = 0
        current_stream = Nothing
        current_pos = 0
        current_filepos = 0
        current_permutation = New List(Of Integer)
        For i As Integer = 0 To files.Count
            current_permutation.Add(0)
        Next
    End Sub

    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return file_access = System.IO.FileAccess.Read Or file_mode = System.IO.FileAccess.ReadWrite
        End Get
    End Property

    Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
            Return file_access = System.IO.FileAccess.Write Or file_mode = System.IO.FileAccess.ReadWrite
        End Get
    End Property

    Public Overrides Sub Flush()
        If current_stream IsNot Nothing Then current_stream.Flush()
    End Sub

    Public Overrides ReadOnly Property Length() As Long
        Get
            Length = 0
            For Each file As FileInfo In files
                Length += file.Length
            Next
        End Get
    End Property

    Public Overrides Property Position() As Long
        Get
            Return current_pos
        End Get
        Set(ByVal value As Long)
            Dim new_current_pos As Long = 0
            Dim new_current_file As Integer = 0
            Do While new_current_file < files.Count AndAlso new_current_pos + files(new_current_file).Length <= value
                new_current_pos += files(new_current_file).Length
                new_current_file += 1
            Loop
            If value > new_current_pos + files(new_current_file).Length Then
                Throw New ApplicationException("Can't move beyond end of files")
            End If
            If current_stream IsNot Nothing Then
                If new_current_file = current_file Then
                    current_stream.Position = value - new_current_pos
                End If
            End If

            current_filepos = value - new_current_pos
            current_file = new_current_file
            current_pos = value
        End Set
    End Property

    Public Function GetPermutation() As List(Of Integer)
        GetPermutation = New List(Of Integer)
        For Each i As Integer In current_permutation
            GetPermutation.Add(i)
        Next
    End Function

    Public Sub NextPermutation(ByVal Position As Long, ByVal Length As Integer)
        Dim current_file As Integer = 0
        Dim current_pos As Long = 0
        Do While Position >= current_pos + files(current_file).Length
            current_pos += files(current_file).Length
            current_file += 1
        Loop
        Do
            Dim old_permutation As Integer = current_permutation(current_file)
            current_permutation(current_file) = (current_permutation(current_file) + 1) Mod files(current_file).Path.Count
            If current_file = Me.current_file And old_permutation <> current_permutation(current_file) And current_stream IsNot Nothing Then
                current_stream.Close()
                current_stream = Nothing
            End If
            current_pos += files(current_file).Length
            current_file += 1
        Loop While current_permutation(current_file - 1) = 0 And Position + Length - 1 >= current_pos
    End Sub

    Public Shared Function ComparePermutation(ByVal p1 As List(Of Integer), ByVal p2 As List(Of Integer)) As Boolean
        If p1.Count <> p2.Count Then
            Return False
        End If
        For i As Integer = 0 To p1.Count - 1
            If p1(i) <> p2(i) Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Function CountPermutations(ByVal Position As Integer, ByVal Length As Integer) As Integer
        Dim current_file As Integer = 0
        Dim current_pos As Long = 0
        Do While Position >= current_pos + files(current_file).Length
            current_pos += files(current_file).Length
            current_file += 1
        Loop

        CountPermutations = 1
        Do
            current_pos += files(current_file).Length
            CountPermutations *= files(current_file).Path.Count
            current_file += 1
        Loop While Position + Length - 1 >= current_pos + files(current_file).Length
    End Function

    Protected Overridable Function GetStream(ByVal current_file As Integer) As System.IO.Stream
        GetStream = System.IO.File.Open(files(current_file).Path(current_permutation(current_file)), file_mode, file_access, file_share)
        If GetStream.Length <> files(current_file).Length Then
            GetStream.SetLength(files(current_file).Length)
        End If
    End Function

    Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
        Dim buffer_used As Integer = 0

        Do While buffer_used < count
            If current_stream Is Nothing Then
                current_stream = GetStream(current_file)
                current_stream.Position = current_filepos
            End If

            Dim read_len As Integer

            read_len = Math.Min(count - buffer_used, CInt(files(current_file).Length - current_filepos)) 'as much as can be done in one read
            current_stream.Read(buffer, offset + buffer_used, read_len) 'read in as much as possible from this file
            buffer_used += read_len
            current_filepos += read_len
            current_pos += read_len
            If current_filepos = files(current_file).Length Then
                'we are at the end and done with this stream
                current_stream.Close()
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

        Do While buffer_used < count
            If current_stream Is Nothing Then
                current_stream = GetStream(current_file)
                current_stream.Position = current_filepos
            End If

            Dim write_len As Integer

            write_len = Math.Min(count - buffer_used, CInt(files(current_file).Length - current_filepos)) 'as much as can be done in one read
            current_stream.Write(buffer, offset + buffer_used, CInt(write_len)) 'write out as much as possible to this file
            buffer_used += write_len
            current_filepos += write_len
            current_pos += write_len
            If current_filepos = files(current_file).Length Then
                'we are at the end and done with this stream
                current_stream.Close()
                current_stream = Nothing
                current_file += 1
                current_filepos = 0
            End If
        Loop
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
        If current_stream IsNot Nothing Then
            current_stream.Close()
        End If
    End Sub
End Class
