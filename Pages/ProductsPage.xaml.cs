namespace FoodStorePOS;

public partial class ProductsPage : ContentPage
{
    private readonly DatabaseService _database;
    public Command<Product> DeleteCommand { get; }

    public ProductsPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;
        DeleteCommand = new Command<Product>(OnDeleteProduct);
        LoadProducts();
    }

    private async void LoadProducts()
    {
        var products = await _database.GetProductsAsync();
        ProductsList.ItemsSource = products;
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("New Product", "Enter product name:");
        if (string.IsNullOrEmpty(name)) return;

        string priceStr = await DisplayPromptAsync("New Product", "Enter price:");
        if (!decimal.TryParse(priceStr, out decimal price)) return;

        string category = await DisplayPromptAsync("New Product", "Enter category:");
        if (string.IsNullOrEmpty(category)) return;

        string stockStr = await DisplayPromptAsync("New Product", "Enter initial stock:");
        if (!int.TryParse(stockStr, out int stock)) return;

        var product = new Product
        {
            Name = name,
            Price = price,
            Category = category,
            Stock = stock
        };

        await _database.AddProductAsync(product);
        LoadProducts();
    }

    private async void OnDeleteProduct(Product product)
    {
        bool answer = await DisplayAlert("Confirm Delete", 
            $"Are you sure you want to delete {product.Name}?", "Yes", "No");
            
        if (answer)
        {
            await _database.DeleteProductAsync(product);
            LoadProducts();
        }
    }
}