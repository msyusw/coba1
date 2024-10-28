namespace FoodStorePOS.Models;

public class TransactionItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TransactionId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}