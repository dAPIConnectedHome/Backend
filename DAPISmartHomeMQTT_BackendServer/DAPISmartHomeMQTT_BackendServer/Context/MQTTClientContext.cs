using DAPISmartHomeMQTT_BackendServer.Models;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAPISmartHomeMQTT_BackendServer.Context
{
    public class MQTTClientContext : DbContext
    {
        public MQTTClientContext(DbContextOptions<MQTTClientContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MQTTClient>().ToTable("MQTTClients");
            modelBuilder.Entity<MQTTClient>().HasKey(b => new { b.ClientID });
            modelBuilder.Entity<MQTTClient>().Property(b => b.ClientID).HasColumnName("MQTTCLIENT");
            modelBuilder.Entity<MQTTClient>().Property(b => b.Topic).HasColumnName("TOPIC");
            modelBuilder.Entity<MQTTClient>().Property(b => b.Room).HasColumnName("ROOM");
            modelBuilder.Entity<MQTTClient>().HasOne<MQTTClientType>(x => x.type);
            modelBuilder.Entity<MQTTClient>().Property(b => b.type).HasColumnName("typeid");
        }

        public DbSet<MQTTClient> MQTTClients { get; set; }
    }
}
