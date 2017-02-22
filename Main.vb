Imports WindowsHookLib
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Main
    Dim ip As IPAddress, tnSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), ep As IPEndPoint, startupcmd As Boolean = False
    Dim WithEvents kHook As New KeyboardHook
    Dim oncmd, offcmd As String
    Dim closeapp As Boolean = False
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kHook.InstallHook() 'installs global hook
        Me.Hide() ' hides form
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
                    If arg.ToLower.Contains("/offcommands=") Then
                        offcmd = arg.Replace("/offcommands=", "").Replace("""", "").Trim
                    End If
                Case Else
            End Select
        Next

        'loop through on commands
        If Not oncmd = "" Then
            For Each cmd In oncmd.Split(",")
                SendCommands(cmd.ToUpper)
                System.Threading.Thread.Sleep(3000)
            Next
        End If
    End Sub

    'run off commands on application exit (system logoff/shutdown)
    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' If closeapp = False Then e.Cancel = True
        If Not offcmd = "" Then
            For Each cmd In offcmd.Split(",")
                SendCommands(cmd.ToUpper)
                System.Threading.Thread.Sleep(3000)
            Next
        End If
        ' closeapp = True
        'Application.Exit()
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
        ConnectToVSX()
        Dim SendBytes As [Byte]() = Encoding.ASCII.GetBytes(cmd & vbCrLf)
        tnSocket.Send(SendBytes, SendBytes.Length, SocketFlags.None)
        Return Nothing
    End Function

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
End Class