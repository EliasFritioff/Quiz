using System;
using WebSocketSharp;
using System.Collections.Generic;

// Basquizklass
public class QuizQuestion
{
    public string Question { get; set; }
    public string[] Choices { get; set; }
    public int CorrectAnswer { get; set; }

    public virtual bool CheckAnswer(int answer)
    {
        return answer == CorrectAnswer;
    }
}

// Child-klass som använder polymorfism
public class TimedQuizQuestion : QuizQuestion
{
    public int TimeLimit { get; set; }

    public override bool CheckAnswer(int answer)
    {
        Console.WriteLine("Svar med tidsgräns kontrolleras.");
        return base.CheckAnswer(answer);
    }
}

// Hanterar quizlogik
public class QuizGame
{
    private List<QuizQuestion> questions;
    private int score;

    public QuizGame()
    {
        questions = new List<QuizQuestion>
        {
            new QuizQuestion { Question = "Vad är 2+2?", Choices = new [] { "3", "4", "5" }, CorrectAnswer = 1 },
            new TimedQuizQuestion { Question = "Vad är huvudstaden i Sverige?", Choices = new [] { "Göteborg", "Stockholm", "Malmö" }, CorrectAnswer = 1, TimeLimit = 10 }
        };
        score = 0;
    }

    public void Start(WebSocket ws)
    {
        foreach (var q in questions)
        {
            Console.Clear();
            Console.WriteLine(q.Question);
            for (int i = 0; i < q.Choices.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {q.Choices[i]}");
            }

            Console.Write("Ditt svar: ");
            if (int.TryParse(Console.ReadLine(), out int answer) && q.CheckAnswer(answer - 1))
            {
                Console.WriteLine("Rätt!");
                score++;
            }
            else
            {
                Console.WriteLine("Fel!");
            }

            // Skicka poäng till spelare2
            ws.Send($"Score:{score}");
            System.Threading.Thread.Sleep(1000);
        }

        Console.WriteLine($"Din poäng: {score}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Ansluter till server...");

        using (var ws = new WebSocket("ws://localhost:8080"))
        {
            ws.OnMessage += (sender, e) =>
            {
                Console.WriteLine($"\n[Spelare2]: {e.Data}");
            };

            ws.Connect();
            var game = new QuizGame();
            game.Start(ws);
        }

        Console.WriteLine("Tryck på en tangent för att avsluta...");
        Console.ReadKey();
    }
}
