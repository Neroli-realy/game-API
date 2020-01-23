using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_API
{
    

    class Player
    {
        public string name { get; set; }
        public int health {get; set;}
        public int gold { get; set; }
        public int mana { get; set; }
        public int level { get; set; }
        public int attack { get; set; }
        public int deffence { get; set; }
        public int MagicAttack { get; set; }

        public List<Player> Friends = new List<Player>();
        public List<Item> Items = new List<Item>();

        

        public Player(Client c)
        {
            this.health = Convert.ToInt32((c.getInfo()[0]));
            this.mana = Convert.ToInt32((c.getInfo()[1]));
            this.attack = Convert.ToInt32((c.getInfo()[2]));
            this.deffence = Convert.ToInt32((c.getInfo()[3]));
            this.gold = Convert.ToInt32((c.getInfo()[4]));
            this.level = Convert.ToInt32((c.getInfo()[5]));
            this.MagicAttack = Convert.ToInt32((c.getInfo()[6]));

        }
        public Player()
        {

        }
        public Player(string name)
        {
            Player p = Mysqlcommands.get(new Player(), name, false);
            this.name = p.name;
            this.attack = p.attack;
            this.gold = p.gold;
            this.health = p.health;
            this.level = p.level;
            this.deffence = p.deffence;
            this.mana = p.mana;
            this.MagicAttack = p.MagicAttack;
        }


        



    }
}
