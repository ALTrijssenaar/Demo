# Alt.Chess

This is the start of the Alt.Chess solution.

## Getting Started


1. Open `Alt.Chess.sln` in Visual Studio or VS Code.
2. Build the solution:
   ```sh
   dotnet build solution/Alt.Chess/Alt.Chess.sln
   ```
3. Run tests:
   ```sh
   dotnet test solution/Alt.Chess/src/Alt.Chess.Application.Tests
   ```
4. Add new projects as needed (e.g., class library, console app) using `dotnet new` and add them to the solution with `dotnet sln add`.


## Structure

- `Alt.Chess.sln` - Solution file
- `src/Alt.Chess.Web` - ASP.NET Core + React web app
- `src/Alt.Chess.Application` - Chess engine logic
- `src/Alt.Chess.Application.Tests` - Unit tests
- `missing-features.md` - Features to implement
- `change-log.md` - Change log
- `README.md` - This file
## How to Run the Web App

To start the React web app:

```sh
cd solution/Alt.Chess/src/Alt.Chess.Web
dotnet run
```

This will launch the ASP.NET Core backend and the React frontend together.
Open your browser at the URL shown in the terminal (usually https://localhost:44469 or similar).


## How to Run Tests

Run all tests for the solution:

```sh
dotnet test solution/Alt.Chess/src/Alt.Chess.Application.Tests
```

You can also use the VS Code launch configuration ".NET Core Test (Alt.Chess.Application.Tests)" for debugging tests.

## 📋 The Basics of Chess
🎯 Objective of the Game
The goal is to checkmate your opponent’s king — meaning the king is under attack and cannot escape. The game ends immediately when checkmate occurs.

### 📦 1. Board Setup
The board has 8 rows (1–8) and 8 columns (a–h) — 64 squares in total.

One player plays white, the other black. White always moves first.

Starting from left to right (from each player’s perspective), the first rank is set up as:
Rook - Knight - Bishop - Queen - King - Bishop - Knight - Rook

Second rank: all 8 pawns.

Tip: "Queen on her own color" – the white queen starts on a white square, the black queen on a black square.

###🚶‍♂️ 2. How Each Piece Moves
Piece	How it Moves
Pawn	Moves 1 square forward (2 on first move); captures diagonally
Rook	Moves any number of squares vertically or horizontally
Bishop	Moves diagonally any number of squares
Queen	Moves like a rook and bishop combined
King	Moves 1 square in any direction
Knight	Moves in an "L" shape (2 squares in one direction, then 1 to the side). Can jump over other pieces

### ⚔️ 3. Capturing Pieces
You can capture enemy pieces by landing on their square, following your movement rules.

Only pawns capture differently: they move straight ahead but capture diagonally.

### 🔐 4. Check & Checkmate
Check: The king is under attack and must be saved immediately.

Options: move the king, block the check, or capture the attacking piece.

Checkmate: The king is under check and no legal move can save it.

Result: Game over. You win!

### 🔁 5. Special Moves
#### ✅ Castling
A move involving the king and one rook:

The king moves 2 squares toward the rook.

The rook jumps over the king to land next to it.

Conditions:

No pieces between them

Neither the king nor rook has moved before

The king is not in, through, or into check

#### ✅ En Passant
If an opposing pawn moves two squares forward from its starting position and lands next to your pawn, you can capture it as if it had only moved one square.

Must be done immediately on the next move, or you lose the chance.

#### ✅ Pawn Promotion
When a pawn reaches the opposite side of the board, it must be promoted to a queen, rook, bishop, or knight (usually a queen).

#### 🤝 6. Draw (Tie Game) Conditions
Stalemate: Player has no legal moves and is not in check → draw.

Threefold repetition: Same position occurs three times.

Fifty-move rule: No pawn moves or captures in 50 consecutive moves.

Mutual agreement: Both players agree to a draw.

#### 🕰️ 7. Time Controls (Optional)
Many games use a chess clock.

If a player runs out of time, they lose, unless the opponent does not have enough material to checkmate (then it’s a draw).

### ✅ Summary Table
Rule Area	Key Points
Setup	Queen on her color, white moves first
Movement	Each piece has unique movement rules
Captures	Like movement, except pawns capture diagonally
Check/Checkmate	You must react to check; checkmate ends the game
Special Moves	Castling, en passant, pawn promotion
Draws	Stalemate, repetition, 50 moves, or agreement
Time	Clock may be used; time over = loss (usually)

