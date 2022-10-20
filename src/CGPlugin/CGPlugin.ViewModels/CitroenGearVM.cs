namespace CGPlugin.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using CGPlugin.Models;

/// <summary>
///     ViewModel для работы с данными из MainWindow 
/// </summary>
public class CitroenGearVM : INotifyPropertyChanged
{
    private readonly CitroenGearModel _gear;

    public CitroenGearVM()
    {
        _gear = new CitroenGearModel();
    }

    public uint GearDiameter
    {
        get => _gear.Diameter;
        set
        {
            _gear.Diameter = value;
            OnPropertyChanged();
        }
    }

    public uint GearModule
    {
        get => _gear.Module;
        set
        {
            _gear.Module = value;
            OnPropertyChanged();
        }
    }

    public int GearTeethAngle
    {
        get => _gear.TeethAngle;
        set
        {
            _gear.TeethAngle = value;
            OnPropertyChanged();
        }
    }

    public uint GearTeethCount
    {
        get => _gear.TeethCount;
        set
        {
            _gear.TeethCount = value;
            OnPropertyChanged();
        }
    }

    public uint GearWidth
    {
        get => _gear.Width;
        set
        {
            _gear.Width = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}