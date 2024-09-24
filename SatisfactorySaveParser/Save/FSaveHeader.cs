using SatisfactorySaveParser.Exceptions;
using Serilog;

namespace SatisfactorySaveParser.Save;

/// <summary>
///     Engine class: FSaveHeader
///     Header: FGSaveSystem.h
/// </summary>
public class FSaveHeader
{
    /// <summary>
    ///     Save version number
    /// </summary>
    public SaveHeaderVersion HeaderVersion { get; set; }
    /// <summary>
    ///     Save build (feature) number
    /// </summary>
    public FSaveCustomVersion SaveVersion { get; set; }

    /// <summary>
    ///     Unknown magic int
    ///     Seems to always be 66297
    /// </summary>
    public int BuildVersion { get; set; }

    /// <summary>
    ///     The name of what appears to be the root object of the save.
    ///     Seems to always be "Persistent_Level"
    /// </summary>
    public string MapName { get; set; }
    /// <summary>
    ///     An URL style list of arguments of the session.
    ///     Contains the startloc, sessionName and Visibility
    /// </summary>
    public string MapOptions { get; set; }
    /// <summary>
    ///     Name of the saved game as entered when creating a new game
    /// </summary>
    public string SessionName { get; set; }

    /// <summary>
    ///     Amount of seconds spent in this save
    /// </summary>
    public int PlayDuration { get; set; }
    /// <summary>
    ///     Unix timestamp of when the save was saved
    /// </summary>
    public long SaveDateTime { get; set; }

    public ESessionVisibility SessionVisibility { get; set; }

    /// <summary>
    ///     The FEditorObjectVersion that this save file was written with
    /// </summary>
    public int EditorObjectVersion { get; set; }

    /// <summary>
    ///     Generic MetaData - Requested by Mods
    /// </summary>
    public string ModMetadata { get; set; }

    /// <summary>
    ///     Was this save ever saved with mods enabled?
    /// </summary>
    public bool IsModdedSave { get; set; }

    /// <summary>
    ///     Header data unable to parse before magic hex
    /// </summary>
    public byte[] PreMagicHexDumpedBytes { get; set; }

    public void Serialize(BinaryWriter writer)
    {
        if (HeaderVersion != SaveHeaderVersion.SupportedVersion)
            throw new UnsupportedSaveVersionException(HeaderVersion);

        writer.Write((int)HeaderVersion);
        writer.Write((int)SaveVersion);
        writer.Write(BuildVersion);
        writer.WriteLengthPrefixedString(MapName);
        writer.WriteLengthPrefixedString(MapOptions);
        writer.WriteLengthPrefixedString(SessionName);
        writer.Write(PlayDuration);
        writer.Write(SaveDateTime);
        writer.Write((byte)SessionVisibility);
        writer.Write(EditorObjectVersion);
        writer.WriteLengthPrefixedString(ModMetadata);
        writer.Write(IsModdedSave ? 1 : 0);
    }

    public static FSaveHeader Parse(BinaryReader reader)
    {
        var header = new FSaveHeader
        {
            HeaderVersion = (SaveHeaderVersion)reader.ReadInt32(),
            SaveVersion = (FSaveCustomVersion)reader.ReadInt32(),
            BuildVersion = reader.ReadInt32(),

            MapName = reader.ReadLengthPrefixedString(),
            MapOptions = reader.ReadLengthPrefixedString(),
            SessionName = reader.ReadLengthPrefixedString(),

            PlayDuration = reader.ReadInt32(),
            SaveDateTime = reader.ReadInt64(),
            SessionVisibility = (ESessionVisibility)reader.ReadByte(),
            EditorObjectVersion = reader.ReadInt32(),
            ModMetadata = reader.ReadLengthPrefixedString(),
            IsModdedSave = reader.ReadInt32() > 0,
            PreMagicHexDumpedBytes = reader.ReadBytes(16 + 16 + 16 + 7), // dumped data
        };

        Log.Debug($"Read save header: \n" +
            $"HeaderVersion={header.HeaderVersion} \n" +
            $"SaveVersion={(int)header.SaveVersion} \n" +
            $"BuildVersion={header.BuildVersion} \n" +
            $"MapName={header.MapName} \n" +
            $"MapOpts={header.MapOptions} \n" +
            $"Session={header.SessionName} \n" +
            $"PlayTime={header.PlayDuration} \n" +
            $"SaveTime={header.SaveDateTime} \n" +
            $"SessionVisibility={header.SessionVisibility} \n" +
            $"EditorObjectVersion={header.EditorObjectVersion} \n" +
            $"ModMetadata={header.ModMetadata} \n" +
            $"IsModdedSave={header.IsModdedSave} \n" +
            $"PreMagicHexDumpedBytes={Convert.ToHexString(header.PreMagicHexDumpedBytes)} \n");

        if (header.HeaderVersion != SaveHeaderVersion.SupportedVersion)
            throw new UnsupportedSaveVersionException(header.HeaderVersion);

        if (header.SaveVersion != FSaveCustomVersion.SupportedVersion)
            throw new UnsupportedBuildVersionException(header.SaveVersion);

        return header;
    }

    public static void DumpData(BinaryReader reader)
    {
        var magicHex = Convert.ToInt64(Convert.ToHexString(reader.ReadBytes(4)), 16);
        if (magicHex != ChunkInfo.Magic)
            throw new Exception("Save file header mismatch. Unable to open save file.");
        reader.ReadBytes(16 + 16 + 13); // dumped data, next 2 bytes should be 78 9C (zlib header)
    }
}
