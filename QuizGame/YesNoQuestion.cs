namespace QuizGameApp
{
    public class YesNoQuestion : QuizQuestion
    {
        public YesNoQuestion(string questionText, bool correctAnswer)
            : base(questionText, new[] { "Ja", "Nej" }, correctAnswer ? 0 : 1)
        { }

        public override bool CheckAnswer(string input)
        {
            return input.ToLower() == Options[CorrectAnswerIndex].ToLower();
        }
    }
}
