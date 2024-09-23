using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveEditor.Util;
using SatisfactorySaveParser.PropertyTypes;
using System.Collections.ObjectModel;

namespace SatisfactorySaveEditor.ViewModel.Property;

public partial class ArrayPropertyViewModel : SerializedPropertyViewModel
{
    private readonly ArrayProperty model;

    [ObservableProperty]
    private bool isExpanded;

    public IRelayCommand AddElementCommand => new RelayCommand(AddElement);
    public IRelayCommand RemoveElementCommand => new RelayCommand<SerializedPropertyViewModel>(RemoveElement);

    public ObservableCollection<SerializedPropertyViewModel> Elements { get; }

    public string Type => model.Type;

    public override string ShortName => $"Array ({AddViewModel.FromStringType(Type)})";

    public ArrayPropertyViewModel(ArrayProperty arrayProperty) : base(arrayProperty)
    {
        model = arrayProperty;

        Elements = new ObservableCollection<SerializedPropertyViewModel>(arrayProperty.Elements.Select(PropertyViewModelMapper.Convert));
        for (var i = 0; i < Elements.Count; i++)
            Elements[i].Index = i.ToString();

        IsExpanded = Elements.Count <= 3;
    }

    private void AddElement()
    {
        // TODO: Is copying the last PropertyName ok?
        var property = AddViewModel.CreateProperty(AddViewModel.FromStringType(Type), Elements.Last().PropertyName);
        var viewModel = PropertyViewModelMapper.Convert(property);
        viewModel.Index = Elements.Count.ToString();

        Elements.Add(viewModel);
    }

    private void RemoveElement(SerializedPropertyViewModel property) => Elements.Remove(property);

    public override void ApplyChanges()
    {
        model.Elements.Clear();
        foreach (var element in Elements)
        {
            element.ApplyChanges();
            model.Elements.Add(element.Model);
        }
    }
}
