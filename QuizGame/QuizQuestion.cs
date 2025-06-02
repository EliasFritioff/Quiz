public abstract class QuizQuestion : IQuestion
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
