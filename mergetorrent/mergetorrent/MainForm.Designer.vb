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
        Me.lbxSources = New System.Windows.Forms.ListBox()
        Me.btnAddTorrents = New System.Windows.Forms.Button()
        Me.btnAddFiles = New System.Windows.Forms.Button()
        Me.btnAddDirectory = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lbxSources
        '
        Me.lbxSources.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbxSources.FormattingEnabled = True
        Me.lbxSources.IntegralHeight = False
        Me.lbxSources.Location = New System.Drawing.Point(12, 12)
        Me.lbxSources.Name = "lbxSources"
        Me.lbxSources.Size = New System.Drawing.Size(655, 316)
        Me.lbxSources.TabIndex = 0
        '
        'btnAddTorrents
        '
        Me.btnAddTorrents.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAddTorrents.Location = New System.Drawing.Point(673, 12)
        Me.btnAddTorrents.Name = "btnAddTorrents"
        Me.btnAddTorrents.Size = New System.Drawing.Size(97, 23)
        Me.btnAddTorrents.TabIndex = 1
        Me.btnAddTorrents.Text = "Add Torrents..."
        Me.btnAddTorrents.UseVisualStyleBackColor = True
        '
        'btnAddFiles
        '
        Me.btnAddFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAddFiles.Location = New System.Drawing.Point(673, 41)
        Me.btnAddFiles.Name = "btnAddFiles"
        Me.btnAddFiles.Size = New System.Drawing.Size(97, 23)
        Me.btnAddFiles.TabIndex = 2
        Me.btnAddFiles.Text = "Add Files..."
        Me.btnAddFiles.UseVisualStyleBackColor = True
        '
        'btnAddDirectory
        '
        Me.btnAddDirectory.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAddDirectory.Location = New System.Drawing.Point(673, 70)
        Me.btnAddDirectory.Name = "btnAddDirectory"
        Me.btnAddDirectory.Size = New System.Drawing.Size(97, 23)
        Me.btnAddDirectory.TabIndex = 3
        Me.btnAddDirectory.Text = "Add Directory..."
        Me.btnAddDirectory.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStart.Location = New System.Drawing.Point(673, 305)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(97, 23)
        Me.btnStart.TabIndex = 4
        Me.btnStart.Text = "Start!"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(782, 340)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.btnAddDirectory)
        Me.Controls.Add(Me.btnAddFiles)
        Me.Controls.Add(Me.btnAddTorrents)
        Me.Controls.Add(Me.lbxSources)
        Me.Name = "MainForm"
        Me.Text = "mergetorrent"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lbxSources As System.Windows.Forms.ListBox
    Friend WithEvents btnAddTorrents As System.Windows.Forms.Button
    Friend WithEvents btnAddFiles As System.Windows.Forms.Button
    Friend WithEvents btnAddDirectory As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button

End Class
