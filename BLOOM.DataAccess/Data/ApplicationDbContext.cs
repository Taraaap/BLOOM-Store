using BLOOM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BLOOM.DataAccess.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Floral",DisplayOrder=1 },
                new Category { Id = 2, Name = "Woody", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Fresh ", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Product>().HasData(
              new Product
              {
                  Id = 1,
                  Title = "Heart of the Jungle",
                  Description = "A thrilling, untamed blend of crushed green leaves, wild moss, and exotic wet flora deep in the heart of an uncharted rainforest.",
                  Brand= "Aura Fragrances",
                  SKU = "JNG7777770001",
                  ListPrice = 45,
                  Price = 40,
                  Price50 = 35,
                  Price100 = 30,
                  CategoryId = 1 // Floral (Exotic & Wild)
              },
              new Product
                {
                    Id = 2,
                    Title = "Liquid Gold & Time",
                    Brand = "Maison Luxe",
                    Description = "An upscale, crisp composition of sparkling bergamot, metallic silver sage, and clean aldehydes. The ultimate scent of ambition and luxury.",
                    SKU = "MNT8888880001",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 45,
                    Price100 = 40,
                    CategoryId = 3,
                    ImageUrl = ""
                },
                    new Product
                    {
                        Id = 3,
                        Title = "Secret of the Lake",
                        Brand = "Aura Fragrances",
                        Description = "A mysterious, aquatic floral scent. Deep water lilies, morning mist, and wet pebbles layered beneath a cool, crisp marine breeze.",
                        SKU = "LKE9999990001",
                        ListPrice = 35,
                        Price = 30,
                        Price50 = 28,
                        Price100 = 25,
                        CategoryId = 1,
        
                    },
                    new Product
                    {
                        Id = 4,
                        Title = "Cosmic Eclipse",
                        Brand = "Bloom Signature",
                        Description = "A captivating journey through the dark cosmos, blending smoky cedarwood, meteorite dust accord, and warm, starry amber notes.",
                        SKU = "SPC1010100001",
                        ListPrice = 65,
                        Price = 60,
                        Price50 = 55,
                        Price100 = 50,
                        CategoryId = 2,
        
                    },
                    new Product
                    {
                        Id = 5,
                        Title = "Forest at Dawn",
                        Brand = "Aura Fragrances",
                        Description = "At the edge of an ancient forest, dawn breaks with a clean splash of morning dew, refreshing eucalyptus, and bright, sunlit lemons.",
                        SKU = "FRD1111110001",
                        ListPrice = 30,
                        Price = 27,
                        Price50 = 24,
                        Price100 = 20,
                        CategoryId = 3,
        
                    },
                    new Product
                    {
                        Id = 6,
                        Title = "The Whispering Grove",
                        Brand = "Bloom Signature",
                        Description = "Deep within a forgotten grove, velvet roses hold secrets of the past. A hauntingly beautiful powdery floral scent with a legacy trail.",
                        SKU = "WGR1212120001",
                        ListPrice = 42,
                        Price = 38,
                        Price50 = 34,
                        Price100 = 30,
                        CategoryId = 1,
        
                    },
                    new Product
                    {
                        Id = 7,
                        Title = "The Forgotten Cipher",
                        Brand = "Maison Luxe",
                        Description = "A brilliant, complex formulation. Sharp black pepper and dark spices give way to a centuries-old base of heavy, resinous oud wood.",
                        SKU = "CIP1313130001",
                        ListPrice = 50,
                        Price = 45,
                        Price50 = 40,
                        Price100 = 35,
                        CategoryId = 2,
        
                    },
                    new Product
                    {
                        Id = 8,
                        Title = "The Silent Orchard",
                        Brand = "Aura Fragrances",
                        Description = "When quiet falls over the trees, a crisp, clean scent remains. A crisp, minimalist melody of green apple skin, white pear, and breezy air.",
                        SKU = "ORC1414140001",
                        ListPrice = 38,
                        Price = 34,
                        Price50 = 30,
                        Price100 = 26,
                        CategoryId = 3,
        
                    }
           );
        }
    }
}
