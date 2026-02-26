using System.Windows;
using System.Windows.Input;

namespace SENTINEL;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        else
        {
            DragMove();
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void AnalyzePassword_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as ViewModels.MainViewModel;
        if (viewModel != null && !string.IsNullOrEmpty(viewModel.PasswordInput))
        {
            await viewModel.AnalyzePasswordManualAsync();
        }
    }

    private async void RefreshNetwork_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as ViewModels.MainViewModel;
        if (viewModel != null)
        {
            await viewModel.LoadNetworkConnectionsManualAsync();
        }
    }

    private async void EnableDefender_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as ViewModels.MainViewModel;
        if (viewModel != null && viewModel.EnableDefenderCommand.CanExecute(null))
        {
            viewModel.EnableDefenderCommand.Execute(null);
        }
    }

    private async void EnableFirewall_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as ViewModels.MainViewModel;
        if (viewModel != null && viewModel.EnableFirewallCommand.CanExecute(null))
        {
            viewModel.EnableFirewallCommand.Execute(null);
        }
    }

    private async void CheckUpdates_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as ViewModels.MainViewModel;
        if (viewModel != null && viewModel.CheckUpdatesCommand.CanExecute(null))
        {
            viewModel.CheckUpdatesCommand.Execute(null);
        }
    }

    private async void ExportReport_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as ViewModels.MainViewModel;
        if (viewModel != null && viewModel.ExportReportCommand.CanExecute(null))
        {
            viewModel.ExportReportCommand.Execute(null);
        }
    }
}
