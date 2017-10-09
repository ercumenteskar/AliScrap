namespace WindowsFormsApplication2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ProductImages = new HashSet<ProductImage>();
            ProductProperties = new HashSet<ProductProperty>();
            ProductVariants = new HashSet<ProductVariant>();
        }

        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string DescHtml { get; set; }

        public int? StoreId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Rate { get; set; }

        public int RateCount { get; set; }

        public int OrderCount { get; set; }

        public DateTime? DiscountTime { get; set; }

        public int? CategoryId { get; set; }

        public int UnitId { get; set; }

        public string Size { get; set; }

        public decimal Weight { get; set; }

        public int WeightUnit { get; set; }

        [Required]
        public string Link { get; set; }

        public string MainImg { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public int IdentId { get; set; }

        [StringLength(32)]
        public string VariantHash { get; set; }

        public DateTime? LastCheckDT { get; set; }

        [StringLength(50)]
        public string PriceRange { get; set; }

        public int? Last6MOrderCount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShopifyId { get; set; }

        public virtual Category Category { get; set; }

        public virtual Store Store { get; set; }

        public virtual Unit Unit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductProperty> ProductProperties { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
