Imports System.Drawing.Text
Imports System.Runtime.InteropServices

Module CustomFont
    'PRIVATE FONT COLLECTION TO HOLD THE DYNAMIC FONT
    Private _pfc As PrivateFontCollection = Nothing
    Public ReadOnly Property GetInstance(ByVal Size As Single, ByVal style As FontStyle) As Font
        Get
            If _pfc Is Nothing Then LoadFont()
            Return New Font(_pfc.Families(0), Size, style)
        End Get
    End Property
    Private Sub LoadFont()
        Try
            _pfc = New PrivateFontCollection
            Dim fontMemPointer As IntPtr = Marshal.AllocCoTaskMem(My.Resources.LCD_Solid.Length)
            Marshal.Copy(My.Resources.LCD_Solid, 0, fontMemPointer, My.Resources.LCD_Solid.Length)
            _pfc.AddMemoryFont(fontMemPointer, My.Resources.LCD_Solid.Length)
            Marshal.FreeCoTaskMem(fontMemPointer)
        Catch ex As Exception
            'Error loading font, Ignore and use internal fonts
        End Try
    End Sub
End Module