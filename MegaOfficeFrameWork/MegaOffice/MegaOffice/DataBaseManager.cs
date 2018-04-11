using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace MegaOffice
{
    public class DatabaseManager
    {
        private string _connectionString = @"Server = (localdb)\mssqllocaldb; Database = MegaOfficeDB; Trusted_Connection = True";

        public DatabaseManager(string connectionString)
        {
            this._connectionString = connectionString;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
            }

        }

        public DatabaseManager(string server, string database, bool trustedConnection) : this($@"Server = {server}; Database = {database}; Trusted_Connection = {trustedConnection}")
        {
        }


        public DatabaseManager()
        {

        }


        public List<Customer> ReadAllCustomers()
        {
            var sql = @"SELECT ID, FirstName, LastName, Email, PhoneNr
                        FROM Customer
                        LEFT JOIN PhoneNr ON Customer.ID=PhoneNr.CustomerID";

            var customerList = new List<Customer>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string email = reader.GetString(3);
                    string phoneNr = null;
                    if (!reader.IsDBNull(4))
                        phoneNr = reader.GetString(4);

                    customerList = UpdateCustomerList(customerList, id, firstName, lastName, email, phoneNr);
                }
            }
            return customerList;
        }

        private List<Customer> UpdateCustomerList(List<Customer> customerList, int id, string firstName, string lastName, string email, string phoneNr)
        {
            if (customerList.All(customer => customer.Kundnummer != id))
                customerList.Add(new Customer(id, firstName, lastName, email, new List<string> {phoneNr}));
            else if (phoneNr != null)
                customerList.Find(customer => customer.Kundnummer == id).Phone.Add(phoneNr);
            return customerList;
        }


        public Customer ReadCustomer(int kundnummer)
        {
            var sql = @"SELECT ID, FirstName, LastName, Email, PhoneNr
                        FROM Customer
                        LEFT JOIN PhoneNr ON Customer.ID=PhoneNr.CustomerID
                        WHERE ID=@ID";

            Customer customer = null;

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

                    
                    if (!reader.IsDBNull(4))
                    {
                        string phone = reader.GetString(4);

                        if (customer == null)
                            customer = new Customer(id, firstName, lastName, email, new List<string> { phone });
                        else
                            customer.Phone.Add(phone);
                    }
                    else
                    {
                        customer = new Customer(id, firstName, lastName, email);
                    }


                }
            }

            return customer;
        }

        public void CreateCustomer(Customer c)
        {
            var sql = $@"INSERT INTO Customer (FirstName, LastName, Email)
                         VALUES (@FirstName, @LastName, @Email);
                         DECLARE @DynID AS int;
                         SET @DynID = @@IDENTITY;";


            for (int i = 0; i < c.Phone.Count; i++)
            {
                sql += $@"INSERT INTO PhoneNr (CustomerID, PhoneNr)
                            VALUES (@DynID, @PhoneNr{i});";
            }



            /*
                DECLARE @DynID AS int;
				INSERT INTO Customer (FirstName, LastName, Email)
                VALUES ('Kalle', 'Karlsson', 'pedal@asdf.com');
				SET @DynID = @@IDENTITY;
                INSERT INTO PhoneNr (CustomerID, PhoneNr)
                VALUES (@DynID, '045678909');
                INSERT INTO PhoneNr (CustomerID, PhoneNr)
                VALUES (@DynID, '076985145556');
 
             */


            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("FirstName", c.FirstName));
                command.Parameters.Add(new SqlParameter("LastName", c.LastName));
                command.Parameters.Add(new SqlParameter("Email", c.Email));
                for (int i = 0; i < c.Phone.Count; i++)
                {
                    command.Parameters.Add(new SqlParameter($"PhoneNr{i}", c.Phone[i]));
                }

                command.ExecuteNonQuery();
            }

        }

        public void DeleteCustomer(int kundnummer)
        {
            DeletePhoneNumbers(kundnummer);

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

        public void DeletePhoneNumbers(int kundnummer)
        {
            var sql = $@"DELETE FROM PhoneNr
                        WHERE CustomerID=@ID";

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
                        SET FirstName=@FirstName,LastName=@LastName,Email=@Email
                        WHERE ID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("FirstName", c.FirstName));
                command.Parameters.Add(new SqlParameter("LastName", c.LastName));
                command.Parameters.Add(new SqlParameter("Email", c.Email));
                command.Parameters.Add(new SqlParameter("ID", c.Kundnummer));
                command.ExecuteNonQuery();
            }

            UpdatePhoneNumbers(c);
        }

        private void UpdatePhoneNumbers(Customer c)
        {
            DeletePhoneNumbers(c.Kundnummer);
            AddPhoneNumbers(c);

        }

        private void AddPhoneNumbers(Customer c)
        {
            if (c.Phone.Count > 0)
            {
                string sql = "";

                if (c.Phone.Count > 0)
                {
                    sql = "";

                    for (int i = 0; i < c.Phone.Count; i++)
                    {
                        sql += $@"INSERT INTO PhoneNr (CustomerID, PhoneNr)
                                  VALUES ({c.Kundnummer}, @PhoneNr{i});";
                    }
                }



                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    for (int i = 0; i < c.Phone.Count; i++)
                    {
                        command.Parameters.Add(new SqlParameter($"PhoneNr{i}", c.Phone[i]));
                    }

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}