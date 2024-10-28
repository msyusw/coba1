using System.Collections.ObjectModel;
using DevExpress.Maui.DataGrid;
using FoodStorePOS.Models;

namespace FoodStorePOS.ViewModels;

public class MainViewModel : BaseViewModel
{
    private ObservableCollection<Product> _products;
    private ObservableCollection<TransactionItem> _cart;
    private decimal _total;

    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public ObservableCollection<TransactionItem> Cart
    {
        get => _cart;
        set => SetProperty(ref _cart, value);
    }

    public decimal Total
    {
        get => _total;
        set => SetProperty(ref _total, value);
    }

    public ICommand AddToCartCommand { get; }
    public ICommand CompleteSaleCommand { get; }
    public ICommand NavigateToProductsCommand { get; }
    public ICommand NavigateToTransactionsCommand { get; }

    public MainViewModel(NavigationService navigationService, DatabaseService databaseService) 
        : base(navigationService, databaseService)
    {
        Products = new ObservableCollection<Product>();
        Cart = new ObservableCollection<TransactionItem>();

        AddToCartCommand = new Command<Product>(AddToCart);
        CompleteSaleCommand = new Command(async () => await CompleteSale());
        NavigateToProductsCommand = new Command(async () => await NavigationService.NavigateToAsync<ProductsViewModel>());
        NavigateToTransactionsCommand = new Command(async () => await NavigationService.NavigateToAsync<TransactionsViewModel>());

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

    private void AddToCart(Product product)
    {
        if (product?.Stock <= 0) return;

        var item = Cart.FirstOrDefault(i => i.ProductId == product.Id);
        if (item == null)
        {
            item = new TransactionItem
            {
                ProductId = product.Id,
                Price = product.Price,
                Quantity = 1,
                Subtotal = product.Price
            };
            Cart.Add(item);
        }
        else
        {
            item.Quantity++;
            item.Subtotal = item.Price * item.Quantity;
        }

        Total = Cart.Sum(i => i.Subtotal);
    }

    private async Task CompleteSale()
    {
        if (!Cart.Any()) return;
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            var transaction = new Transaction
            {
                Date = DateTime.Now,
                Total = Total,
                PaymentMethod = "Cash"
            };

            await DatabaseService.SaveTransactionAsync(transaction, Cart.ToList());

            foreach (var item in Cart)
            {
                var product = Products.First(p => p.Id == item.ProductId);
                product.Stock -= item.Quantity;
                await DatabaseService.UpdateProductAsync(product);
            }

            Cart.Clear();
            Total = 0;
            LoadProducts();

            await Application.Current.MainPage.DisplayAlert("Success", "Sale completed!", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}