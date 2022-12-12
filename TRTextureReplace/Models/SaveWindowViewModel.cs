using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TRTextureReplace.Utils;

namespace TRTextureReplace.Models;

public class SaveWindowViewModel : BaseNotifyPropertyChanged
{
    private readonly TextureModifier _modifier;

    public Logger Logger { get; private set; }

    private bool _complete;
    public bool Complete
    {
        get => _complete;
        private set
        {
            _complete = value;
            NotifyPropertyChanged();
            NotifyCommandsChanged();
        }
    }

    private bool _cancelled;
    public bool Cancelled
    {
        get => _cancelled;
        private set
        {
            _cancelled = value;
            NotifyPropertyChanged();
            NotifyCommandsChanged();
        }
    }

    private void NotifyCommandsChanged()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            CloseCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        });
    }

    private readonly CancellationTokenSource _cancellationToken;

    public SaveWindowViewModel(string dataFolder, IEnumerable<BaseTextureMod> mods, CancellationTokenSource cancellationToken)
    {
        _modifier = new(dataFolder, mods);
        Logger = new();
        _cancellationToken = cancellationToken;
    }

    public void Save()
    {
        Task.Run(async () =>
        {
            try
            {
                await _modifier.Run(Logger, _cancellationToken.Token);
                Complete = true;
            }
            catch (OperationCanceledException e)
            {
                Logger.Log(e.Message);
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
        }, _cancellationToken.Token);
    }

    private RelayCommand<Window> _closeCommand;
    public RelayCommand<Window> CloseCommand
    {
        get => _closeCommand ??= new RelayCommand<Window>(Close, CanClose);
    }

    private void Close(Window window)
    {
        window.Close();
    }

    private bool CanClose(Window window)
    {
        return Complete;
    }

    private RelayCommand _cancelCommand;
    public RelayCommand CancelCommand
    {
        get => _cancelCommand ??= new RelayCommand(Cancel, CanCancel);
    }

    public void Cancel()
    {
        if (!Complete && !_cancellationToken.IsCancellationRequested)
        {
            Cancelled = true;
            _cancellationToken.Cancel();
        }
    }

    private bool CanCancel()
    {
        return !Cancelled && !Complete;
    }
}
