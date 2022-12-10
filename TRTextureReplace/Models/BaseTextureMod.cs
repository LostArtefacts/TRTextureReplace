using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TRLevelReader.Model;
using TRModelTransporter.Packing;
using TRTexture16Importer.Helpers;
using TRTextureReplace.Utils;

namespace TRTextureReplace.Models;

public abstract class BaseTextureMod : BaseNotifyPropertyChanged
{
    private bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            NotifyPropertyChanged();
        }
    }

    public abstract string Title { get; }

    public List<BaseProperty> Properties { get; private set; } = new();

    public abstract bool IsSupported(uint version);

    public virtual void Apply(TR1TexturePacker packer)
    {
        throw new NotSupportedException();
    }

    public virtual void Apply(TR2TexturePacker packer)
    {
        throw new NotSupportedException();
    }

    public virtual void Apply(TR3TexturePacker packer)
    {
        throw new NotSupportedException();
    }

    protected BitmapGraphics GetResourceBitmap(string path)
    {
        path = "pack://application:,,,/TRTextureReplace;component/Resources/Mods/" + path;
        BitmapImage bitmapImage = new(new Uri(path, UriKind.Absolute));

        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        MemoryStream ms = new();
        encoder.Save(ms);
        ms.Flush();

        return new(new Bitmap(ms));
    }
}
