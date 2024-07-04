using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.Models.ViewModels
{
    public class ShoppingCartVM
    {
       public IEnumerable<ShoppingCart> ListCart { get; set; }
      //  public List<ShoppingCart>ListCarts { get; set; }
        public ApplicationUser User { get; set; } 
        public OrderHeader OrderHeader { get; set; }
        //public OrderDetail OrderDetail { get; set; }
        //public Product Product { get; set; }
        public IEnumerable<ShoppingCart> CartItems { get; set; }
        public AddressViewModel Address { get; set; }

    }
}
