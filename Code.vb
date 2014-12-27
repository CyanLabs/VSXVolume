Imports WindowsHookLib
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Module Module1
    Dim ip
    Public Sub Main()
        kHook.InstallHook()
        If Not My.Application.CommandLineArgs.Count = 0 Then
            If IPAddress.TryParse(My.Application.CommandLineArgs(0), ip) Then
                ep = New IPEndPoint(System.Net.IPAddress.Parse(My.Application.CommandLineArgs(0)), 8102)
            Else
                MsgBox("Enter a valid IP Address as the first command line argument", MsgBoxStyle.Exclamation, "Error")
                Application.Exit()
            End If
        Else
            MsgBox("Enter a valid IP Address as the first command line argument", MsgBoxStyle.Exclamation, "Error")
            Application.Exit()
        End If
        Application.Run()
    End Sub
    Dim hostIp As IPAddress, serverIp As Byte()
    Dim tnSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    Dim ep As IPEndPoint
    Dim WithEvents kHook As New KeyboardHook
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

    'Sends "cmd" to VSX.
    Private Function SendCommands(ByVal cmd As String)
        ConnectToVSX()
        Dim SendBytes As [Byte]() = Encoding.ASCII.GetBytes(cmd & vbCrLf)
        Dim NumBytes As Integer = 0
        tnSocket.Send(SendBytes, SendBytes.Length, SocketFlags.None)
        Return Nothing
    End Function

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
End Module