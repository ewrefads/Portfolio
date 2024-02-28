using System;
using System.Collections.Generic;
using System.Text;

namespace Meerkat_Mining
{
    public interface IGameListner
    {
        void Notify(GameEvent gameEvent);
    }
}
