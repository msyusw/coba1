namespace FoodStorePOS;

public partial class TransactionsPage : ContentPage
{
    private readonly DatabaseService _database;

    public TransactionsPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;
        LoadTransactions();
    }

    private async void LoadTransactions()
    {
        var transactions = await _database.GetTransactionsAsync();
        TransactionsList.ItemsSource = transactions;
    }
}