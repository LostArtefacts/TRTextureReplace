using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using TRTextureReplace.Models;
using TRTextureReplace.Utils;

namespace TRTextureReplace.Controls;

public partial class SaveWindow : Window
{
    public SaveWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowUtils.TidyMenu(this);
        WindowUtils.EnableCloseButton(this, false);
        SaveWindowViewModel model = DataContext as SaveWindowViewModel;
        model.Logger.LogChanged += Logger_LogChanged;
        model.PropertyChanged += Model_PropertyChanged;

        model.Save();
    }

    private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            SaveWindowViewModel model = DataContext as SaveWindowViewModel;
            if ((e.PropertyName == nameof(model.Complete) && model.Complete)
                || (e.PropertyName == nameof(model.Cancelled) && model.Cancelled))
            {
            
                WindowUtils.EnableCloseButton(this, true);
                Title = model.Cancelled ? "Saving - Cancelled" : "Saving - Done";
            }
        });
    }

    private void Logger_LogChanged(object sender, LogArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            _logBox.AppendText(e.Message + Environment.NewLine);
            _logBox.ScrollToEnd();
        });
    }
}
