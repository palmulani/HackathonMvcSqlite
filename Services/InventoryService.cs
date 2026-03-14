using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;
using HackathonMvcSqlite.Models;

namespace HackathonMvcSqlite.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _db;

        public InventoryService(AppDbContext db) => _db = db;

        public decimal GetStock(int productId, int warehouseId)
        {
            return _db.StockBalances
                .Where(b => b.ProductId == productId && b.WarehouseId == warehouseId)
                .Select(b => b.Quantity)
                .FirstOrDefault();
        }

        public void EnsureStockBalance(int productId, int warehouseId)
        {
            var exists = _db.StockBalances
                .Any(b => b.ProductId == productId && b.WarehouseId == warehouseId);
            if (!exists)
            {
                _db.StockBalances.Add(new StockBalance
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    Quantity = 0
                });
                _db.SaveChanges();
            }
        }

        public void AddStock(int productId, int warehouseId, decimal qty, DocumentType docType, int? docId, string? description)
        {
            EnsureStockBalance(productId, warehouseId);
            var balance = _db.StockBalances
                .First(b => b.ProductId == productId && b.WarehouseId == warehouseId);
            balance.Quantity += qty;
            _db.StockLedgerEntries.Add(new StockLedgerEntry
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                DocumentType = docType,
                DocumentId = docId,
                QuantityIn = qty,
                QuantityOut = 0,
                BalanceAfter = balance.Quantity,
                Description = description
            });
            _db.SaveChanges();
        }

        public void RemoveStock(int productId, int warehouseId, decimal qty, DocumentType docType, int? docId, string? description)
        {
            EnsureStockBalance(productId, warehouseId);
            var balance = _db.StockBalances
                .First(b => b.ProductId == productId && b.WarehouseId == warehouseId);
            balance.Quantity -= qty;
            if (balance.Quantity < 0) balance.Quantity = 0;
            _db.StockLedgerEntries.Add(new StockLedgerEntry
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                DocumentType = docType,
                DocumentId = docId,
                QuantityIn = 0,
                QuantityOut = qty,
                BalanceAfter = balance.Quantity,
                Description = description
            });
            _db.SaveChanges();
        }

        public void ValidateReceipt(int receiptId)
        {
            var receipt = _db.Receipts
                .Include(r => r.Lines)
                .ThenInclude(l => l.Product)
                .FirstOrDefault(r => r.Id == receiptId);
            if (receipt == null || receipt.Status == DocumentStatus.Done) return;
            foreach (var line in receipt.Lines)
                AddStock(line.ProductId, receipt.WarehouseId, line.Quantity, DocumentType.Receipt, receiptId, $"Receipt {receipt.Reference}");
            receipt.Status = DocumentStatus.Done;
            receipt.ValidatedAt = DateTime.UtcNow;
            _db.SaveChanges();
        }

        public void ValidateDeliveryOrder(int deliveryOrderId)
        {
            var order = _db.DeliveryOrders
                .Include(d => d.Lines)
                .FirstOrDefault(d => d.Id == deliveryOrderId);
            if (order == null || order.Status == DocumentStatus.Done) return;
            foreach (var line in order.Lines)
                RemoveStock(line.ProductId, order.WarehouseId, line.Quantity, DocumentType.Delivery, deliveryOrderId, $"Delivery {order.Reference}");
            order.Status = DocumentStatus.Done;
            order.ValidatedAt = DateTime.UtcNow;
            _db.SaveChanges();
        }

        public void ValidateInternalTransfer(int transferId)
        {
            var transfer = _db.InternalTransfers
                .Include(t => t.Lines)
                .FirstOrDefault(t => t.Id == transferId);
            if (transfer == null || transfer.Status == DocumentStatus.Done) return;
            foreach (var line in transfer.Lines)
            {
                RemoveStock(line.ProductId, transfer.SourceWarehouseId, line.Quantity, DocumentType.Internal, transferId, $"Transfer out {transfer.Reference}");
                AddStock(line.ProductId, transfer.DestinationWarehouseId, line.Quantity, DocumentType.Internal, transferId, $"Transfer in {transfer.Reference}");
            }
            transfer.Status = DocumentStatus.Done;
            transfer.ValidatedAt = DateTime.UtcNow;
            _db.SaveChanges();
        }

        public void ApplyAdjustment(StockAdjustment adj)
        {
            EnsureStockBalance(adj.ProductId, adj.WarehouseId);
            var balance = _db.StockBalances.First(b => b.ProductId == adj.ProductId && b.WarehouseId == adj.WarehouseId);
            adj.PreviousQuantity = balance.Quantity;
            balance.Quantity = adj.CountedQuantity;
            var diff = adj.CountedQuantity - adj.PreviousQuantity;
            _db.StockAdjustments.Add(adj);
            _db.SaveChanges();
            _db.StockLedgerEntries.Add(new StockLedgerEntry
            {
                ProductId = adj.ProductId,
                WarehouseId = adj.WarehouseId,
                DocumentType = DocumentType.Adjustment,
                DocumentId = adj.Id,
                QuantityIn = diff > 0 ? diff : 0,
                QuantityOut = diff < 0 ? -diff : 0,
                BalanceAfter = balance.Quantity,
                Description = adj.Reason ?? $"Adjustment {adj.Reference}"
            });
            _db.SaveChanges();
        }
    }
}
