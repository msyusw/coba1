using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FoodStorePOS.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    protected readonly NavigationService NavigationService;
    protected readonly DatabaseService DatabaseService;
    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public BaseViewModel(NavigationService navigationService, DatabaseService databaseService)
    {
        NavigationService = navigationService;
        DatabaseService = databaseService;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected virtual Task InitializeAsync() => Task.CompletedTask;
}