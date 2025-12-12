namespace VoltStream.WPF.Commons.Messages;

public class OpenDialogMessage<TViewModel>
{
    public TViewModel ViewModelData { get; set; } = default!;
}

