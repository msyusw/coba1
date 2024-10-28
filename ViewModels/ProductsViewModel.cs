using System.Collections.ObjectModel;
using DevExpress.Maui.DataGrid;
using FoodStorePOS.Models;

namespace FoodStorePOS.ViewModels;

public class ProductsViewModel : BaseViewModel
{
    private ObservableCollection<Product> _products;

    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public ICommand AddProductCommand { get; }
    public ICommand DeleteProductCommand { get; }

    public ProductsViewModel(NavigationService navigationService, DatabaseService databaseService) 
        : base(navigationService, databaseService)
    {
        Products = new ObservableCollection<Product>();
        AddProductCommand = new Command(async () => await AddProduct());
        DeleteProductCommand = new Command<Product>(async (p) => await DeleteProduct(p));

        LoadProducts();
    }

    private async void LoadProducts()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var products = await DatabaseService.GetProductsAsync();
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddProduct()
    {
        string name = await Application.Current.MainPage.DisplayPromptAsync(
            "New Product", "Enter product name:");
        if (string.IsNullOrEmpty(name)) return;

        string priceStr = await Application.Current.MainPage.DisplayPromptAsync(
            "New Product", "Enter price:");
        if (!decimal.TryParse(priceStr, out decimal price)) return;

        string category = await Application.Current.MainPage.DisplayPromptAsync(
            "New Product", "Enter category:");
        if (string.IsNullOrEmpty(category)) return;

        string stockStr = await Application.Current.MainPage.DisplayPromptAsync(
            "New Product", "Enter initial stock:");
        if (!int.TryParse(stockStr, out int stock)) return;

        var product = new Product
        {
            Name = name,
            Price = price,
            Category = category,
            Stock = stock
        };

        await DatabaseService.AddProductAsync(product);
        LoadProducts();
    }

    private async Task DeleteProduct(Product product)
    {
        bool answer = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete {product.Name}?", 
            "Yes", "No");
            
        if (answer)
        {
            await DatabaseService.DeleteProductAsync(product);
            LoadProducts();
        }
    }
}