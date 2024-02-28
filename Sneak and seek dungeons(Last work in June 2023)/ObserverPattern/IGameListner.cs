namespace Sneak_and_seek_dungeons.ObserverPattern
{
    public interface IGameListner
    {
        void Notify(GameEvent gameEvent);
    }
}