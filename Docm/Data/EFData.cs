using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Docm.Data
{
    public class EFData : DbContext
    {
        public DbSet<workusertoken> workusertokens;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Test3;Persist Security Info=True;User ID=sa;Password=123456");
        }
    }

    public class workusertoken
    {
        public string token { get; set; }
        public string userno { get; set; }
        public DateTime logtime{ get; set; }
        public string logip { get; set; }
    }
}
