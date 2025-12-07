using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ToolMain;

public enum eProcess
{
    None,
    Path,
    Script,
    Packaging
}

public class ViewProcessChangedMessage : ValueChangedMessage<eProcess>
{
    public ViewProcessChangedMessage(eProcess process) : base(process)
    {
    }
}