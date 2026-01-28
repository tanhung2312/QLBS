using Microsoft.EntityFrameworkCore;

namespace QLBS.Models
{
    public class QLBSDbContext : DbContext
    {
        public QLBSDbContext(DbContextOptions<QLBSDbContext> options)
            : base(options)
        {
        }

        #region DbSet

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role_Permission> RolePermissions { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookImage> BookImages { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<FavoriteBook> FavoriteBooks { get; set; }

        public DbSet<DiscountCode> DiscountCodes { get; set; }

        public DbSet<OrderTable> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<BookReceipt> BookReceipts { get; set; }
        public DbSet<BookReceiptDetail> BookReceiptDetails { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<RevenueReport> RevenueReports { get; set; }
        public DbSet<BestSellingBookReport> BestSellingBookReports { get; set; }
        public DbSet<InventoryReport> InventoryReports { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role_Permission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.BookId });

            modelBuilder.Entity<BookReceiptDetail>()
                .HasKey(brd => new { brd.ReceiptId, brd.RowNumber });

            modelBuilder.Entity<FavoriteBook>()
                .HasKey(f => new { f.UserId, f.BookId });

            modelBuilder.Entity<Cart>()
                .HasKey(c => new { c.UserId, c.BookId });

            modelBuilder.Entity<BestSellingBookReport>()
                .HasIndex(b => new { b.Month, b.Year, b.BookId })
                .IsUnique();

            modelBuilder.Entity<InventoryReport>()
                .HasIndex(i => new { i.UpdateDate, i.BookId })
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasOne(a => a.UserProfile)
                .WithOne(u => u.Account)
                .HasForeignKey<UserProfile>(u => u.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookReceipt>()
                .HasOne(br => br.User)
                .WithMany()
                .HasForeignKey(br => br.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookReceipt>()
                .HasOne(br => br.ConfirmedBy)
                .WithMany()
                .HasForeignKey(br => br.ConfirmedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderTable>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BookReceipt>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BookReceiptDetail>()
                .Property(b => b.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaidAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DiscountCode>()
                .Property(d => d.DiscountValue)
                .HasColumnType("decimal(18,2)");

        }
    }
}
