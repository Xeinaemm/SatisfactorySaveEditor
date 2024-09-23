using SatisfactorySaveParser.Save;

namespace SatisfactorySaveParser.Exceptions;

public class UnknownSaveVersionException(SaveHeaderVersion saveVersion) : Exception
{
    public SaveHeaderVersion SaveVersion { get; set; } = saveVersion;
}
