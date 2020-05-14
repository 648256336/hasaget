using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Docm.Business
{
    public class Repository
    {
        private readonly DbProviderFactory _factory;
        private readonly IConfiguration Configuration;
        public Repository(DbProviderFactory factory, IConfiguration configuration)
        {
            _factory = factory;
            Configuration = configuration;
        }

        public async Task<T> DbAsync<T>(Func<IDbConnection, Task<T>> action)
        {
            using (IDbConnection conn= _factory.CreateConnection())
            {
                conn.ConnectionString = Configuration["ConnectionString:DefaultConnection"];
                conn.Open();
                return await action(conn);
            }
        }
    }
}
