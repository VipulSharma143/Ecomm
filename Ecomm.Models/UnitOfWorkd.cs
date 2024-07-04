//using Ecomm.DataAccess.Data;
//using Ecomm.DataAccess.Repository.IRepository;
//using Ecomm.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Ecomm.DataAccess.Repository
//{
//    public class UnitOfWorkd : IUnitOfWork
//    {
//        private readonly ApplicationDbContext _context;
//        public UnitOfWorkd(ApplicationDbContext context)
//        {
//            _context = context;
//            Category=new CategoryRepository(_context);
//            CoverType = new CoverTypeRepository(_context);  
//           SPCall=new SPCall(_context);
//            Product=new ProductRepository(_context);
//            company=new CompanyRepository(_context);
//        }
//        public ICategoryRepository Category { private set; get; }

//        public ICoverTypeRepository CoverType {private set; get; }

//        public ISPCall  SPCall { private set; get; }
//        public IProductRepository Product { private set; get; }
//        public ICompanyRepository company { private set; get; }

//        public void save()
//        {
//            _context.SaveChanges();
//        }   
//    }
//}
