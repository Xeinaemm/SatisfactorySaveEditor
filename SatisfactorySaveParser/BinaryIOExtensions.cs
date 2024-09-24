﻿using SatisfactorySaveParser.Save;
using SatisfactorySaveParser.Structures;
using System.Text;

namespace SatisfactorySaveParser;

public static class BinaryIOExtensions
{
    public static char[] ReadCharArray(this BinaryReader reader)
    {
        var count = reader.ReadInt32();
        if (count >= 0)
        {
            return reader.ReadChars(count);
        }
        else
        {
            var bytes = reader.ReadBytes(count * -2);
            return Encoding.Unicode.GetChars(bytes);
        }
    }

    public static string ReadLengthPrefixedString(this BinaryReader reader) => new string(reader.ReadCharArray()).TrimEnd('\0');

    public static void WriteLengthPrefixedString(this BinaryWriter writer, string str)
    {
        if (str == null || str.Length == 0)
        {
            writer.Write(0);
            return;
        }

        if (str.Any(c => c > 127))
        {
            var bytes = Encoding.Unicode.GetBytes(str);

            writer.Write(-((bytes.Length / 2) + 1));
            writer.Write(bytes);
            writer.Write((short)0);
        }
        else
        {
            var bytes = Encoding.ASCII.GetBytes(str);

            writer.Write(bytes.Length + 1);
            writer.Write(bytes);
            writer.Write((byte)0);
        }
    }

    public static int GetSerializedLength(this string str)
    {
        if (str == null || str.Length == 0)
            return 4;

        return str.Any(c => c > 127) ? (str.Length * 2) + 6 : str.Length + 5;
    }

    public static Vector2 ReadVector2(this BinaryReader reader) => new()
    {
        X = reader.ReadSingle(),
        Y = reader.ReadSingle(),
    };

    public static void Write(this BinaryWriter writer, Vector2 vec)
    {
        writer.Write(vec.X);
        writer.Write(vec.Y);
    }

    public static Vector3 ReadVector3(this BinaryReader reader) => new()
    {
        X = reader.ReadSingle(),
        Y = reader.ReadSingle(),
        Z = reader.ReadSingle()
    };

    public static void Write(this BinaryWriter writer, Vector3 vec)
    {
        writer.Write(vec.X);
        writer.Write(vec.Y);
        writer.Write(vec.Z);
    }

    public static Vector4 ReadVector4(this BinaryReader reader) => new()
    {
        X = reader.ReadSingle(),
        Y = reader.ReadSingle(),
        Z = reader.ReadSingle(),
        W = reader.ReadSingle()
    };

    public static void Write(this BinaryWriter writer, Vector4 vec)
    {
        writer.Write(vec.X);
        writer.Write(vec.Y);
        writer.Write(vec.Z);
        writer.Write(vec.W);
    }

    public static void Write(this BinaryWriter writer, ChunkInfo info)
    {
        writer.Write(info.CompressedSize);
        writer.Write(info.UncompressedSize);
    }
}
