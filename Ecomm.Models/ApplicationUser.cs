using Microsoft.AspNetCore.Identity;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Display(Name ="Street Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        [Display (Name ="Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name ="Company")]
        public int? companyId { get; set; }
        [ForeignKey("companyId")]
        public Company company { get; set; }
        [NotMapped]
        public string Role {  get; set; }
        public ICollection<Address> Addresses { get; set; }
    }

}

