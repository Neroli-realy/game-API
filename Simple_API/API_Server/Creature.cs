using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Server
{
    abstract class Creature
    {
        public string name;
        public int health;
        public int Damage;
        public int def;
        

        public Creature() { }
    }
}
