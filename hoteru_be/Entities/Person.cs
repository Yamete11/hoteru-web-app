﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Person
    {
        [Key]
        public int IdPerson { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string Email { get; set; }

        public int IdHotel { get; set; }

        [Required]
        [ForeignKey(nameof(IdHotel))]
        public Hotel Hotel { get; set; }
    }
}