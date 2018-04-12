using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace MegaOffice
{
    public class DatabaseManager
    {
        private readonly string _connectionString = @"Server = (localdb)\mssqllocaldb; Database = MegaOfficeDB; Trusted_Connection = True";

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

                    customerList = BuildCustomerList(customerList, id, firstName, lastName, email, phoneNr);
                }
            }
            return customerList;
        }

        private List<Customer> BuildCustomerList(List<Customer> customerList, int id, string firstName, string lastName, string email, string phoneNr)
        {
            if (customerList.All(customer => customer.CustomerID != id))
                customerList.Add(new Customer
                {
                    CustomerID = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = new List<string> {phoneNr}
                });
            else if (phoneNr != null)
                customerList.Find(customer => customer.CustomerID == id).Phone.Add(phoneNr);
            return customerList;
        }

        public Customer ReadCustomer(int kundnummer)
        {
            var sql = @"SELECT Customer.ID, FirstName, LastName, Email, PhoneNr, ProductID, Products.Name
                        FROM Customer
                        LEFT JOIN PhoneNr ON Customer.ID=PhoneNr.CustomerID
                        LEFT JOIN Interest ON Customer.ID=Interest.CustomerID
                        LEFT Join Products ON Products.ID=Interest.ProductID
                        WHERE Customer.ID=@ID";

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
                    string phone = null;
                    if (!reader.IsDBNull(4))
                        phone = reader.GetString(4);
                    int productId = reader.GetInt32(5);
                    string productName = reader.GetString(6);

                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            CustomerID = id,
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            Phone = new List<string> {phone},
                            InterestingProducts = new List<Product>{new Product(productId)}
                        };
                    }
                    if (customer.Phone.All(phoneNr => phoneNr != phone))
                        customer.Phone.Add(phone);
                    if (customer.InterestingProducts.All(product => product.ProductID != productId))
                        customer.InterestingProducts.Add(new Product(productId, productName));
                }
            }

            customer.InterestingProducts.Select(product => ReadProduct(product.ProductID));
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
                command.Parameters.Add(new SqlParameter("ID", c.CustomerID));
                command.ExecuteNonQuery();
            }

            UpdatePhoneNumbers(c);
        }

        private void UpdatePhoneNumbers(Customer c)
        {
            DeletePhoneNumbers(c.CustomerID);
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
                                  VALUES ({c.CustomerID}, @PhoneNr{i});";
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

        public List<Product> ReadAllProducts()
        {
            var sql = @"SELECT Products.ID, Products.Name, Products.Price, ProductCategories.ID, ProductCategories.Name
                        FROM Products
                        LEFT JOIN ProductCategories ON Products.ProductCategoryID=ProductCategories.ID";

            var productList = new List<Product>();
            var categoryList = new List<Category>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    decimal price = reader.GetDecimal(2);
                    int categoryID = reader.GetInt32(3);
                    string categoryName = null;
                    if (!reader.IsDBNull(4))
                        categoryName = reader.GetString(4);

                    Category category = BuildCategoryList(categoryList, categoryID, categoryName);
                    productList = UpdateProductList(productList, id, name, price, category);
                }
            }
            return productList;
        }

        private Category BuildCategoryList(List<Category> categoryList, int categoryId, string categoryName)
        {
            if (categoryList.All(c => c.CategoryID != categoryId))
            {
                var category = new Category()
                {
                    CategoryID = categoryId,
                    Name = categoryName,
                };
                categoryList.Add(category);
                return category;
            }
            return categoryList.Find(c => c.CategoryID == categoryId);
        }

        private List<Product> UpdateProductList(List<Product> productList, int id, string name, decimal price, Category category)
        {
            if (productList.All(product => product.ProductID != id))
                productList.Add(new Product
                {
                    ProductID = id,
                    Name = name,
                    Price = price,
                    Category = category
                });
            return productList;
        }

        public Product ReadProduct(int productId)
        {
            var sql = @"SELECT Products.ID, Name, Price, ProductCategories.ID, ProductCategories.Name, CustomerID
                        FROM Products
                        LEFT JOIN Category ON Products.CategoryID=ProductCategories.ID
                        LEFT JOIN Interest ON Products.ID=Interest.ProductID
                        WHERE Products.ID=@ID";

            Product product = null;
            List<Customer> interestedCustomers = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", productId));

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    decimal price = reader.GetDecimal(2);
                    int categoryID = reader.GetInt32(3);
                    string categoryName = null;
                    if (!reader.IsDBNull(4))
                        categoryName = reader.GetString(4);
                    int customerID = reader.GetInt32(5);

                    interestedCustomers = BuildInterestedCustomersList(interestedCustomers, customerID);
                    Category category = new Category(categoryID, categoryName);
                    product = new Product(id, name, price, category, interestedCustomers);
                }
            }
            return product;
        }

        private List<Customer> BuildInterestedCustomersList(List<Customer> customerList, int customerId)
        {
            if (customerList.All(c => c.CustomerID != customerId))
                customerList.Add(new Customer{CustomerID = customerId});
            return customerList;
        }

        public List<Category> ReadAllCategories()
        {
            string sql = @"SELECT CategoryID, Name
                           FROM ProductCategories";
            List<Category> categories = new List<Category>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);

                    categories.Add(new Category(id, name));
                }
            }

            return categories;
        }

        public Category ReadCategory(int categoryID)
        {
            string sql = @"SELECT Name
                           FROM ProductCategories
                           WHERE CategoryID=@ID";
            string name = "";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", categoryID));

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    name = reader.GetString(0); 
                }
            }
            return new Category(categoryID, name);
        }
    }
}