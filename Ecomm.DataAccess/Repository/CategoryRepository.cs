﻿using Ecomm.DataAccess.Data;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.DataAccess.Repository
{
    public class CategoryRepository:Repository<Covertype>,ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
