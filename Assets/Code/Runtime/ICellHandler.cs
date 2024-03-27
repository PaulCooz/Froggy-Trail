namespace Runtime
{
    public interface ICellHandler
    {
        void Setup(GameLoop loop, Block block);
        void FrogMoved(bool isJumpedOnCell);
    }
}