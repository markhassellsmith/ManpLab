using ManpWinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ManpWinUI.Views
{
    /// <summary>
    /// Main fractal explorer page with MVVM architecture.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            this.InitializeComponent();

            // Get ViewModel from DI container
            ViewModel = App.Current.Services.GetRequiredService<MainViewModel>();
            DataContext = ViewModel;
        }
    }
}
