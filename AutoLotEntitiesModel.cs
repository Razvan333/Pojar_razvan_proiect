namespace AutoLotModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AutoLotEntitiesModel : DbContext
    {
        public AutoLotEntitiesModel()
            : base("name=AutoLotEntitiesModel")
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Masina> Masinas { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
