Imports WindowsHookLib
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Main
    Dim ip As IPAddress, tnSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), ep As IPEndPoint, startupcmd As Boolean = False
    Dim WithEvents kHook As New KeyboardHook
    Dim CheckScreen As New System.Threading.Thread(AddressOf UpdateScreen)
    Dim oncmd, offcmd As String
    Dim showosd As Boolean = False, hidesplash As Boolean = False

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kHook.InstallHook() 'installs global hook
        Me.Opacity = 0
        If Environment.GetCommandLineArgs.Count > 1 Then
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
                    Case Else
                End Select
            Next
        Else
            MsgBox("No arguments entered, please read the documentation at http://cyanlabs.net/application/vsx-volume for further information", MsgBoxStyle.Exclamation, "Error")
        End If

        ConnectToVSX()

        'loop through on commands
        If Not oncmd = "" Then
            For Each cmd In oncmd.Split(",")
                SendCommands(cmd.ToUpper)
                System.Threading.Thread.Sleep(3000)
            Next
        End If

        If showosd Then
            'Starts OSD thread and injects LCD font.  
            CheckForIllegalCrossThreadCalls = False
            Dim x As Long = (My.Computer.Screen.WorkingArea.Right - 5) - Me.Width
            Dim y As Long = (My.Computer.Screen.WorkingArea.Bottom - 5) - Me.Height
            Me.Location = New Point(x, y)
            If hidesplash = False Then
                BackgroundWorker1.RunWorkerAsync()
            End If
            CheckScreen.IsBackground = True
            CheckScreen.Start()
        End If
    End Sub

    'run off commands on application exit (system logoff/shutdown)
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
        'Try
        output = output.Replace(vbLf, "").Replace(vbCrLf, "")
        If output.ToString.Contains("FL") Then
            Dim decryptedOSD As String = DecryptScreen(output)
            Me.lblOSD.Font = CustomFont.GetInstance(24.0!, FontStyle.Regular)
            lblOSD.Text = decryptedOSD.ToString
            If Not BackgroundWorker1.IsBusy Then
                BackgroundWorker1.RunWorkerAsync()
            End If
        End If
        'Catch
        'TODO error handling
        'End Try
    End Sub

    'Background Sub to constantly update the UI with updated information from the screen.
    Private Sub UpdateScreen()
        Dim output As String = ""
        Dim result As String()
        Dim RecvString As String = String.Empty
        Dim NumBytes As Integer = 0
        Dim OSD As String = ""
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