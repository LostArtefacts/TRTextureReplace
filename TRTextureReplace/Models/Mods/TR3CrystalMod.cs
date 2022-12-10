using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TRLevelReader.Model.Enums;
using TRModelTransporter.Packing;
using TRTexture16Importer;
using TRTexture16Importer.Helpers;
using TRTextureReplace.Utils;

namespace TRTextureReplace.Models;

public class TR3CrystalMod : BaseTextureMod
{
    public override string Title => "TR3 Crystals";

    private readonly EnumProperty _crystalMode;

    public TR3CrystalMod()
    {
        List<EnumOption> options = new()
        {
            new EnumOption
            {
                ID = (int)CrystalMode.PSX,
                Title = "PSX"
            },
            new EnumOption
            {
                ID = (int)CrystalMode.PC,
                Title = "PC"
            }
        };
        Properties.Add(_crystalMode = new EnumProperty
        {
            Description = "Change TR3 crystals to match the PSX version, or use the original PC version.",
            Options = options,
            Value = options.First()
        });
    }

    public override bool IsSupported(uint version)
    {
        return version == TRVersion.TR3a
            || version == TRVersion.TR3b;
    }

    public override void Apply(TR3TexturePacker packer)
    {
        BitmapGraphics crystal = GetResourceBitmap("tr3crystal.png");
        int y = (CrystalMode)_crystalMode.Value.ID == CrystalMode.PSX ? 0 : 16;
        
        using Bitmap largeClip = crystal.Extract(new Rectangle(0, y, 32, 16));
        using Bitmap smallClip = crystal.Extract(new Rectangle(32, y, 16, 16));
        
        Dictionary<TexturedTile, List<TexturedTileSegment>> segments = packer.GetModelSegments(TR3Entities.SaveCrystal_P);
        foreach (TexturedTile tile in segments.Keys)
        {
            using BitmapGraphics tileBmp = new(packer.GetTile(tile.Index));
            foreach (TexturedTileSegment seg in segments[tile])
            {
                tileBmp.Import(seg.Bounds.Width == largeClip.Width ? largeClip : smallClip, seg.Bounds);
            }
            packer.Level.Images16[tile.Index].Pixels = TextureUtilities.ImportFromBitmap(tileBmp.Bitmap);
        }
    }
}

public enum CrystalMode
{
    PSX, PC
}
