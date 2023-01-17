namespace CGPlugin.Views;

using System.Windows;

using CGPlugin.ViewModels;

/// <summary>
///     Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new CitroenGearVM();
    }
}