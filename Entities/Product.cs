﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MansorySupplyHub.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public double Price { get; set; }
        public string Image { get; set; } 
        public int CategoryId { get; set; } 
        public Category Category { get; set; }
        public int ApplicationTypeId { get; set; }
        public ApplicationType ApplicationType { get; set; }
    }
}