using System;
using WebSocketSharp;
using System.Collections.Generic;
using System.Threading;

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
            Console.WriteLine(Question);
            for (int i = 0; i < Choices.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Choices[i]}");
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

        public QuizGame(WebSocket socket, string name)
        {
            ws = socket;
            playerName = name;
            questions = new List<QuizQuestion>
            {
                new StandardQuestion { Question = "Vad är 3+5?", Choices = new [] { "6", "8", "10" }, CorrectAnswer = 1 },
                new TimedQuestion { Question = "Vilket år började andra världskriget?", Choices = new [] { "1935", "1939", "1941" }, CorrectAnswer = 1, TimeLimitSeconds = 10 },
                new StandardQuestion { Question = "Vilken planet är närmast solen?", Choices = new [] { "Venus", "Merkurius", "Mars" }, CorrectAnswer = 1 },
            };
            score = 0;
        }

        public void Start()
        {
            foreach (var q in questions)
            {
                Console.Clear();
                q.DisplayQuestion();
                Console.Write("Ditt svar: ");

                bool valid = int.TryParse(Console.ReadLine(), out int answer);
                bool correct = valid && q.CheckAnswer(answer - 1);

                if (correct)
                {
                    Console.WriteLine("Rätt!");
                    score++;
                }
                else
                {
                    Console.WriteLine("Fel.");
                }

                ws.Send($"{playerName} har nu {score} poäng.");
                Thread.Sleep(1000);
            }

            Console.WriteLine($"Spelet slut! Total poäng: {score}");
            ws.Send($"{playerName} är klar med {score} poäng.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.Write("Ange ditt namn: ");
            string name = Console.ReadLine();

            using (var ws = new WebSocket("ws://localhost:8080"))
            {
                ws.OnMessage += (sender, e) =>
                {
                    // Detta funkade inte så jag kommenterar bort för nu:
                    // Console.WriteLine($"\n[Meddelande från Spelare2]: {e.Data}");
                };

                ws.Connect();
                var game = new QuizGame(ws, name);
                game.Start();
            }

            Console.WriteLine("Tryck på valfri tangent för att avsluta...");
            Console.ReadKey();
        }
    }
}