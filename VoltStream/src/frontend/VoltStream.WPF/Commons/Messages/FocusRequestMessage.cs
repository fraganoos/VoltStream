namespace VoltStream.WPF.Commons.Messages;

using CommunityToolkit.Mvvm.Messaging.Messages;

public class FocusRequestMessage : ValueChangedMessage<string>
{
    public FocusRequestMessage(string value) : base(value) { }
}
