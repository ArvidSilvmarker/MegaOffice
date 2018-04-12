using System;
using System.Collections.Generic;
using System.Text;

namespace MegaOffice
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Phone { get; set; }
        public int CustomerID { get; set; }
        public List<Product> InterestingProducts { get; set; }

        public Customer(string firstName, string lastName, string email, List<string> phone, List<Product> products) : this(0, firstName, lastName,
            email, phone, products)
        {
        }

        public Customer(string firstName, string lastName, string email, List<string> phone): this(0, firstName, lastName,
            email, phone, new List<Product>())
        {
        

        }

        public Customer(int id, string firstName, string lastName, string email) : this (id, firstName, lastName, email, new List<string>(), new List<Product>())
        {

        }

        public Customer()
        {
            
        }

        public Customer(int id)
        {
            CustomerID = id;
        }
        public Customer(int id, string firstName, string lastName, string email, List<string> phone, List<Product> interestingProducts)
        {
            CustomerID = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            InterestingProducts = interestingProducts;
        }
    }
}