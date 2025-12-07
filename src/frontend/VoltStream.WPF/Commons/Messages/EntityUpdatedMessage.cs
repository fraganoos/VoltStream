namespace VoltStream.WPF.Commons.Messages;

using CommunityToolkit.Mvvm.Messaging.Messages;

public class EntityUpdatedMessage<T>(T entity) : ValueChangedMessage<T>(entity)
{
}
