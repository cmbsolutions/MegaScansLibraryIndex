Public Class CustomTextTraceListener
    Inherits TraceListener

    Private _target As RichTextBox
    'Private _invokeWrite As StringSendDelegate
    Private _scrollingEnabled As Boolean = True

    Public Property Autoscroll() As Boolean
        Get
            Return _scrollingEnabled
        End Get
        Set(ByVal value As Boolean)
            _scrollingEnabled = value
        End Set
    End Property

    Public Sub New(target As RichTextBox)
        _target = target
        '_invokeWrite = New StringSendDelegate(AddressOf SendString)
    End Sub

    ' Override the TraceEvent method to capture additional information
    Public Overrides Sub TraceEvent(eventCache As TraceEventCache, source As String, eventType As TraceEventType, id As Integer, message As String)
        Dim msg As String = $"[{source}][{eventType}] {message}"
        Me.WriteColorMessage(msg, eventType)
    End Sub

    Public Overrides Sub Write(message As String)
        Me.WriteColorMessage(message, TraceEventType.Information)
    End Sub

    Public Overrides Sub WriteLine(message As String)
        Me.Write(message & Environment.NewLine)
    End Sub

    Private Delegate Sub StringSendDelegate(message As String)
    Private Sub SendString(message As String)
        ' No need to lock text box as this function will only 
        ' ever be executed from the UI thread
        If _target.TextLength + message.Length + 2 > _target.MaxLength Then
            _target.SelectionStart = 0
            _target.SelectionLength = message.Length + 2
            _target.SelectedText = ""
            _target.SelectionStart = _target.TextLength
        End If

    End Sub

    ' Method to write colored messages based on TraceEventType
    Private Sub WriteColorMessage(message As String, eventType As TraceEventType)
        ' Invoke UI changes on the main thread
        _target.Invoke(Sub()
                           ' Set the color based on the event type
                           Select Case eventType
                               Case TraceEventType.Information
                                   _target.SelectionColor = Color.LightBlue
                               Case TraceEventType.Warning
                                   _target.SelectionColor = Color.Goldenrod
                               Case TraceEventType.Error
                                   _target.SelectionColor = Color.IndianRed
                               Case TraceEventType.Critical
                                   _target.SelectionColor = Color.MediumVioletRed
                               Case Else
                                   _target.SelectionColor = Color.WhiteSmoke
                           End Select

                           ' Append the message
                           _target.AppendText(message)

                           ' Reset color to default
                           _target.SelectionColor = Color.WhiteSmoke

                           ' Trim lines if necessary
                           TrimLines()
                       End Sub)
    End Sub

    Private Sub TrimLines()
        If _target.Lines.Length > 5000 Then
            _target.Lines = _target.Lines.Skip(_target.Lines.Length - 5000).ToArray
        End If

        If Autoscroll Then
            _target.SelectionStart = _target.Text.Length
            _target.ScrollToCaret()
        End If
    End Sub
End Class