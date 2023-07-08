using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WinUI3.Data
{
public class WinUIContext : DbContext
{
        public WinUIContext(DbContextOptions<WinUIContext> options)
            : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "WinUITest.db");
        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlite($"Data Source={DbPath}");
//        }

        public DbSet<EntityFirst> EntityFirsts
        {
            get;
            set;
        }
        public object DbPath
        {
            get;
        }
    }

}
