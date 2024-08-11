using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OgTech.Core.Entities;

namespace OgTech.Repository.Data
{
    public class ImageDbContext : DbContext
    {
        public ImageDbContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<Image> Images { get; set; }
    }


}
