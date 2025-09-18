namespace VoltStream.WPF.Commons.Services;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VoltStream.Wpf.Common.Enums;
using VoltStream.WPF.Commons.Extensions;

public static class NotificationService
{
    private static StackPanel? host;

    public static void Init(Window mainWindow)
    {
        host = mainWindow.FindName("NotificationHost") as StackPanel
               ?? throw new InvalidOperationException("NotificationHost topilmadi");
    }

    public static void Show(
        string message,
        NotificationType type = NotificationType.Error,
        int durationSeconds = 5,
        double opacity = 0.9,
        int maxLineLength = 40)
    {
        if (host is null) return;

        var background = GetBackground(type);
        var wrappedMessage = message.WrapWithNewLines(maxLineLength);

        // ✉️ Message UI
        var messageBorder = new Border
        {
            Background = background,
            CornerRadius = new CornerRadius(8),
            Opacity = 0,
            Margin = new Thickness(0, 0, 0, 10),
            Child = new TextBlock
            {
                Text = wrappedMessage,
                Foreground = Brushes.White,
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15, 10, 15, 10)
            }
        };

        // ➕ Insert at top
        host.Children.Add(messageBorder);

        // 🎞️ Fade-in
        var fadeIn = new DoubleAnimation(0, opacity, TimeSpan.FromSeconds(0.3));
        messageBorder.BeginAnimation(UIElement.OpacityProperty, fadeIn);

        // ⏳ Auto-remove after duration
        Task.Delay(durationSeconds * 1000).ContinueWith(_ =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var fadeOut = new DoubleAnimation(opacity, 0, TimeSpan.FromSeconds(0.3));
                fadeOut.Completed += (_, _) => host.Children.Remove(messageBorder);
                messageBorder.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            });
        });
    }

    private static SolidColorBrush GetBackground(NotificationType type) => type switch
    {
        NotificationType.Info => new SolidColorBrush(Color.FromRgb(33, 150, 243)),
        NotificationType.Success => new SolidColorBrush(Color.FromRgb(76, 175, 80)),
        NotificationType.Warning => new SolidColorBrush(Color.FromRgb(255, 152, 0)),
        NotificationType.Error => new SolidColorBrush(Color.FromRgb(244, 67, 54)),
        _ => new SolidColorBrush(Colors.Gray)
    };
}
