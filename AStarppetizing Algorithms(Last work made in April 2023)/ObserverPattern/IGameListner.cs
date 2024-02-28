namespace AStarppetizing_Algorithms.ObserverPattern
{
    public interface IGameListner
    {
        void Notify(GameEvent gameEvent);
    }
}