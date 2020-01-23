using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Simple_API
{
    public  class Mysqlcommands
    {
        public static T get<T>(T data, string name, bool ALL){

            string connection = "datasource=localhost;port=3306;username=root;password=root";
            MySqlConnection mysqlconnection = new MySqlConnection(connection);
            MySqlCommand command = null;
            MySqlDataReader reader;
            try
            {
                string query = "select  * from Data.PlayersData where Name=@uname;";
                command = new MySqlCommand(query, mysqlconnection);
                command.Parameters.AddWithValue("uname", name);
                mysqlconnection.Open();
                reader = command.ExecuteReader();
                if (data.GetType() == typeof(List<Player>) && ALL == false)
                {
                    List<Player> frinds = new List<Player>();
                    while (reader.Read())
                    {

                        foreach (string FN in reader[8].ToString().Split(','))
                        {
                            Player p = new Player(FN);
                            frinds.Add(p);
                        }
                    }
                    return (T)Convert.ChangeType(frinds, typeof(T));
                }
                else if (data.GetType() == typeof(List<Player>) && ALL == true)
                {
                    reader.Close();
                    List<Player> players = new List<Player>();
                    query = "select * from data.Playersdata";
                    command = new MySqlCommand(query, mysqlconnection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Player p = new Player(reader[0].ToString());
                        players.Add(p);
                    }

                    return (T)Convert.ChangeType(players, typeof(T));
                }

                else if (data.GetType() == typeof(List<Item>))
                {
                    List<Item> items = new List<Item>();
                    while (reader.Read())
                    {

                        foreach (string IT in reader[7].ToString().Split(','))
                        {
                            Item p = new Item(IT);
                            items.Add(p);
                        }
                    }
                    return (T)Convert.ChangeType(items, typeof(T));
                }
                else if(data.GetType() == typeof(Player)){
                    Player p = new Player();
                    while(reader.Read()){
                        p.name = name;
                        p.level = Convert.ToInt32(reader[6].ToString());
                        p.health = Convert.ToInt32(reader[1].ToString());
                        p.gold = Convert.ToInt32(reader[5].ToString());
                        p.mana = Convert.ToInt32(reader[2].ToString());
                        p.attack = Convert.ToInt32(reader[3].ToString());
                        p.deffence = Convert.ToInt32(reader[4].ToString());
                    }
                    return (T)Convert.ChangeType(p, typeof(T));
                }


                reader.Close();
                mysqlconnection.Close();
            }
            catch (MySqlException ex)
            {

            }
            return default(T);
        }

        
    }
}
