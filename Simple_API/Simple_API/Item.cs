using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_API
{
    class Item
    {
        private string name;

        private int id { get; set; }

        public Item() { }
        public Item(string id)
        {
            this.id  = Convert.ToInt32(id);
        }
    }
}
