using Microsoft.Xna.Framework;

namespace Chaos_in_Colosseum
{
    /// <summary>
    /// Unarmed er det våben som gives til en actor når de ikke har nogen våben for at undgå fejl
    /// </summary>
    public class Unarmed : Weapon
    {
        public Unarmed(string[] name, Vector2 pos) : base(name, pos, 0f, 2)
        {
        }
    }
}