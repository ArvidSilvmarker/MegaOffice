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


        public void CreateCustomer(Customer c)
        {
            var sql = $@"INSERT INTO Customer (FirstName, LastName, Email)
                         VALUES (@FirstName, @LastName, @Email);
                         DECLARE @DynID AS int;
                         SET @DynID = @@IDENTITY;";

            for (int i = 0; i < c.Phone.Count; i++)
            {
                sql += $@"INSERT INTO PhoneNr (customerID, PhoneNr)
                          VALUES (@DynID, @PhoneNr{i});";
            }
            for (int j = 0; j < c.InterestingProducts.Count; j++)
            {
                sql += $@"INSERT INTO Interest (customerID, ProductID)
                          VALUES (@DynID, @ProductID{j});";
            }

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
                for (int j = 0; j < c.InterestingProducts.Count; j++)
                {
                    command.Parameters.Add(new SqlParameter($"ProductID{j}",
                        c.InterestingProducts[j].ProductID));
                }

                command.ExecuteNonQuery();
            }

        }
        public void CreateProduct(Product p)
        {
            var sql = $@"INSERT INTO Products (Name, Price, ProductCategoryID)
                         VALUES (@Name, @Price, @ProductCategoryID);
                         DECLARE @DynID AS int;
                         SET @DynID = @@IDENTITY;";

            for (int i = 0; i < p.InterestedCustomers.Count; i++)
            {
                sql += $@"INSERT INTO Interest (ProductID, customerID)
                          VALUES (@DynID, @customerID{i});";
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Name", p.Name));
                command.Parameters.Add(new SqlParameter("Price", p.Price));
                command.Parameters.Add(new SqlParameter("ProductCategoryID", p.Category.CategoryID));
                for (int i = 0; i < p.InterestedCustomers.Count; i++)
                {
                    command.Parameters.Add(new SqlParameter($"customerID{i}",
                        p.InterestedCustomers[i].CustomerID));
                }

                command.ExecuteNonQuery();
            }
        }
        public void CreateCategory(Category c)
        {
            var sql = $@"INSERT INTO ProductCategories (ID, Name)
                         VALUES (@Name)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Name", c.Name));
                command.ExecuteNonQuery();
            }
        }


        public List<Customer> ReadAllCustomers()
        {
            var sql = @"SELECT Customer.ID, FirstName, LastName, Email, PhoneNr, ProductID
                        FROM Customer
                        LEFT JOIN PhoneNr ON Customer.ID=PhoneNr.customerID
                        LEFT JOIN Interest ON Customer.ID=Interest.customerID";

            List<Customer> customerList = null;

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
                    string phone = null;
                    if (!reader.IsDBNull(4))
                        phone = reader.GetString(4);
                    int productId = reader.GetInt32(5);

                    customerList = BuildCustomerList(customerList, id, firstName, lastName, email, phone, productId);
                }
            }

            return customerList;
        }
        public Customer ReadCustomer(int productID)
        {
            var sql = @"SELECT Customer.ID, FirstName, LastName, Email, PhoneNr, ProductID
                        FROM Customer
                        LEFT JOIN PhoneNr ON Customer.ID=PhoneNr.customerID
                        LEFT JOIN Interest ON Customer.ID=Interest.customerID
                        WHERE Customer.ID=@ID";

            Customer customer = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", productID));

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

                    customer = BuildCustomer(customer, id, firstName, lastName, email, phone, productId);
                }
            }

            return customer;
        }
        public List<Product> ReadAllProducts()
        {
            var sql = @"SELECT Products.ID, Products.Name, Products.Price, ProductCategories.ID, ProductCategories.Name, customerID
                        FROM Products
                        LEFT JOIN ProductCategories ON Products.ProductCategoryID=ProductCategories.ID
                        LEFT JOIN Interest ON Products.ID=Interest.ProductID";

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
                    string categoryName = reader.GetString(4);
                    int productID = reader.GetInt32(5);


                    Category category = BuildCategory(categoryList, categoryID, categoryName);
                    productList = BuildProductList(productList, id, name, price, category, productID);
                }
            }
            return productList;
        }
        public Product ReadProduct(int productId)
        {
            var sql = @"SELECT Products.ID, Name, Price, ProductCategories.ID, ProductCategories.Name, customerID
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
                    product = BuildProduct(product, id, name, price, category, interestedCustomers);
                }
            }
            return product;
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

                    categories = BuildCategoryList(categories, id, name);
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
            UpdateInterestingProducts(c);
        }
        private void UpdateProduct(Product p)
        {
            var sql = $@"UPDATE Products
                        SET Name=@Name,Price=@Price,ProductCategoryID=@ProductCategoryID
                        WHERE ID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Name", p.Name));
                command.Parameters.Add(new SqlParameter("Price", p.Price));
                command.Parameters.Add(new SqlParameter("ProductCategoryID", p.Category.CategoryID));
                command.Parameters.Add(new SqlParameter("ID", p.ProductID));
                command.ExecuteNonQuery();
            }

            UpdateInterestedCustomers(p);
        }
        private void UpdateCategory(Category c)
        {
            var sql = $@"Update ProductCategories
                         SET Name=@Name
                         WHERE ID=@ID";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Name", c.Name));
                command.Parameters.Add(new SqlParameter("ID", c.CategoryID));
            }
        }
        private void UpdateInterestingProducts(Customer c)
        {
            DeleteInterestingProducts(c.CustomerID);
            InsertInterestingProducts(c);
        }
        private void UpdatePhoneNumbers(Customer c)
        {
            DeletePhoneNumbers(c.CustomerID);
            InsertPhoneNumbers(c);
        }
        private void UpdateInterestedCustomers(Product p)
        {
            DeleteInterestedCustomers(p.ProductID);
            InsertInterestedCustomers(p);
        }


        public void DeleteCustomer(int customerID)
        {
            DeletePhoneNumbers(customerID);
            DeleteInterestingProducts(customerID);

            var sql = $@"DELETE FROM Customer
                         WHERE ID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", customerID));
                command.ExecuteNonQuery();
            }
        }
        public void DeleteProduct(int productID)
        {
            DeleteInterestedCustomers(productID);

            var sql = $@"DELETE FROM Products
                         WHERE ID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", productID));
                command.ExecuteNonQuery();
            }
        }
        public void DeleteCategory(int categoryID)
        {
            DeleteProductByCategory(categoryID);

            var sql = $@"DELETE FROM ProductCategories
                         WHERE ID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", categoryID));
                command.ExecuteNonQuery();
            }
        }
        public void DeleteProductByCategory(int categoryID)
        {
            DeleteInterestedCustomers(categoryID);

            var sql = $@"SELECT ID
                         FROM Products
                         WHERE ProductCategoryID=@ID";

            List<int> productIDs = new List<int>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", categoryID));
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    productIDs.Add(reader.GetInt32(0));
                }
            }

            foreach (var productID in productIDs)
            {
                DeleteProduct(productID);
            }

        }
        public void DeletePhoneNumbers(int customerID)
        {
            var sql = $@"DELETE FROM PhoneNr
                        WHERE customerID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", customerID));
                command.ExecuteNonQuery();
            }
        }
        public void DeleteInterestingProducts(int customerID)
        {
            var sql = $@"DELETE FROM Interest
                         WHERE customerID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", customerID));
                command.ExecuteNonQuery();
            }  
        }
        public void DeleteInterestedCustomers(int productID)
        {
            var sql = $@"DELETE FROM Interest
                         WHERE ProductID=@ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("ID", productID));
                command.ExecuteNonQuery();
            }
        }


        private void InsertPhoneNumbers(Customer c)
        {
            if (c.Phone.Count > 0)
            {
                string sql = "";

                for (int i = 0; i < c.Phone.Count; i++)
                {
                    sql += $@"INSERT INTO PhoneNr (customerID, PhoneNr)
                                VALUES ({c.CustomerID}, @PhoneNr{i});";
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
        private void InsertInterestingProducts(Customer c)
        {
            if (c.InterestingProducts.Count > 0)
            {
                string sql = "";

                for (int i = 0; i < c.InterestingProducts.Count; i++)
                {
                    sql += $@"INSERT INTO Interest (customerID, ProductID)
                                VALUES ({c.CustomerID}, @ProductID{i});";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    for (int i = 0; i < c.InterestingProducts.Count; i++)
                    {
                        command.Parameters.Add(new SqlParameter($"ProductID{i}", c.InterestingProducts[i].ProductID));
                    }

                    command.ExecuteNonQuery();
                }
            }
        }
        private void InsertInterestedCustomers(Product p)
        {
            if (p.InterestedCustomers.Count > 0)
            {
                string sql = "";

                for (int i = 0; i < p.InterestedCustomers.Count; i++)
                {
                    sql += $@"INSERT INTO Interest (ProductID, customerID)
                                VALUES ({p.ProductID}, @customerID{i});";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    for (int i = 0; i < p.InterestedCustomers.Count; i++)
                    {
                        command.Parameters.Add(new SqlParameter($"customerID{i}", p.InterestedCustomers[i].CustomerID));
                    }

                    command.ExecuteNonQuery();
                }
            }
        }


        private List<Customer> BuildInterestedCustomersList(List<Customer> customerList, int customerId)
        {
            if (customerList.All(c => c.CustomerID != customerId))
                customerList.Add(new Customer{CustomerID = customerId});
            return customerList;
        }
        private List<Customer> BuildCustomerList(List<Customer> customerList, int id, string firstName, string lastName, string email, string phoneNr, int productID)
        {
            Customer customer;
            if (customerList.All(c => c.CustomerID != id))
            {
                customer = new Customer
                {
                    CustomerID = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = new List<string>(),
                    InterestingProducts = new List<Product>()
                };
                customerList.Add(customer);
            }
            else
            {
                customer = customerList.Find(c => c.CustomerID == id);
            }

            if (phoneNr != null && !customer.Phone.Contains(phoneNr))
                customer.Phone.Add(phoneNr);
            if (customer.InterestingProducts.All(p => p.ProductID != productID))
                customer.InterestingProducts.Add(new Product(productID));

            return customerList;
        }
        private Customer BuildCustomer(Customer customer, int id, string firstName, string lastName, string email, string phone, int productId)
        {
            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerID = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = new List<string>(),
                    InterestingProducts = new List<Product>( )
                };
            }
            if (customer.Phone.All(phoneNr => phoneNr != phone))
                customer.Phone.Add(phone);
            if (customer.InterestingProducts.All(product => product.ProductID != productId))
                customer.InterestingProducts.Add(new Product(productId));

            return customer;
        }
        private List<Product> BuildProductList(List<Product> productList, int id, string name, decimal price, Category category, int customerID)
        {
            Product product;
            if (productList.All(p => p.ProductID != id))
            {
                product = new Product
                {
                    ProductID = id,
                    Name = name,
                    Price = price,
                    Category = category,
                    InterestedCustomers = new List<Customer>()
                };
                productList.Add(product);
            }
            else
            {
                product = productList.Find(p => p.ProductID == id);
            }

            if (product.InterestedCustomers.All(c => c.CustomerID != customerID))
                product.InterestedCustomers.Add(new Customer(customerID));

            return productList;
        }
        private Product BuildProduct(Product product, int id, string name, decimal price, Category category, List<Customer> interestedCustomers)
        {
            if (product == null)
            {
                product = new Product
                {
                    ProductID = id,
                    Name = name,
                    Price = price,
                    Category = category,
                    InterestedCustomers = interestedCustomers
                };
            }

            return product;
        }
        private List<Category> BuildCategoryList(List<Category> categoryList, int categoryId, string categoryName)
        {
            if (categoryList.All(c => c.CategoryID != categoryId))
                categoryList.Add(new Category()
                {
                    CategoryID = categoryId,
                    Name = categoryName,
                });
            return categoryList;
        }
        private Category BuildCategory(List<Category> categoryList, int categoryId, string categoryName)
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
    }
}