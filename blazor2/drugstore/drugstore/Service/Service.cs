using Drugstore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace Drugstore.Service;
public class PharmacyService(DatabaseContext context)
{
    private readonly DatabaseContext _context = context;
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<List<pharmacy>> GetPharmaciesAsync()
    {
        return await _context.pharmacy.ToListAsync();
    }

    public async Task<pharmacy> GetPharmacyByIdAsync(int id)
    {
        var pharmacy = await _context.pharmacy
            .Include(p => p.PharmacyProducts)
                .ThenInclude(pp => pp.product)
                .ThenInclude(p=>p.packing)
                .ThenInclude(p => p.productpackingpharmas)
            .Include(p => p.PharmacySuppliers)
                .ThenInclude(ps => ps.supplier)
            .FirstOrDefaultAsync(p => p.id == id);

        return pharmacy;
    }
    public async Task RemoveProductAsync(int productId)
    {
        // Найти все записи в pharmacyproduct, связанные с продуктом
        var pharmacyProducts = await _context.pharmacyproduct
            .Where(pp => pp.productid == productId)
            .ToListAsync();

        if (pharmacyProducts != null)
        {
            _context.pharmacyproduct.RemoveRange(pharmacyProducts);
        }

        // Найти все записи в productpacking, связанные с продуктом
        var productPackings = await _context.productpackingpharma
            .Where(pp => pp.productid == productId)
            .ToListAsync();

        if (productPackings != null)
        {
            _context.productpackingpharma.RemoveRange(productPackings);
        }
        var productPackingSuppliers = await _context.productpackingsupplier
        .Where(pps => pps.productid == productId)
        .ToListAsync();

        if (productPackingSuppliers != null)
        {
            _context.productpackingsupplier.RemoveRange(productPackingSuppliers);
        }
        await _context.SaveChangesAsync();
    }
    public async Task<List<product>> GetProductsAsync()
    {
        return await _context.product.ToListAsync();
    }

    public async Task<product> GetProductByIdAsync(int id)
    {
        var product = await _context.product
        .Include(p => p.packing)
        .Include(p => p.supplierproducts) // Включаем навигационное свойство supplierproducts из модели product
        .ThenInclude(sp => sp.supplier) // Включаем навигационное свойство supplier из модели supplierproduct
        .Where(p => p.id == id)
        .FirstOrDefaultAsync();
        return product;
    }

    public async Task<List<supplier>> GetSuppliersByProductIdAsync(int productId)
    {
        var suppliers = await _context.suplierproduct
            .Where(sp => sp.productid == productId)
            .Select(sp => sp.supplier)
            .ToListAsync();

        return suppliers;
    }
    public async Task<List<supplier>> GetSuppliersAsync()
    {
        return await _context.supplier.ToListAsync();
    }

    public async Task<supplier> GetSupplierByIdAsync(int id)
    {
        var supplier = await _context.supplier
            .Include(s => s.supplierproducts)
                .ThenInclude(sp => sp.product)
                .ThenInclude(p=>p.packing)
                    .ThenInclude(p => p.pps)
            .FirstOrDefaultAsync(s => s.id == id);

        return supplier;
    }

    public async Task<List<product>> GetProductsBySupplierIdAsync(int supplierId)
    {
        var products = await _context.suplierproduct
            .Where(sp => sp.supplierid == supplierId)
            .Select(sp => sp.product)
            .ToListAsync();

        return products;
    }

    public async Task<productpackingsupplier> GetPackingSupplierAsync(int productId, int supplierId, int packingId)
    {
        return await _context.productpackingsupplier.FirstOrDefaultAsync(p =>
            p.productid == productId && p.id == packingId);
    }

public async Task AddToPharmacyProductAsync(int productId, int pharmacyId)
{
    var pharmacyProduct = await _context.pharmacyproduct.FirstOrDefaultAsync(p =>
        p.productid == productId && p.pharmacyid == pharmacyId);

        if (pharmacyProduct == null || (pharmacyProduct.pharmacyid != pharmacyId && pharmacyProduct.productid != productId))
        {
            pharmacyProduct = new pharmacyproduct
            {
                productid = productId,
                pharmacyid = pharmacyId
            };
            _context.pharmacyproduct.Add(pharmacyProduct);
        }

        await _context.SaveChangesAsync();
}
    public async Task AddToPharmacySuppliersAsync(int pharmacyId, int supplierId)
    {
        var pharmacySupplier = await _context.pharmacysupplier.FirstOrDefaultAsync(p=>
        p.pharmacyid==pharmacyId && p.supplierid==supplierId);
        if (pharmacySupplier == null || (pharmacySupplier.pharmacyid != pharmacyId && pharmacySupplier.supplierid != supplierId))
        {
            pharmacySupplier = new pharmacysupplier
            {
                pharmacyid = pharmacyId,
                supplierid = supplierId
            };
            _context.pharmacysupplier.Add(pharmacySupplier);
        }

        await _context.SaveChangesAsync();
    }
    public async Task AddToPharmacyPackingsAsync(int productId, int packingId, int quantity)
    {
        var pharmacyPacking = await _context.productpackingpharma.FirstOrDefaultAsync(p =>
    p.productid == productId && p.packingid == packingId);

        if (pharmacyPacking != null)
        {
            pharmacyPacking.quantity += quantity;
        }
        else
        {
            pharmacyPacking = new productpackingpharma
            {
                quantity = quantity,
                productid = productId,
                packingid = packingId
            };
            _context.productpackingpharma.Add(pharmacyPacking);
        }

        await _context.SaveChangesAsync();
    }

    }