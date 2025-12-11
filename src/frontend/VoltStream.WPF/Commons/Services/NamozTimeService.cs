namespace VoltStream.WPF.Commons.Services;

using System.IO;
using System.Net.Http;
using System.Text.Json;

public class NamozTimeService
{
    private static readonly string ApiUrl =
        "https://namoz-vaqti.uz/?format=json&lang=lotin&period=month&region=qoqon-shahri";
    private static readonly string CachePath =
        Path.Combine(AppContext.BaseDirectory, "namoz_times.json");

    public async Task<NamozData?> GetTodayAsync()
    {
        NamozApiResponse? data = await LoadOrUpdateAsync();
        if (data?.PeriodTable is null || data.PeriodTable.Count == 0)
            return null;

        string today = DateTime.Now.ToString("dd.MM.yyyy");
        var todayRecord = data.PeriodTable.FirstOrDefault(d => d.Date == today)
                          ?? data.PeriodTable.LastOrDefault();

        if (todayRecord is null)
            return null;

        return new NamozData
        {
            Date = todayRecord.Date,
            Bomdod = todayRecord.Times.Bomdod,
            Quyosh = todayRecord.Times.Quyosh,
            Peshin = todayRecord.Times.Peshin,
            Asr = todayRecord.Times.Asr,
            Shom = todayRecord.Times.Shom,
            Xufton = todayRecord.Times.Xufton
        };
    }

    private async Task<NamozApiResponse?> LoadOrUpdateAsync()
    {
        if (File.Exists(CachePath))
        {
            var json = await File.ReadAllTextAsync(CachePath);
            var data = JsonSerializer.Deserialize<NamozApiResponse>(json);

            var lastDateStr = data?.PeriodTable?.LastOrDefault()?.Date;
            if (DateTime.TryParseExact(lastDateStr, "dd.MM.yyyy", null,
                System.Globalization.DateTimeStyles.None, out var lastDate))
            {
                lastDate = lastDate.AddDays(-15); // Refresh 15 days before the end of the month
                if (lastDate < DateTime.Now.Date)
                    if (await DownloadFromApiAsync()==null)
                        return data;
            }

            return data;
        }

        return await DownloadFromApiAsync();
    }

    private async Task<NamozApiResponse?> DownloadFromApiAsync()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync(ApiUrl);

            await File.WriteAllTextAsync(CachePath, response);
            return JsonSerializer.Deserialize<NamozApiResponse>(response);
        }
        catch (Exception)
        {
            // Если API недоступен, просто возвращаем null, не выбрасываем исключение
            return null;
        }
    }
}

public class NamozData
{
    public string Date { get; set; }
    public string Bomdod { get; set; }
    public string Quyosh { get; set; }
    public string Peshin { get; set; }
    public string Asr { get; set; }
    public string Shom { get; set; }
    public string Xufton { get; set; }
}

