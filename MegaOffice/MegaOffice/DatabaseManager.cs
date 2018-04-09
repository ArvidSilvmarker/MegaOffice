using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MegaOffice
{
    public class DatabaseManager
    {
        private string _connectionString = @"Server = (localdb)\mssqllocaldb; Database = MegaOfficeDB; Trusted_Connection = True";

        public DatabaseManager(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DatabaseManager(string server, string database, bool trustedConnection)
        {
            _connectionString = $@"Server = {server}; Database = {database}; Trusted_Connection = {trustedConnection}";
        }

        public DatabaseManager()
        {

        }

        public List<Customer> ReadAllCustomers()
        {
            var sql = @"SELECT ID, FirstName, LastName, Email, PhoneNr
                        FROM Customer";

            var customerList = new List<Customer>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                //command.Parameters.Add(new SqlParameter("Title", title));

                SqlDataReader reader = command.ExecuteReader();
                

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string email = reader.GetString(3);
                    string phone = reader.GetString(4);

                    customerList.Add(new Customer(id, firstName, lastName, email, phone));
                }           
            }
            return customerList;
        }

        public Customer ReadCustomer(int kundnummer)
        {
            var sql = @"SELECT ID, FirstName, LastName, Email, PhoneNr
                        FROM Customer
                        WHERE ID=@ID";
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", kundnummer));

                SqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string email = reader.GetString(3);
                    string phone = reader.GetString(4);

                    return new Customer(id, firstName, lastName, email, phone);
                }
            }

            return null;
        }

        public void CreateCustomer(Customer c)
        {
            var sql = $@"INSERT INTO Customer (FirstName, LastName, Email, PhoneNr)
                        VALUES (@FirstName, @LastName, @Email, @PhoneNr);";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("FirstName", c.FirstName));
                command.Parameters.Add(new SqlParameter("LastName", c.LastName));
                command.Parameters.Add(new SqlParameter("Email", c.Email));
                command.Parameters.Add(new SqlParameter("PhoneNr", c.Phone));
                command.ExecuteNonQuery();
            }

        }

        public void DeleteCustomer(int kundnummer)
        {
            var sql = $@"DELETE FROM Customer
                        WHERE ID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", kundnummer));
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCustomer(Customer c)
        {
            var sql = $@"UPDATE Customer
                        SET FirstName=@FirstName,LastName=@LastName,Email=@Email,PhoneNr=@PhoneNr
                        WHERE ID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("FirstName", c.FirstName));
                command.Parameters.Add(new SqlParameter("LastName", c.LastName));
                command.Parameters.Add(new SqlParameter("Email", c.Email));
                command.Parameters.Add(new SqlParameter("PhoneNr", c.Phone));
                command.Parameters.Add(new SqlParameter("ID", c.Kundnummer));
                command.ExecuteNonQuery();
            }
        }


    }
}
