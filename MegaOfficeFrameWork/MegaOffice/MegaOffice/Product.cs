using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MegaOffice
{
    public class Product
    {
        public int ProductID { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
        public List<Customer> InterestedCustomers { get; set; }
        public string Name { get; set; }

        public Product()
        {
            
        }

        public Product(int id)
        {
            ProductID = id;
        }

        public Product(int id, string name)
        {
            ProductID = id;
            Name = name;
        }

        public Product(int id, string name, decimal price, Category category, List<Customer> interestedCustomers)
        {
            ProductID = id;
            Name = name;
            Price = price;
            Category = category;
            InterestedCustomers = interestedCustomers;
        }
    }
}