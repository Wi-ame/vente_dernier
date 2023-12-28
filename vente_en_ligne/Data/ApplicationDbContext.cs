using Microsoft.EntityFrameworkCore;
using vente_en_ligne.Models;

namespace vente_en_ligne.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Proprietaire> Proprietaires { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Favori> Favorites { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Utilisateur>().HasKey(u => u.ID);
            modelBuilder.Entity<Favori>().HasKey(f => f.Id_fav);
            modelBuilder.Entity<Panier>().HasKey(c => c.Id);
            modelBuilder.Entity<Ban>().HasKey(b => b.Id_Ban);

            modelBuilder.Entity<Admin>().HasKey(u => u.CIN);

            modelBuilder.Entity<Proprietaire>().HasKey(u => u.INterID);

            modelBuilder.Entity<Categories>().HasKey(c => c.CategorieID);
            base.OnModelCreating(modelBuilder);
        }
  
    }
}
