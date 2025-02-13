using EShop.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Repository.Migrations
{

    public class DataInitializer: IDataInitializer
    {
        private ApplicationDbContext _context;
        public DataInitializer(ApplicationDbContext context) 
        {
            _context = context;
        }
        public void Migrate()
        {
            _context.Database.Migrate();
        }
    }
}
