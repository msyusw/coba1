using SQLite;
using FoodStorePOS.Models;

namespace FoodStorePOS.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "pos.db");
        _database = new SQLiteAsyncConnection(dbPath);
        InitializeAsync().Wait();
    }

    private async Task InitializeAsync()
    {
        await _database.CreateTableAsync<Product>();
        await _database.CreateTableAsync<Transaction>();
        await _database.CreateTableAsync<TransactionItem>();
    }

    // Product Operations
    public async Task<List<Product>> GetProductsAsync()
        => await _database.Table<Product>().ToListAsync();

    public async Task<int> AddProductAsync(Product product)
        => await _database.InsertAsync(product);

    public async Task<int> UpdateProductAsync(Product product)
        => await _database.UpdateAsync(product);

    public async Task<int> DeleteProductAsync(Product product)
        => await _database.DeleteAsync(product);

    // Transaction Operations
    public async Task<int> SaveTransactionAsync(Transaction transaction, List<TransactionItem> items)
    {
        await _database.InsertAsync(transaction);
        foreach (var item in items)
        {
            item.TransactionId = transaction.Id;
            await _database.InsertAsync(item);
        }
        return transaction.Id;
    }

    public async Task<List<Transaction>> GetTransactionsAsync()
        => await _database.Table<Transaction>().OrderByDescending(t => t.Date).ToListAsync();

    public async Task<List<TransactionItem>> GetTransactionItemsAsync(int transactionId)
        => await _database.Table<TransactionItem>().Where(t => t.TransactionId == transactionId).ToListAsync();
}