// Grid values:
// -1 = mine
//  0 = no adjacent mines
// 1-8 = number of adjacent mines

const int ROWS = 9;
const int COLUMNS = 9;


/// <summary>
/// Processes a player's turn by revealing a cell
/// and checking the win and loss conditions.
/// </summary>
/// <param name="grid">The game grid.</param>
/// <param name="revealedGrid">A two-dimensional boolean array containing the revealed cells.</param>
/// <param name="flags">A two-dimensional boolean array containing the cell flags.</param>
/// <param name="totalMines">The total number of mines.</param>
/// <param name="isFirstTurn"> Indicates whether this is the first cell reveal.</param>
/// <returns>
/// A tuple containing:
/// - isWin: Whether the player has won.
/// - isLose: Whether the player has lost.
/// - isFirstTurn: Whether the first cell reveal is still pending.
/// </returns>
(bool isWin, bool isLose, bool isFirstTurn) PlayTurn(int[,] grid, bool[,] revealedGrid, bool[,] flags, int totalMines, bool isFirstTurn)
{
    bool isWin = false;
    bool isLose = false;

    (int inputRow, int inputColumn) = GetCellInput();

    // Ignore attempts to reveal flagged or already revealed cells.
    if (flags[inputRow, inputColumn] || revealedGrid[inputRow, inputColumn])
    {
        return (isWin, isLose, isFirstTurn);
    }

    // Generate the grid on the first valid reveal.
    if (isFirstTurn) 
    {
        GenerateGrid(grid, inputRow, inputColumn, totalMines);
        isFirstTurn = false;
    } 

    revealedGrid[inputRow, inputColumn] = true;

    if (grid[inputRow, inputColumn] == 0)
    {
        RevealCells(revealedGrid, grid, inputRow, inputColumn, flags);
    }

    isLose = CheckLose(grid, revealedGrid, inputRow, inputColumn);

    if (isLose)
    {
        return (isWin, isLose, isFirstTurn);
    } 

    isWin = CheckWin(grid, revealedGrid, totalMines);

    return (isWin, isLose, isFirstTurn);
}


/// <summary>
/// Randomly places mines on the grid, excluding the player's
/// first selected cell, and updates adjacent mine counts.
/// </summary>
/// <param name="grid">The game grid to modify.</param>
/// <param name="inputRow">The user selected row.</param>
/// <param name="inputColumn">The user selected column.</param>
/// <param name="totalMines">The number of mines to place.</param>
void GenerateGrid(int[,] grid, int inputRow, int inputColumn, int totalMines)
{
    Random randomGenerator = new ();
    int randomRow, randomColumn; 
    int placedMines = 0;

    do
    {
        // Choose a random position for the next mine.
        randomRow = randomGenerator.Next(ROWS);
        randomColumn = randomGenerator.Next(COLUMNS);

        if (grid[randomRow, randomColumn] == -1 || 
            (randomRow == inputRow && randomColumn == inputColumn))
        {
            continue;
        } 

        placedMines++;
        grid[randomRow, randomColumn] = -1;

        // Update adjacent mine counts.
        for (int row = randomRow - 1; row <= randomRow + 1; row++)
        {
            if (row < 0 || row >= ROWS)
            {
               continue;
            } 

            for (int column = randomColumn - 1; column <= randomColumn + 1; column++)
            {
                if (column < 0 || column >= COLUMNS)
                {
                    continue;
                }

                if (row == randomRow && column == randomColumn)
                {
                    continue;
                } 

                if (grid[row, column] == -1)
                {
                    continue;
                } 

                grid[row, column] += 1;
            }
        }

    } while (placedMines < totalMines);
}


/// <summary>
/// Recursively reveals adjacent empty cells and their neighboring numbered cells.
/// Flagged and already revealed cells are skipped.
/// </summary>
/// <param name="revealedGrid">A two-dimensional boolean array containing the revealed cells.</param>
/// <param name="grid">The game grid.</param>
/// <param name="inputRow">The user selected row.</param>
/// <param name="inputColumn">The user selected column.</param>
/// <param name="flags">A two-dimensional boolean array containing the cell flags.</param>
void RevealCells(bool[,] revealedGrid, int [,] grid, int inputRow, int inputColumn, bool[,] flags)
{
    for (int row = inputRow - 1; row <= inputRow + 1; row++)
    {
        if (row < 0 || row >= ROWS)
        {
            continue;
        } 

        for (int column = inputColumn - 1; column <= inputColumn + 1; column++)
        {
            if (column < 0 || column >= COLUMNS)
            {
                continue;
            } 

            if (row == inputRow && column == inputColumn)
            {
                continue;
            } 

            if (!revealedGrid[row, column] && !flags[row, column] && grid[row, column] != -1)
            {
                revealedGrid[row, column] = true;

                if (grid[row, column] == 0)
                {
                    RevealCells(revealedGrid, grid, row, column, flags);
                }
            }
        }
    }
}


