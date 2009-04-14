Public Class Bencode
    'This class provides functions to encode and decode bencoded text

    Class ParseErrorException
        Inherits Exception
    End Class

    Public Shared Function Decode(ByVal inStream As System.IO.Stream) As Object
        'Returns one of the encoded types: byte string, integer, list or dictionary.
        'return nothing if the input is empty
        'throws exceptions on errors in the input
        Dim br As New System.IO.BinaryReader(inStream)
        Dim NextChar As Char
        Try
            NextChar = Chr(br.PeekChar)
        Catch e As InvalidOperationException
            Return Nothing
        End Try

        Return DecodeElement(br)
    End Function

    Private Shared Function DecodeElement(ByVal br As System.IO.BinaryReader) As Object
        Dim NextChar As Char
        Try
            NextChar = Chr(br.PeekChar)
        Catch ex As Exception
            Throw New ParseErrorException
        End Try

        Select Case NextChar
            Case "d"
                Return DecodeDictionary(br)
            Case "l"
                Return DecodeList(br)
            Case "i"
                Return DecodeInteger(br)
        End Select
        If Char.IsDigit(NextChar) Then
            Return DecodeByteString(br)
        End If
        Throw New ParseErrorException
    End Function

    Private Shared Function DecodeDictionary(ByVal br As System.IO.BinaryReader) As Dictionary(Of String, Object)
        If Chr(br.ReadByte) <> "d" Then
            Throw New ParseErrorException
        End If
        Dim ret As New Dictionary(Of String, Object)
        Do While Chr(br.PeekChar) <> "e"
            ret.Add(System.Text.Encoding.ASCII.GetString(DecodeByteString(br)), DecodeElement(br))
        Loop
        br.ReadByte() 'throw away the e
        Return ret
    End Function

    Private Shared Function DecodeList(ByVal br As System.IO.BinaryReader) As List(Of Object)
        If Chr(br.ReadByte) <> "l" Then
            Throw New ParseErrorException
        End If
        Dim ret As New List(Of Object)
        Do While Chr(br.PeekChar) <> "e"
            ret.Add(DecodeElement(br))
        Loop
        br.ReadByte() 'throw away the e
        Return ret
    End Function

    Private Shared Function DecodeInteger(ByVal br As System.IO.BinaryReader) As Int64
        If Chr(br.ReadByte) <> "i" Then
            Throw New ParseErrorException
        End If
        Dim ret As Int64
        Dim negative As Boolean = False
        Select Case Chr(br.PeekChar)
            Case "e"
                br.ReadByte() 'throw away the e
                Return 0
            Case "-"
                br.ReadByte() 'throw away the -
                negative = True
        End Select
        ret = 0
        Dim nextchar As Char
        Do
            nextchar = Chr(br.ReadByte)
            If Char.IsDigit(nextchar) Then
                ret *= 10
                ret += Int64.Parse(nextchar)
            ElseIf nextchar = "e" Then
                If negative Then ret = -ret
                Return ret
                Exit Do
            Else
                Throw New ParseErrorException
            End If
        Loop
    End Function

    Private Shared Function DecodeByteString(ByVal br As System.IO.BinaryReader) As Byte()
        Dim nextchar As Char
        Dim len As Int64 = 0
        Do
            nextchar = Chr(br.ReadByte)
            Select Case nextchar
                Case ":"
                    Exit Do
                Case "0" To "9"
                    len *= 10
                    len += Int64.Parse(nextchar)
                Case Else
                    Throw New ParseErrorException
            End Select
        Loop
        ' : already consumed, now read in the bytes
        Return br.ReadBytes(len)
    End Function
End Class
