namespace FoodStorePOS.Models;

public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; }
}