Public Class MainForm
    Dim torrent As Dictionary(Of String, Object)
    Dim multifile As Boolean

    Private Const SHA1_HASHBYTES As Int64 = 20

    Private Sub Merge_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Merge.Click
        Merge.Enabled = False
        Dim out_stream As System.IO.Stream
        Dim in_stream(0 To 1) As System.IO.Stream
        Dim info As Dictionary(Of String, Object) = DirectCast(torrent("info"), Dictionary(Of String, Object))
        Dim best_stream As Integer = 0
        Dim out_stream_id As Integer
        If multifile Then
            Dim new_files As New List(Of Dictionary(Of String, Object))
            For Each d As Dictionary(Of String, Object) In DirectCast(info("files"), List(Of Object))
                new_files.Add(d)
            Next
            If FileA.Text = FileTarget.Text Then
                in_stream(0) = New MultiFileStream(FileA.Text, new_files)
                in_stream(1) = New MultiFileStream(FileB.Text, new_files)
                out_stream = in_stream(0)
                best_stream = 0
                out_stream_id = 0
            ElseIf FileB.Text = FileTarget.Text Then
                in_stream(0) = New MultiFileStream(FileA.Text, new_files)
                in_stream(1) = New MultiFileStream(FileB.Text, new_files)
                out_stream = in_stream(1)
                out_stream_id = 1
                best_stream = 1
            Else
                in_stream(0) = New MultiFileStream(FileA.Text, new_files)
                in_stream(1) = New MultiFileStream(FileB.Text, new_files)
                out_stream = New MultiFileStream(FileTarget.Text, new_files)
                out_stream_id = -1
            End If
        Else
            If FileA.Text = FileTarget.Text Then
                in_stream(0) = System.IO.File.Open(FileA.Text, IO.FileMode.Open, IO.FileAccess.ReadWrite)
                in_stream(1) = System.IO.File.OpenRead(FileB.Text)
                out_stream = in_stream(0)
                best_stream = 0
                out_stream_id = 0
            ElseIf FileB.Text = FileTarget.Text Then
                in_stream(0) = System.IO.File.OpenRead(FileA.Text)
                in_stream(1) = System.IO.File.Open(FileB.Text, IO.FileMode.Open, IO.FileAccess.ReadWrite)
                out_stream = in_stream(1)
                best_stream = 1
                out_stream_id = 1
            Else
                in_stream(0) = System.IO.File.OpenRead(FileA.Text)
                in_stream(1) = System.IO.File.OpenRead(FileB.Text)
                out_stream = System.IO.File.OpenWrite(FileTarget.Text)
                out_stream_id = -1
            End If
        End If
        Dim piece_len As Integer = CType(info("piece length"), Integer)
        Dim buffer(0 To (piece_len - 1)) As Byte
        Dim hash_result() As Byte
        Dim pieces() As Byte = DirectCast(info("pieces"), Byte())
        Dim pieces_position As Integer = 0
        Dim complete_pieces As Int64 = 0
        Dim doevents_period As Int64
        Dim doevents_counter As Int64 = 0

        doevents_period = CInt((pieces.Length) / SHA1_HASHBYTES / 50) 'update the screen after each ~2%
        If doevents_period > 20 Then doevents_period = 20 'but no less frequently then every 30 hashes

        CompleteTarget.Visible = True
        CompleteTarget.Text = (CDbl(complete_pieces) / CDbl(pieces.Length)).ToString("P02")

        Do While pieces_position < pieces.Length
            Dim read_len As Integer

            If in_stream(best_stream).Position + piece_len < in_stream(best_stream).Length Then
                read_len = piece_len
            Else
                read_len = CInt(in_stream(best_stream).Length - in_stream(best_stream).Position)
            End If
            in_stream(best_stream).Read(buffer, 0, read_len)
            hash_result = CheckHash.Hash(buffer, read_len)

            Dim i As Integer = 0
            Do While i < 20 AndAlso pieces(pieces_position + i) = hash_result(i)
                i += 1
            Loop
            If i = 20 Then
                'piece match, so far only in_stream(best_stream) has been advanced
                complete_pieces += 20
                If out_stream_id = best_stream Then
                    'in_stream(best_stream) already advanced
                    'out_stream is in_stream(best_stream) and has already advanced
                    'out_stream already has a copy of in_stream(best_stream)
                    in_stream(1 - best_stream).Position += read_len 'advance in_stream(1-best_stream)
                ElseIf out_stream_id = 1 - best_stream Then
                    'in_stream(best_stream) already advanced
                    out_stream.Write(buffer, 0, read_len) 'advance in_stream(1-best_stream) and out_stream together while writing
                Else 'out_stream_id = -1
                    'in_stream(best_stream) already advanced
                    out_stream.Write(buffer, 0, read_len) 'advance out_stream while writing
                    in_stream(1 - best_stream).Position += read_len 'advance in_stream(1-best_stream)
                End If
            Else 'no match, check the other stream
                best_stream = 1 - best_stream
                in_stream(best_stream).Read(buffer, 0, read_len)
                hash_result = CheckHash.Hash(buffer, read_len)
                Dim j As Integer = 0
                Do While i < 20 AndAlso pieces(pieces_position + i) = hash_result(i)
                    j += 1
                Loop
                If j = 20 Then
                    'piece match, both in_streams already advanced
                    complete_pieces += 20
                    If out_stream_id = best_stream Then
                        'in_stream(0,1) already advanced
                        'out_stream_id = in_stream(0 or 1) so no need to advance
                        'out_stream_id = best_stream so no need to copy
                    ElseIf out_stream_id = 1 - best_stream Then
                        'in_stream(0,1) already advanced
                        out_stream.Position -= read_len 'out_stream already advanced so we need to back up
                        out_stream.Write(buffer, 0, read_len) 'now back to where it was
                    Else
                        'in_stream(0,1) already advanced
                        out_stream.Write(buffer, 0, read_len) 'out_stream is advanced and written
                    End If
                Else
                    'piece not found in either source
                    best_stream = 1 - best_stream 'back to our original best
                    out_stream.Position += read_len 'nothing to copy, too bad
                End If
            End If
            pieces_position += 20
            doevents_counter += 1
            If doevents_counter >= doevents_period Then
                CompleteTarget.Text = (CDbl(complete_pieces) / CDbl(pieces.Length)).ToString("P02")
                Merge.Text = (CDbl(pieces_position) / CDbl(pieces.Length)).ToString("P02")
                My.Application.DoEvents()
                doevents_counter = 0
            End If
        Loop
        CompleteTarget.Text = (CDbl(complete_pieces) / CDbl(pieces.Length)).ToString("P02")
        Merge.Text = (CDbl(pieces_position) / CDbl(pieces.Length)).ToString("P02")
        in_stream(0).Flush()
        in_stream(0).Close()
        in_stream(0).Dispose()
        in_stream(1).Flush()
        in_stream(1).Close()
        in_stream(1).Dispose()
        If out_stream_id = -1 Then
            out_stream.Flush()
            out_stream.Close()
            out_stream.Dispose()
        End If
        Merge.Enabled = True
        Merge.Text = "Merge"
    End Sub

    Private Sub CheckFile(ByVal Source As String, ByVal btn As Button, ByVal completion As Label)
        btn.Enabled = False
        Dim in_stream As System.IO.Stream
        Dim info As Dictionary(Of String, Object) = DirectCast(torrent("info"), Dictionary(Of String, Object))
        If multifile Then
            Dim new_files As New List(Of Dictionary(Of String, Object))
            For Each d As Dictionary(Of String, Object) In DirectCast(info("files"), List(Of Object))
                new_files.Add(d)
            Next
            in_stream = New MultiFileStream(Source, new_files)
        Else
            in_stream = System.IO.File.OpenRead(Source)
        End If

        Dim piece_len As Integer = CType(info("piece length"), Integer)
        Dim buffer(0 To (piece_len - 1)) As Byte
        Dim hash_result() As Byte
        Dim pieces() As Byte = DirectCast(info("pieces"), Byte())
        Dim pieces_position As Integer = 0
        Dim complete_pieces As Int64 = 0
        Dim doevents_period As Int64
        Dim doevents_counter As Int64 = 0

        doevents_period = CInt((pieces.Length) / SHA1_HASHBYTES / 50) 'update the screen after each ~2%
        If doevents_period > 20 Then doevents_period = 20 'but no less frequently then every 30 hashes

        completion.Visible = True
        completion.Text = (CDbl(complete_pieces) / CDbl(pieces.Length)).ToString("P02")

        Do While pieces_position < pieces.Length
            Dim read_len As Integer

            If in_stream.Position + piece_len <= in_stream.Length Then
                read_len = piece_len
            Else
                read_len = CInt(in_stream.Length - in_stream.Position)
            End If
            in_stream.Read(buffer, 0, read_len)
            hash_result = CheckHash.Hash(buffer, read_len)

            Dim i As Integer = 0
            Do While i < 20 AndAlso pieces(pieces_position + i) = hash_result(i)
                i += 1
            Loop
            If i = 20 Then
                'piece match
                complete_pieces += 20
            End If
            pieces_position += 20
            doevents_counter += 1
            If doevents_counter >= doevents_period Then
                completion.Text = (CDbl(complete_pieces) / CDbl(pieces.Length)).ToString("P02")
                btn.Text = (CDbl(pieces_position) / CDbl(pieces.Length)).ToString("P02")
                My.Application.DoEvents()
                doevents_counter = 0
            End If
        Loop
        completion.Text = (CDbl(complete_pieces) / CDbl(pieces.Length)).ToString("P02")
        btn.Text = (CDbl(pieces_position) / CDbl(pieces.Length)).ToString("P02")
        in_stream.Flush()
        in_stream.Close()
        in_stream.Dispose()
        btn.Enabled = True
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

    Private Sub FindTarget_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindTarget.Click
        If multifile Then
            Dim fbd As New FolderBrowserDialog

            fbd.ShowNewFolderButton = True
            Dim di As New System.IO.DirectoryInfo(FileA.Text)
            fbd.SelectedPath = FileA.Text 'by default, merge into a
            If fbd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                FileTarget.Text = fbd.SelectedPath
            End If
        Else
            Dim sfd As New SaveFileDialog

            sfd.AutoUpgradeEnabled = True
            sfd.CheckPathExists = True
            sfd.FileName = FileA.Text
            Dim fi As New System.IO.FileInfo(FileA.Text)
            sfd.InitialDirectory = fi.DirectoryName
            If sfd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                FileTarget.Text = sfd.FileName
            End If
        End If
    End Sub

    Private Sub TorrentFile_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TorrentFile.TextChanged
        If System.IO.File.Exists(TorrentFile.Text) Then
            Try
                torrent = Bencode.DecodeDictionary(New System.IO.BinaryReader(System.IO.File.OpenRead(TorrentFile.Text)))
            Catch ex As Bencode.ParseErrorException
                MessageBox.Show("Can't parse torrent file")
                Exit Sub
            End Try

            Dim info As Dictionary(Of String, Object)
            info = DirectCast(torrent("info"), Dictionary(Of String, Object))
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
            lblA.Enabled = True
            lblB.Enabled = True
            FileA.Enabled = True
            FileB.Enabled = True
            FindA.Enabled = True
            FindB.Enabled = True
        Else
            lblA.Enabled = False
            lblB.Enabled = False
            FileA.Enabled = False
            FileB.Enabled = False
            FindA.Enabled = False
            FindB.Enabled = False
            CheckA.Enabled = False
            CheckB.Enabled = False
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
        CompleteA.Visible = False
        If CheckA.Enabled And CheckB.Enabled Then
            lblTarget.Enabled = True
            FileTarget.Enabled = True
            FindTarget.Enabled = True
        End If
    End Sub

    Private Sub FileB_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileB.TextChanged
        If multifile Then
            CheckB.Enabled = System.IO.Directory.Exists(FileB.Text)
        Else
            CheckB.Enabled = System.IO.File.Exists(FileB.Text)
        End If
        CompleteB.Visible = False
        If CheckA.Enabled And CheckB.Enabled Then
            lblTarget.Enabled = True
            FileTarget.Enabled = True
            FindTarget.Enabled = True
        End If
    End Sub

    Private Sub FileTarget_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileTarget.TextChanged
        If multifile Then
            Dim valid As Boolean = False
            Try
                Dim d As System.IO.DirectoryInfo = New System.IO.DirectoryInfo(FileTarget.Text)
                valid = True
            Catch ex As ArgumentException
            Catch ex As System.IO.PathTooLongException
            Catch ex As NotSupportedException
            End Try
            Merge.Enabled = valid
        Else
            Dim valid As Boolean = False
            Try
                Dim f As System.IO.FileInfo = New System.IO.FileInfo(FileTarget.Text)
                valid = True
            Catch ex As ArgumentException
            Catch ex As System.IO.PathTooLongException
            Catch ex As NotSupportedException
            End Try
            Merge.Enabled = valid
        End If
    End Sub
End Class
