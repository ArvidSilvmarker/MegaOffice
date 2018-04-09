using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MegaOffice
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }   
        public string Phone { get; set; }
        public int Kundnummer { get; set; }

        public Customer(string firstName, string lastName, string email, string phone) : this(0, firstName, lastName,
            email, phone)
        {
        }


        public Customer(int id, string firstName, string lastName, string email, string phone)
        {
            Kundnummer = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
        }
    }
}
