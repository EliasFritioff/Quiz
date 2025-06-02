namespace QuizGameApp
{
    public class ScoreManager
    {
        public int Player1Score { get; private set; }
        public int Player2Score { get; private set; }

        public void AddPoint(int playerNumber)
        {
            if (playerNumber == 1) Player1Score++;
            else if (playerNumber == 2) Player2Score++;
        }
    }
}
