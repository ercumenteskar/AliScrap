namespace WindowsFormsApplication2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShopifyCollectionConn")]
    public partial class ShopifyCollectionConn
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string ShopName { get; set; }

        [Required]
        [StringLength(50)]
        public string CollectionId { get; set; }
    }
}