/// <summary>
/// Reveals all mines when the player loses.
/// </summary>
/// <param name="grid">The game grid.</param>
/// <param name="revealedGrid">The revealed state of each cell.</param>
void RevealMineCells(int[,] grid, bool[,] revealedGrid)
{
    for (int row = 0; row < ROWS; row++)
    {
        for (int column = 0; column < COLUMNS; column++)
        {
            if (grid[row, column] == -1)
            {
                revealedGrid[row, column] = true;
            }
        }
    }
}


/// <summary>
///  Checks whether all cells without mines have been revealed.
/// </summary>
/// <param name="grid">The game grid.</param>
/// <param name="revealedGrid">A two-dimensional boolean array containing the revealed cells.</param>
/// <param name="totalMines">The total number of mines.</param>
/// <returns>
/// True if all non-mine cells have been revealed;
/// otherwise, false.
/// </returns>
bool CheckWin(int[,] grid, bool[,] revealedGrid, int totalMines)
{
    int cellsCount = 0;

    for (int row = 0; row < ROWS; row++)
    {
        for (int column = 0; column < COLUMNS; column++)
        {
            if (revealedGrid[row, column] && grid[row, column] != -1)
            {
                cellsCount++;
            }
        }
    }

    if (cellsCount == (ROWS * COLUMNS) - totalMines)
    {
        return true;
    }

    return false;
}


/// <summary>
/// Checks whether the player has revealed a mine.
/// </summary>
/// <param name="grid">The game grid.</param>
/// <param name="revealedGrid">The revealed state of each cell.</param>
/// <param name="inputRow">The selected row.</param>
/// <param name="inputColumn">The selected column.</param>
/// <returns>
/// True if the selected cell contains a revealed mine;
/// otherwise, false.
/// </returns>
bool CheckLose(int[,] grid, bool[,] revealedGrid, int inputRow, int inputColumn)
{
    if (grid[inputRow, inputColumn] == -1 && revealedGrid[inputRow, inputColumn])
    {
        return true;
    }

    return false;
}


/// <summary>
/// Places a flag on the selected cell if it is not revealed.
/// </summary>
/// <param name="flags">
/// A two-dimensional boolean array containing the cell flags.
/// </param>
/// <param name="revealedGrid">
/// A two-dimensional boolean array containing the revealed cells.
/// </param>
void AddFlag(bool[,] revealedGrid, bool[,] flags)
{
    (int inputRow, int inputColumn) = GetCellInput();

    if (!revealedGrid[inputRow, inputColumn])
    {
        flags[inputRow, inputColumn] = true;
    }
}


/// <summary>
/// Removes the flag from the selected cell.
/// </summary>
/// <param name="flags">
/// A two-dimensional boolean array containing the cell flags.
/// </param>
void RemoveFlag(bool[,] flags)
{
    (int inputRow, int inputColumn) = GetCellInput();
    flags[inputRow, inputColumn] = false;
}


/// <summary>
/// Prompts the user to enter a valid row and column.
/// </summary>
/// <returns>
/// The zero-based row and column selected by the user.
/// </returns>
(int inputRow, int inputColumn) GetCellInput()
{
    int inputRow = 0;
    int inputColumn = 0;

    bool validInput;

    do
    {
        Console.Write("Insert the row: ");
        validInput = int.TryParse(Console.ReadLine(), out inputRow);
        inputRow -= 1;

        if (inputRow < 0 || !validInput || inputRow >= ROWS)
        {
            Console.WriteLine("Error: You must enter a valid, positive number that is not greater than the number of rows.");
            Console.WriteLine();
            validInput = false;
            continue;
        }

        Console.Write("Insert the column: ");
        validInput = int.TryParse(Console.ReadLine(), out inputColumn);
        inputColumn -= 1;

        if (inputColumn < 0 || !validInput || inputColumn >= COLUMNS)
        {
            Console.WriteLine("Error: You must enter a valid, positive number that is no greater than the number of columns.");
            Console.WriteLine();
            validInput = false;
            continue;
        }

    } while (!validInput);

    return (inputRow, inputColumn);
}


