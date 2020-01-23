using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Threading;

namespace API_Server
{
    abstract class Commands
    {
        private static  MySqlDataReader reader;

        public static bool doLogin(Player player)
        {
            string query = "select * from data.users where Name= @uname and Password = @upass ;";
            string[] param = {"@uname", "@upass"};
            string[] values = { player.name, player.password };
            if(sendquery(query, param ,values))
            {
                //find user with given credintioals
                int i = 0;
                while (reader.Read()) i++;

                if (i == 1)
                {
                   // Alerts.Success(String.Format("user {0} logged in", player.name));
                    player.send("Success:Welcome " + player.name);
                    sendInfo(player);
                    return true;
                }
                else
                {
                   // Alerts.Debug(String.Format("{0} Entered Wrong Username or Password",player.name));
                    player.send("ERROR:Wrong username or password");
                    return false;
                }
            }
            return false;
            
        }

        public static bool doREG(Player player)
        {
            //check if exist
            string query = "select * from data.users where Name= @uname ;";
            string[] param = { "@uname" };
            string[] values = { player.name };

            if (sendquery(query, param, values))
            {
                int i = 0;
                while (reader.Read()) i++;
                if (i > 0)
                {
                    player.send("ERROR:username is taken");
                    return false;
                }
                else
                {
                    query = "insert into data.users(Name, Password) values(@uname, @upass);";
                    param = new string[2]{"@uname" , "@upass"};
                    values = new string[2] { player.name, player.password };
                    if (sendquery(query, param, values))
                    {
                        player.send("Success:Registrated successfully");

                        //initialize info 
                        query = "insert into data.playersdata(Name) values(@uname)";
                        param = new string[1] { "@uname" };
                        values = new string[1] { player.name };
                        sendquery(query, param, values);

                        //initialize req 
                        query = "insert into data.frequests(Name) values(@uname)";
                        param = new string[1] { "@uname" };
                        values = new string[1] { player.name };
                        sendquery(query, param, values);

                        return true;
                    }

                }
            }
            player.send("ERROR: Try agian later");
            return false;
        }

        public static bool getUsers(Player player)
        {
            string query = "select Name from data.playersdata where 1= @uname ;";
            string[] param = { "@uname" };
            string[] values = { "1" };
            if (sendquery(query, param, values))
            {
                string names = "PLAYERS:";
                while (reader.Read()) { if (!reader[0].ToString().Equals(player.name)) names += reader[0].ToString() + ":"; }
                player.send(names);
                return true;
            }
            return false;
        }

        public static bool sendAdd(Player from, string pname)
        {
            string query = "select req from data.frequests where Name= @uname";
            string[] param = {"@uname"};
            string[] values = {pname};
            if(sendquery(query, param, values)){
                reader.Read();
                string[] names = reader[0].ToString().Split(':');
                var res = Array.Find(names, s => s.Equals(from.name));

                if (String.IsNullOrEmpty(res))
                {
                    //add from.name to player friend requests list
                    query = "UPDATE data.frequests SET req = CONCAT(req, @fname, ':') WHERE Name = @uname ;";
                    param = new string[2]{ "@fname", "@uname"};
                    values = new string[2]{ from.name,  pname };

                    if (sendquery(query, param, values))
                    {
                        from.send("friend Request sent");
                        return true;
                    }
                }
                else
                {
                    from.send("ERROR: you already sent request to this player");
                    return false;
                }
            }
            return false;
        
        }

        public static bool acceptAdd(Player player, string name)
        {
            string query = "select req from data.frequests where Name= @uname ;";
            string[] param = { "@uname" };
            string[] values = { player.name };

            if (sendquery(query, param, values))
            {
                reader.Read();
                string res = "";
                string[] names = new string[reader[0].ToString().Split(':').Length];
                foreach (string s in reader[0].ToString().Split(':'))
                    if (!s.Equals(name))
                        res += s + ":";
                query = "update data.frequests set req= @req where Name= @name ;";
                param = new string[2] { "@req", "@name" };
                values = new string[2] { res, player.name };
                if (sendquery(query, param, values))
                {
                    sendFR(player);
                    query = "update data.playersdata set FL= CONCAT(FL, @ufr, ':') where Name= @uname ;";
                    param = new string[2] { "@ufr", "@uname" };
                    values = new string[2] { name, player.name };
                    if(sendquery(query, param, values))
                    {
                        //add both players to each other
                        query = "update data.playersdata set FL= CONCAT(FL, @ufr, ':') where Name= @uname ;";
                        param = new string[2] { "@ufr", "@uname" };
                        values = new string[2] { player.name, name };
                        if (sendquery(query, param, values))
                        {
                            player.send(String.Format("now you and {0} are friends!", name));
                            return true;
                        }
                    }
                   
                }
            }
            return false;
        }

        public static bool sendFR(Player player)
        {
            string query = "select req from data.frequests where Name= @uname ;";
            string[] param = { "@uname" };
            string[] values = { player.name };

            if (sendquery(query, param, values))
            {
                while (reader.Read())
                {
                    player.send("FR:" + reader[0].ToString());
                }
                return true;
            }
            return false;
        }

        public static bool getMonsters(Player player)
        {
            string query = "select * from data.monsters where 1 = @uval;";
            string[] param = { "@uval" };
            string[] values = { "1" };

            if (sendquery(query, param, values))
            {
                List<Monster> monsters = new List<Monster>();
                while (reader.Read())
                {
                    player.send(String.Format("MONST:{0}:{1}:{2}:{3}:{4}", reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString()));
                    //Thread.Sleep(1000);
                    Monster s = new Monster(reader[1].ToString(), reader[2].ToString(), reader[3].ToString(),reader[4].ToString(),reader[5].ToString());
                    monsters.Add(s);
                }
                player.send("END");
                player.monsters = monsters;
                return true;
            }
            return false;
        }
        public static bool sendInfo(Player player)
        {
            //send info about character
            string query = "select * from data.playersdata where Name= @uname ;";
            string[] param = { "@uname" };
            string[] values = { player.name };

            if (sendquery(query, param, values))
            {
                string data = "INFO:";
                reader.Read();
                for (int i = 0; i <= 6; i++) data += reader[i] + ":";
                player.name = reader[0].ToString();
                player.health = Convert.ToInt32(reader[1].ToString());
                player.mana = Convert.ToInt32(reader[2].ToString());
                player.Damage = Convert.ToInt32(reader[3].ToString());
                player.def = Convert.ToInt32(reader[4].ToString());
                player.Gold = Convert.ToInt32(reader[5].ToString());
                player.Level = Convert.ToInt32(reader[6].ToString());
                player.send(data);
                //Alerts.Debug(data);
                return true;
            }
            return false;
        }
        private static bool sendquery(string cmd,string[] param, string[] values)
        {
            //connect to database
            string connection = "datasource=localhost;port=3306;username=root;password=root";
            MySqlConnection mysqlconnection = new MySqlConnection(connection);
            MySqlCommand command = new MySqlCommand(cmd, mysqlconnection);

            //using parametrized query to avoid sqlinjection attack
            for (int i = 0; i < param.Length; i++)
                command.Parameters.AddWithValue(param[i], values[i]);

            try
            {
                //start excute the query
                mysqlconnection.Open();
                reader = command.ExecuteReader();
                
            }
            catch (MySqlException ex)
            {
                Alerts.Error(ex.Message);
                return false;
            }
            return true;
        }

    }
}
