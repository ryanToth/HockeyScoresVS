namespace HockeyScoresVS
{
    public class Player
    {
        public int Number { get; }
        public string Name { get; }

        public Player(string name, int number)
        {
            this.Number = number;
            this.Name = name;
        }
    }
}
