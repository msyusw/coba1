using System.Collections.ObjectModel;
using FoodStorePOS.Models;

namespace FoodStorePOS.ViewModels;

public class TransactionsViewModel : BaseViewModel
{
    private ObservableCollection<Transaction> _transactions;

    public ObservableCollection<Transaction> Transactions
    {
        get => _transactions;
        set => SetProperty(ref _transactions, value);
    }

    public TransactionsViewModel(NavigationService navigationService, DatabaseService databaseService) 
        : base(navigationService, databaseService)
    {
        Transactions = new ObservableCollection<Transaction>();
        LoadTransactions();
    }

    private async void LoadTransactions()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var transactions = await DatabaseService.GetTransactionsAsync();
            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}