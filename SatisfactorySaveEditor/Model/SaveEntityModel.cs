using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactorySaveParser;
using Vector3 = SatisfactorySaveParser.Structures.Vector3;
using Vector4 = SatisfactorySaveParser.Structures.Vector4;

namespace SatisfactorySaveEditor.Model;

public partial class SaveEntityModel : SaveObjectModel
{
    [ObservableProperty]
    private bool needTransform;

    [ObservableProperty]
    private bool wasPlacedInLevel;

    [ObservableProperty]
    private Vector4 rotation;

    [ObservableProperty]
    private Vector3 position;

    [ObservableProperty]
    private Vector3 scale;

    [ObservableProperty]
    private string parentObjectRoot;

    [ObservableProperty]
    private string parentObjectName;

    public SaveEntityModel(SaveEntity ent) : base(ent)
    {
        NeedTransform = ent.NeedTransform;
        WasPlacedInLevel = ent.WasPlacedInLevel;
        ParentObjectRoot = ent.ParentObjectRoot;
        ParentObjectName = ent.ParentObjectName;

        Rotation = ent.Rotation;
        Position = ent.Position;
        Scale = ent.Scale;
    }

    public override void ApplyChanges()
    {
        base.ApplyChanges();

        var model = (SaveEntity)Model;

        model.NeedTransform = NeedTransform;
        model.Rotation = Rotation;
        model.Position = Position;
        model.Scale = Scale;
        model.WasPlacedInLevel = WasPlacedInLevel;
        model.ParentObjectRoot = ParentObjectRoot;
        model.ParentObjectName = ParentObjectName;
    }
}
