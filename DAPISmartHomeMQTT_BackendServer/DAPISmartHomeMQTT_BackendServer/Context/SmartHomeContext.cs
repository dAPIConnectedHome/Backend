using DAPISmartHomeMQTT_BackendServer.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAPISmartHomeMQTT_BackendServer.Context
{
    public class SmartHomeContext : DbContext
    {
        public SmartHomeContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost; Database=SmarHomeDB; User=BackendUser; Password=qawsed", mysqlOptions => mysqlOptions.ServerVersion(new ServerVersion(new Version(10,3,22), ServerType.MariaDb))); 
        }
    }
}
