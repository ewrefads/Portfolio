using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin_Server
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public byte[] passwordSalt { get; set; }
        public float[] position { get; set; }

        public DateTime creationDate { get; set; }
    }
}
