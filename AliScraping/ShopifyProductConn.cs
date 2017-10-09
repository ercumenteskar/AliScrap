namespace WindowsFormsApplication2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShopifyProductConn")]
    public partial class ShopifyProductConn
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string ProductId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string ShopName { get; set; }

        [Required]
        [StringLength(50)]
        public string ShopifyId { get; set; }
    }
}
