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
        Me.CheckForupdatesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.sliderVol = New System.Windows.Forms.TrackBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.imgPower = New System.Windows.Forms.PictureBox()
        Me.imgMute = New System.Windows.Forms.PictureBox()
        Me.ctxtExit.SuspendLayout()
        CType(Me.sliderVol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgPower, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgMute, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblOSD
        '
        Me.lblOSD.BackColor = System.Drawing.Color.Transparent
        Me.lblOSD.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblOSD.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOSD.ForeColor = System.Drawing.Color.White
        Me.lblOSD.Location = New System.Drawing.Point(0, 0)
        Me.lblOSD.Name = "lblOSD"
        Me.lblOSD.Size = New System.Drawing.Size(335, 53)
        Me.lblOSD.TabIndex = 19
        Me.lblOSD.Text = "VSX Volume" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Cyanlabs 2017"
        Me.lblOSD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        Me.ctxtExit.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CheckForupdatesToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.ctxtExit.Name = "ctxtExit"
        Me.ctxtExit.Size = New System.Drawing.Size(171, 54)
        '
        'CheckForupdatesToolStripMenuItem
        '
        Me.CheckForupdatesToolStripMenuItem.Name = "CheckForupdatesToolStripMenuItem"
        Me.CheckForupdatesToolStripMenuItem.Size = New System.Drawing.Size(170, 22)
        Me.CheckForupdatesToolStripMenuItem.Text = "Check for &updates"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(167, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(170, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'sliderVol
        '
        Me.sliderVol.AutoSize = False
        Me.sliderVol.Location = New System.Drawing.Point(48, 56)
        Me.sliderVol.Maximum = 185
        Me.sliderVol.Name = "sliderVol"
        Me.sliderVol.Size = New System.Drawing.Size(233, 23)
        Me.sliderVol.SmallChange = 2
        Me.sliderVol.TabIndex = 21
        Me.sliderVol.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(284, 50)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 30)
        Me.Label1.TabIndex = 23
        Me.Label1.Text = "100"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'imgPower
        '
        Me.imgPower.BackColor = System.Drawing.Color.Transparent
        Me.imgPower.Cursor = System.Windows.Forms.Cursors.Hand
        Me.imgPower.Image = Global.VSX_Volume.My.Resources.Resources.off
        Me.imgPower.Location = New System.Drawing.Point(310, 3)
        Me.imgPower.Name = "imgPower"
        Me.imgPower.Size = New System.Drawing.Size(20, 20)
        Me.imgPower.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.imgPower.TabIndex = 24
        Me.imgPower.TabStop = False
        '
        'imgMute
        '
        Me.imgMute.BackColor = System.Drawing.Color.Transparent
        Me.imgMute.Cursor = System.Windows.Forms.Cursors.Hand
        Me.imgMute.Image = CType(resources.GetObject("imgMute.Image"), System.Drawing.Image)
        Me.imgMute.Location = New System.Drawing.Point(12, 49)
        Me.imgMute.Name = "imgMute"
        Me.imgMute.Size = New System.Drawing.Size(32, 32)
        Me.imgMute.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.imgMute.TabIndex = 22
        Me.imgMute.TabStop = False
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(335, 90)
        Me.Controls.Add(Me.imgPower)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.imgMute)
        Me.Controls.Add(Me.sliderVol)
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
        CType(Me.sliderVol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgPower, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgMute, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblOSD As Label
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents ntfyMain As NotifyIcon
    Friend WithEvents ctxtExit As ContextMenuStrip
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents sliderVol As TrackBar
    Friend WithEvents imgMute As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents imgPower As PictureBox
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents CheckForupdatesToolStripMenuItem As ToolStripMenuItem
End Class
