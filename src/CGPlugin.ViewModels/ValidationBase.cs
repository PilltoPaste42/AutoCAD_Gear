namespace CGPlugin.ViewModels;

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
///     Базовый класс для реализации валидации
/// </summary>
public abstract class ValidationBase : INotifyPropertyChanged, INotifyDataErrorInfo
{
    /// <inheritdoc chref="base" />
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <inheritdoc chref="base" />
    public virtual IEnumerable GetErrors([CallerMemberName] string propertyName = "")
    {
        return Enumerable.Empty<object>();
    }

    /// <inheritdoc chref="base" />
    public virtual bool HasErrors => GetErrors().OfType<object>().Any();

    /// <inheritdoc chref="base" />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Вызов события ErrorsChanged
    /// </summary>
    /// <param name="propertyName">Валидируемое свойство</param>
    protected void OnErrorsChanged([CallerMemberName] string propertyName = "")
    {
        if (ErrorsChanged != null)
        {
            OnErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    ///     Вызов события ErrorsChanged
    /// </summary>
    /// <param name="sender">Отправитель события</param>
    /// <param name="e">Входные параметры события</param>
    protected virtual void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
        var handler = ErrorsChanged;
        handler?.Invoke(sender, e);
    }

    /// <summary>
    ///     Вызов события PropertyChanged
    /// </summary>
    /// <param name="propertyName">Имя измененного свойства</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///     Вызов события PropertyChanged
    /// </summary>
    /// <param name="sender">Отправитель события</param>
    /// <param name="e">Входные параметры события</param>
    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var handler = PropertyChanged;
        handler?.Invoke(sender, e);
    }
}