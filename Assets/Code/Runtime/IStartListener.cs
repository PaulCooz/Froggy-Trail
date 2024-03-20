namespace Runtime
{
    public interface IStartListener
    {
        int Order => 0;

        void GameStart(LevelState state);
    }
}