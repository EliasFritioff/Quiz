namespace QuizGameApp
{
    public interface IQuestion
    {
        void DisplayQuestion();
        bool CheckAnswer(int answer);
    }
}
