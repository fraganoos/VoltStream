namespace VoltStream.WPF.Commons.Services;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VoltStream.WPF.Commons.Enums;
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

        // 📋 Copy Button (Custom Template to remove hover background)
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

        // Override Default Template to avoid standard grey hover background
        var template = new ControlTemplate(typeof(Button));
        var borderFactory = new FrameworkElementFactory(typeof(Border));
        borderFactory.SetValue(Border.BackgroundProperty, Brushes.Transparent); // Always transparent
        borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0));
        
        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        
        borderFactory.AppendChild(contentPresenter);
        template.VisualTree = borderFactory;
        copyButton.Template = template;

        // Z-Index yuqori
        Panel.SetZIndex(copyButton, 100);

        copyButton.Click += (_, _) => 
        {
            try 
            { 
                // 📝 Format: [TYPE] YYYY-MM-DD HH:mm:ss -> Message
                var formattedMessage = $"[{type.ToString().ToUpper()}] {DateTime.Now:yyyy-MM-dd HH:mm:ss} -> {message}";
                Clipboard.SetText(formattedMessage);
                
                // ✅ Visual Feedback (Change Icon to Check)
                if (copyButton.Content is FontAwesome.Sharp.IconImage icon)
                {
                    var originalIcon = icon.Icon;
                    icon.Icon = FontAwesome.Sharp.IconChar.Check;
                    copyButton.ToolTip = "Buferga saqlandi!";
                    
                    // Revert back after 1.5 seconds
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

        // 📝 Text Message
        var textBlock = new TextBlock
        {
            Text = wrappedMessage,
            Foreground = Brushes.White,
            FontSize = 16,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(35, 10, 15, 10) // Button uchun joy ajratish (Left Margin oshirildi)
        };

        // Ikkalasini ustma-ust qo'yamiz (Grid Column sizing muammosi bo'lmasligi uchun)
        // Yoki ColumnDefinition ishlatish mumkin, lekin Grid ichida oddiy joylashtirish ishonchliroq.
        // Hozirgi Grid logikasini biroz o'zgartiramiz:
        
        var contentGrid = new Grid();
        contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Button
        contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Text

        // Qayta joylashtirish
        copyButton.Margin = new Thickness(10, 12, 0, 0); // Marginni to'g'irlash
        textBlock.Margin = new Thickness(10, 10, 15, 10); // Buttondan keyingi margin

        Grid.SetColumn(copyButton, 0);
        Grid.SetColumn(textBlock, 1);
        
        contentGrid.Children.Add(copyButton);
        contentGrid.Children.Add(textBlock);

        // ✉️ Message UI
        var messageBorder = new Border
        {
            Background = background,
            CornerRadius = new CornerRadius(8),
            Opacity = 0,
            Margin = new Thickness(0, 0, 0, 10),
            Child = contentGrid
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
