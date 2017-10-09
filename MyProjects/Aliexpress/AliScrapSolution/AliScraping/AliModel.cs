namespace WindowsFormsApplication2
{
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class AliModel : DbContext
  {
    public AliModel()
        : base("name=AliModel")
    {
    }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<ProductProperty> ProductProperties { get; set; }
    public virtual DbSet<ProductVariant> ProductVariants { get; set; }
    public virtual DbSet<PropertyKey> PropertyKeys { get; set; }
    public virtual DbSet<PropertyValue> PropertyValues { get; set; }
    public virtual DbSet<ShopifyCollectionConn> ShopifyCollectionConns { get; set; }
    public virtual DbSet<ShopifyProductConn> ShopifyProductConns { get; set; }
    public virtual DbSet<Store> Stores { get; set; }
    public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    public virtual DbSet<Unit> Units { get; set; }
    public virtual DbSet<VariantKey> VariantKeys { get; set; }
    public virtual DbSet<VariantValue> VariantValues { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Category>()
          .HasMany(e => e.Products)
          .WithOptional(e => e.Category)
          .WillCascadeOnDelete();

      modelBuilder.Entity<Product>()
          .Property(e => e.Id)
          .IsUnicode(false);

      modelBuilder.Entity<Product>()
          .Property(e => e.Rate)
          .HasPrecision(5, 2);

      modelBuilder.Entity<Product>()
          .Property(e => e.Weight)
          .HasPrecision(5, 2);

      modelBuilder.Entity<Product>()
          .Property(e => e.VariantHash)
          .IsUnicode(false);

      modelBuilder.Entity<ProductImage>()
          .Property(e => e.ProductId)
          .IsUnicode(false);

      modelBuilder.Entity<ProductImage>()
          .Property(e => e.Hash)
          .IsUnicode(false);

      modelBuilder.Entity<ProductProperty>()
          .Property(e => e.ProductId)
          .IsUnicode(false);

      modelBuilder.Entity<ProductVariant>()
          .Property(e => e.ProductId)
          .IsUnicode(false);

      modelBuilder.Entity<ProductVariant>()
          .Property(e => e.AliSKU)
          .IsUnicode(false);

      modelBuilder.Entity<ProductVariant>()
          .Property(e => e.OldPrice)
          .HasPrecision(9, 2);

      modelBuilder.Entity<ProductVariant>()
          .Property(e => e.Price)
          .HasPrecision(9, 2);

      modelBuilder.Entity<ProductVariant>()
          .Property(e => e.Discount)
          .HasPrecision(26, 12);

      modelBuilder.Entity<PropertyKey>()
          .HasMany(e => e.ProductProperties)
          .WithRequired(e => e.PropertyKey)
          .HasForeignKey(e => e.KId);

      modelBuilder.Entity<PropertyKey>()
          .HasMany(e => e.PropertyValues)
          .WithRequired(e => e.PropertyKey)
          .HasForeignKey(e => e.KeyId);

      modelBuilder.Entity<ShopifyCollectionConn>()
          .Property(e => e.ShopName)
          .IsUnicode(false);

      modelBuilder.Entity<ShopifyCollectionConn>()
          .Property(e => e.CollectionId)
          .IsUnicode(false);

      modelBuilder.Entity<ShopifyProductConn>()
          .Property(e => e.ProductId)
          .IsUnicode(false);

      modelBuilder.Entity<ShopifyProductConn>()
          .Property(e => e.ShopName)
          .IsUnicode(false);

      modelBuilder.Entity<ShopifyProductConn>()
          .Property(e => e.ShopifyId)
          .IsUnicode(false);

      modelBuilder.Entity<Store>()
          .Property(e => e.Rating)
          .HasPrecision(5, 2);

      modelBuilder.Entity<Store>()
          .HasMany(e => e.Products)
          .WithOptional(e => e.Store)
          .WillCascadeOnDelete();

      modelBuilder.Entity<VariantKey>()
          .HasMany(e => e.VariantValues)
          .WithRequired(e => e.VariantKey)
          .HasForeignKey(e => e.KeyId);
    }
  }
}
