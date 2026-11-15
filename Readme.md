# Simple Minesweeper

A console-based Minesweeper game written in **C#**. I built it as a programming exercise and later revisited it to make the code clearer, better organized, and easier to understand.

This is one of the small projects I'm sharing to document my progress as I learn C#.

## Features

- A **9 × 9** game board.
- A configurable number of mines (**1–80**).
- A safe first reveal: the first selected cell cannot contain a mine.
- Automatic, recursive revealing of connected empty areas and their neighboring numbered cells.
- Flags to mark suspected mines.
- Colored numbers showing how many mines surround a cell.
- Win detection when every safe cell is revealed, and mine reveal on defeat.

## How to play

1. Start the game and choose how many mines to place.
2. Choose an action from the menu:

   | Key | Action |
   | --- | --- |
   | `A` | Reveal a cell |
   | `B` | Place a flag |
   | `C` | Remove a flag |

3. Enter a row and column between **1 and 9** when prompted.
4. Reveal every cell without a mine to win. Revealing a mine ends the game.

Flagged cells cannot be revealed until their flags are removed.

## Run locally

You need the **.NET SDK** installed.

1. Download or clone this repository.
2. Open a terminal in the directory containing the project's `.csproj` file.
3. Run:

   ```bash
   dotnet run
   ```

For the best display, use a terminal with UTF-8 character support.

## How the board works

The game uses three two-dimensional arrays:

| Array | Purpose |
| --- | --- |
| `grid` (`int[,]`) | Stores mines (`-1`) and adjacent mine counts (`0` to `8`) |
| `revealedGrid` (`bool[,]`) | Tracks which cells have been revealed |
| `flags` (`bool[,]`) | Tracks which cells have flags |

Mines are generated on the first valid reveal so that the selected cell is always safe. When an empty cell is revealed, a recursive method opens the connected empty area and its numbered boundary cells.

## What I practiced

- Working with two-dimensional arrays.
- Breaking game logic into focused methods.
- Using recursion to reveal neighboring cells.
- Handling and validating console input with `int.TryParse()`.
- Managing game state and win/loss conditions.
- Improving readability through naming and C# XML documentation comments (`<summary>`, `<param>`, and `<returns>`).

I had already encountered docstrings in Python, which encouraged me to learn how documentation works in C#.

## Acknowledgments

The initial grid-rendering code was provided by my programming instructor as part of the exercise. I extended it with **flag visualization and colored numbers**. I also revisited and refactored other parts of the game as a learning exercise.
