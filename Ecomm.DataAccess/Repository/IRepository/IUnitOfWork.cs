using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        ISPCall SPCall { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IApplicationUser ApplicationUser { get; }
        IShoppingCartRepo ShoppingCart { get; }
        IOrderHeaderRepo OrderHeader { get; }
        IOrderDetailRepo OrderDetailRepo { get; }
        IAddress address { get; }
        void save();
    }
}
