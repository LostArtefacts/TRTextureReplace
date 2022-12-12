using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TRLevelReader;
using TRLevelReader.Model;
using TRModelTransporter.Packing;
using TRTextureReplace.Utils;

namespace TRTextureReplace.Models;

public class TextureModifier
{
    private static readonly List<string> _levelExtensions = new List<string>
    {
        ".PHD", ".TR2"
    };

    private readonly string _dataFolder;
    private readonly IEnumerable<BaseTextureMod> _mods;

    public TextureModifier(string dataFolder, IEnumerable<BaseTextureMod> mods)
    {
        _dataFolder = dataFolder;
        _mods = mods;
    }

    public async Task Run(Logger logger, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            Dictionary<uint, List<string>> levelMap = new();
            logger.Log("Scanning " + _dataFolder);

            foreach (string file in Directory.GetFiles(_dataFolder))
            {
                if (_levelExtensions.Contains(Path.GetExtension(file).ToUpper()))
                {
                    uint version = GetLevelVersion(file);
                    if (!levelMap.ContainsKey(version))
                    {
                        levelMap[version] = new();
                    }
                    levelMap[version].Add(file);
                }
            }

            if (levelMap.Count == 0)
            {
                logger.Log("No level files identified");
            }
            else
            {
                foreach (uint version in levelMap.Keys)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    switch (version)
                    {
                        case TRVersion.TR1:
                            RunTR1(levelMap[version], logger, cancellationToken);
                            break;
                        case TRVersion.TR2:
                            RunTR2(levelMap[version], logger, cancellationToken);
                            break;
                        case TRVersion.TR3a:
                            RunTR3(levelMap[version], logger, cancellationToken);
                            break;
                    }
                }
            }

            logger.Log("Done");
        }, cancellationToken);
    }

    private uint GetLevelVersion(string file)
    {
        using BinaryReader reader = new(File.OpenRead(file));
        uint version = reader.ReadUInt32();
        if (version == TRVersion.TR3b)
        {
            version = TRVersion.TR3a;
        }
        return version;
    }

    private void RunTR1(List<string> files, Logger logger, CancellationToken cancellationToken)
    {
        TR1LevelReader reader = new();
        TR1LevelWriter writer = new();
        foreach (string file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                TRLevel level = reader.ReadLevel(file);
                using TR1TexturePacker packer = new(level);
                foreach (BaseTextureMod mod in _mods)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (mod.IsSupported(TRVersion.TR1))
                    {
                        logger.Log(string.Format("Running {0} mod on {1}", mod.Title, Path.GetFileName(file)));
                        mod.Apply(packer);
                    }
                    else
                    {
                        logger.Log(string.Format("{0} mod not supported for {1}", mod.Title, Path.GetFileName(file)));
                    }
                }

                writer.WriteLevelToFile(level, file);
            }
            catch (Exception e)
            {
                logger.Log(e.Message);
            }
        }
    }

    private void RunTR2(List<string> files, Logger logger, CancellationToken cancellationToken)
    {
        TR2LevelReader reader = new();
        TR2LevelWriter writer = new();
        foreach (string file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                TR2Level level = reader.ReadLevel(file);
                using TR2TexturePacker packer = new(level);
                foreach (BaseTextureMod mod in _mods)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (mod.IsSupported(TRVersion.TR2))
                    {
                        logger.Log(string.Format("Running {0} mod on {1}", mod.Title, Path.GetFileName(file)));
                        mod.Apply(packer);
                    }
                    else
                    {
                        logger.Log(string.Format("{0} mod not supported for {1}", mod.Title, Path.GetFileName(file)));
                    }
                }

                writer.WriteLevelToFile(level, file);
            }
            catch (Exception e)
            {
                logger.Log(e.Message);
            }
        }
    }

    private void RunTR3(List<string> files, Logger logger, CancellationToken cancellationToken)
    {
        TR3LevelReader reader = new();
        TR3LevelWriter writer = new();
        foreach (string file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                TR3Level level = reader.ReadLevel(file);
                using TR3TexturePacker packer = new(level);
                foreach (BaseTextureMod mod in _mods)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (mod.IsSupported(TRVersion.TR3a))
                    {
                        logger.Log(string.Format("Running {0} mod on {1}", mod.Title, Path.GetFileName(file)));
                        mod.Apply(packer);
                    }
                    else
                    {
                        logger.Log(string.Format("{0} mod not supported for {1}", mod.Title, Path.GetFileName(file)));
                    }
                }

                writer.WriteLevelToFile(level, file);
            }
            catch (Exception e)
            {
                logger.Log(e.Message);
            }
        }
    }
}
