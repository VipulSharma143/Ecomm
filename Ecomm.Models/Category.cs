﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.Models
{
    public class Covertype
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }   
    }
}