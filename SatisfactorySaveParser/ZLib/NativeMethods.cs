using System.Runtime.InteropServices;

namespace SatisfactorySaveParser.ZLib;

internal static class NativeMethods
{
    private const string Name32 = "zlib32.dll";
    private const string Name64 = "zlib64.dll";
    private const string ZLibVersion = "1.2.8";

    [DllImport(Name32, EntryPoint = "inflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int inflateInit2_32(ref z_streamp strm, int windowBits, string version, int stream_size);
    [DllImport(Name64, EntryPoint = "inflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int inflateInit2_64(ref z_streamp strm, int windowBits, string version, int stream_size);

    [DllImport(Name32, EntryPoint = "deflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int deflateInit2_32(ref z_streamp strm, int level, int method, int windowBits, int memLevel, int strategy, string version, int stream_size);
    [DllImport(Name64, EntryPoint = "deflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int deflateInit2_64(ref z_streamp strm, int level, int method, int windowBits, int memLevel, int strategy, string version, int stream_size);

    [DllImport(Name32, EntryPoint = "inflate", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int inflate_32(ref z_streamp strm, ZLibFlush flush);
    [DllImport(Name64, EntryPoint = "inflate", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int inflate_64(ref z_streamp strm, ZLibFlush flush);

    [DllImport(Name32, EntryPoint = "deflate", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int deflate_32(ref z_streamp strm, ZLibFlush flush);
    [DllImport(Name64, EntryPoint = "deflate", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int deflate_64(ref z_streamp strm, ZLibFlush flush);

    [DllImport(Name32, EntryPoint = "inflateEnd", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int inflateEnd_32(ref z_streamp strm);
    [DllImport(Name64, EntryPoint = "inflateEnd", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int inflateEnd_64(ref z_streamp strm);

    [DllImport(Name32, EntryPoint = "deflateEnd", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int deflateEnd_32(ref z_streamp strm);
    [DllImport(Name64, EntryPoint = "deflateEnd", ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    static extern int deflateEnd_64(ref z_streamp strm);

    internal static ZLibReturnCode InflateInit(ref z_streamp strm, int windowBits) => Environment.Is64BitProcess
            ? (ZLibReturnCode)inflateInit2_64(ref strm, windowBits, ZLibVersion, Marshal.SizeOf(typeof(z_streamp)))
            : (ZLibReturnCode)inflateInit2_32(ref strm, windowBits, ZLibVersion, Marshal.SizeOf(typeof(z_streamp)));

    internal static ZLibReturnCode DeflateInit(ref z_streamp strm, CompressionLevel level, int windowBits) => Environment.Is64BitProcess
            ? (ZLibReturnCode)deflateInit2_64(ref strm, (int)level, (int)ZLibCompressionMethod.Deflated, windowBits, (int)ZLibMemLevel.Default, (int)ZLibCompressionStrategy.Default, ZLibVersion, Marshal.SizeOf(typeof(z_streamp)))
            : (ZLibReturnCode)deflateInit2_32(ref strm, (int)level, (int)ZLibCompressionMethod.Deflated, windowBits, (int)ZLibMemLevel.Default, (int)ZLibCompressionStrategy.Default, ZLibVersion, Marshal.SizeOf(typeof(z_streamp)));

    internal static ZLibReturnCode Inflate(ref z_streamp strm, ZLibFlush flush) => Environment.Is64BitProcess ? (ZLibReturnCode)inflate_64(ref strm, flush) : (ZLibReturnCode)inflate_32(ref strm, flush);

    internal static ZLibReturnCode Deflate(ref z_streamp strm, ZLibFlush flush) => Environment.Is64BitProcess ? (ZLibReturnCode)deflate_64(ref strm, flush) : (ZLibReturnCode)deflate_32(ref strm, flush);

    internal static ZLibReturnCode InflateEnd(ref z_streamp strm) => Environment.Is64BitProcess ? (ZLibReturnCode)inflateEnd_64(ref strm) : (ZLibReturnCode)inflateEnd_32(ref strm);

    internal static ZLibReturnCode DeflateEnd(ref z_streamp strm) => Environment.Is64BitProcess ? (ZLibReturnCode)deflateEnd_64(ref strm) : (ZLibReturnCode)deflateEnd_32(ref strm);
}
