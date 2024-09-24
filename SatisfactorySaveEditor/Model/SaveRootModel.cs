using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser.Save;

namespace SatisfactorySaveEditor.Model;

public partial class SaveRootModel : SaveObjectModel
{
    private readonly FSaveHeader model;

    [ObservableProperty]
    private int buildVersion;

    [ObservableProperty]
    private string mapName;

    [ObservableProperty]
    private string mapOptions;

    [ObservableProperty]
    private string sessionName;

    [ObservableProperty]
    private int playDuration;

    [ObservableProperty]
    private long saveDateTime;

    [ObservableProperty]
    private ESessionVisibility sessionVisibility;

    public SaveHeaderVersion HeaderVersion => model.HeaderVersion;

    public FSaveCustomVersion SaveVersion => model.SaveVersion;

    public SaveRootModel(FSaveHeader header) : base(header.SessionName)
    {
        model = header;
        Type = "Root";

        buildVersion = model.BuildVersion;
        mapName = model.MapName;
        mapOptions = model.MapOptions;
        sessionName = model.SessionName;
        playDuration = model.PlayDuration;
        sessionVisibility = model.SessionVisibility;
        saveDateTime = model.SaveDateTime;
    }

    public override void ApplyChanges()
    {
        base.ApplyChanges();

        model.BuildVersion = BuildVersion;
        model.MapName = MapName;
        model.MapOptions = MapOptions;
        model.SessionName = SessionName;
        model.PlayDuration = PlayDuration;
        model.SessionVisibility = SessionVisibility;
        model.SaveDateTime = SaveDateTime;
    }
}
