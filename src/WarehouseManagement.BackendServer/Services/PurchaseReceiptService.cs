using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.PurchaseReceipts;

namespace WarehouseManagement.BackendServer.Services
{
    public class PurchaseReceiptService : IPurchaseReceiptService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseReceiptService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseReceiptResponse> CreateAsync(PurchaseReceiptRequest request)
        {
            // validate supplier
            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            if (supplier == null || supplier.IsDeleted)
                throw new ArgumentException("Supplier not found");

            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException("Purchase must contain at least one item");

            var date = request.ReceiptDate == default ? DateTime.UtcNow.Date : request.ReceiptDate.Date;

            // generate receipt code PO-YYYYMMDD-###
            var ym = date.ToString("yyyyMMdd");
            var countToday = await _context.Purchases.CountAsync(p => p.CreateDate.Date == date);
            var sequence = countToday + 1;
            var receiptCode = $"PO-{ym}-{sequence:000}";

            using var trx = await _context.Database.BeginTransactionAsync();
            try
            {
                var purchase = new Purchase
                {
                    SupplierId = request.SupplierId,
                    SupplierName = supplier.SupplierName,
                    PurchaseDate = request.ReceiptDate,
                    ReceiptCode = receiptCode,
                    ReferenceCode = request.ReferenceCode,
                    Note = request.Note,
                    CreateDate = DateTime.UtcNow,
                    Status = (WarehouseManagement.BackendServer.Data.Enums.PurchaseStatus)1,
                    IsCanceled = false
                };

                decimal totalCost = 0m;

                foreach (var item in request.Items)
                {
                    if (item.Quantity <= 0)
                        throw new ArgumentException($"Invalid quantity for product {item.ProductId}");

                    // find variant
                    var variant = await _context.ProductVariants
                        .Where(v => v.ProductId == item.ProductId && v.IsActive)
                        .OrderBy(v => v.Id)
                        .FirstOrDefaultAsync();

                    if (variant == null)
                        throw new ArgumentException($"No product variant found for product {item.ProductId}");

                    var pi = new PurchaseItem
                    {
                        ProductVariantId = variant.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost,
                        CreateDate = DateTime.UtcNow,
                        TotalPrice = item.UnitCost * item.Quantity
                    };

                    purchase.PurchaseItems.Add(pi);
                    totalCost += pi.TotalPrice ?? 0m;
                }

                purchase.TotalCost = totalCost;

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                await trx.CommitAsync();

                // prepare response
                var response = new PurchaseReceiptResponse
                {
                    PurchaseId = purchase.Id,
                    SupplierId = purchase.SupplierId,
                    WarehouseId = request.WarehouseId,
                    SupplierName = purchase.SupplierName,
                    ReceiptDate = purchase.PurchaseDate ?? DateTime.UtcNow,
                    ReceiptCode = purchase.ReceiptCode,
                    ReferenceCode = purchase.ReferenceCode,
                    Note = purchase.Note,
                    TotalCost = purchase.TotalCost,
                    CreateDate = purchase.CreateDate,
                    LastModifiedDate = purchase.LastModifiedDate,
                    Status = (int)purchase.Status,
                    IsCanceled = purchase.IsCanceled,
                };

                response.Items = purchase.PurchaseItems.Select(x => new PurchaseReceiptItemResponse
                {
                    PurchaseId = purchase.Id,
                    ProductId = x.ProductId ?? 0,
                    ProductVariantId = x.ProductVariantId,
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost,
                    TotalPrice = x.TotalPrice ?? 0m
                }).ToList();

                return response;
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var purchase = await _context.Purchases.Include(p => p.PurchaseItems).FirstOrDefaultAsync(p => p.Id == id);
            if (purchase == null) return false;

            // soft delete
            purchase.IsDeleted = true;
            foreach (var item in purchase.PurchaseItems)
                item.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PurchaseReceiptResponse>> GetAllAsync()
        {
            var purchases = await _context.Purchases
                .Where(p => !p.IsDeleted)
                .Include(p => p.PurchaseItems)
                .ToListAsync();

            return purchases.Select(p => new PurchaseReceiptResponse
            {
                PurchaseId = p.Id,
                SupplierId = p.SupplierId,
                WarehouseId = 0,
                SupplierName = p.SupplierName,
                ReceiptDate = p.PurchaseDate ?? DateTime.UtcNow,
                ReceiptCode = p.ReceiptCode,
                ReferenceCode = p.ReferenceCode,
                Note = p.Note,
                TotalCost = p.TotalCost,
                CreateDate = p.CreateDate,
                LastModifiedDate = p.LastModifiedDate,
                Status = (int)p.Status,
                IsCanceled = p.IsCanceled,
                Items = p.PurchaseItems.Where(i => !i.IsDeleted).Select(i => new PurchaseReceiptItemResponse
                {
                    PurchaseId = p.Id,
                    ProductId = i.ProductId ?? 0,
                    ProductVariantId = i.ProductVariantId,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost,
                    TotalPrice = i.TotalPrice ?? 0m
                }).ToList()
            }).ToList();
        }

        public async Task<PurchaseReceiptResponse?> GetByIdAsync(int id)
        {
            var p = await _context.Purchases
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.PurchaseItems)
                .FirstOrDefaultAsync();
            if (p == null) return null;

            var resp = new PurchaseReceiptResponse
            {
                PurchaseId = p.Id,
                SupplierId = p.SupplierId,
                WarehouseId = 0,
                SupplierName = p.SupplierName,
                ReceiptDate = p.PurchaseDate ?? DateTime.UtcNow,
                ReceiptCode = p.ReceiptCode,
                ReferenceCode = p.ReferenceCode,
                Note = p.Note,
                TotalCost = p.TotalCost,
                CreateDate = p.CreateDate,
                LastModifiedDate = p.LastModifiedDate,
                Status = (int)p.Status,
                IsCanceled = p.IsCanceled,
                Items = p.PurchaseItems.Where(i => !i.IsDeleted).Select(i => new PurchaseReceiptItemResponse
                {
                    PurchaseId = p.Id,
                    ProductId = i.ProductId ?? 0,
                    ProductVariantId = i.ProductVariantId,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost,
                    TotalPrice = i.TotalPrice ?? 0m
                }).ToList()
            };

            return resp;
        }

