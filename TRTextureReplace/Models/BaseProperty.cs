namespace TRTextureReplace.Models;

public abstract class BaseProperty : BaseNotifyPropertyChanged
{
    public abstract DataType DataType { get; }

    public string Description { get; set; }
}
