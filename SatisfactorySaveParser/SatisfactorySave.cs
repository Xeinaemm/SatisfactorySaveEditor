﻿using SatisfactorySaveParser.Save;
using SatisfactorySaveParser.Structures;
using Serilog;
using System.Buffers;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace SatisfactorySaveParser;

/// <summary>
///     SatisfactorySave is the main class for parsing a savegame
/// </summary>
public class SatisfactorySave
{
    /// <summary>
    ///     Path to save on disk
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    ///     Header part of the save containing things like the version and metadata
    /// </summary>
    public FSaveHeader Header { get; private set; }

    /// <summary>
    ///     Main content of the save game
    /// </summary>
    public List<SaveObject> Entries { get; set; } = [];

    /// <summary>
    ///     List of object references of all collected objects in the world (Nut/berry bushes, slugs, etc)
    /// </summary>
    public List<ObjectReference> CollectedObjects { get; set; } = [];

    /// <summary>
    ///     Open a savefile from disk
    /// </summary>
    /// <param name="file">Full path to the .sav file, usually found in %localappdata%/FactoryGame/Saved/SaveGames</param>
    public SatisfactorySave(string file)
    {
        Log.Information($"Opening save file: {file}");

        FileName = Environment.ExpandEnvironmentVariables(file);

        //const int DefaultCopyBufferSize = 1024 * 1024;
        var fileHex = Convert.ToHexString(File.ReadAllBytes(FileName));
        var zlibFiles = Regex.Matches(fileHex, @"(?<=020000000000)789C(.*?)(?=C1832A9E)").Select(x => x.Value);
        using (var buffer = new MemoryStream())
        {
            foreach (var f in zlibFiles)
            {
                var zlibFile = Enumerable.Range(0, f.Length / 2).Select(x => Convert.ToByte(f.Substring(x * 2, 2), 16)).ToArray();
                using (var zStream = new ZLibStream(new MemoryStream(zlibFile), CompressionMode.Decompress, true))
                    zStream.CopyTo(buffer);
            }
            buffer.Position = 0;

#if DEBUG
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".bin"), buffer.ToArray());
#endif
            using (var bufferReader = new BinaryReader(buffer))
            {
                var dataLength = bufferReader.ReadInt32();
                LoadData(bufferReader);
            }
        }

//        using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, DefaultCopyBufferSize))
//        using (var reader = new BinaryReader(stream))
//        {
//            if (stream.Length == 0)
//                throw new Exception("Save file is completely empty");

//            Header = FSaveHeader.Parse(reader);

//            using (var buffer = new MemoryStream())
//            {
//                FSaveHeader.DumpData(reader);
//                while (stream.Position < stream.Length)
//                {
//                    using (var zStream = new ZLibStream(stream, CompressionMode.Decompress, true))
//                    {
//                        var bufferPool = new byte[DefaultCopyBufferSize];
//                        try
//                        {
//                            int bytesRead;
//                            while ((bytesRead = zStream.Read(bufferPool, 0, bufferPool.Length)) != 0)
//                            {
//                                buffer.Write(bufferPool, 0, bytesRead);
//                            }
//                        }
//                        finally
//                        {
//                            bufferPool = null;
//                        }
//                    }
//                }

//                buffer.Position = 0;

//#if DEBUG
//                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".bin"), buffer.ToArray());
//#endif
//                using (var bufferReader = new BinaryReader(buffer))
//                {
//                    var dataLength = bufferReader.ReadInt32();

