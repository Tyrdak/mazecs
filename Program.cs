using System;

// ── Constantes numériques ──

const int width = 50;
const int height = 20;

const int offsetX = 0;
const int offsetY = 3;

const int cellW = width / 2;
const int cellH = height / 2;

const int startCX = 0;
const int startCY = 0;

const int exitX = (cellW - 1) * 2;
const int exitY = (cellH - 1) * 2;

const int controlsLineY = offsetY + height + 1;
const int endMessageLineY = offsetY + height + 3;
const int promptLineY = offsetY + height + 8;

// ── Constantes couleurs ──

const ConsoleColor wallColor = ConsoleColor.DarkGray;
const ConsoleColor playerColor = ConsoleColor.Yellow;
const ConsoleColor exitColor = ConsoleColor.Green;
const ConsoleColor corridorColor = ConsoleColor.DarkBlue;

const ConsoleColor titleColor = ConsoleColor.Cyan;
const ConsoleColor controlsColor = ConsoleColor.DarkCyan;
const ConsoleColor victoryColor = ConsoleColor.Green;
const ConsoleColor quitColor = ConsoleColor.Red;

// ── Constantes messages ──

const string titleMessage = """
    ╔══════════════════════════════════════════════════╗
    ║          🏃 LABYRINTHE ASCII  C#  🏃             ║
    ╚══════════════════════════════════════════════════╝
    """;

const string controlsMessage = "  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter";

const string victoryMessage = """
      ╔════════════════════════════════╗
      ║   🎉  FÉLICITATIONS !  🎉      ║
      ║   Vous avez trouvé la sortie ! ║
      ╚════════════════════════════════╝
    """;

const string quitMessage = "\n  Partie abandonnée. À bientôt !";

const string promptMessage = "  Appuyez sur une touche pour quitter...";

// ── Programme principal ──

var grid = new CellType[width, height];

GenerateMaze(grid, startCX, startCY);

int playerX = startCX * 2, playerY = startCY * 2;

DrawGameScreen(grid);

// ── Boucle de jeu ──

var won = false;
var running = true;

while (running && !won)
{
    var key = Console.ReadKey(true).Key;

    var nx2 = playerX;
    var ny2 = playerY;

    switch (key)
    {
        case ConsoleKey.Z or ConsoleKey.UpArrow:
            ny2--;
            break;

        case ConsoleKey.S or ConsoleKey.DownArrow:
            ny2++;
            break;

        case ConsoleKey.Q or ConsoleKey.LeftArrow:
            nx2--;
            break;

        case ConsoleKey.D or ConsoleKey.RightArrow:
            nx2++;
            break;

        case ConsoleKey.Escape:
            running = false;
            break;
    }

    if (nx2 >= 0 && nx2 < width && ny2 >= 0 && ny2 < height && grid[nx2, ny2] != CellType.Wall)
    {
        if (grid[nx2, ny2] == CellType.Exit) won = true;

        grid[playerX, playerY] = CellType.Corridor;
        DrawCell(playerX, playerY);

        playerX = nx2;
        playerY = ny2;

        grid[playerX, playerY] = CellType.Player;
        DrawCell(playerX, playerY);
    }
}

// ── Écran de fin ──

if (won)
{
    DrawTextXY(0, endMessageLineY, victoryMessage, victoryColor);
}
else
{
    DrawTextXY(0, endMessageLineY, quitMessage, quitColor);
}

DrawTextXY(0, promptLineY, promptMessage);
Console.CursorVisible = true;
Console.ReadKey(true);

// ── Fonctions ──

void DrawTextXY(int x, int y, string text, ConsoleColor? color = null)
{
    Console.SetCursorPosition(x, y);

    if (color != null)
    {
        Console.ForegroundColor = color.Value;
    }

    Console.WriteLine(text);
    Console.ResetColor();
}

void DrawCell(int cx, int cy)
{
    Console.SetCursorPosition(offsetX + cx, offsetY + cy);

    var (color, pattern) = grid[cx, cy] switch
    {
        CellType.Wall   => (wallColor,     "█"),
        CellType.Player => (playerColor,   "@"),
        CellType.Exit   => (exitColor,     "★"),
        _               => (corridorColor, "·")
    };

    Console.ForegroundColor = color;
    Console.Write(pattern);
    Console.ResetColor();
}

void GenerateMaze(CellType[,] grid, int startX, int startY)
{
    for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            grid[x, y] = CellType.Wall;

    var stackX = new int[cellW * cellH];
    var stackY = new int[cellW * cellH];
    var stackTop = 0;

    var visited = new bool[cellW, cellH];

    var dx = new[] { 0, 1, 0, -1 };
    var dy = new[] { -1, 0, 1, 0 };

    var rng = new Random();

    visited[startX, startY] = true;
    grid[startX * 2, startY * 2] = CellType.Corridor;

    stackX[stackTop] = startX;
    stackY[stackTop] = startY;
    stackTop++;

    while (stackTop > 0)
    {
        var cx = stackX[stackTop - 1];
        var cy = stackY[stackTop - 1];

        var directions = new[] { 0, 1, 2, 3 };
        rng.Shuffle(directions);

        var found = false;

        foreach (var dir in directions)
        {
            var nx = cx + dx[dir];
            var ny = cy + dy[dir];

            if (nx >= 0 && nx < cellW && ny >= 0 && ny < cellH && !visited[nx, ny])
            {
                grid[cx * 2 + dx[dir], cy * 2 + dy[dir]] = CellType.Corridor;
                grid[nx * 2, ny * 2] = CellType.Corridor;

                visited[nx, ny] = true;

                stackX[stackTop] = nx;
                stackY[stackTop] = ny;
                stackTop++;

                found = true;
                break;
            }
        }

        if (!found) stackTop--;
    }

    grid[startX * 2, startY * 2] = CellType.Player;
    grid[exitX, exitY] = CellType.Exit;
}

void DrawGameScreen(CellType[,] grid)
{
    Console.Clear();
    Console.CursorVisible = false;

    DrawTextXY(0, 0, titleMessage, titleColor);

    for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            DrawCell(x, y);

    DrawTextXY(0, controlsLineY, controlsMessage, controlsColor);
}

// ── Enum ──

enum CellType { Corridor, Wall, Player, Exit }
