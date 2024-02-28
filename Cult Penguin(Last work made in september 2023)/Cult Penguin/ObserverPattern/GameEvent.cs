using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin
{
    public class GameEvent
    {
        private List<IGameListner> listners = new List<IGameListner>();

        public void Attach(IGameListner listner)
        {
            listners.Add(listner);
        }

        public void Detach(IGameListner listner)
        {
            listners.Remove(listner);
        }

        public void Notify()
        {
            foreach (IGameListner listner in listners)
            {
                listner.Notify(this);
            }
        }


    }
}
