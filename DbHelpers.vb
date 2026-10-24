Public Class DbHelpers
    Public Shared Function SoundEx(word As String) As String
        Const kLength As Integer = 4
        Dim value As String = ""

        Dim size As Integer = word.Length

        ' Make sure the word is at least two characters in length
        If (size > 1) Then
            word = word.ToUpper()

            ' Convert the word to a character array for faster processing
            Dim chars() As Char = word.ToCharArray()

            ' For storing the SoundEx character codes
            Dim code As New List(Of String)

            ' The current and previous character codes
            Dim prevCode As Integer = 0
            Dim currCode As Integer = 0

            ' Add the first character
            code.Add(chars(0).ToString())

            ' Loop through all the characters and convert them to the proper character code
            For i As Integer = 1 To size - 1
                Select Case chars(i)
                    Case "H"c, "W"c
                        currCode = -1
                    Case "A"c, "E"c, "I"c, "O"c, "U"c, "Y"c
                        currCode = 0
                    Case "B"c, "F"c, "P"c, "V"c
                        currCode = 1
                    Case "C"c, "G"c, "J"c, "K"c, "Q"c, "S"c, "X"c, "Z"c
                        currCode = 2
                    Case "D"c, "T"c
                        currCode = 3
                    Case "L"c
                        currCode = 4
                    Case "M"c, "N"c
                        currCode = 5
                    Case "R"c
                        currCode = 6
                End Select

                If i > 0 Then
                    ' Two letters with the same number separated by 'H' or 'W' are coded as a single number
                    If currCode = -1 Then currCode = prevCode

                    ' Check to see if the current code is the same as the last one
                    If currCode <> prevCode Then
                        ' Check to see if the current code is 0 (a vowel); do not proceed
                        If currCode <> 0 Then
                            code.Add(currCode.ToString())
                        End If
                    End If
                End If

                prevCode = currCode

                ' If the buffer size meets the length limit, then exit the loop
                If (code.Count = kLength) Then
                    Exit For
                End If
            Next

            ' Pad the code if required
            While code.Count < kLength
                code.Add("0")
            End While

            ' Set the return value
            value = String.Join("", code)
        End If

        ' Return the computed SoundEx
        Return value
    End Function
End Class