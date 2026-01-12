Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Public Class Form1
    Inherits Form

    Private game As New Game()
    Private tileSize As Integer = 48
    Private tileImages As New Dictionary(Of TileType, Image)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.DoubleBuffered = True
        Me.KeyPreview = True
        LoadTileImages()
        LoadLevel("Levels\level1.txt")
        Me.ClientSize = New Size(game.Map.GetLength(0) * tileSize, game.Map.GetLength(1) * tileSize)
    End Sub

    Private Sub LoadTileImages()
        tileImages(TileType.Floor) = Image.FromFile("Resources\floor.png")
        tileImages(TileType.Wall) = Image.FromFile("Resources\wall.png")
        tileImages(TileType.Goal) = Image.FromFile("Resources\goal.png")
        tileImages(TileType.Box) = Image.FromFile("Resources\box.png")
        tileImages(TileType.BoxOnGoal) = Image.FromFile("Resources\box_on_goal.png")
        tileImages(TileType.Player) = Image.FromFile("Resources\player.png")
        tileImages(TileType.PlayerOnGoal) = Image.FromFile("Resources\player.png") ' 同じ画像を使う
    End Sub

    Private Sub LoadLevel(path As String)
        Dim lines = LevelLoader.LoadLevel(path)
        game.LoadLevel(lines)
        Me.Invalidate()
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        For y = 0 To game.Map.GetLength(1) - 1
            For x = 0 To game.Map.GetLength(0) - 1
                Dim tile = game.Map(x, y)
                Dim img As Image = tileImages(TileType.Floor) ' デフォルトは床

                Select Case tile
                    Case TileType.Wall : img = tileImages(TileType.Wall)
                    Case TileType.Goal : img = tileImages(TileType.Goal)
                    Case TileType.Box : img = tileImages(TileType.Box)
                    Case TileType.BoxOnGoal : img = tileImages(TileType.BoxOnGoal)
                    Case TileType.Player, TileType.PlayerOnGoal : img = tileImages(TileType.Floor)
                End Select

                e.Graphics.DrawImage(img, x * tileSize, y * tileSize, tileSize, tileSize)

                ' プレイヤーや箱を上に描画
                If tile = TileType.Player Or tile = TileType.PlayerOnGoal Then
                    e.Graphics.DrawImage(tileImages(TileType.Player), x * tileSize, y * tileSize, tileSize, tileSize)
                ElseIf tile = TileType.BoxOnGoal Then
                    e.Graphics.DrawImage(tileImages(TileType.BoxOnGoal), x * tileSize, y * tileSize, tileSize, tileSize)
                End If
            Next
        Next
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim dx As Integer = 0, dy As Integer = 0

        Select Case e.KeyCode
            Case Keys.Up : dy = -1
            Case Keys.Down : dy = 1
            Case Keys.Left : dx = -1
            Case Keys.Right : dx = 1
            Case Else : Return
        End Select

        If MovePlayer(dx, dy) Then
            Me.Invalidate()
            If game.IsClear() Then
                MessageBox.Show("クリア！🎉", "Sokoban")
            End If
        End If
    End Sub

    Private Function MovePlayer(dx As Integer, dy As Integer) As Boolean
        Dim x = game.PlayerX
        Dim y = game.PlayerY
        Dim tx = x + dx
        Dim ty = y + dy
        Dim tx2 = x + 2 * dx
        Dim ty2 = y + 2 * dy

        Dim current = game.Map(x, y)
        Dim target = game.Map(tx, ty)

        ' 壁なら動けない
        If target = TileType.Wall Then Return False

        ' 箱を押す場合
        If target = TileType.Box Or target = TileType.BoxOnGoal Then
            Dim nextTile = game.Map(tx2, ty2)
            If nextTile = TileType.Floor Or nextTile = TileType.Goal Then
                ' 箱を移動
                game.Map(tx2, ty2) = If(nextTile = TileType.Goal, TileType.BoxOnGoal, TileType.Box)
                game.Map(tx, ty) = If(target = TileType.BoxOnGoal, TileType.Goal, TileType.Floor)
            Else
                Return False
            End If
        ElseIf target <> TileType.Floor AndAlso target <> TileType.Goal Then
            Return False
        End If

        ' プレイヤーを移動
        game.Map(x, y) = If(current = TileType.PlayerOnGoal, TileType.Goal, TileType.Floor)
        game.Map(tx, ty) = If(target = TileType.Goal, TileType.PlayerOnGoal, TileType.Player)
        game.PlayerX = tx
        game.PlayerY = ty
        Return True
    End Function
End Class
