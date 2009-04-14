Public Class MainForm

    Private Sub TestBencode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestBencode.Click
        Bencode.Decode(New System.IO.FileStream("C:\Documents and Settings\Eyal\Application Data\uTorrent\Lost.S05E00.HDTV.XviD-2HD.avi.torrent", IO.FileMode.Open, IO.FileAccess.Read))

    End Sub
End Class
