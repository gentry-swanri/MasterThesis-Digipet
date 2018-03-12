using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DigipetServer
{
    class MySQLDatabase
    {

        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public MySQLDatabase()
        {
            DBInit();
        }

        private void DBInit()
        {
            server = "localhost";
            database = "digipetdb";
            uid = "root";
            password = "";

            string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch(MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public int Insert(string tableName, string columns, string values)
        {
            int affected = -1;
            string query = "INSERT INTO " + tableName + " (" + columns + ") VALUES(" + values + ")";

            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                affected = command.ExecuteNonQuery();
                this.CloseConnection();
            }

            return affected;
        }

        public int Update(string tableName, string data, string terms)
        {
            int affected = -1;
            string query = "UPDATE "+ tableName + " SET " + data + " WHERE " + terms;
            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                affected = command.ExecuteNonQuery();
                this.CloseConnection();
            }

            return affected;
        }

        public void Delete(string tableName, string terms)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + terms;
            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        public int Count(string tableName, string terms)
        {
            string query = "SELECT Count(*) FROM " + tableName + " WHERE " + terms;
            int count = -1;

            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                count = int.Parse(command.ExecuteScalar()+"");
                this.CloseConnection();

                return count;
            }else
            {
                return count;
            }
        }

        public int CreateNewAccount(string firstName, string lastName, string email, string username, string petName, string password)
        {
            int affected = -1;

            int count = this.Count("user", "username='" + username + "'" );
            if (count == 0)
            {
                affected = this.Insert("user", "username, password, is_active, first_name, last_name, email", "'" + username + "', '" + password + "', 0, '" + firstName + "', '" + lastName + "', '" + email + "'");
                if (affected == 1)
                {
                    int userId = this.FindUserId(username);
                    affected = this.Insert("game_data", "pet_name, rest, energy, agility, stress, heart, money, xp, user_id", "'" + petName + "', 100, 100, 100, 100, 6, 100, 0, " + userId);
                }
            }else
            {
                affected = -2;
            }

            return affected;
        }

        public List<Object> GetPetData(string username)
        {
            List<Object> data = new List<Object>();
            int userId = this.FindUserId(username);

            string query = "SELECT pet_name, rest, energy, agility, stress, heart, money, xp, created_at, last_modified FROM game_data WHERE user_id='" + userId + "'";
            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                string pet_name = "";
                int rest = -1;
                int energy = -1;
                int agility = -1;
                int stress = -1;
                int heart = -1;
                int money = -1;
                int xp = -1;
                string created_at = "";
                string last_modified = "";

                while (reader.Read())
                {
                    pet_name = (string)reader["pet_name"];
                    rest = (int)reader["rest"];
                    energy = (int)reader["energy"];
                    agility = (int)reader["agility"];
                    stress = (int)reader["stress"];
                    heart = (int)reader["heart"];
                    money = (int)reader["money"];
                    xp = (int)reader["xp"];
                    created_at = reader["created_at"].ToString();
                    last_modified = reader["last_modified"].ToString();
                }

                reader.Close();
                this.CloseConnection();

                data.Add(heart);
                data.Add(money);
                data.Add(xp);
                data.Add(rest);
                data.Add(energy);
                data.Add(agility);
                data.Add(stress);
                data.Add(pet_name);
            }

            return data;
        }

        public int UpdateStatus(string username, int rest, int energy, int agility, int stress, int heart, int money, int xp)
        {
            int result = -1;

            int affected = this.Update("user", "is_active=0", "username='" + username + "'");
            if (affected == 1)
            {
                int user_id = this.FindUserId(username);
                affected = this.Update("game_data", "rest=" + rest + ", energy=" + energy + ", agility=" + agility + ", stress=" + stress + ", heart=" + heart + ", money=" + money + ", xp=" + xp, "user_id=" + user_id);
                if (affected == 1)
                {
                    result = affected;
                }
            }

            return result;
        }

        public int FindUserIdByEmail(string email)
        {
            int id = -1;
            string query = "SELECT id FROM User WHERE email='" + email + "'";

            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    id = (int)reader["id"];
                }

                reader.Close();
                this.CloseConnection();
            }

            return id;
        }

        private int FindUserId(string username)
        {
            int id = -1;
            string query = "SELECT id FROM User WHERE username='" + username + "'";
            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    id = (int)reader["id"];
                }

                reader.Close();
                this.CloseConnection();
            }

            return id;
        }

        public string FindEmail(string username)
        {
            string email = "";
            string query = "SELECT email FROM User WHERE username='" + username + "'";
            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    email = (string)reader["email"];
                }

                reader.Close();
                this.CloseConnection();
            }

            return email;
        }

    }
}