/// <summary>
/// Prompts the user to enter a valid number of mines.
/// </summary>
/// <returns>
/// The number of mines selected by the user.
/// </returns>
int GetMineCount()
{
    int mines;
    bool validInput;

    do
    {
        Console.Write($"\nEnter the number of mines (MAX {ROWS * COLUMNS - 1}): ");
        validInput = int.TryParse(Console.ReadLine(), out mines);

        if (mines < 1 || mines >= ROWS * COLUMNS)
        {
            Console.WriteLine("Invalid number of mines!");
            validInput = false;
        }

    } while (!validInput);

    return mines;
}


/// <summary>
/// Renders the game grid in the console, displaying revealed cells, flags, and colored numbers indicating nearby mines.
/// </summary>
/// <param name="showAll">If true, displays all cells regardless of their revealed state.</param>
/// <param name="grid">The game grid.</param>
/// <param name="revealedGrid">A two-dimensional boolean array containing the revealed cells.</param>
/// <param name="flags">A two-dimensional boolean array containing the cell flags.</param>
void DrawGrid(bool showAll, int[,] grid, bool[,] revealedGrid, bool[,] flags)
{
    Console.WriteLine();
    for (int i = grid.GetLength(0) - 1; i >= 0; i--)
    {
        Console.Write($"{i + 1,2}  ");
        for (int j = 0; j < grid.GetLength(1); j++)
        {
            if (revealedGrid[i, j] || showAll)
            {
                if (grid[i, j] == -1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(" ✹");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = grid[i, j] switch
                    {
                        1 => ConsoleColor.Blue,
                        2 => ConsoleColor.Green,
                        3 => ConsoleColor.Red,
                        4 => ConsoleColor.DarkBlue,
                        5 => ConsoleColor.DarkRed,
                        6 => ConsoleColor.Cyan,
                        7 => ConsoleColor.DarkGray,
                        8 => ConsoleColor.Gray,
                        _ => ConsoleColor.White
                    };

                    Console.Write(grid[i, j] == 0 ? "  " : $" {grid[i, j]}");
                    Console.ResetColor();
                }
            }
            else if (flags[i, j])
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" ⚑");
                Console.ResetColor();
            }
            else
            {
                Console.Write(" ■");
            }
        }
        Console.WriteLine();
    }

    Console.Write("\n    ");
    for (int j = 0; j < grid.GetLength(1); j++)
    {
        Console.Write($"{j + 1,2}");
    }
        
    Console.WriteLine();
}


int[,] grid = new int[ROWS, COLUMNS];
bool[,] flags = new bool[ROWS, COLUMNS];
bool[,] revealedGrid = new bool[ROWS, COLUMNS];

bool isWin = false;
bool isLose = false;
bool isFirstTurn = true;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("~ Minesweeper ~\n");

int mines = GetMineCount();

DrawGrid(false, grid, revealedGrid, flags);

do
{
    Console.WriteLine("A) Reveal a cell.");
    Console.WriteLine("B) Place a flag.");
    Console.WriteLine("C) Remove a flag.");
    Console.Write("Enter a choice: ");
    char choice = char.ToUpper(Console.ReadKey().KeyChar);
    Console.WriteLine();

    switch (choice)
    {
        case 'A':
            (isWin, isLose, isFirstTurn) = PlayTurn(grid, revealedGrid, flags, mines, isFirstTurn);
            break;
        case 'B':
            AddFlag(revealedGrid, flags);
            break;
        case 'C':
            RemoveFlag(flags);
            break;
        default:
            break;
    }

    Console.Clear();

    if (isLose)
    {
        RevealMineCells(grid, revealedGrid);
        Console.WriteLine("\n You Lose!");
    }

    if (isWin)
    {
       Console.WriteLine("\n You Won!");
    }

    DrawGrid(false, grid, revealedGrid, flags);

} while (!isWin && !isLose);
