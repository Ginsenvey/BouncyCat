using CommunityToolkit.Mvvm.Messaging.Messages;
namespace BouncyCat.Messengers;
public class ToggleLeftPanelMessage : ValueChangedMessage<bool>
{
    public ToggleLeftPanelMessage(bool value) : base(value) { }
}