        public async Task<bool> UpdateAsync(int id, PurchaseReceiptRequest request)
        {
            var purchase = await _context.Purchases.Include(p => p.PurchaseItems).FirstOrDefaultAsync(p => p.Id == id);
            if (purchase == null) return false;

            if (purchase.IsCanceled) return false;

            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            if (supplier == null || supplier.IsDeleted) throw new ArgumentException("Supplier not found");

            using var trx = await _context.Database.BeginTransactionAsync();
            try
            {
                // remove old items (physical delete or mark deleted)
                var oldItems = purchase.PurchaseItems.ToList();
                foreach (var oi in oldItems)
                    _context.PurchaseItems.Remove(oi);

                purchase.SupplierId = request.SupplierId;
                purchase.SupplierName = supplier.SupplierName;
                purchase.PurchaseDate = request.ReceiptDate;
                purchase.ReferenceCode = request.ReferenceCode;
                purchase.Note = request.Note;
                purchase.LastModifiedDate = DateTime.UtcNow;

                decimal totalCost = 0m;
                foreach (var item in request.Items)
                {
                    if (item.Quantity <= 0) throw new ArgumentException($"Invalid quantity for product {item.ProductId}");

                    var variant = await _context.ProductVariants
                        .Where(v => v.ProductId == item.ProductId && v.IsActive)
                        .OrderBy(v => v.Id)
                        .FirstOrDefaultAsync();

                    if (variant == null) throw new ArgumentException($"No product variant found for product {item.ProductId}");

                    var pi = new PurchaseItem
                    {
                        PurchaseId = purchase.Id,
                        ProductVariantId = variant.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost,
                        CreateDate = DateTime.UtcNow,
                        TotalPrice = item.UnitCost * item.Quantity
                    };

                    _context.PurchaseItems.Add(pi);
                    totalCost += pi.TotalPrice ?? 0m;
                }

                purchase.TotalCost = totalCost;

                await _context.SaveChangesAsync();
                await trx.CommitAsync();
                return true;
            }
            catch
            {
                await trx.RollbackAsync();
                throw;
            }
        }
    }
}
