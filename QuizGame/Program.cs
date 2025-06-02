using System;
using WebSocketSharp;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

namespace QuizGameApp
{
    // Abstrakt basklass
    public abstract class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Choices { get; set; }
        public int CorrectAnswer { get; set; }

        public abstract bool CheckAnswer(int answer);
        public virtual void DisplayQuestion()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(Question);
            Console.ResetColor();
            for (int i = 0; i < Choices.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{i + 1}. {Choices[i]}");
                Console.ResetColor();
            }
        }
    }

    // Normal fråga
    public class StandardQuestion : QuizQuestion
    {
        public override bool CheckAnswer(int answer)
        {
            return answer == CorrectAnswer;
        }
    }

    // Fråga med tidsbegränsning
    public class TimedQuestion : QuizQuestion
    {
        public int TimeLimitSeconds { get; set; }

        public override bool CheckAnswer(int answer)
        {
            Console.WriteLine("(Tidsbegränsad fråga kontrolleras)");
            return answer == CorrectAnswer;
        }
    }

    // Quizspelhanterare
    public class QuizGame
    {
        private List<QuizQuestion> questions;
        private int score;
        private WebSocket ws;
        private string playerName;

        private int correctAnswers = 0;
        private int wrongAnswers = 0;

        public QuizGame(WebSocket socket, string name)
        {
            ws = socket;
            playerName = name;
            questions = new List<QuizQuestion>
            {
                new StandardQuestion { Question = "Vad är 3+5?", Choices = new[] { "6", "8", "10" }, CorrectAnswer = 1 },
                new TimedQuestion { Question = "Vilket år började andra världskriget?", Choices = new[] { "1935", "1939", "1941" }, CorrectAnswer = 1, TimeLimitSeconds = 10 },
                new StandardQuestion { Question = "Vilken planet är närmast solen?", Choices = new[] { "Venus", "Merkurius", "Mars" }, CorrectAnswer = 1 },
                new TimedQuestion { Question = "Vad heter Sveriges huvudstad?", Choices = new[] { "Göteborg", "Stockholm", "Malmö" }, CorrectAnswer = 1, TimeLimitSeconds = 8 }
            };
            score = 0;
        }

        public void Start()
        {
            foreach (var q in questions)
            {
                // ...innan frågan visas:
                Console.Clear();
                DrawBox("QUIZ TIME!");
                q.DisplayQuestion();

                int answer = -1;
                bool valid = false;
                bool answered = false;
                DateTime start = DateTime.MinValue;

                if (q is TimedQuestion timedQ)
                {
                    int secondsLeft = timedQ.TimeLimitSeconds;
                    Console.Write($"Du har {secondsLeft} sekunder på dig! Ditt svar: ");
                    string input = "";
                    answered = false;
                    start = DateTime.Now;
                    while ((DateTime.Now - start).TotalSeconds < timedQ.TimeLimitSeconds && !answered)
                    {
                        int newSecondsLeft = timedQ.TimeLimitSeconds - (int)(DateTime.Now - start).TotalSeconds;
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write($"Du har {newSecondsLeft,2} sekunder på dig! Ditt svar: {input} ");

                        if (Console.KeyAvailable)
                        {
                            input = Console.ReadLine();
                            answered = true;
                        }
                        Thread.Sleep(100);
                    }
                    Console.WriteLine();

                    valid = int.TryParse(input, out answer);
                    if (!answered)
                    {
                        Console.WriteLine("Tiden är slut!");
                    }
                    else if (!valid)
                    {
                        Console.WriteLine("Ogiltigt svar.");
                    }
                }
                else
                {
                    Console.Write("Ditt svar: ");
                    valid = int.TryParse(Console.ReadLine(), out answer);
                }

                bool correct = valid && q.CheckAnswer(answer - 1);

                if (correct)
                {
                    ShowAsciiGubbe(true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Rätt!");
                    Console.Beep(1000, 200); // Ljust pip
                    Console.ResetColor();
                    score++;
                    correctAnswers++;
                }
                else
                {
                    ShowAsciiGubbe(false);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fel.");
                    Console.Beep(400, 400); // Mörkt pip
                    Console.ResetColor();
                    wrongAnswers++;
                }

                // Tidsbonus
                if (q is TimedQuestion && correct && answered)
                {
                    int timeUsed = (int)(DateTime.Now - start).TotalSeconds;
                    int bonus = Math.Max(0, ((TimedQuestion)q).TimeLimitSeconds - timeUsed);
                    if (bonus > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Tidsbonus! +{bonus} poäng");
                        Console.ResetColor();
                        score += bonus;
                    }
                }

                ws.Send($"{playerName} har nu {score} poäng.");
                Thread.Sleep(1000);
            }

            // ...efter att spelet är slut:
            Console.WriteLine($"Spelet slut! Total poäng: {score}");
            Console.WriteLine($"Rätt: {correctAnswers}, Fel: {wrongAnswers}");
            ws.Send($"{playerName} är klar med {score} poäng.");

            SaveScore();
            ShowHighscore();
        }

        public void SaveScore()
        {
            string scoreLine = $"{playerName};{score}";
            string file = "highscore.txt";
            // Lägg till poäng på ny rad
            File.AppendAllLines(file, new[] { scoreLine });
        }

        public void ShowHighscore()
        {
            string file = "highscore.txt";
            if (!File.Exists(file))
            {
                Console.WriteLine("Ingen highscore finns ännu.");
                return;
            }
            var lines = File.ReadAllLines(file);
            var scores = lines
                .Select(line => line.Split(';'))
                .Where(parts => parts.Length == 2 && int.TryParse(parts[1], out _))
                .Select(parts => new { Name = parts[0], Points = int.Parse(parts[1]) })
                .OrderByDescending(entry => entry.Points)
                .Take(3)
                .ToList();

            Console.WriteLine("Topp 3 Highscores:");
            foreach (var entry in scores)
            {
                Console.WriteLine($"{entry.Name}: {entry.Points} poäng");
            }
        }

        private void DrawBox(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("+----------------------+");
            Console.WriteLine($"| {text,-20} |");
            Console.WriteLine("+----------------------+");
            Console.ResetColor();
        }

        private void ShowAsciiGubbe(bool correct)
        {
            if (correct)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"
  \O/
   |
  / \
");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(@"
  \_/
   |
  / \
");
                Console.ResetColor();
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Ange ditt namn: ");
            string name = Console.ReadLine() ?? "Spelare";

            using var ws = new WebSocketSharp.WebSocket("ws://localhost:8080");
            ws.Connect();

            var game = new QuizGame(ws, name);
            game.Start();
        }
    }
}