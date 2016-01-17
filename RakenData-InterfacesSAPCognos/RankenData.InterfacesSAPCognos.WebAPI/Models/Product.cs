using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RankenData.InterfacesSAPCognos.WebAPI.Models
{
    public class Product
    {
        //public string Description { get; set; }
        //public decimal Price { get; set; }
        //public string ProductCode { get; set; }
        //public int ProductId { get; set; }
        //public string ProductName { get; set; }
        

        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}