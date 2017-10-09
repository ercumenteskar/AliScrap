namespace WindowsFormsApplication2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductImages
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string ProductId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Position { get; set; }

        [Required]
        public string Src { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        [StringLength(32)]
        public string Hash { get; set; }

        public virtual Product Product { get; set; }
    }
}
