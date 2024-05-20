using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Drugstore.Data;

public class DatabaseContext : DbContext
{
    public DbSet<pharmacy> pharmacy { get; set; }
    public DbSet<product> product { get; set; }
    public DbSet<supplier> supplier { get; set; }
    public DbSet<pharmacysupplier> pharmacysupplier { get; set; }
    public DbSet<pharmacyproduct> pharmacyproduct { get; set; }
    public DbSet <suplierproduct> suplierproduct { get; set; }
    public DbSet<productpackingpharma> productpackingpharma { get; set; }
    public DbSet<productpacking> productpacking { get; set; }
    public DbSet<productpackingsupplier> productpackingsupplier { get; set; }

    public DatabaseContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=Roflanxlebalo;Database=pharma;Port=5432");
    }
}
public class pharmacy //для аптек
{
    [Key]
    public int id { get; set; }
    public string name { get; set; }
    public DateTime date { get; set; }
    public List<pharmacysupplier>? PharmacySuppliers { get; set; }
    public List<pharmacyproduct>? PharmacyProducts { get; set; }
}

public class product //для продуктов
{
    [Key]
    public int id { get; set; }
    public string name { get; set; }
    public List<productpackingpharma> packingPharma { get; set; }
    public decimal cost { get; set; }
    public int quantity { get; set; }
    public DateTime expirationdate { get; set; }
    public int favouritesupplierid { get; set; }
    public supplier favouritesupplier { get; set; }
    public List<suplierproduct> supplierproducts { get; set; }
    public List<productpacking> packing { get; set; }
    public List<productpackingsupplier> packingSupplier { get; set; }


}
public class productpackingpharma //массовка для аптеки
{
    [Key]
    public int id { get; set; }
    public int productid { get; set; }
    public int packingid { get; set; }
    public int quantity { get; set; }
    public product product { get; set; }
    public productpacking packing { get; set; }
}

public class supplier //поставщики
{
    [Key]
    public int id { get; set; }
    public string name { get; set; }
    public List<suplierproduct> supplierproducts { get; set; }
}
public class pharmacysupplier // Поставщик для аптеки
{
    [Key]
    public int id { get; set; }
    public int pharmacyid { get; set; }
    public int supplierid { get; set; }
    public pharmacy pharmacy { get; set; } 
    public supplier supplier { get; set; }
}

public class pharmacyproduct //Товары в аптеке
{
    [Key]
    public int id { get; set; }
    public int pharmacyid { get; set; }
    public int productid { get; set; }
    public pharmacy Pharmacy { get; set; } 
    public product product { get; set; }
}
public class suplierproduct //Товары поставщика 
{
    [Key]
    public int id { get; set; }
    public int supplierid { get; set; }
    public int productid { get; set; }
    public supplier supplier { get; set; }
    public product product { get; set; }
}
public class productpackingsupplier //Массовка для товаров поставщика
{
    [Key]
    public int id { get; set; }
    public int productid { get; set; }
    public int packingid { get; set; }
    public int quantity { get; set; }
    public product product { get; set; }
    public productpacking packing { get; set; }
}
public class productpacking //Возможная массовка товара
{
    [Key]
    public int id { get; set; }
    public int productid { get; set; }
    public string packing { get; set; }
    public product product { get; set; }
    public List<productpackingpharma> productpackingpharmas { get; set;  }
    public List<productpackingsupplier> pps { get; set; }
}