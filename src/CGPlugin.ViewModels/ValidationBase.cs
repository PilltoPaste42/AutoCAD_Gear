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
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public virtual IEnumerable GetErrors([CallerMemberName] string propertyName = "")
    {
        return Enumerable.Empty<object>();
    }

    public virtual bool HasErrors => GetErrors().OfType<object>().Any();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnErrorsChanged([CallerMemberName] string propertyName = "")
    {
        if (ErrorsChanged != null)
        {
            OnErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    protected virtual void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
        var handler = ErrorsChanged;
        handler?.Invoke(sender, e);
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var handler = PropertyChanged;
        handler?.Invoke(sender, e);
    }
}