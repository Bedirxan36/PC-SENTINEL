using System.Windows;
using System.Windows.Input;
using SENTINEL.ViewModels;

namespace SENTINEL.Views;

public partial class DashboardWindow : Window
{
    public DashboardWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        // Önerileri ve ağ bağlantılarını yükle
        viewModel.ViewRecommendationsCommand.Execute(null);
        viewModel.ViewNetworkCommand.Execute(null);
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
