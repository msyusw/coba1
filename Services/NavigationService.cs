using FoodStorePOS.ViewModels;

namespace FoodStorePOS.Services;

public class NavigationService
{
    private INavigation Navigation => Application.Current?.MainPage?.Navigation 
        ?? throw new InvalidOperationException("Navigation not available");

    public async Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel
    {
        var page = ResolvePage<TViewModel>();
        await Navigation.PushAsync(page);
    }

    public async Task GoBackAsync()
    {
        await Navigation.PopAsync();
    }

    private Page ResolvePage<TViewModel>() where TViewModel : BaseViewModel
    {
        var viewModel = Application.Current?.Handler?.MauiContext?.Services
            .GetService<TViewModel>();
        var pageType = typeof(TViewModel).Name.Replace("ViewModel", "Page");
        var page = Application.Current?.Handler?.MauiContext?.Services
            .GetService(Type.GetType($"FoodStorePOS.Views.{pageType}")) as Page;

        if (page != null && viewModel != null)
        {
            page.BindingContext = viewModel;
            return page;
        }

        throw new InvalidOperationException($"Page not found for {typeof(TViewModel).Name}");
    }
}