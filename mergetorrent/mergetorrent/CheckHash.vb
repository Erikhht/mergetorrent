Public Class CheckHash
    Public Shared Function Hash(ByRef buffer() As Byte, ByVal length As Int64) As Byte()
        Dim sha1 As New System.Security.Cryptography.SHA1CryptoServiceProvider
        Return sha1.ComputeHash(buffer, 0, length)
    End Function
End Class
