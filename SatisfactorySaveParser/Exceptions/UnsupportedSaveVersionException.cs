using SatisfactorySaveParser.Save;

namespace SatisfactorySaveParser.Exceptions;

public class UnsupportedSaveVersionException(SaveHeaderVersion saveVersion) : Exception
{
    public SaveHeaderVersion SaveVersion { get; set; } = saveVersion;
}
