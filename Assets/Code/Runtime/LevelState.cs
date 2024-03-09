namespace Runtime
{
    public sealed class LevelState
    {
        public Trail Trail  { get; private set; }
        public Frog  Player { get; private set; }

        public LevelState(Trail trail, Frog player)
        {
            Trail  = trail;
            Player = player;
        }
    }
}