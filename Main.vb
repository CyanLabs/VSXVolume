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
    Dim WithEvents kHook As New KeyboardHook
    Dim CheckScreen As New System.Threading.Thread(AddressOf UpdateScreen)
    Dim oncmd As String, offcmd As String, showosd As Boolean = False, hidesplash As Boolean = False, statecmd As Boolean = False

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AutoUpdaterDotNET.AutoUpdater.Start("http://cyanlabs.net/raw/latest.php?product=" & My.Application.Info.ProductName)
        kHook.InstallHook() 'installs keyboard hook
        Me.Opacity = 0 'hides form
        CheckForIllegalCrossThreadCalls = False  'TODO: do invoke etc to prevent needing this override
        If Environment.GetCommandLineArgs.Count > 1 Then 'loops through command line arguments
            For Each arg In Environment.GetCommandLineArgs
                Select Case True
                    Case arg.ToLower.Contains("/ip=")
                        If IPAddress.TryParse(arg.Replace("/ip=", ""), ip) Then
                            ep = New IPEndPoint(System.Net.IPAddress.Parse(arg.Replace("/ip=", "")), 8102) 'parse and validate IP
                        Else
                            MsgBox("Enter a valid IP Address as a command line argument (VSX Volume.exe /ip=xxx.xxx.xxx.xxx)", MsgBoxStyle.Exclamation, "Error")
                            Application.Exit()
                        End If

                    Case arg.ToLower.Contains("/oncommands=")
                        If arg.ToLower.Contains("/oncommands=") Then
                            oncmd = arg.Replace("/oncommands=", "").Replace("""", "").Trim
                        End If

                    Case arg.ToLower.Contains("/offcommands=")
                        offcmd = arg.Replace("/offcommands=", "").Replace("""", "").Trim

                    Case arg.ToLower.Contains("/osd")
                        showosd = True

                    Case arg.ToLower.Contains("/hidesplash")
                        hidesplash = True

                    Case arg.ToLower = "/p"
                        statecmd = True
                    Case Else
                End Select
            Next
        Else
            MsgBox("No arguments entered, please read the documentation at http://cyanlabs.net/application/vsx-volume for further information", MsgBoxStyle.Exclamation, "Error")
            Application.Exit()
        End If

        ConnectToVSX()

        'checks command line arguments to see if osd is enabled
        If showosd Then
            Dim x As Integer
            Dim y As Integer
            x = Screen.PrimaryScreen.WorkingArea.Width - (Me.Width + 5)
            y = Screen.PrimaryScreen.WorkingArea.Height - (Me.Height + 5)
            Me.Location = New Point(x, y)
            If hidesplash = False Then BackgroundWorker1.RunWorkerAsync() 'shows splashscreen if /hidesplash isn't set
            CheckScreen.IsBackground = True
            CheckScreen.Start()
        End If

        'loop through on commands
        If Not oncmd = "" Then
            Dim RunOnCMD As New System.Threading.Thread(AddressOf OnCommands)
            RunOnCMD.IsBackground = True
            RunOnCMD.Start()
        End If

        If statecmd Then
            AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf SystemEvents_PowerModeChanged
        End If
    End Sub

    Private Sub OnCommands()
        For Each cmd In oncmd.Split(",")
            SendCommands(cmd.ToUpper)
            System.Threading.Thread.Sleep(3000)
        Next
    End Sub

    Private Sub SystemEvents_PowerModeChanged(ByVal sender As Object, ByVal e As PowerModeChangedEventArgs)
        Select Case e.Mode
            Case PowerModes.Resume
                Dim RunOnCMD As New System.Threading.Thread(AddressOf OnCommands)
                    RunOnCMD.IsBackground = True
                    RunOnCMD.Start()
                    Case PowerModes.StatusChange
            Case PowerModes.Suspend
                For Each cmd In offcmd.Split(",")
                    SendCommands(cmd.ToUpper)
                    System.Threading.Thread.Sleep(3000)
                Next
        End Select
    End Sub

    'loop through off commands
    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not offcmd = "" Then
            For Each cmd In offcmd.Split(",")
                SendCommands(cmd.ToUpper)
                System.Threading.Thread.Sleep(3000)
            Next
        End If
    End Sub

    'connects to VSX
    Private Function ConnectToVSX()
        If tnSocket.Connected Then Return True
        Try
            tnSocket.Connect(ep)
            If tnSocket.Connected Then Return True
        Catch oEX As Exception
            Return False
        End Try
        Return False
    End Function

    'Sends command to VSX.
    Private Function SendCommands(ByVal cmd As String)
        Dim SendBytes As [Byte]() = Encoding.ASCII.GetBytes(cmd & vbCrLf)
        tnSocket.Send(SendBytes, SendBytes.Length, SocketFlags.None)
        Return Nothing
    End Function

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
    Private Sub kHook_KeyDown(ByVal sender As Object, ByVal e As WindowsHookLib.KeyboardEventArgs) Handles kHook.KeyDown
        If e.KeyCode = Keys.VolumeUp And Control.ModifierKeys = Keys.Control = False Then
            e.Handled = True
            SendCommands("VU")
        ElseIf e.KeyCode = Keys.VolumeDown And Control.ModifierKeys = Keys.Control = False Then
            e.Handled = True
            SendCommands("VD")
        ElseIf e.KeyCode = Keys.VolumeMute And Control.ModifierKeys = Keys.Control = False Then
            e.Handled = True
            SendCommands("MZ")
        ElseIf e.KeyCode = Keys.Pause And Control.ModifierKeys = Keys.Control = False Then
            e.Handled = True
            SendCommands("PZ")
        End If
    End Sub

    'fade form in, wait 5 seconds, fade form out
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        For iCount = 10 To 90 Step 10
            Me.Opacity = iCount / 100
            Threading.Thread.Sleep(25)
        Next
        Threading.Thread.Sleep(5000)
        For iCount = 90 To 10 Step -10
            Me.Opacity = iCount / 100
            Threading.Thread.Sleep(25)
        Next
        Me.Opacity = 0
    End Sub

    'Converts pioneers FL strings such as "FL022020202053544552454F20202020" to readable text "STEREO".
    'First we remove the FL and then any 00's are ignored as they basically are alignments.
    'Basically it is hex code for a ASCII character so we substring each 2 characters from the string.
    'Then we "convert.toint32" them to get a integer we can then simply "Chr" the number to get the character.
    Function DecryptScreen(temp As String)
        Dim OSD As String = temp.ToString.Replace("FL", "").Replace(vbLf, "").Replace(vbCrLf, "").Replace("2020", "")
        Dim output As String = ""
        For x As Integer = 0 To OSD.Length - 1 Step 2
            If Not OSD.Substring(x, 2) = "00" Then
                output = output & Chr(Convert.ToInt32("0x" & OSD.Substring(x, 2), 16))
            End If
        Next
        Return output
    End Function

    'parses the output recieved from the screen
    Sub ParseScreen(output As String)
        Try
            output = output.Replace(vbLf, "").Replace(vbCrLf, "")
            If output.ToString.Contains("FL") Then
                Dim decryptedOSD As String = DecryptScreen(output)
                Me.lblOSD.Font = CustomFont.GetInstance(24.0!, FontStyle.Regular)
                lblOSD.Text = decryptedOSD.ToString
                If Not BackgroundWorker1.IsBusy Then BackgroundWorker1.RunWorkerAsync()
            End If
        Catch ex As Exception
            Throw ex
            'TODO error handling
        End Try
    End Sub

    'Background Sub to constantly update the UI with updated information from the screen.
    Private Sub UpdateScreen()
        Dim output As String = ""
        Dim result As String()
        Dim RecvString As String = String.Empty
        Dim NumBytes As Integer = 0
        Dim RecvBytes(255) As [Byte]
        Do
            NumBytes = tnSocket.Receive(RecvBytes, RecvBytes.Length, 0)
            RecvString = RecvString + Encoding.ASCII.GetString(RecvBytes, 0, NumBytes)
            output = output & RecvString
            result = output.Split(vbCrLf)
        Loop While NumBytes = 256

        'loops through all response strings
        For Each i In result
            If i = vbCrLf Or i = vbLf Then Continue For
            ParseScreen(i)
        Next
        'Repeats sub
        UpdateScreen()
    End Sub
End Class