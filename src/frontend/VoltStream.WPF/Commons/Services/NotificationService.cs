namespace VoltStream.WPF.Commons.Services;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VoltStream.WPF.Commons.Enums;
using VoltStream.WPF.Commons.Extensions;

public static class NotificationService
{
    private static WeakReference<StackPanel>? hostRef;

    public static void Init(Window mainWindow)
    {
        var panel = mainWindow.FindName("NotificationHost") as StackPanel
               ?? throw new InvalidOperationException("NotificationHost topilmadi");
        hostRef = new WeakReference<StackPanel>(panel);
    }

    public static void Show(
        string message,
        NotificationType type = NotificationType.Error,
        int durationSeconds = 5,
        double opacity = 0.9,
        int maxLineLength = 40)
    {
        if (hostRef is null || !hostRef.TryGetTarget(out var host)) return;

        var background = GetBackground(type);
        var wrappedMessage = message.WrapWithNewLines(maxLineLength);

        var copyButton = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10, 12, 5, 0),
            Cursor = System.Windows.Input.Cursors.Hand,
            ToolTip = "Nusxa olish",
            Focusable = false,
            IsHitTestVisible = true,
            Content = new FontAwesome.Sharp.IconImage
            {
                Icon = FontAwesome.Sharp.IconChar.Copy,
                Foreground = Brushes.White,
                Width = 16,
                Height = 16,
                Opacity = 1.0
            }
        };

        var template = new ControlTemplate(typeof(Button));
        var borderFactory = new FrameworkElementFactory(typeof(Border));
        borderFactory.SetValue(Border.BackgroundProperty, Brushes.Transparent);
        borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0));
        
        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        
        borderFactory.AppendChild(contentPresenter);
        template.VisualTree = borderFactory;
        copyButton.Template = template;

        Panel.SetZIndex(copyButton, 100);

        copyButton.Click += (_, _) => 
        {
            try 
            { 
                var formattedMessage = $"[{type.ToString().ToUpper()}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} -> {message}";
                Clipboard.SetText(formattedMessage);
                
                if (copyButton.Content is FontAwesome.Sharp.IconImage icon)
                {
                    var originalIcon = icon.Icon;
                    icon.Icon = FontAwesome.Sharp.IconChar.Check;
                    copyButton.ToolTip = "Buferga saqlandi!";
                    
                    Task.Delay(1500).ContinueWith(_ => 
                    {
                        Application.Current.Dispatcher.Invoke(() => 
                        {
                            icon.Icon = originalIcon;
                            copyButton.ToolTip = "Nusxa olish";
                        });
                    });
                }
            } 
            catch {}
        };

        var textBlock = new TextBlock
        {
            Text = wrappedMessage,
            Foreground = Brushes.White,
            FontSize = 16,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(35, 10, 15, 10)
        };

        var contentGrid = new Grid();
        contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Button
        contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Text

        copyButton.Margin = new Thickness(10, 12, 0, 0);
        textBlock.Margin = new Thickness(10, 10, 15, 10);

        Grid.SetColumn(copyButton, 0);
        Grid.SetColumn(textBlock, 1);
        
        contentGrid.Children.Add(copyButton);
        contentGrid.Children.Add(textBlock);

        var messageBorder = new Border
        {
            Background = background,
            CornerRadius = new CornerRadius(8),
            Opacity = 0,
            Margin = new Thickness(0, 0, 0, 10),
            Child = contentGrid
        };

        if (host.Children.Count >= 5)
            host.Children.RemoveAt(0);

        host.Children.Add(messageBorder);

        var fadeIn = new DoubleAnimation(0, opacity, TimeSpan.FromSeconds(0.3));
        messageBorder.BeginAnimation(UIElement.OpacityProperty, fadeIn);

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
