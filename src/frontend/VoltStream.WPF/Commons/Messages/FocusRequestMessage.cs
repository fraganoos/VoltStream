namespace VoltStream.WPF.Commons.Messages;

using CommunityToolkit.Mvvm.Messaging.Messages;

public class FocusRequestMessage(string value) : ValueChangedMessage<string>(value);
