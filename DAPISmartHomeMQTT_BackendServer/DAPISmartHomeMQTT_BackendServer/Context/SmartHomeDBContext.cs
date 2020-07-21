using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Diagnostics;
using MQTTnet.Server;

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
                optionsBuilder.UseMySql("Mariadbaccess", x => x.ServerVersion("10.3.22-mariadb"));
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

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)")
                    .HasColumnName("Name")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

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

                entity.Property(e => e.CurrentValue)
                    .HasColumnName("CurrentValue")
                    .HasColumnType("int(11)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public class ClientConnection
    {
        public string Clientid;
        private IMqttClient _mqttClient;

        public ClientConnection(string clientid, IMqttClientOptions mqttClientOptions)
        {
            Clientid = clientid;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            _mqttClient.UseApplicationMessageReceivedHandler(Receivehandler);
            _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            _mqttClient.UseConnectedHandler(async e =>
            {
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("shlogo/" + Clientid + "/").Build());
            });
        }

        /// <summary>
        /// writes changes to db when value changes at sensor or actor
        /// </summary>
        private async void Receivehandler(MqttApplicationMessageReceivedEventArgs e)
        {
            string[] strings = e.ApplicationMessage.Topic.Split('/');
            int index = strings.Length-2;

            using (var context =  new SmartHomeDBContext())
            {
                var tempcoll = context.Mqttclients.Where(x => x.ClientId.Equals(strings.ElementAt(index)));
                if(tempcoll.Any())
                {
                    Mqttclients changedelement = tempcoll.First();

                    changedelement.CurrentValue =  int.Parse(Convert.ToChar(e.ApplicationMessage.Payload.Last()).ToString());
                    context.Entry(changedelement).State = EntityState.Modified;
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch
                    {
                        throw new Exception("Receivhandler messed up saving");
                    }

                }
            }
        }

        /// <summary>
        /// sends data to actor or senrsor
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            while (!_mqttClient.IsConnected) ;
            var mqttmessage = new MqttApplicationMessageBuilder()
                .WithTopic("shlogo/" + Clientid + "/set/")
                .WithPayload(message)
                //.WithExactlyOnceQoS()
                .WithAtMostOnceQoS()
                .WithRetainFlag(false)
                .Build();
            _mqttClient.PublishAsync(mqttmessage, CancellationToken.None);
        }
    }
}
