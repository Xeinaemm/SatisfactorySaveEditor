using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactorySaveEditor.Util;
using SatisfactorySaveEditor.View;
using SatisfactorySaveEditor.ViewModel;
using SatisfactorySaveEditor.ViewModel.Property;
using SatisfactorySaveParser;
using System.Collections.ObjectModel;
using System.Windows;

namespace SatisfactorySaveEditor.Model;

public partial class SaveObjectModel : ObservableObject
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string rootObject;

    [ObservableProperty]
    private string type;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isExpanded;

    public RelayCommand CopyNameCommand { get; }
    public RelayCommand CopyPathCommand { get; }
    public RelayCommand AddPropertyCommand { get; }
    public RelayCommand<SerializedPropertyViewModel> RemovePropertyCommand { get; }

    public ObservableCollection<SaveObjectModel> Items { get; } = [];

    public ObservableCollection<SerializedPropertyViewModel> Fields { get; }

    /// <summary>
    /// Recursively gets all the SaveObject children in the tree plus self
    /// </summary>
    public List<SaveObject> DescendantSelf
    {
        get
        {
            var list = new List<SaveObject>();
            if (Model != null)
                list.Add(Model);

            foreach (var item in Items)
                list.AddRange(item.DescendantSelf);

            return list;
        }
    }

    /// <summary>
    /// Recursively gets all the SaveObjectModel children in the tree plus self
    /// </summary>
    public List<SaveObjectModel> DescendantSelfViewModel
    {
        get
        {
            var list = new List<SaveObjectModel>();
            if (Model != null)
                list.Add(this);

            foreach (var item in Items)
                list.AddRange(item.DescendantSelfViewModel);

            return list;
        }
    }

    public SaveObject Model { get; }

    public SaveObjectModel(SaveObject model)
    {
        Model = model;
        Title = model.InstanceName;
        RootObject = model.RootObject;
        Type = model.TypePath.Split('/').Last();

        Fields = new ObservableCollection<SerializedPropertyViewModel>(Model.DataFields.Select(PropertyViewModelMapper.Convert));

        CopyNameCommand = new RelayCommand(CopyName);
        CopyPathCommand = new RelayCommand(CopyPath);
        AddPropertyCommand = new RelayCommand(AddProperty);
        RemovePropertyCommand = new RelayCommand<SerializedPropertyViewModel>(RemoveProperty);
    }

    public SaveObjectModel(string title)
    {
        Title = title;
        Type = title;

        CopyNameCommand = new RelayCommand(CopyName);
    }

    /// <summary>
    /// Recursively walks through the save tree and finds a child node with 'name' title
    /// Expand also opens all parent nodes on return
    /// </summary>
    /// <param name="name">Name of the searched node</param>
    /// <param name="expand">Whether the parents of searched node should be expanded</param>
    /// <returns></returns>
    public SaveObjectModel FindChild(string name, bool expand)
    {
        if (Title == name)
        {
            if (expand)
                IsSelected = true;
            return this;
        }

        if (name.EndsWith('*') && Title.StartsWith(name[..^1]))
        {
            if (expand)
                IsSelected = true;
            return this;
        }

        foreach (var item in Items)
        {
            var foundChild = item.FindChild(name, expand);
            if (foundChild != null)
            {
                if (expand)
                    IsExpanded = true;
                return foundChild;
            }
        }

        return null;
    }

    public bool Remove(SaveObjectModel model)
    {
        if (this == model)
            throw new InvalidOperationException("Cannot remove root model");

        var found = Items.Remove(model);
        if (found)
            return true;

        foreach (var item in Items)
        {
            found = item.Remove(model);
            if (found)
                return true;
        }

        return false;
    }

    public override string ToString() => $"{Title} ({Items.Count})";

    public virtual void ApplyChanges()
    {
        foreach (var item in Items)
        {
            item.ApplyChanges();
        }

        // This is because the named only (pink) nodes aren't actually a valid object in the game
        if (Model == null)
            return;

        Model.InstanceName = Title;

        var newObjects = Fields.Select(vm => vm.Model).ToList();
        _ = Model.DataFields.RemoveAll(s => !newObjects.Contains(s));
        _ = newObjects.RemoveAll(Model.DataFields.Contains);
        Model.DataFields.AddRange(newObjects);

        foreach (var field in Fields)
            field.ApplyChanges();
    }

    public T FindField<T>(string fieldName, Action<T> edit = null) where T : SerializedPropertyViewModel
    {
        var field = Fields.FirstOrDefault(f => f.PropertyName == fieldName);

        if (field == null)
        {
            return null;
        }

        if (field is T vm)
        {
            edit?.Invoke(vm);
            return vm;
        }

        throw new InvalidOperationException($"A field with the name {fieldName} was found but with a different type ({field.GetType()} != {typeof(T)})");
    }

    public T FindOrCreateField<T>(string fieldName, Action<T> edit = null) where T : SerializedPropertyViewModel
    {
        var field = Fields.FirstOrDefault(f => f.PropertyName == fieldName);

        if (field == null)
        {
            var newVM = (T)PropertyViewModelMapper.Create<T>(fieldName);
            Fields.Add(newVM);

            edit?.Invoke(newVM);

            return newVM;
        }

        if (field is T vm)
        {
            edit?.Invoke(vm);
            return vm;
        }

        throw new InvalidOperationException($"A field with the name {fieldName} already exists but with a different type ({field.GetType()} != {typeof(T)})");
    }

    public virtual bool MatchesFilter(string filter) => Model?.InstanceName.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ?? false;

    private void AddProperty()
    {
        var window = new AddWindow
        {
            Owner = Application.Current.MainWindow
        };
        var avm = (AddViewModel)window.DataContext;
        avm.ObjectModel = this;
        _ = window.ShowDialog();
    }

    private void RemoveProperty(SerializedPropertyViewModel property) => Fields.Remove(property);

    private void CopyName() => Clipboard.SetText(Title);

    private void CopyPath() => Clipboard.SetText(Model.TypePath);
}
