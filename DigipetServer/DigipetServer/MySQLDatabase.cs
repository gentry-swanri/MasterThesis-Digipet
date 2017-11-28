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

            //string lowerFirstName = firstName.ToLower();
            //string lowerLastName = lastName.ToLower();
            //string lowerUsername = username.ToLower();

            int count = this.Count("user", "username='" + username + "'" );
            if (count == 0)
            {
                affected = this.Insert("user", "username, password, is_active", "'" + username + "', '" + password + "', 1");
                if (affected == 1)
                {
                    int userId = this.FindUserId(username);
                    affected = this.Insert("user_detail","first_name, last_name, email, user_id", "'" + firstName + "', '" + lastName + "', '" + email + "', " + userId);
                    if (affected == 1)
                    {
                        affected = this.Insert("pet_detail", "pet_name, energy, hunger, fun, hygiene, environment, user_id", "'" + petName + "', 100, 100, 100, 100, 100, " + userId);
                    }
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

            string query = "SELECT pet_name, energy, hunger, fun, hygiene, environment, created_at, last_modified FROM pet_detail WHERE user_id='" + userId + "'";
            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                string pet_name = "";
                int energy = -1;
                int hunger = -1;
                int fun = -1;
                int hygiene = -1;
                int environment = -1;
                string created_at = "";
                string last_modified = "";

                while (reader.Read())
                {
                    pet_name = (string)reader["pet_name"];
                    energy = (int)reader["energy"];
                    hunger = (int)reader["hunger"];
                    fun = (int)reader["fun"];
                    hygiene = (int)reader["hygiene"];
                    environment = (int)reader["environment"];
                    created_at = reader["created_at"].ToString();
                    last_modified = reader["last_modified"].ToString();
                }

                reader.Close();
                this.CloseConnection();

                // update status since last_modified and calculate age
                DateTime created_at_datetime = Convert.ToDateTime(created_at);
                DateTime last_modified_datetime = Convert.ToDateTime(last_modified);
                DateTime currentTime = DateTime.Now;

                TimeSpan rangeUpdate = currentTime.Subtract(last_modified_datetime);
                int age = currentTime.Year - created_at_datetime.Year;

                int totalReduce = (int)rangeUpdate.TotalHours * 5;

                energy -= totalReduce;
                hunger += totalReduce;
                fun -= totalReduce;
                hygiene -= totalReduce;
                environment -= totalReduce;

                data.Add(age);
                data.Add((energy < 0) ? 0 : energy);
                data.Add((hunger >= 100) ? 100 : hunger);
                data.Add((fun < 0) ? 0 : fun);
                data.Add((hygiene < 0) ? 0 : hygiene);
                data.Add((environment < 0) ? 0 : environment);
                data.Add(pet_name);
            }

            return data;
        }

        public int UpdateStatus(string username, int energy, int hunger, int fun, int hygiene, int environment)
        {
            int result = -1;

            int affected = this.Update("user", "is_active=0", "username='" + username + "'");
            if (affected == 1)
            {
                int user_id = this.FindUserId(username);
                affected = this.Update("pet_detail", "energy=" + energy + ", hunger=" + hunger + ", fun=" + fun + ", hygiene=" + hygiene + ", environment=" + environment, "user_id=" + user_id);
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
            string query = "SELECT user_id FROM User_detail WHERE email='" + email + "'";

            if (this.OpenConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    id = (int)reader["user_id"];
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
            int id = FindUserId(username);
            string query = "SELECT email FROM User_detail WHERE user_id='" + id + "'";
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
