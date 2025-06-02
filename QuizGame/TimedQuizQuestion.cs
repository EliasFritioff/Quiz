namespace QuizGameApp
{
    public class TimedQuizQuestion : QuizQuestion
    {
        public int TimeLimitSeconds { get; private set; }

        public TimedQuizQuestion(string questionText, string[] options, int correctIndex, int timeLimit)
            : base(questionText, options, correctIndex)
        {
            TimeLimitSeconds = timeLimit;
        }

        public override bool CheckAnswer(string input)
        {
            return base.CheckAnswer(input);
        }
    }
}
