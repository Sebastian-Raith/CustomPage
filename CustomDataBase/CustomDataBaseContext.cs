using Microsoft.EntityFrameworkCore;

namespace CustomDataBase
{
    public class CustomDataBaseContext : DbContext
    {
        public DbSet<Shift> Shifts { get; set; }
        public CustomDataBaseContext(DbContextOptions<CustomDataBaseContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.Date).IsRequired();
                entity.Property(s => s.ShiftName);
                entity.Property(s => s.StartTime);
                entity.Property(s => s.EndTime);
                entity.Property(s => s.Duration);
                entity.Property(s => s.Activity);
                entity.Property(s => s.Department);
                entity.Property(s => s.AllocationInfo);
                entity.Property(s => s.DutyType);
                entity.Property(s => s.Info);
            });
        }
    }
}
