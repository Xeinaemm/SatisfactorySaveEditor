using SatisfactorySaveParser.Save;

namespace SatisfactorySaveParser.Exceptions;

public class UnknownBuildVersionException(FSaveCustomVersion buildVersion) : Exception
{
    public FSaveCustomVersion BuildVersion { get; set; } = buildVersion;
}
