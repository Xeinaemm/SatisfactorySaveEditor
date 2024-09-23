using SatisfactorySaveParser;

namespace SatisfactorySaveEditor.Util;

/// <summary>
///     Helper class to build editor node tree
/// </summary>
public class EditorTreeNode(string name)
{
    /// <summary>
    ///     Name of this specific node in the tree
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    ///     SaveObjects located at this specific node in the tree
    ///     May be empty if not at an end of the tree
    /// </summary>
    public List<SaveObject> Content { get; set; } = [];

    /// <summary>
    ///     Child nodes below this node in the tree
    /// </summary>
    public Dictionary<string, EditorTreeNode> Children { get; set; } = [];

    public override string ToString() => $"{Name} ({Children.Count})";

    public void AddChild(IEnumerable<string> path, SaveObject entry)
    {
        if (!path.Any())
        {
            Content.Add(entry);
            return;
        }

        var first = path.First();
        if (!Children.TryGetValue(first, out var child))
        {
            child = Children[first] = new EditorTreeNode(first);
        }

        child.AddChild(path.Skip(1), entry);
    }
}