//                    LoadData(bufferReader);
//                }
//            }
//        }
    }

    private void LoadData(BinaryReader reader)
    {
        // Does not need to be a public property because it's equal to Entries.Count
        var totalSaveObjects = reader.ReadUInt32();
        Log.Information($"Save contains {totalSaveObjects} object headers");

        // Saved entities loop
        for (var i = 0; i < totalSaveObjects; i++)
        {
            var type = reader.ReadInt32();
            switch (type)
            {
                case SaveEntity.TypeID:
                    Entries.Add(new SaveEntity(reader));
                    break;
                case SaveComponent.TypeID:
                    Entries.Add(new SaveComponent(reader));
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected type {type}");
            }
        }

        var totalSaveObjectData = reader.ReadInt32();
        Log.Information($"Save contains {totalSaveObjectData} object data");

        for (var i = 0; i < Entries.Count; i++)
        {
            var len = reader.ReadInt32();
            var before = reader.BaseStream.Position;

            Entries[i].ParseData(len, reader, Header.BuildVersion);
            var after = reader.BaseStream.Position;

            if (before + len != after)
            {
                throw new InvalidOperationException($"Expected {len} bytes read but got {after - before}");
            }
        }

        var collectedObjectsCount = reader.ReadInt32();
        Log.Information($"Save contains {collectedObjectsCount} collected objects");
        for (var i = 0; i < collectedObjectsCount; i++)
            CollectedObjects.Add(new ObjectReference(reader));

        Log.Debug($"Read {reader.BaseStream.Position} of total {reader.BaseStream.Length} bytes");
    }

    public void Save() => Save(FileName);

    public void Save(string file)
    {
        Log.Information($"Writing save file: {file}");

        FileName = Environment.ExpandEnvironmentVariables(file);
        using var stream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
        using var writer = new BinaryWriter(stream);
        stream.SetLength(0); // Clear any original content

        Header.Serialize(writer);

        using var buffer = new MemoryStream();
        using var bufferWriter = new BinaryWriter(buffer);
        bufferWriter.Write(0); // Placeholder size

        SaveData(bufferWriter, Header.BuildVersion);

        buffer.Position = 0;
        bufferWriter.Write((int)buffer.Length - 4);
        buffer.Position = 0;

        for (var i = 0; i < (int)Math.Ceiling((double)buffer.Length / ChunkInfo.ChunkSize); i++)
        {
            using var zBuffer = new MemoryStream();
            var remaining = (int)Math.Min(ChunkInfo.ChunkSize, buffer.Length - (ChunkInfo.ChunkSize * i));

            using (var zStream = new ZLibStream(zBuffer, CompressionLevel.Optimal))
            {
                var tmpBuf = new byte[remaining];
                _ = buffer.Read(tmpBuf, 0, remaining);
                zStream.Write(tmpBuf, 0, remaining);
            }

            writer.Write(new ChunkInfo()
            {
                CompressedSize = ChunkInfo.Magic,
                UncompressedSize = ChunkInfo.ChunkSize
            });

            writer.Write(new ChunkInfo()
            {
                CompressedSize = zBuffer.Length,
                UncompressedSize = remaining
            });

            writer.Write(new ChunkInfo()
            {
                CompressedSize = zBuffer.Length,
                UncompressedSize = remaining
            });

            writer.Write(zBuffer.ToArray());
        }
    }

    private void SaveData(BinaryWriter writer, int buildVersion)
    {
        writer.Write(Entries.Count);

        var entities = Entries.Where(e => e is SaveEntity).ToArray();
        for (var i = 0; i < entities.Length; i++)
        {
            writer.Write(SaveEntity.TypeID);
            entities[i].SerializeHeader(writer);
        }

        var components = Entries.Where(e => e is SaveComponent).ToArray();
        for (var i = 0; i < components.Length; i++)
        {
            writer.Write(SaveComponent.TypeID);
            components[i].SerializeHeader(writer);
        }

        writer.Write(entities.Length + components.Length);

        using (var ms = new MemoryStream())
        using (var dataWriter = new BinaryWriter(ms))
        {
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i].SerializeData(dataWriter, buildVersion);

                var bytes = ms.ToArray();
                writer.Write(bytes.Length);
                writer.Write(bytes);

                ms.SetLength(0);
            }
            for (var i = 0; i < components.Length; i++)
            {
                components[i].SerializeData(dataWriter, buildVersion);

                var bytes = ms.ToArray();
                writer.Write(bytes.Length);
                writer.Write(bytes);

                ms.SetLength(0);
            }
        }

        writer.Write(CollectedObjects.Count);
        foreach (var collectedObject in CollectedObjects)
        {
            writer.WriteLengthPrefixedString(collectedObject.LevelName);
            writer.WriteLengthPrefixedString(collectedObject.PathName);
        }
    }
}
