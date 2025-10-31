namespace VoltStream.Domain.Entities;

public class Cash : Auditable
{
    public decimal Balance { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool IsActive { get; set; } = true;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;
}


//public class AppSettings : Auditable
//{
//    public string CompanyName { get; set; } = string.Empty;
//    public string AppName { get; set; } = string.Empty;
//    public string LogoUrl { get; set; } = string.Empty;
//    public string Description { get; set; } = string.Empty;

//    public ThemeType DefaultTheme { get; set; } = ThemeType.Light;
//    public AppMode DefaultMode { get; set; } = AppMode.Standard;
//    public string PrimaryColor { get; set; } = "#02BF67";
//    public string SecondaryColor { get; set; } = "#FFFFFF";
//    public string Language { get; set; } = "en";
//    public string Timezone { get; set; } = "UTC+5";
//    public string DateFormat { get; set; } = "dd.MM.yyyy";
//    public string NumberFormat { get; set; } = "1,234.56";
//    public string SupportEmail { get; set; } = string.Empty;
//    public string SupportPhone { get; set; } = string.Empty;
//    public bool ShowWatermark { get; set; } = false;
//    public bool IsMaintenanceMode { get; set; } = false;
//    public bool AllowUserThemeChange { get; set; } = true;
//    public bool AllowUserModeChange { get; set; } = false;
//}

//public class UserPreferences : Auditable
//{
//    public long UserId { get; set; }

//    public ThemeType? Theme { get; set; }    // null bo‘lsa AppSettings’dan oladi
//    public AppMode? Mode { get; set; }
//    public string? Language { get; set; }
//    public string? Timezone { get; set; }
//}
