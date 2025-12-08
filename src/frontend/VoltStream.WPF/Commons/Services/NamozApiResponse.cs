namespace VoltStream.WPF.Commons.Services;

using System.Text.Json.Serialization;

public class NamozApiResponse
{
    [JsonPropertyName("meta")]
    public NamozMeta? Meta { get; set; }

    [JsonPropertyName("labels")]
    public NamozLabels? Labels { get; set; }

    [JsonPropertyName("today")]
    public NamozToday? Today { get; set; }

    [JsonPropertyName("period_table")]
    public List<NamozDay>? PeriodTable { get; set; }
}

public class NamozMeta
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = "";
}

public class NamozLabels
{
    [JsonPropertyName("bomdod")] public string Bomdod { get; set; } = "";
    [JsonPropertyName("quyosh")] public string Quyosh { get; set; } = "";
    [JsonPropertyName("peshin")] public string Peshin { get; set; } = "";
    [JsonPropertyName("asr")] public string Asr { get; set; } = "";
    [JsonPropertyName("shom")] public string Shom { get; set; } = "";
    [JsonPropertyName("xufton")] public string Xufton { get; set; } = "";
}

public class NamozToday
{
    [JsonPropertyName("times")]
    public NamozTimes? Times { get; set; }

    [JsonPropertyName("current")]
    public NamozCurrent? Current { get; set; }
}

public class NamozCurrent
{
    [JsonPropertyName("key")] public string Key { get; set; } = "";
    [JsonPropertyName("label")] public string Label { get; set; } = "";
    [JsonPropertyName("start")] public string Start { get; set; } = "";
    [JsonPropertyName("end")] public string End { get; set; } = "";
}

public class NamozDay
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = "";

    [JsonPropertyName("times")]
    public NamozTimes Times { get; set; } = new();
}

public class NamozTimes
{
    [JsonPropertyName("bomdod")] public string Bomdod { get; set; } = "";
    [JsonPropertyName("quyosh")] public string Quyosh { get; set; } = "";
    [JsonPropertyName("peshin")] public string Peshin { get; set; } = "";
    [JsonPropertyName("asr")] public string Asr { get; set; } = "";
    [JsonPropertyName("shom")] public string Shom { get; set; } = "";
    [JsonPropertyName("xufton")] public string Xufton { get; set; } = "";
}
