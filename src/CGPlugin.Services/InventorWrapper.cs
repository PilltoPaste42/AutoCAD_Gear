namespace CGPlugin.Services;

using System;

using Inventor;

/// <summary>
///     Сервис для подключения к API Autodesk Inventor
/// </summary>
public static class InventorWrapper
{
    private static Application? _app;

    /// <summary>
    ///     Получение ссылки на экземпляр приложения Autodesk Inventor
    /// </summary>
    /// <returns>
    ///     Ссылка на экземпляр Inventor.Application
    /// </returns>
    /// <exception cref="ApplicationException"></exception>
    public static Application Connect()
    {
        if (_app != null) return _app;

        try
        {
            var applicationType = Type.GetTypeFromProgID("Inventor.Application");

            _app = (Application)Activator.CreateInstance(applicationType)!;
        }
        catch (Exception)
        {
            throw new ApplicationException(@"Failed to start Autodesk Inventor.");
        }

        return _app;
    }
}