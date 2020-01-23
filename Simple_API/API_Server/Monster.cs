using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Server
{
    class Monster : Creature
    {
        public int mr;
        public int def;
        public Monster() { }

        public Monster(string name, string hp, string def, string damage, string mr)
        {
            this.mr = Convert.ToInt32(mr);
            this.name = name;
            this.health = Convert.ToInt32(hp);
            this.def = Convert.ToInt32(def);
            this.Damage = Convert.ToInt32(damage);
        }
    }
}
