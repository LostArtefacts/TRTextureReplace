using System;

namespace TRTextureReplace.Utils;

public class Logger
{
    public event EventHandler<LogArgs> LogChanged;

    public void Log(string message)
    {
        LogChanged?.Invoke(this, new LogArgs(message));
    }
}

public class LogArgs : EventArgs
{
    public string Message { get; private set; }
    public LogArgs(string message)
    {
        Message = message;
    }
}
