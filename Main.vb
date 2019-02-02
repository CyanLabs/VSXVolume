' VSX Volume
' Cyanlabs 2017
' http://cyanlabs.net/application/vsx-volume

' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.

' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' You should have received a copy of the GNU General Public License
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports WindowsHookLib
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports Microsoft.Win32

Public Class Main
    Dim ip As IPAddress, tnSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), ep As IPEndPoint, startupcmd As Boolean = False
    Dim WithEvents KHook As New KeyboardHook
    Dim CheckScreen As New System.Threading.Thread(AddressOf UpdateScreen)
    Dim oncmd As String, offcmd As String, showosd As Boolean = False, statecmd As Boolean = False, manual As Boolean = False, crash As Boolean = False, screennum As Integer = 0, setscreen As Boolean = False, defaultinput As String
    Dim x As Integer, y As Integer

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AutoUpdaterDotNET.AutoUpdater.Start("http://cyanlabs.net/raw/latest.php?product=" & My.Application.Info.ProductName)
        KHook.InstallHook()
        Me.Opacity = 0

        'Collects the command line arguments and sets needed variables
        If Environment.GetCommandLineArgs.Count > 1 Then
            For Each arg In Environment.GetCommandLineArgs
                Select Case True
                    Case arg.ToLower.Contains("/ip=")
                        If IPAddress.TryParse(arg.Replace("/ip=", ""), ip) Then
                            ep = New IPEndPoint(System.Net.IPAddress.Parse(arg.Replace("/ip=", "")), 8102)
                        Else
                            MsgBox("Enter a valid IP Address as a command line argument (VSX Volume.exe /ip=xxx.xxx.xxx.xxx)", MsgBoxStyle.Exclamation, "VSX Volume - Error")
                            Application.Exit()
                        End If
                    Case arg.ToLower.Contains("/oncommands=")
                        oncmd = arg.Replace("/oncommands=", "").Replace("""", "").Trim
                    Case arg.ToLower.Contains("/defaultinput=")
                        defaultinput = arg.Replace("/defaultinput=", "").Replace("""", "").Trim
                        imgInput.Visible = True
                    Case arg.ToLower.Contains("/inputs=")
                        ''inputs = arg.Replace("/inputs=", "").Replace("""", "").Trim
                        imgInput.Visible = True
                    Case arg.ToLower.Contains("/offcommands=")
                        offcmd = arg.Replace("/offcommands=", "").Replace("""", "").Trim
                    Case arg.ToLower.Contains("/osd")
                        showosd = True
                    Case arg.ToLower = "/p"
                        statecmd = True
                    Case arg.ToLower = "/nokeybinds"
                        KHook.RemoveHook()
                        KHook.Dispose()
                    Case arg.ToLower.Contains("/screen=")
                        Try
                            screennum = arg.Replace("/screen=", "").Replace("""", "").Trim - 1
                            'Sets which display application will be shown on
                            If screennum > Screen.AllScreens.Length Then
                                ntfyMain.ShowBalloonTip(5000, "VSX Volume - Monitor not found", "Could not display application on your preferred monitor, defaulting to your primary monitor.", ToolTipIcon.Info)
                            Else
                                setscreen = True
                                x = Screen.AllScreens(screennum).WorkingArea.Width - (Me.Width + 5)
                                y = Screen.AllScreens(screennum).WorkingArea.Height - (Me.Height + 5)
                            End If
                        Catch ex As System.InvalidCastException
                            MsgBox("""" & arg.Replace("/screen=", "").Replace("""", "").Trim & """ is not a valid number for ""/screen""." & vbNewLine & vbNewLine & "Please choose a value between 1 and " & Screen.AllScreens.Length, MsgBoxStyle.Exclamation, "VSX Volume - Error")
                            crash = True
                            Application.Exit()
                        End Try
                    Case arg.ToLower.Contains("/x=")
                        Dim tmpx As String = arg.Replace("/x=", "").Replace("""", "").Trim
                        If IsNumeric(tmpx) Then x = tmpx
                    Case arg.ToLower.Contains("/y=")
                        Dim tmpy As String = arg.Replace("/y=", "").Replace("""", "").Trim
                        If IsNumeric(tmpy) Then y = tmpy
                    Case Else
                End Select

                Location = Screen.AllScreens(screennum).Bounds.Location + New Point(x, y)
            Next
        Else
            MsgBox("No arguments entered, please read the documentation at http://cyanlabs.net/application/vsx-volume for further information", MsgBoxStyle.Exclamation, "VSX Volume - Error")
            Application.Exit()
        End If
        Try
            tnSocket.Connect(ep)
        Catch ex As System.Net.Sockets.SocketException
            MsgBox("Connection to the Pioneer AVR could not be established, please check your network connectivity." & vbNewLine & vbNewLine & "VSX Volume will now close", MsgBoxStyle.Exclamation, "VSX Volume - Connection error")
            Environment.Exit(1)
        End Try
        If setscreen = False Then
            x = Screen.PrimaryScreen.WorkingArea.Width - (Me.Width + 5)
            y = Screen.PrimaryScreen.WorkingArea.Height - (Me.Height + 5)
            Location = New Point(x, y)
        End If

        SendCommands("?V")
        SendCommands("?M")
        SendCommands("?P")
        CheckScreen.IsBackground = True

        'checks command line arguments to see if osd is enabled
        If showosd Then CheckScreen.Start()

        'loop through on commands
        If Not oncmd = "" Then
            Dim RunOnCMD As New Threading.Thread(AddressOf OnCommands) With {.IsBackground = True}
            RunOnCMD.Start()
        End If

        'enables the state detection handler for resume/suspend commands
        If statecmd Then AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf SystemEvents_PowerModeChanged
    End Sub

    'loop through commands that run on startup
    Private Sub OnCommands()
        For Each cmd In oncmd.Split(",")
            SendCommands(cmd.ToUpper)
            System.Threading.Thread.Sleep(3000)
        Next
    End Sub

    'loop through commands that run on exit
    Private Sub OffCommands()
        If Not offcmd = "" Then
            For Each cmd In offcmd.Split(",")
                SendCommands(cmd.ToUpper)
                System.Threading.Thread.Sleep(3000)
            Next
        End If
    End Sub

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If crash = False Then OffCommands()
    End Sub

    'runs commands on resume / suspend
    Private Sub SystemEvents_PowerModeChanged(ByVal sender As Object, ByVal e As PowerModeChangedEventArgs)
        Try
            Select Case e.Mode
                Case PowerModes.Resume
                    If Not oncmd = "" Then
                        Dim RunOnCMD As New Threading.Thread(AddressOf OnCommands) With {.IsBackground = True}
                        RunOnCMD.Start()
                    End If
                Case PowerModes.Suspend
                    OffCommands()
            End Select
        Catch ex As Exception

        End Try
    End Sub

    'Sends command to VSX.
    Private Function SendCommands(ByVal cmd As String)
        Try
            Dim SendBytes As [Byte]() = Encoding.ASCII.GetBytes(cmd & vbCrLf)
            tnSocket.Send(SendBytes, SendBytes.Length, SocketFlags.None)
        Catch ex As Exception
            Dim msgresult As Integer = MsgBox("Connection to the Pioneer AVR has been lost, would you like to attempt to reconnect?" & vbNewLine & vbNewLine & "Click 'YES' to rety or 'NO' to exit VSX Volume", MessageBoxButtons.YesNo + MsgBoxStyle.Information, "VSX Volume - Connection lost")
            If msgresult = DialogResult.Yes Then
                crash = True
                Application.Restart()
            Else
                crash = True
                Application.Exit()
            End If
        End Try
        Return Nothing
    End Function

    'Quick sub to parse volume and remove/add needed amount of zero's.
    Private Sub ValidateVolume(volume As String)
        'if value is less than 10 pre-fix 2 "0"s else if less than 100 pre-fix 1 "0" else just send the command without added "0"s
        If imgMute.Image Is My.Resources.mute Then SendCommands("MF")
        If volume >= 185 Then
            SendCommands("185VL")
        ElseIf volume <= 0 Then
            SendCommands("000VL")
        ElseIf volume < 10 Then
            SendCommands("00" & volume & "VL")
        ElseIf volume < 100 Then
            SendCommands("0" & volume & "VL")
        Else
            SendCommands(volume & "VL")
        End If
    End Sub

    'update volume label on slider value change
    Private Sub SliderVol_ValueChanged(sender As Object, e As EventArgs) Handles sliderVol.ValueChanged
        Label1.Text = Math.Round(sliderVol.Value / sliderVol.Maximum * 100)
    End Sub

    'hide form when form has lost focus
    Private Sub Main_Deactivate(sender As Object, e As EventArgs) Handles MyBase.Deactivate
        Try
            If manual = True Then Me.Opacity = 0
            manual = False
        Catch ex As System.ComponentModel.Win32Exception
        End Try
    End Sub

    'send volume command when volume slider moves
    Private Sub SliderVol_Scroll(sender As Object, e As EventArgs) Handles sliderVol.Scroll
        ValidateVolume(sliderVol.Value)
    End Sub

    'send mute toggle command
    Private Sub ImgMute_Click(sender As Object, e As EventArgs) Handles imgMute.Click
        SendCommands("MZ")
    End Sub

    'send power toggle command, wait 5 seconds to allow AVR to fully power on or off
    Private Sub ImgPower_Click(sender As Object, e As EventArgs) Handles imgPower.Click
        SendCommands("PZ")
        Threading.Thread.Sleep(5000)
    End Sub

    'manual check for updates from notificaiton right click menu
    Private Sub CheckForupdatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckForupdatesToolStripMenuItem.Click
        AutoUpdaterDotNET.AutoUpdater.Start("http://cyanlabs.net/raw/latest.php?product=" & My.Application.Info.ProductName)
    End Sub

    'exits application will run off commands
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    'used for osd display, adds a border
    Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaintBackground(e)
        Dim rect As New Rectangle(0, 0, Me.ClientSize.Width - 1, Me.ClientSize.Height - 1)
        e.Graphics.DrawRectangle(Pens.White, rect)
    End Sub

    'check keypresses
    Private Sub KHook_KeyDown(ByVal sender As Object, ByVal e As WindowsHookLib.KeyboardEventArgs) Handles kHook.KeyDown
        If e.KeyCode = Keys.VolumeUp And Control.ModifierKeys = Keys.Control = False Then
            e.Handled = True
            SendCommands("VU")
        ElseIf e.KeyCode = Keys.VolumeDown And Control.ModifierKeys = Keys.Control = False Then
            e.Handled = True
            SendCommands("VD")
        ElseIf e.KeyCode = Keys.VolumeMute And Control.ModifierKeys = Keys.Control = False Then
            e.Handled = True
            SendCommands("MZ")
        ElseIf e.KeyCode = Keys.Pause And Control.ModifierKeys = Keys.Control = False And Control.ModifierKeys = Keys.Shift = False Then
            e.Handled = True
            SendCommands("PZ")
        ElseIf e.KeyCode = Keys.Pause And Control.ModifierKeys = Keys.Control = False And Control.ModifierKeys = Keys.Shift = True Then
            e.Handled = True
            SendCommands(defaultinput)
        End If
    End Sub

    'fade form in, wait 5 seconds, fade form out
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If manual = False Then
            UpdateOpacity(Me, 1)
            Threading.Thread.Sleep(5000)
            UpdateOpacity(Me, 0)
        End If
    End Sub

    Private Sub imgInput_MouseClick(sender As Object, e As Windows.Forms.MouseEventArgs) Handles imgInput.MouseClick
        If e.Button = MouseButtons.Left Then
            SendCommands(defaultinput)
        Else

        End If
    End Sub

    'Converts pioneers FL strings such as "FL022020202053544552454F20202020" to readable text "STEREO".
    'First we remove the FL. Basically it is hex code for a ASCII character so we substring each 2 characters from the string.
    'Then we "convert.toint32" them to get a integer we can then simply "Chr" the number to get the character.
    Function DecryptScreen(temp As String)
        Dim OSD As String = temp.ToString.Replace("FL", "").Replace(vbLf, "").Replace(vbCrLf, "")
        Dim output As String = ""
        For x As Integer = 0 To OSD.Length - 1 Step 2
            If OSD.Substring(x, 2) = "00" Then
            ElseIf OSD.Substring(x, 2) = "02" Then
            ElseIf OSD.Substring(x, 2) = "05" Then
                output = output & "DOLBY "
            ElseIf OSD.Substring(x, 2) = "06" Then
            ElseIf OSD.Substring(x, 2) = "08" Then
                output = output & "2"
            Else
                output = output & Chr(Convert.ToInt32("0x" & OSD.Substring(x, 2), 16))
            End If
        Next
        Return output
    End Function

    'Cross thread functions to safely update textbox, label and other controls
    Private Delegate Sub UpdateTrackbarDelegate(ByVal TB As TrackBar, ByVal value As String)
    Private Sub UpdateTrackbar(ByVal TB As TrackBar, ByVal value As String)
        If TB.InvokeRequired Then
            TB.Invoke(New UpdateTrackbarDelegate(AddressOf UpdateTrackbar), New Object() {TB, value})
        Else
            TB.Value = value
        End If
    End Sub
    Private Delegate Sub UpdateLabelDelegate(ByVal label As Label, ByVal value As String)

    'show OSD when notification is clicked
    Private Sub NtfyMain_MouseClick(sender As Object, e As Windows.Forms.MouseEventArgs) Handles ntfyMain.MouseClick
        If e.Button = MouseButtons.Left Then
            If Not CheckScreen.IsAlive Then
                CheckScreen.IsBackground = True
                CheckScreen.Start()
            End If
            manual = True
            Me.Opacity = 1
            Me.Activate()
        End If
    End Sub

    Private Sub UpdateLabel(ByVal label As Label, ByVal value As String)
        If label.InvokeRequired Then
            label.Invoke(New UpdateLabelDelegate(AddressOf UpdateLabel), New Object() {label, value})
        Else
            label.Text = value
        End If
    End Sub
    Private Delegate Sub EnableControlDelegate(ByVal control As Control, ByVal state As Boolean)
    Private Sub EnableControl(ByVal control As Control, ByVal state As Boolean)
        If control.InvokeRequired Then
            control.Invoke(New EnableControlDelegate(AddressOf EnableControl), New Object() {control, state})
        Else
            control.Enabled = state
        End If
    End Sub
    Private Delegate Sub UpdateOpacityDelegate(ByVal form As Form, ByVal value As Integer)
    Private Sub UpdateOpacity(ByVal form As Form, ByVal value As Integer)
        If form.InvokeRequired Then
            form.Invoke(New UpdateOpacityDelegate(AddressOf UpdateOpacity), New Object() {form, value})
        Else
            form.Opacity = value
        End If
    End Sub

    'parses the output recieved from the screen
    Sub ParseScreen(output As String)
        Try
            If Me.Opacity = 1 Or showosd Then
                output = output.Replace(vbLf, "").Replace(vbCrLf, "")
                If output.ToString.Contains("FL") Then
                    Dim decryptedOSD As String = DecryptScreen(output)
                    Me.lblOSD.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    UpdateLabel(lblOSD, decryptedOSD.ToString)
                    If Not BackgroundWorker1.IsBusy Then BackgroundWorker1.RunWorkerAsync()
                ElseIf output.ToString.Contains("MUT0") Then
                    imgMute.Image = My.Resources.mute
                ElseIf output.ToString.Contains("MUT1") Then
                    imgMute.Image = My.Resources.vol
                ElseIf output.ToString.Contains("VOL") Then
                    UpdateTrackbar(sliderVol, output.ToString.Replace("VOL", ""))
                ElseIf output.ToString.Contains("PWR0") Then
                    imgPower.Image = My.Resources.pwron
                    EnableControl(sliderVol, True)
                    EnableControl(imgMute, True)
                ElseIf output.ToString.Contains("PWR1") Then
                    imgPower.Image = My.Resources.off
                    EnableControl(sliderVol, False)
                    EnableControl(imgMute, False)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'Background Sub to constantly update the UI with updated information from the screen.
    Private Sub UpdateScreen()
        Dim output As String = ""
        Dim result As String()
        Dim RecvString As String = String.Empty
        Dim NumBytes As Integer = 0
        Dim RecvBytes(255) As [Byte]
        Try
            Do
                NumBytes = tnSocket.Receive(RecvBytes, RecvBytes.Length, 0)
                RecvString = RecvString + Encoding.ASCII.GetString(RecvBytes, 0, NumBytes)
                output = output & RecvString
                result = output.Split(vbCrLf)
            Loop While NumBytes = 256
            For Each i In result
                If i = vbCrLf Or i = vbLf Then Continue For
                ParseScreen(i)
            Next
            UpdateScreen()
        Catch ex As System.Net.Sockets.SocketException
            Dim msgresult As Integer = MsgBox("Connection to the Pioneer AVR has been lost, would you like to attempt to reconnect?" & vbNewLine & vbNewLine & "Click 'YES' to rety or 'NO' to exit VSX Volume", MessageBoxButtons.YesNo + MsgBoxStyle.Information, "VSX Volume - Connection lost")
            If msgresult = DialogResult.Yes Then
                crash = True
                Application.Restart()
            Else
                crash = True
                Application.Exit()
            End If
        End Try
    End Sub
End Class