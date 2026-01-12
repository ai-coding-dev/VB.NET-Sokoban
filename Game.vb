Public Class Game
    Public Map(,) As TileType
    Public PlayerX As Integer
    Public PlayerY As Integer

    Public Sub LoadLevel(lines As String())
        Dim height = lines.Length
        Dim width = lines(0).Length
        ReDim Map(width - 1, height - 1)

        For y = 0 To height - 1
            For x = 0 To width - 1
                Select Case lines(y)(x)
                    Case "#"c : Map(x, y) = TileType.Wall
                    Case " "c : Map(x, y) = TileType.Floor
                    Case "."c : Map(x, y) = TileType.Goal
                    Case "$"c : Map(x, y) = TileType.Box
                    Case "*"c : Map(x, y) = TileType.BoxOnGoal
                    Case "@"c
                        Map(x, y) = TileType.Player
                        PlayerX = x : PlayerY = y
                    Case "+"c
                        Map(x, y) = TileType.PlayerOnGoal
                        PlayerX = x : PlayerY = y
                End Select
            Next
        Next
    End Sub

    Public Function IsClear() As Boolean
        For Each tile In Map
            If tile = TileType.Goal Or tile = TileType.PlayerOnGoal Then
                Return False
            End If
        Next
        Return True
    End Function

    ' Move logic は後ほど追加
End Class
