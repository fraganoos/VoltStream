namespace VoltStream.WPF.Commons.Services;

public record NamozTimes(
    string Bomdod, string Quyosh, string Peshin,
    string Asr, string Shom, string Xufton);

public record NamozRegion(string Name, string Slug);

public record NamozMeta(NamozRegion Region, string Date, string Now);

public record NamozCurrent(string Key, string Label, string Start, string End);

public record NamozToday(NamozTimes Times, NamozCurrent Current);

public record NamozDay(string Date, NamozTimes Times);

public record NamozApiResponse(
    NamozMeta Meta,
    NamozTimes Labels,
    NamozToday Today,
    List<NamozDay> PeriodTable);