Detta är ett nätverksbaserat flerspelar-quizspel skrivet i C#. Två spelare kan spela samtidigt via WebSocket-kommunikation, där poäng skickas i realtid till en Node.js-server.

- Flera olika typer av frågor, inklusive tidsbegränsade frågor
- Färgrikt textgränssnitt i konsolen
- ASCII-animationer vid rätt/fel svar
- Poängräkning med tidsbonus
- Highscore-system sparat lokalt
- Kommunikation mellan spelare via WebSocket (servern meddelar poäng i realtid)

- C# (.NET)
- WebSocketSharp (för nätverkskommunikation)
- Node.js (för WebSocket-server)
- Objektorienterad programmering (klasser, arv, polymorfism)

@echo off
echo Startar servern...
start cmd /k "cd /d C:\Users\enter.user.here\Quiz && node server.js"

timeout /t 2 >nul

echo Startar första klienten (enkel version)...
start cmd /k "cd /d C:\Users\enter.user.here\Quiz && dotnet run --project QuizGame\QuizGame.csproj"

timeout /t 2 >nul

echo Bygger avancerad klient...
cd /d C:\Users\enter.user.here\Quiz\QuizGame
dotnet build

echo Startar andra klienten (avancerad version)...
start cmd /k "cd /d C:\Users\enter.user.here\Quiz\QuizGame\bin\Debug\net8.0 && dotnet QuizGame.dll"