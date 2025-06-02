using System;

namespace QuizGameApp
{
    /// <summary>
    /// Abstrakt basklass för spelare.
    /// </summary>
    public abstract class Player
    {
        public string Name { get; }
        public int Score { get; set; }

        public Player(string name)
        {
            Name = name;
            Score = 0;
        }

        /// <summary>
        /// Spelaren svarar på en fråga. Returnerar true om svaret var rätt.
        /// </summary>
        public abstract bool AnswerQuestion(QuizQuestion question);
    }

    /// <summary>
    /// spelare matar in sitt svar själv.
    /// </summary>
    public class LocalPlayer : Player
    {
        public LocalPlayer(string name) : base(name) { }

        public override bool AnswerQuestion(QuizQuestion question)
        {
            Console.WriteLine($"{Name}, {question.Question}");
            for (int i = 0; i < question.Choices.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Choices[i]}");
            }
            Console.Write("Ditt svar: ");
            if (int.TryParse(Console.ReadLine(), out int answer))
            {
                bool correct = question.CheckAnswer(answer - 1);
                if (correct) Score++;
                return correct;
            }
            return false;
        }
    }
}