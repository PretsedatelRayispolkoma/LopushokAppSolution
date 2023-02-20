//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LopushokApp.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Material
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Material()
        {
            this.ProductMaterial = new HashSet<ProductMaterial>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> MaterialTypeId { get; set; }
        public Nullable<int> QuantityInPackage { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> QuantityInStock { get; set; }
        public Nullable<int> MinRemainder { get; set; }
        public Nullable<decimal> Price { get; set; }
    
        public virtual MaterialType MaterialType { get; set; }
        public virtual Unit Unit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductMaterial> ProductMaterial { get; set; }
    }
}
