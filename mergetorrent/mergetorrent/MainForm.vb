Public Class MainForm
    Dim torrent As Dictionary(Of String, Object)
    Dim multifile As Boolean

    Private Const SHA1_HASHBYTES As Int64 = 20

    Private Sub CheckFile(ByVal Source As String, ByVal btn As Button, ByVal completion As Label)
        btn.Enabled = 0
        Dim s As System.IO.Stream
        Dim info As Dictionary(Of String, Object) = torrent("info")
        If multifile Then
            Dim new_files As New List(Of Dictionary(Of String, Object))
            For Each d As Dictionary(Of String, Object) In info("files")
                new_files.Add(d)
            Next
            s = New MultiFileStream(Source, new_files)
        Else
            s = System.IO.File.OpenRead(Source)
        End If

        Dim currentfile As Int64 = 0
        Dim piece_len As Int64 = info("piece length")
        Dim buffer(0 To (piece_len - 1)) As Byte
        Dim hash_result() As Byte
        Dim pieces() As Byte = info("pieces")
        Dim pieces_position As Int64 = 0
        Dim complete As Int64 = 0
        Dim doevents_period As Int64
        Dim doevents_counter As Int64 = 0

        doevents_period = CDbl(pieces.Length) / SHA1_HASHBYTES / 50 'update the screen after each ~2%
        If doevents_period > 20 Then doevents_period = 20 'but no less frequently then every 30 hashes

        completion.Visible = True
        completion.Text = (CDbl(complete) / CDbl(pieces.Length)).ToString("P02")

        Do While pieces_position < pieces.Length
            Dim read_len As Int64

            If s.Position + piece_len <= s.Length Then
                read_len = piece_len
            Else
                read_len = s.Length - s.Position
            End If
            s.Read(buffer, 0, read_len)
            hash_result = CheckHash.Hash(buffer, read_len)

            Dim i As Integer = 0
            Do While i < 20 AndAlso pieces(pieces_position + i) = hash_result(i)
                i += 1
            Loop
            pieces_position += 20
            If i = 20 Then
                'piece match
                complete += 20
            End If
            doevents_counter += 1
            If doevents_counter >= doevents_period Then
                completion.Text = (CDbl(complete) / CDbl(pieces.Length)).ToString("P02")
                btn.Text = (CDbl(pieces_position) / CDbl(pieces.Length)).ToString("P02")
                My.Application.DoEvents()
                doevents_counter = 0
            End If
        Loop
        btn.Enabled = 1
        btn.Text = "Check"
    End Sub

    Private Sub CheckA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckA.Click
        CheckFile(FileA.Text, CheckA, CompleteA)
    End Sub

    Private Sub CheckB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckB.Click
        CheckFile(FileB.Text, CheckB, CompleteB)
    End Sub

    Private Sub FindTorent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindTorent.Click
        Dim ofd As New OpenFileDialog

        ofd.AddExtension = True
        ofd.AutoUpgradeEnabled = True
        ofd.CheckFileExists = True
        ofd.CheckPathExists = True
        ofd.DefaultExt = ".torrent"
        ofd.DereferenceLinks = True
        ofd.Filter = "Torrents (*.torrent)|*.torrent|All files (*.*)|*.*"
        If System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\uTorrent") Then
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\uTorrent"
        End If
        ofd.Multiselect = False
        If ofd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            TorrentFile.Text = ofd.FileName
        End If
    End Sub

    Private Sub FindA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindA.Click
        If multifile Then
            Dim fbd As New FolderBrowserDialog

            If fbd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                FileA.Text = fbd.SelectedPath
            End If
        Else
            Dim ofd As New OpenFileDialog

            ofd.AddExtension = True
            ofd.AutoUpgradeEnabled = True
            ofd.CheckFileExists = True
            ofd.CheckPathExists = True
            ofd.Multiselect = False
            If ofd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                FileA.Text = ofd.FileName
            End If
        End If
    End Sub

    Private Sub FindB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindB.Click
        If multifile Then
            Dim fbd As New FolderBrowserDialog

            If fbd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                FileB.Text = fbd.SelectedPath
            End If
        Else
            Dim ofd As New OpenFileDialog

            ofd.AddExtension = True
            ofd.AutoUpgradeEnabled = True
            ofd.CheckFileExists = True
            ofd.CheckPathExists = True
            ofd.Multiselect = False
            If ofd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                FileB.Text = ofd.FileName
            End If
        End If
    End Sub

    Private Sub TorrentFile_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TorrentFile.TextChanged
        If System.IO.File.Exists(TorrentFile.Text) Then
            Try
                torrent = Bencode.Decode(System.IO.File.OpenRead(TorrentFile.Text))
            Catch ex As Bencode.ParseErrorException
                MessageBox.Show("Can't parse torrent file")
                Exit Sub
            End Try

            Dim info As Dictionary(Of String, Object)
            info = torrent("info")
            If info.ContainsKey("files") Then
                'this is multi-file
                multifile = True
                lblA.Text = "Source Dir"
                lblB.Text = "Source Dir"
            Else
                'this is single-file
                multifile = False
                lblA.Text = "Source File"
                lblB.Text = "Source File"
            End If
            lblA.Enabled = 1
            lblB.Enabled = 1
            FileA.Enabled = 1
            FileB.Enabled = 1
            FindA.Enabled = 1
            FindB.Enabled = 1
        Else
            lblA.Enabled = 0
            lblB.Enabled = 0
            FileA.Enabled = 0
            FileB.Enabled = 0
            FindA.Enabled = 0
            FindB.Enabled = 0
            CheckA.Enabled = 0
            CheckB.Enabled = 0
        End If
        FileA.Text = ""
        FileB.Text = ""
    End Sub

    Private Sub FileA_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileA.TextChanged
        If multifile Then
            CheckA.Enabled = System.IO.Directory.Exists(FileA.Text)
        Else
            CheckA.Enabled = System.IO.File.Exists(FileA.Text)
        End If
        CompleteA.Visible = 0
    End Sub

    Private Sub FileB_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileB.TextChanged
        If multifile Then
            CheckB.Enabled = System.IO.Directory.Exists(FileB.Text)
        Else
            CheckB.Enabled = System.IO.File.Exists(FileB.Text)
        End If
        CompleteB.Visible = 0
    End Sub
End Class
