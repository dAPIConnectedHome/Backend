using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAPISmartHomeMQTT_BackendServer.Models
{
    public partial class SmartHomeDBContext : DbContext
    {
        public SmartHomeDBContext()
        {

        }

        public SmartHomeDBContext(DbContextOptions<SmartHomeDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MqttclientTypes> MqttclientTypes { get; set; }
        public virtual DbSet<Mqttclients> Mqttclients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=192.168.0.10;port=3306;database=SmartHomeDB;user=BackendUser;password=qawsed", x => x.ServerVersion("10.3.22-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MqttclientTypes>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PRIMARY");

                entity.ToTable("MQTTClientTypes");

                entity.Property(e => e.TypeId)
                    .HasColumnName("TypeID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Direction)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Mode)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.RangeMax).HasColumnType("int(11)");

                entity.Property(e => e.RangeMin).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Mqttclients>(entity =>
            {
                entity.HasKey(e => e.ClientId)
                    .HasName("PRIMARY");

                entity.ToTable("MQTTClients");

                entity.HasIndex(e => e.Typeid)
                    .HasName("FK_MQTTClient_MQTTClientTypes");

                entity.Property(e => e.ClientId)
                    .HasColumnName("ClientID")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'A1000'")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.GroupId)
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'-1'")
                    .HasColumnName("GroupID")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'NewClient'")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Room)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Topic)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Typeid).HasColumnType("int(11)");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Mqttclients)
                    .HasForeignKey(d => d.Typeid)
                    .HasConstraintName("FK_MQTTClient_MQTTClientTypes");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
