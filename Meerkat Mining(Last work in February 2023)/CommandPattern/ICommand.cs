using Meerkat_Mining.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meerkat_Mining
{
    public interface ICommand
    {
        public void Execute(Player player);
        public void Execute(Drill drill);
    }
}
