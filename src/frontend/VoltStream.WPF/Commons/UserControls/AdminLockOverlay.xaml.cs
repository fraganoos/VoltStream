namespace VoltStream.WPF.Commons.UserControls;

using ApiServices.Interfaces;
using ApiServices.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Commons.Services;

public partial class AdminLockOverlay : UserControl
{
    public static readonly DependencyProperty IsLockedProperty =
        DependencyProperty.Register(nameof(IsLocked), typeof(bool), typeof(AdminLockOverlay),
            new PropertyMetadata(true, OnIsLockedChanged));

    public static readonly DependencyProperty LockMessageProperty =
        DependencyProperty.Register(nameof(LockMessage), typeof(string), typeof(AdminLockOverlay),
            new PropertyMetadata("Admin ruxsati talab qilinadi"));

    public static readonly DependencyProperty InnerContentProperty =
        DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(AdminLockOverlay),
            new PropertyMetadata(null, OnInnerContentChanged));

    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }

    public string LockMessage
    {
        get => (string)GetValue(LockMessageProperty);
        set => SetValue(LockMessageProperty, value);
    }

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }

    public AdminLockOverlay()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            Debug.WriteLine("=== AdminLockOverlay Loaded ===");
            CheckAuth();
        };
    }

    private static void OnInnerContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AdminLockOverlay control)
        {
            Debug.WriteLine($"=== InnerContent Changed ===");
            Debug.WriteLine($"New Content: {e.NewValue?.GetType().Name}");

            if (e.NewValue is FrameworkElement element)
            {
                Debug.WriteLine($"Element DataContext: {element.DataContext?.GetType().Name ?? "NULL"}");

                // Agar DataContext null bo'lsa, parent'dan olishga harakat qilamiz
                if (element.DataContext == null)
                {
                    element.Loaded += (s, args) =>
                    {
                        Debug.WriteLine($"Element Loaded, trying to set DataContext from parent...");
                        if (element.DataContext == null && control.DataContext != null)
                        {
                            Debug.WriteLine($"Setting DataContext from control: {control.DataContext.GetType().Name}");
                            element.DataContext = control.DataContext;
                        }
                    };
                }
            }
        }
    }

    private static void OnIsLockedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AdminLockOverlay control && e.NewValue is bool isLocked)
        {
            Debug.WriteLine($"=== IsLocked Changed: {isLocked} ===");

            if (!isLocked)
            {
                // Unlock bo'lganda DataContext'ni tekshiramiz
                if (control.InnerContent is FrameworkElement element)
                {
                    Debug.WriteLine($"Content Type: {element.GetType().Name}");
                    Debug.WriteLine($"Content DataContext: {element.DataContext?.GetType().Name ?? "NULL"}");

                    // DataContext borligini ta'minlaymiz
                    if (element.DataContext == null && control.DataContext != null)
                    {
                        Debug.WriteLine($"Setting DataContext to: {control.DataContext.GetType().Name}");
                        element.DataContext = control.DataContext;
                    }
                }
            }
        }
    }

    public void CheckAuth()
    {
        var session = App.Services?.GetService<ISessionService>();
        var isAdmin = session?.IsAdmin ?? false;

        Debug.WriteLine($"=== CheckAuth: IsAdmin={isAdmin} ===");

        if (session != null && isAdmin)
        {
            IsLocked = false;
        }
        else
        {
            IsLocked = true;
        }
    }

    private async void UnlockButton_Click(object sender, RoutedEventArgs e)
        => await AttemptUnlock();

    private async void AdminPasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            await AttemptUnlock();
    }

    private async Task AttemptUnlock()
    {
        var username = AdminUsernameBox.Text;
        var password = AdminPasswordBox.Password;

        Debug.WriteLine($"=== Attempting Unlock: {username} ===");

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return;

        var loginApi = App.Services?.GetService<ILoginApi>();
        if (loginApi == null)
        {
            Debug.WriteLine("ERROR: LoginApi is NULL!");
            return;
        }

        ErrorMessage.Visibility = Visibility.Collapsed;

        try
        {
            var response = await loginApi.VerifyAdminAsync(
                new VerifyAdminRequest(username, password));

            Debug.WriteLine($"Response: Success={response.IsSuccess}, Data={response.Data}");

            if (response.IsSuccess && response.Data)
            {
                Debug.WriteLine("=== Unlock SUCCESS! ===");
                IsLocked = false;
                AdminUsernameBox.Text = string.Empty;
                AdminPasswordBox.Password = string.Empty;

                // DataContext'ni qayta tekshirish
                if (InnerContent is FrameworkElement element)
                {
                    Debug.WriteLine($"After unlock - Content DataContext: {element.DataContext?.GetType().Name ?? "NULL"}");
                }
            }
            else
            {
                ErrorMessage.Text = "Login yoki parol noto'g'ri!";
                ErrorMessage.Visibility = Visibility.Visible;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR: {ex.Message}");
            ErrorMessage.Text = "Xatolik: " + ex.Message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}