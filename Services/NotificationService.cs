using System;
using System.Windows;

namespace SENTINEL.Services;

public class NotificationService
{
    public void ShowSuccess(string title, string message)
    {
        ShowNotification(title, message, "✅");
    }

    public void ShowWarning(string title, string message)
    {
        ShowNotification(title, message, "⚠️");
    }

    public void ShowError(string title, string message)
    {
        ShowNotification(title, message, "❌");
    }

    public void ShowInfo(string title, string message)
    {
        ShowNotification(title, message, "ℹ️");
    }

    private void ShowNotification(string title, string message, string icon)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show($"{icon} {message}", title, MessageBoxButton.OK, 
                icon == "❌" ? MessageBoxImage.Error : 
                icon == "⚠️" ? MessageBoxImage.Warning : 
                MessageBoxImage.Information);
        });
    }
}
