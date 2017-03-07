<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.lblOSD = New System.Windows.Forms.Label()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.ntfyMain = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ctxtExit = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ctxtExit.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblOSD
        '
        Me.lblOSD.BackColor = System.Drawing.Color.Transparent
        Me.lblOSD.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblOSD.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOSD.ForeColor = System.Drawing.Color.White
        Me.lblOSD.Location = New System.Drawing.Point(0, 2)
        Me.lblOSD.Name = "lblOSD"
        Me.lblOSD.Size = New System.Drawing.Size(335, 59)
        Me.lblOSD.TabIndex = 19
        Me.lblOSD.Text = "VSX Volume" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Cyanlabs 2017"
        Me.lblOSD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblOSD.UseCompatibleTextRendering = True
        Me.lblOSD.UseMnemonic = False
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        '
        'ntfyMain
        '
        Me.ntfyMain.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.ntfyMain.BalloonTipText = "Click to exit"
        Me.ntfyMain.BalloonTipTitle = "VSX Volume"
        Me.ntfyMain.ContextMenuStrip = Me.ctxtExit
        Me.ntfyMain.Icon = CType(resources.GetObject("ntfyMain.Icon"), System.Drawing.Icon)
        Me.ntfyMain.Text = "VSX Volume"
        Me.ntfyMain.Visible = True
        '
        'ctxtExit
        '
        Me.ctxtExit.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.ctxtExit.Name = "ctxtExit"
        Me.ctxtExit.Size = New System.Drawing.Size(93, 26)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(335, 55)
        Me.Controls.Add(Me.lblOSD)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "Main"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Main"
        Me.TopMost = True
        Me.ctxtExit.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblOSD As Label
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents ntfyMain As NotifyIcon
    Friend WithEvents ctxtExit As ContextMenuStrip
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
End Class
