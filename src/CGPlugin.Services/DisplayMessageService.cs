namespace CGPlugin.Services;

using System.Windows.Forms;

using CGPlugin.Services.Enums;
using CGPlugin.Services.Interfaces;

/// <summary>
///     Сервис для работы с диалоговыми окнами на основе MessageBox из Windows.Forms
/// </summary>
public class DisplayMessageService : IMessageService
{
    /// <inheritdoc />
    public void Show(string header, string text, MessageType type)
    {
        MessageBoxIcon icon = new();
        var buttons = MessageBoxButtons.OK;

        switch (type)
        {
            case MessageType.Error:
                icon = MessageBoxIcon.Error;
                break;
            case MessageType.Warning:
                icon = MessageBoxIcon.Warning;
                break;
            case MessageType.Information:
                icon = MessageBoxIcon.Information;
                break;
        }

        MessageBox.Show(text, header, buttons, icon);
    }
}