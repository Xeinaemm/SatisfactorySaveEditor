namespace SatisfactorySaveParser.Save;

public class ChunkInfo
{
    public const long Magic = 0xC1832A9E;
    public const int ChunkSize = 131072; // 128 KiB

    public long CompressedSize { get; set; }
    public long UncompressedSize { get; set; }
}
