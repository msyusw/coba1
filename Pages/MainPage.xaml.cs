namespace FoodStorePOS;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _database;
    private List<TransactionItem> _cart = new();
    private List<Product> _products = new();
    private decimal _total = 0;

    public MainPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;
        LoadProducts();
    }

    private async void LoadProducts()
    {
        _products = await _database.GetProductsAsync();
        ProductsCollection.ItemsSource = _products;
    }

    private async void OnProductsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductsPage(_database));
    }

    private async void OnTransactionsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TransactionsPage(_database));
    }

    private void OnAddToCartClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var product = button?.CommandParameter as Product;
        
        if (product == null || product.Stock <= 0) return;

        var item = _cart.FirstOrDefault(i => i.ProductId == product.Id);
        if (item == null)
        {
            item = new TransactionItem
            {
                ProductId = product.Id,
                Price = product.Price,
                Quantity = 1,
                Subtotal = product.Price
            };
            _cart.Add(item);
        }
        else
        {
            item.Quantity++;
            item.Subtotal = item.Price * item.Quantity;
        }

        _total = _cart.Sum(i => i.Subtotal);
        TotalLabel.Text = $"${_total:N2}";
    }

    private async void OnCompleteSaleClicked(object sender, EventArgs e)
    {
        if (_cart.Count == 0) return;

        var transaction = new Transaction
        {
            Date = DateTime.Now,
            Total = _total,
            PaymentMethod = "Cash" // You can add payment method selection
        };

        await _database.SaveTransactionAsync(transaction, _cart);

        // Update stock
        foreach (var item in _cart)
        {
            var product = _products.First(p => p.Id == item.ProductId);
            product.Stock -= item.Quantity;
            await _database.UpdateProductAsync(product);
        }

        _cart.Clear();
        _total = 0;
        TotalLabel.Text = "$0.00";
        LoadProducts();

        await DisplayAlert("Success", "Sale completed!", "OK");
    }
}