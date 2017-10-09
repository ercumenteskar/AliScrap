namespace WindowsFormsApplication2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductVariant")]
    public partial class ProductVariant
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string ProductId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Var1Value { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Var2Value { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Var3Value { get; set; }

        public int? Var1Key { get; set; }

        public int? Var2Key { get; set; }

        public int? Var3Key { get; set; }

        public bool isActive { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(143)]
        public string AliSKU { get; set; }

        public string imgThumb { get; set; }

        public string imgBig { get; set; }

        [Column(TypeName = "numeric")]
        public decimal OldPrice { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Price { get; set; }

        public int Inventory { get; set; }

        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? Discount { get; set; }

        public virtual Product Product { get; set; }
    }
}
