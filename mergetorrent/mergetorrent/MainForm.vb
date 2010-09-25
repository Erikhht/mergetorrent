Public Class MainForm

    Private Const SHA1_HASHBYTES As Int64 = 20

    Private Class ListItemInfo
        Enum ListItemType
            Torrent
            Directory
            File
        End Enum

        Property Path As String
        Property Type As ListItemType
        Property Completion As Double = -1
        Property Checked As Double = -1
        Property Status As String = ""

        Sub New(ByVal Path As String, ByVal Type As ListItemType)
            Me.Path = Path
            Me.Type = Type
        End Sub

        Public Overrides Function ToString() As String
            ToString = Type.ToString & ": " & Path.ToString
            If Completion >= 0 Then
                ToString &= vbTab & "Verified: " & Completion.ToString("P02")
            End If
            If Checked >= 0 Then
                ToString &= vbTab & "Checked: " & Checked.ToString("P02")
            End If
            If Status.Length > 0 Then
                ToString &= " " & Status
            End If
        End Function
    End Class

    Private Sub btnAddTorrents_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddTorrents.Click
        Dim ofd As New OpenFileDialog

        ofd.AddExtension = True
        ofd.AutoUpgradeEnabled = True
        ofd.CheckFileExists = True
        ofd.CheckPathExists = True
        ofd.DefaultExt = ".torrent"
        ofd.DereferenceLinks = True
        ofd.Filter = "Torrents (*.torrent)|*.torrent|All files (*.*)|*.*"
        If System.IO.Directory.Exists(My.Computer.FileSystem.CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "uTorrent")) Then
            ofd.InitialDirectory = My.Computer.FileSystem.CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "uTorrent")
        End If
        ofd.Multiselect = True
        If ofd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            For Each filename As String In ofd.FileNames
                lbxSources.Items.Add(New ListItemInfo(filename, ListItemInfo.ListItemType.Torrent))
            Next
        End If
    End Sub

    Private Sub btnAddFiles_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddFiles.Click
        Dim ofd As New OpenFileDialog

        ofd.AddExtension = True
        ofd.AutoUpgradeEnabled = True
        ofd.CheckFileExists = True
        ofd.CheckPathExists = True
        ofd.DereferenceLinks = True
        ofd.Filter = "All files (*.*)|*.*"
        ofd.Multiselect = True
        If ofd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            For Each filename As String In ofd.FileNames
                lbxSources.Items.Add(New ListItemInfo(filename, ListItemInfo.ListItemType.File))
            Next
        End If
    End Sub

    Private Sub btnAddDirectory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddDirectory.Click
        Dim fbd As New FolderBrowserDialog
        fbd.ShowNewFolderButton = True
        If fbd.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            lbxSources.Items.Add(New ListItemInfo(fbd.SelectedPath, ListItemInfo.ListItemType.Directory))
        End If
    End Sub

    Private Function TorrentFilenameToMultiPath(ByVal torrent_filename As String) As List(Of MultiFileStream.FileInfo)
        TorrentFilenameToMultiPath = New List(Of MultiFileStream.FileInfo)
        Dim resume_dat_fs As System.IO.FileStream = System.IO.File.OpenRead(My.Computer.FileSystem.CombinePath(My.Computer.FileSystem.CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "uTorrent"), "resume.dat"))
        Dim resume_dat As Dictionary(Of String, Object) = DirectCast(Bencode.Decode(resume_dat_fs), Dictionary(Of String, Object))
        resume_dat_fs.Close()

        Dim torrent_fs As System.IO.FileStream = System.IO.File.OpenRead(torrent_filename)
        Dim torrent As Dictionary(Of String, Object) = DirectCast(Bencode.Decode(torrent_fs), Dictionary(Of String, Object))
        torrent_fs.Close()
        Dim current_torrent As Dictionary(Of String, Object) = DirectCast(resume_dat(My.Computer.FileSystem.GetName(torrent_filename)), Dictionary(Of String, Object))

        Dim info As Dictionary(Of String, Object)
        info = DirectCast(torrent("info"), Dictionary(Of String, Object))
        If resume_dat.ContainsKey(My.Computer.FileSystem.GetName(torrent_filename)) Then
            If info.ContainsKey("files") Then
                For file_index As Integer = 0 To DirectCast(DirectCast(torrent("info"), Dictionary(Of String, Object))("files"), List(Of Object)).Count - 1
                    Dim f As Dictionary(Of String, Object) = DirectCast(DirectCast(DirectCast(torrent("info"), Dictionary(Of String, Object))("files"), List(Of Object))(file_index), Dictionary(Of String, Object))
                    Dim source_filename As String = System.Text.Encoding.UTF8.GetString(DirectCast(current_torrent("path"), Byte()))
                    For Each path_element As Byte() In DirectCast(f("path"), List(Of Object))
                        source_filename = My.Computer.FileSystem.CombinePath(source_filename, System.Text.Encoding.UTF8.GetString(path_element))
                    Next
                    If current_torrent.ContainsKey("targets") Then 'override
                        For Each current_target As List(Of Object) In DirectCast(current_torrent("targets"), List(Of Object))
                            If DirectCast(current_target(0), Long) = file_index Then
                                source_filename = System.Text.Encoding.UTF8.GetString(DirectCast(current_target(1), Byte()))
                                Exit For
                            End If
                        Next
                    End If

                    Dim fi As New MultiFileStream.FileInfo(source_filename, DirectCast(f("length"), Long))
                    TorrentFilenameToMultiPath.Add(fi)
                Next
            ElseIf info.ContainsKey("length") Then
                If current_torrent.ContainsKey("path") Then
                    Dim source_filename As String = System.Text.Encoding.UTF8.GetString(DirectCast(current_torrent("path"), Byte()))
                    Dim fi As New MultiFileStream.FileInfo(source_filename, DirectCast(info("length"), Long))
                    TorrentFilenameToMultiPath.Add(fi)
                End If
            End If
        End If
    End Function

    ''' <summary>
    ''' Find all files that are a certain length
    ''' </summary>
    ''' <param name="target_length"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FindAllByLength(ByVal target_length As Long) As List(Of String)
        FindAllByLength = New List(Of String)

        For Each possible_source As ListItemInfo In lbxSources.Items
            Select Case possible_source.Type
                Case ListItemInfo.ListItemType.Torrent
                    Dim br As New System.IO.BinaryReader(System.IO.File.OpenRead(possible_source.Path))
                    Dim possible_source_torrent As Dictionary(Of String, Object) = Bencode.DecodeDictionary(br)
                    br.Close()
                    Dim possible_source_info As Dictionary(Of String, Object)
                    possible_source_info = DirectCast(possible_source_torrent("info"), Dictionary(Of String, Object))
                    Dim m As List(Of MultiFileStream.FileInfo) = TorrentFilenameToMultiPath(possible_source.Path)
                    'now we have files to look at
                    For Each fi As MultiFileStream.FileInfo In m
                        For Each s As String In fi.Path
                            If Not FindAllByLength.Contains(s) AndAlso _
                               My.Computer.FileSystem.FileExists(s) AndAlso _
                               My.Computer.FileSystem.GetFileInfo(s).Length = target_length Then
                                FindAllByLength.Add(s)
                            End If
                        Next
                    Next
                Case ListItemInfo.ListItemType.File
                    If Not FindAllByLength.Contains(possible_source.Path) AndAlso _
                       My.Computer.FileSystem.FileExists(possible_source.Path) AndAlso _
                       My.Computer.FileSystem.GetFileInfo(possible_source.Path).Length = target_length Then
                        FindAllByLength.Add(possible_source.Path)
                    End If

                Case ListItemInfo.ListItemType.Directory
                    Dim directory_stack As New Queue(Of System.IO.DirectoryInfo)
                    directory_stack.Enqueue(My.Computer.FileSystem.GetDirectoryInfo(possible_source.Path))
                    Do While directory_stack.Count > 0
                        For Each f As System.IO.FileInfo In directory_stack.Peek.GetFiles
                            If Not FindAllByLength.Contains(f.FullName) AndAlso _
                               f.Length = target_length Then
                                FindAllByLength.Add(f.FullName)
                            End If
                        Next
                        For Each d As System.IO.DirectoryInfo In directory_stack.Peek.GetDirectories
                            directory_stack.Enqueue(d)
                        Next
                        directory_stack.Dequeue() 'don't need it anymore
                    Loop
            End Select
        Next
    End Function

    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        btnStart.Enabled = False
        btnStart.Text = "Running..."
        Dim current_listitem_index As Integer = 0
        Do While current_listitem_index < lbxSources.Items.Count
            Dim current_listitem As ListItemInfo = DirectCast(lbxSources.Items(current_listitem_index), ListItemInfo)
            If current_listitem.Type = ListItemInfo.ListItemType.Torrent Then
                Dim out_stream As MultiFileStream
                Dim in_stream As MultiFileStream
                current_listitem.Status = "Finding destination files..."
                lbxSources.Items(current_listitem_index) = current_listitem
                My.Application.DoEvents()
                Dim files As List(Of MultiFileStream.FileInfo) = TorrentFilenameToMultiPath(current_listitem.Path)
                out_stream = New MultiFileStream(files, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.ReadWrite)

                current_listitem.Status = "Finding source files..."
                lbxSources.Items(current_listitem_index) = current_listitem
                My.Application.DoEvents()
                Dim lfi As New List(Of MultiFileStream.FileInfo)
                For Each fi As MultiFileStream.FileInfo In files
                    Dim new_paths As List(Of String) = FindAllByLength(fi.Length)
                    If new_paths.Contains(fi.Path(0)) AndAlso new_paths.IndexOf(fi.Path(0)) <> 0 Then 'make it first if it's there
                        new_paths.Remove(fi.Path(0))
                        new_paths.Insert(0, fi.Path(0))
                    End If

                    Dim new_fi As New MultiFileStream.FileInfo(new_paths, fi.Length)
                    lfi.Add(new_fi)

                    current_listitem.Status &= "."
                    lbxSources.Items(current_listitem_index) = current_listitem
                    My.Application.DoEvents()
                Next
                in_stream = New MultiFileStream(lfi, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)

                current_listitem.Status = "" 'we'll update very soon anyway
                'now we have all the files that might work.  Start checking and merging.

                Dim torrent As Dictionary(Of String, Object)
                Dim br As New System.IO.BinaryReader(System.IO.File.OpenRead(current_listitem.Path))
                torrent = Bencode.DecodeDictionary(br)
                br.Close()
                Dim info As Dictionary(Of String, Object)
                info = DirectCast(torrent("info"), Dictionary(Of String, Object))

                Dim piece_len As Integer = CType(info("piece length"), Integer)
                Dim buffer(0 To (piece_len - 1)) As Byte
                Dim hash_result() As Byte
                Dim pieces() As Byte = DirectCast(info("pieces"), Byte())
                Dim pieces_position As Integer = 0
                Dim complete_pieces As Int64 = 0
                Dim doevents_period As TimeSpan = New TimeSpan(0, 0, 0, 0, 500) 'every 1/2 second
                Dim last_doevents As Date = Date.MinValue

                Do While pieces_position < pieces.Length
                    If last_doevents + doevents_period <= Now Then
                        current_listitem.Completion = CDbl(complete_pieces) / CDbl(pieces.Length)
                        current_listitem.Checked = CDbl(pieces_position) / CDbl(pieces.Length)
                        lbxSources.Items(current_listitem_index) = current_listitem
                        My.Application.DoEvents()
                        last_doevents = Now
                    End If
                    Dim read_len As Integer

                    read_len = CInt(Math.Min(piece_len, in_stream.Length - in_stream.Position))
                    'try the out_stream first
                    out_stream.Read(buffer, 0, read_len)
                    hash_result = CheckHash.Hash(buffer, read_len)
                    Dim i As Integer = 0
                    Do While i < 20 AndAlso pieces(pieces_position + i) = hash_result(i)
                        i += 1
                    Loop
                    If i = 20 Then
                        'match!  No need to read from the in_stream
                        in_stream.Position += read_len
                        complete_pieces += 20
                    Else
                        out_stream.Position -= read_len 'back up
                        Dim useful_permutation As List(Of Integer) = in_stream.GetPermutation()
                        Do
                            in_stream.Read(buffer, 0, read_len)
                            hash_result = CheckHash.Hash(buffer, read_len)
                            i = 0
                            Do While i < 20 AndAlso pieces(pieces_position + i) = hash_result(i)
                                i += 1
                            Loop
                            If i = 20 Then
                                'match!
                                complete_pieces += 20
                                out_stream.Write(buffer, 0, read_len)
                                Exit Do
                            Else
                                'no match, try the next permutation
                                in_stream.NextPermutation(in_stream.Position - read_len, read_len)
                                If (MultiFileStream.ComparePermutation(in_stream.GetPermutation, useful_permutation)) Then
                                    'this piece can't be completed, let's move on
                                    out_stream.Position += read_len
                                    Exit Do
                                Else
                                    in_stream.Position -= read_len 'try again with the new permutation
                                End If
                            End If
                        Loop
                    End If
                    pieces_position += 20
                Loop
                current_listitem.Completion = CDbl(complete_pieces) / CDbl(pieces.Length)
                current_listitem.Checked = CDbl(pieces_position) / CDbl(pieces.Length)
                lbxSources.Items(current_listitem_index) = current_listitem
                My.Application.DoEvents()
                last_doevents = Now
            End If
            current_listitem_index = current_listitem_index + 1
        Loop
        btnStart.Enabled = True
        btnStart.Text = "Start!"
    End Sub
End Class
