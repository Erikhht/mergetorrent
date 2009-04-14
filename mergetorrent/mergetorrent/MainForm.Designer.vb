<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.CheckA = New System.Windows.Forms.Button
        Me.TorrentFile = New System.Windows.Forms.TextBox
        Me.lblTorrentFile = New System.Windows.Forms.Label
        Me.FindTorent = New System.Windows.Forms.Button
        Me.FileA = New System.Windows.Forms.TextBox
        Me.lblA = New System.Windows.Forms.Label
        Me.FindA = New System.Windows.Forms.Button
        Me.CompleteA = New System.Windows.Forms.Label
        Me.CompleteB = New System.Windows.Forms.Label
        Me.FindB = New System.Windows.Forms.Button
        Me.lblB = New System.Windows.Forms.Label
        Me.FileB = New System.Windows.Forms.TextBox
        Me.CheckB = New System.Windows.Forms.Button
        Me.CompleteTarget = New System.Windows.Forms.Label
        Me.Merge = New System.Windows.Forms.Button
        Me.FindTarget = New System.Windows.Forms.Button
        Me.FileTarget = New System.Windows.Forms.TextBox
        Me.lblTarget = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'CheckA
        '
        Me.CheckA.Enabled = False
        Me.CheckA.Location = New System.Drawing.Point(540, 32)
        Me.CheckA.Name = "CheckA"
        Me.CheckA.Size = New System.Drawing.Size(68, 20)
        Me.CheckA.TabIndex = 0
        Me.CheckA.Text = "Check"
        Me.CheckA.UseVisualStyleBackColor = True
        '
        'TorrentFile
        '
        Me.TorrentFile.Location = New System.Drawing.Point(96, 6)
        Me.TorrentFile.Name = "TorrentFile"
        Me.TorrentFile.Size = New System.Drawing.Size(364, 20)
        Me.TorrentFile.TabIndex = 1
        '
        'lblTorrentFile
        '
        Me.lblTorrentFile.AutoSize = True
        Me.lblTorrentFile.Location = New System.Drawing.Point(12, 9)
        Me.lblTorrentFile.Name = "lblTorrentFile"
        Me.lblTorrentFile.Size = New System.Drawing.Size(60, 13)
        Me.lblTorrentFile.TabIndex = 2
        Me.lblTorrentFile.Text = "Torrent File"
        '
        'FindTorent
        '
        Me.FindTorent.Location = New System.Drawing.Point(466, 6)
        Me.FindTorent.Name = "FindTorent"
        Me.FindTorent.Size = New System.Drawing.Size(68, 20)
        Me.FindTorent.TabIndex = 3
        Me.FindTorent.Text = "Browse..."
        Me.FindTorent.UseVisualStyleBackColor = True
        '
        'FileA
        '
        Me.FileA.Enabled = False
        Me.FileA.Location = New System.Drawing.Point(96, 32)
        Me.FileA.Name = "FileA"
        Me.FileA.Size = New System.Drawing.Size(364, 20)
        Me.FileA.TabIndex = 4
        '
        'lblA
        '
        Me.lblA.AutoSize = True
        Me.lblA.Enabled = False
        Me.lblA.Location = New System.Drawing.Point(12, 36)
        Me.lblA.Name = "lblA"
        Me.lblA.Size = New System.Drawing.Size(78, 13)
        Me.lblA.TabIndex = 5
        Me.lblA.Text = "Source File/Dir"
        '
        'FindA
        '
        Me.FindA.Enabled = False
        Me.FindA.Location = New System.Drawing.Point(466, 32)
        Me.FindA.Name = "FindA"
        Me.FindA.Size = New System.Drawing.Size(68, 20)
        Me.FindA.TabIndex = 6
        Me.FindA.Text = "Browse..."
        Me.FindA.UseVisualStyleBackColor = True
        '
        'CompleteA
        '
        Me.CompleteA.AutoSize = True
        Me.CompleteA.Location = New System.Drawing.Point(633, 35)
        Me.CompleteA.Name = "CompleteA"
        Me.CompleteA.Size = New System.Drawing.Size(33, 13)
        Me.CompleteA.TabIndex = 8
        Me.CompleteA.Text = "100%"
        Me.CompleteA.Visible = False
        '
        'CompleteB
        '
        Me.CompleteB.AutoSize = True
        Me.CompleteB.Location = New System.Drawing.Point(633, 61)
        Me.CompleteB.Name = "CompleteB"
        Me.CompleteB.Size = New System.Drawing.Size(33, 13)
        Me.CompleteB.TabIndex = 13
        Me.CompleteB.Text = "100%"
        Me.CompleteB.Visible = False
        '
        'FindB
        '
        Me.FindB.Enabled = False
        Me.FindB.Location = New System.Drawing.Point(466, 58)
        Me.FindB.Name = "FindB"
        Me.FindB.Size = New System.Drawing.Size(68, 20)
        Me.FindB.TabIndex = 12
        Me.FindB.Text = "Browse..."
        Me.FindB.UseVisualStyleBackColor = True
        '
        'lblB
        '
        Me.lblB.AutoSize = True
        Me.lblB.Enabled = False
        Me.lblB.Location = New System.Drawing.Point(12, 62)
        Me.lblB.Name = "lblB"
        Me.lblB.Size = New System.Drawing.Size(78, 13)
        Me.lblB.TabIndex = 11
        Me.lblB.Text = "Source File/Dir"
        '
        'FileB
        '
        Me.FileB.Enabled = False
        Me.FileB.Location = New System.Drawing.Point(96, 58)
        Me.FileB.Name = "FileB"
        Me.FileB.Size = New System.Drawing.Size(364, 20)
        Me.FileB.TabIndex = 10
        '
        'CheckB
        '
        Me.CheckB.Enabled = False
        Me.CheckB.Location = New System.Drawing.Point(540, 58)
        Me.CheckB.Name = "CheckB"
        Me.CheckB.Size = New System.Drawing.Size(68, 20)
        Me.CheckB.TabIndex = 9
        Me.CheckB.Text = "Check"
        Me.CheckB.UseVisualStyleBackColor = True
        '
        'CompleteTarget
        '
        Me.CompleteTarget.AutoSize = True
        Me.CompleteTarget.Location = New System.Drawing.Point(633, 105)
        Me.CompleteTarget.Name = "CompleteTarget"
        Me.CompleteTarget.Size = New System.Drawing.Size(33, 13)
        Me.CompleteTarget.TabIndex = 18
        Me.CompleteTarget.Text = "100%"
        Me.CompleteTarget.Visible = False
        '
        'Merge
        '
        Me.Merge.Enabled = False
        Me.Merge.Location = New System.Drawing.Point(540, 101)
        Me.Merge.Name = "Merge"
        Me.Merge.Size = New System.Drawing.Size(68, 20)
        Me.Merge.TabIndex = 14
        Me.Merge.Text = "Merge"
        Me.Merge.UseVisualStyleBackColor = True
        '
        'FindTarget
        '
        Me.FindTarget.Enabled = False
        Me.FindTarget.Location = New System.Drawing.Point(466, 101)
        Me.FindTarget.Name = "FindTarget"
        Me.FindTarget.Size = New System.Drawing.Size(68, 20)
        Me.FindTarget.TabIndex = 17
        Me.FindTarget.Text = "Browse..."
        Me.FindTarget.UseVisualStyleBackColor = True
        '
        'FileTarget
        '
        Me.FileTarget.Enabled = False
        Me.FileTarget.Location = New System.Drawing.Point(96, 101)
        Me.FileTarget.Name = "FileTarget"
        Me.FileTarget.Size = New System.Drawing.Size(364, 20)
        Me.FileTarget.TabIndex = 15
        '
        'lblTarget
        '
        Me.lblTarget.AutoSize = True
        Me.lblTarget.Enabled = False
        Me.lblTarget.Location = New System.Drawing.Point(15, 104)
        Me.lblTarget.Name = "lblTarget"
        Me.lblTarget.Size = New System.Drawing.Size(75, 13)
        Me.lblTarget.TabIndex = 16
        Me.lblTarget.Text = "Target File/Dir"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(685, 133)
        Me.Controls.Add(Me.CompleteTarget)
        Me.Controls.Add(Me.FindTarget)
        Me.Controls.Add(Me.lblTarget)
        Me.Controls.Add(Me.FileTarget)
        Me.Controls.Add(Me.Merge)
        Me.Controls.Add(Me.CompleteB)
        Me.Controls.Add(Me.FindB)
        Me.Controls.Add(Me.lblB)
        Me.Controls.Add(Me.FileB)
        Me.Controls.Add(Me.CheckB)
        Me.Controls.Add(Me.CompleteA)
        Me.Controls.Add(Me.FindA)
        Me.Controls.Add(Me.lblA)
        Me.Controls.Add(Me.FileA)
        Me.Controls.Add(Me.FindTorent)
        Me.Controls.Add(Me.lblTorrentFile)
        Me.Controls.Add(Me.TorrentFile)
        Me.Controls.Add(Me.CheckA)
        Me.Name = "MainForm"
        Me.Text = "mergetorrent"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CheckA As System.Windows.Forms.Button
    Friend WithEvents TorrentFile As System.Windows.Forms.TextBox
    Friend WithEvents lblTorrentFile As System.Windows.Forms.Label
    Friend WithEvents FindTorent As System.Windows.Forms.Button
    Friend WithEvents FileA As System.Windows.Forms.TextBox
    Friend WithEvents lblA As System.Windows.Forms.Label
    Friend WithEvents FindA As System.Windows.Forms.Button
    Friend WithEvents CompleteA As System.Windows.Forms.Label
    Friend WithEvents CompleteB As System.Windows.Forms.Label
    Friend WithEvents FindB As System.Windows.Forms.Button
    Friend WithEvents lblB As System.Windows.Forms.Label
    Friend WithEvents FileB As System.Windows.Forms.TextBox
    Friend WithEvents CheckB As System.Windows.Forms.Button
    Friend WithEvents CompleteTarget As System.Windows.Forms.Label
    Friend WithEvents Merge As System.Windows.Forms.Button
    Friend WithEvents FindTarget As System.Windows.Forms.Button
    Friend WithEvents FileTarget As System.Windows.Forms.TextBox
    Friend WithEvents lblTarget As System.Windows.Forms.Label

End Class
