using System;
using System.Collections.Generic;

namespace QuizGameApp
{
    /// <summary>
    /// Hanterar en samling frågor av typen T (som ärver QuizQuestion).
    /// </summary>
    public class QuestionBank<T> where T : QuizQuestion
    {
        private List<T> questions = new List<T>();

        /// <summary>
        /// Lägger till en fråga i frågebanken.
        /// </summary>
        public void AddQuestion(T question)
        {
            questions.Add(question);
        }

        /// <summary>
        /// Hämtar en fråga (t.ex. slumpmässigt).
        /// </summary>
        public T GetRandomQuestion()
        {
            if (questions.Count == 0)
                throw new InvalidOperationException("Inga frågor i frågebanken.");
            var rnd = new Random();
            int index = rnd.Next(questions.Count);
            return questions[index];
        }
    }
}