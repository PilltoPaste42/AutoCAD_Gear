namespace CGPlugin.Services.Interfaces;

using CGPlugin.Services.Enums;

/// <summary>
///     Интерфейс сервиса диалоговых окон
/// </summary>
public interface IMessageService
{
    /// <summary>
    ///     Вызов диалогового окна
    /// </summary>
    /// <param name="header"> Заголовок </param>
    /// <param name="text"> Текст сообщения </param>
    /// <param name="type"> Тип сообщения </param>
    public void Show(string header, string text, MessageType type);
}