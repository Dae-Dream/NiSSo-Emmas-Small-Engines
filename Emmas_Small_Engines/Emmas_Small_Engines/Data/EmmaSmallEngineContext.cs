using Emmas_Small_Engines.Models;
using Microsoft.EntityFrameworkCore;

/*Written by Jia Ni Zhao
 On Feb 17, 2023*/

/*Updated by Jia Ni Zhao
 On Mar 21, 2023*/


namespace Emmas_Small_Engines.Data
{
    public class EmmaSmallEngineContext : DbContext
    {
        public EmmaSmallEngineContext(DbContextOptions<EmmaSmallEngineContext> options)
            : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }
        public DbSet<OrderRequest> OrderRequests { get; set; }
        public DbSet<OrderRequest_Inventory> OrderRequest_Inventories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<EmpLogin> EmpLogins { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<EmployeePosition> EmployeePositions { get; set; }
        public DbSet<SalesReport> SalesReports { get; set; }
        public DbSet<HourlyReport> HourlyReports { get; set; }
        public DbSet<COGSReport> COGSReports { get; set; }
        public DbSet<ReportData> ReportDatas { get; set; }
        public DbSet<SalesReportItem> SalesReportItems { get; set; }
        public DbSet<SalesReportEmp> SalesReportEmps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //many to many intersection
            modelBuilder.Entity<EmployeePosition>()
                .HasKey(t => new { t.EmployeeID, t.Title });//not sure about the title

            //many to many intersection
            modelBuilder.Entity<InvoiceLine>()
                .HasKey(t => new { t.InventoryID, t.InvoiceID });

            modelBuilder.Entity<OrderRequest_Inventory>()
                .HasKey(t => new { t.OrderRequestID, t.InventoryID });

            modelBuilder.Entity<InvoicePayment>()
                .HasKey(t => new { t.PaymentID, t.InvoiceID });

            //prevent cascade delete from position to employee Position
            modelBuilder.Entity<EmployeePosition>()
                .HasOne(ep => ep.Position)
                .WithMany(p => p.EmployeePositions)
                .HasForeignKey(ep => ep.Title)//not sure about this
                .OnDelete(DeleteBehavior.Restrict);

            //prevent cascade delete from Invoice to InvoiceLine
            modelBuilder.Entity<Invoice>()
                .HasMany<InvoiceLine>(s=>s.InvoiceLines)
                .WithOne(s=>s.Invoice)
                .HasForeignKey(s=>s.InvoiceID)
                .OnDelete(DeleteBehavior.Restrict);

            //prevent cascade delete from Inventory to InvoiceLine
            modelBuilder.Entity<Inventory>()
                .HasMany<InvoiceLine>(s => s.InvoiceLines)
                .WithOne(s => s.Inventory)
                .HasForeignKey(s => s.InventoryID)
                .OnDelete(DeleteBehavior.Restrict);

            //prevent cascade delete from supplier to price
            modelBuilder.Entity<Supplier>()
                .HasMany<Price>(s=>s.Prices)
                .WithOne(s=>s.Supplier)
                .HasForeignKey(s=>s.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);

            //prevent cascade delete from employee to invoice
            modelBuilder.Entity<Employee>()
                .HasMany<Invoice>(s => s.Invoices)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            //prevent cascade delete from inventory to OrderRequest
            //which allows cascade delete from OrderRequest to OrderRequest_Inventory
            modelBuilder.Entity<OrderRequest_Inventory>()
                .HasOne(ori => ori.Inventory)
                .WithMany(s => s.OrderRequest_Inventories)
                .HasForeignKey(ori => ori.InventoryID)
                .OnDelete(DeleteBehavior.Restrict);

            ////prevent cascade delete from invoice to payment
            //which allows cascade delete from invoice to invociePayment
            modelBuilder.Entity<InvoicePayment>()
                .HasOne(ip => ip.Payment)
                .WithMany(i => i.InvoicePayments)
                .HasForeignKey(ip => ip.PaymentID)
                .OnDelete(DeleteBehavior.Restrict);

            //unique email and phone number
            modelBuilder.Entity<Customer>()
                .HasIndex(p => p.Phone)
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(p => p.Phone)
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(p => p.Email)
                .IsUnique();
        }
    }
}
