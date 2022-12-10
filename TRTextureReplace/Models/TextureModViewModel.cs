using System.Collections.Generic;

namespace TRTextureReplace.Models;

public class TextureModViewModel
{
    public BaseTextureMod TextureMod { get; private set; }

    public TextureModViewModel(BaseTextureMod textureMod)
    {
        TextureMod = textureMod;
    }

    public bool Enabled
    {
        get => TextureMod.Enabled;
        set => TextureMod.Enabled = value;
    }

    public string Title
    {
        get => TextureMod.Title;
    }

    public List<BaseProperty> Properties
    {
        get => TextureMod.Properties;
    }
}
