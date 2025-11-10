using Microsoft.EntityFrameworkCore;
using inmobiliariaApi.Models;
namespace inmobiliariaApi
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Propietario> propietario { get; set; }
        public DbSet<Inmueble> inmueble { get; set; }
        public DbSet<Inquilino> inquilino { get; set; }
        public DbSet<Contrato> contrato { get; set; }
        public DbSet<Pago> pago { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("pago");
                entity.HasKey(p => p.idPago);
                entity.Property(p => p.idPago).HasColumnName("id");
                entity.Property(p => p.idContrato).HasColumnName("id_contrato");
                entity.Property(p => p.nroPago).HasColumnName("nro_pago");
                entity.Property(p => p.fechaPago).HasColumnName("fecha_pago");
                entity.Property(p => p.concepto).HasColumnName("concepto");
                entity.Property(p => p.importe).HasColumnName("importe");

                //  modelBuilder.Entity<Pago>()
                // .Property(p => p.estado)
                // .HasColumnName("estado")
                // .HasConversion<int>();
                entity.Property(p => p.estado)
                .HasColumnName("estado")
                .HasConversion<string>();
            });
            modelBuilder.Entity<Contrato>(entity =>
            {
                entity.ToTable("contrato");
                entity.HasKey(i => i.idContrato);
                entity.Property(i => i.idContrato).HasColumnName("id");
                entity.Property(c => c.idInmueble).HasColumnName("id_inmueble");
                entity.Property(c => c.idInquilino).HasColumnName("id_inquilino");

                entity.HasOne(c => c.inmueble)
                  .WithMany()
                  .HasForeignKey(c => c.idInmueble)
                  .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(c => c.inquilino)
                    .WithMany()
                    .HasForeignKey(c => c.idInquilino)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Inmueble>(entity =>
            {
                entity.ToTable("inmueble");
                entity.HasKey(i => i.id);
                entity.Property(i => i.id).HasColumnName("id");
                 entity.Property(i => i.idPropietario).HasColumnName("id_propietario");
            });
            modelBuilder.Entity<Inquilino>(entity =>
            {
                entity.ToTable("inquilino");
                entity.HasKey(i => i.idInquilino);
                entity.Property(i => i.idInquilino).HasColumnName("id");
            });
        }
    }
}