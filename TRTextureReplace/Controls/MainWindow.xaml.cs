using System.Windows;
using TRTextureReplace.Models;

namespace TRTextureReplace.Controls;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
