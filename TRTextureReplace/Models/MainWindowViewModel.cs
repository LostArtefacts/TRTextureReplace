using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TRTextureReplace.Controls;
using TRTextureReplace.Utils;
using Application = System.Windows.Application;

namespace TRTextureReplace.Models;

public class MainWindowViewModel : BaseNotifyPropertyChanged
{
    private static readonly string _defaultDataName = "data";

    public IEnumerable<TextureModViewModel> TextureMods { get; private set; }

    public MainWindowViewModel()
    {
        // If running from the recommended location, initialise to the data folder here
        DataFolder = Directory.Exists(_defaultDataName) 
            ? _defaultDataName
            : Directory.GetCurrentDirectory();

        TextureMods = new List<TextureModViewModel>
        {
            new TextureModViewModel(new TR3CrystalMod
            {
                Enabled = true
            })
        };
        SelectedMod = TextureMods.FirstOrDefault();

        foreach (TextureModViewModel mod in TextureMods)
        {
            mod.TextureMod.PropertyChanged += TextureMod_PropertyChanged;
        }
        IsEditorDirty = TextureMods.Any(m => m.Enabled);
    }

    private void TextureMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        BaseTextureMod mod = sender as BaseTextureMod;
        if (e.PropertyName == nameof(mod.Enabled))
        {
            IsEditorDirty = TextureMods.Any(m => m.Enabled);
        }
    }

    private bool _isEditorDirty;
    public bool IsEditorDirty
    {
        get => _isEditorDirty;
        set
        {
            if (_isEditorDirty != value)
            {
                _isEditorDirty = value;
                NotifyPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private string _dataFolder;
    public string DataFolder
    {
        get => _dataFolder;
        set
        {
            _dataFolder = Path.GetFullPath(value);
            NotifyPropertyChanged();
        }
    }

    private TextureModViewModel _selectedMod;
    public TextureModViewModel SelectedMod
    {
        get => _selectedMod;
        set
        {
            _selectedMod = value;
            NotifyPropertyChanged();
        }
    }

    private RelayCommand _browseCommand;
    public ICommand BrowseCommand
    {
        get => _browseCommand ??= new RelayCommand(Browse);
    }

    private void Browse()
    {
        using FolderBrowserDialog browser = new()
        {
            UseDescriptionForTitle = true,
            Description = "Select data directory",
            InitialDirectory = DataFolder
        };
        if (browser.ShowDialog() == DialogResult.OK)
        {
            DataFolder = browser.SelectedPath;
        }
    }

    private RelayCommand _saveCommand;
    public RelayCommand SaveCommand
    {
        get => _saveCommand ??= new RelayCommand(Save, CanSave);
    }

    private void Save()
    {
        CancellationTokenSource cancellationToken = new();
        SaveWindow pw = new()
        {
            DataContext = new SaveWindowViewModel(DataFolder, TextureMods
                .Where(m => m.Enabled)
                .Select(m => m.TextureMod), cancellationToken),
            Owner = Application.Current.MainWindow
        };
        cancellationToken.Token.Register(() =>
        {
            pw.Close();
        });

        pw.ShowDialog();
    }

    private bool CanSave()
    {
        return IsEditorDirty;
    }

    private RelayCommand<Window> _exitCommand;
    public ICommand ExitCommand
    {
        get => _exitCommand ??= new RelayCommand<Window>(Exit);
    }

    private void Exit(Window window)
    {
        window.Close();
    }
}
