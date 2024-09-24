using SatisfactorySaveParser.Save;

namespace SatisfactorySaveParser.Exceptions;

public class UnsupportedBuildVersionException(FSaveCustomVersion buildVersion) : Exception
{
    public FSaveCustomVersion BuildVersion { get; set; } = buildVersion;
}
