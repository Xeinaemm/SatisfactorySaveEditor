using SatisfactorySaveEditor.Model;
using SatisfactorySaveEditor.ViewModel.Property;
using SatisfactorySaveParser;
using SatisfactorySaveParser.PropertyTypes;
using SatisfactorySaveParser.PropertyTypes.Structs;
using SatisfactorySaveParser.Structures;
using System.IO;
using System.Windows;

namespace SatisfactorySaveEditor.Cheats;

public class MassDismantleCheat : ICheat
{
    public string Name => "Mass dismantle...";

    public static int GetNextStorageID(int currentId, SaveObjectModel rootItem)
    {
        while (rootItem.FindChild($"Persistent_Level:PersistentLevel.BP_Crate_C_{currentId}.inventory", false) != null)
            currentId++;
        return currentId;
    }

    public static bool IsPointInPolygon(Vector3 p, Vector3[] polygon)
    {
        var minX = polygon[0].X;
        var maxX = polygon[0].X;
        var minY = polygon[0].Y;
        var maxY = polygon[0].Y;
        for (var i = 1; i < polygon.Length; i++)
        {
            var q = polygon[i];
            minX = Math.Min(q.X, minX);
            maxX = Math.Max(q.X, maxX);
            minY = Math.Min(q.Y, minY);
            maxY = Math.Max(q.Y, maxY);
        }

        if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            return false;

        var inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                 p.X < ((polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y)) + polygon[i].X)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    private Vector3[] polygon;
    private float minZ, maxZ;

    private void BuildPolygon()
    {
        var points = new List<Vector3>();
        var done = false;
        while (!done)
        {
            var massDismantleWindow = new MassDismantleWindow();
            if (!massDismantleWindow.ShowDialog().Value)
                break;
            if (!massDismantleWindow.ResultSet)
                break;
            points.Add(massDismantleWindow.Result);
            if (massDismantleWindow.Done)
                done = true;
        }
        polygon = [.. points];
        var zWindow = new MassDismantleWindow(isZWindow: true);
        if (!zWindow.ShowDialog().Value || !zWindow.ResultSet)
        {
            minZ = float.NegativeInfinity;
            maxZ = float.PositiveInfinity;
        }
        else
        {
            minZ = zWindow.Result.X;
            maxZ = zWindow.Result.Y;
        }
    }

    public static byte[] PrepareForParse(string itemPath, int itemAmount)
    {
        using (var ms = new MemoryStream())
        {
            using (var writer = new BinaryWriter(ms))
            {
                writer.WriteLengthPrefixedString("mInventoryStacks");
                writer.WriteLengthPrefixedString("StructProperty");
                writer.Write("Item".GetSerializedLength() + "StructProperty".GetSerializedLength() + 4 + 4 + "InventoryItem".GetSerializedLength() + 4 + 4 + 4 + 4 + 1 + 4 + itemPath.GetSerializedLength() + "".GetSerializedLength() + "".GetSerializedLength() + "NumItems".GetSerializedLength() + "IntProperty".GetSerializedLength() + 4 + 4 + 1 + 4 + "None".GetSerializedLength()); // TODO
                writer.Write(0);
                writer.WriteLengthPrefixedString("InventoryStack");
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write((byte)0);
                writer.WriteLengthPrefixedString("Item");
                writer.WriteLengthPrefixedString("StructProperty");
                writer.Write(4 + itemPath.GetSerializedLength() + "".GetSerializedLength() + "".GetSerializedLength());
                writer.Write(0);
                writer.WriteLengthPrefixedString("InventoryItem"); // ItemType
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write((byte)0);
                writer.Write(0); // Unknown1
                writer.WriteLengthPrefixedString(itemPath); // ItemType
                writer.WriteLengthPrefixedString(""); // Unknown2
                writer.WriteLengthPrefixedString(""); // Unknown3
                writer.WriteLengthPrefixedString("NumItems");
                writer.WriteLengthPrefixedString("IntProperty");
                writer.Write(4); // Length
                writer.Write(0); // Index
                writer.Write((byte)0);
                writer.Write(itemAmount); // Value
                writer.WriteLengthPrefixedString("None");

            }
            return ms.ToArray();
        }
    }

    private int MassDismantle(List<SaveObjectModel> objects, ArrayProperty inventory, SaveObjectModel rootItem, int buildVersion)
    {
        var count = 0;
        foreach (var item in objects)
        {
            if (item is SaveEntityModel)
                if (IsPointInPolygon(((SaveEntityModel)item).Position, polygon) && minZ <= ((SaveEntityModel)item).Position.Z && ((SaveEntityModel)item).Position.Z <= maxZ)
                {
                    var dismantleRefund = ((SaveEntityModel)item).FindField<ArrayPropertyViewModel>("mDismantleRefund");
                    if (dismantleRefund != null)
                    {
                        foreach (var property in dismantleRefund.Elements)
                        {
                            var itemAmountStruct = (DynamicStructData)((StructProperty)property.Model).Data;
                            var itemPath = ((ObjectProperty)itemAmountStruct.Fields[0]).PathName;
                            var itemAmount = ((IntProperty)itemAmountStruct.Fields[1]).Value;
                            var bytes = PrepareForParse(itemPath, itemAmount);
                            using (var ms = new MemoryStream(bytes))
                            using (var reader = new BinaryReader(ms))
                            {
                                var prop = SerializedProperty.Parse(reader, buildVersion);
                                inventory.Elements.Add(prop);
                            }
                        }
                    }
                    if (item.FindField<ObjectPropertyViewModel>("mInventory") != null)
                        inventory.Elements.AddRange(((ArrayProperty)rootItem.FindChild(item.FindField<ObjectPropertyViewModel>("mInventory").Str2, false).FindField<ArrayPropertyViewModel>("mInventoryStacks").Model).Elements);
                    if (item.FindField<ObjectPropertyViewModel>("mStorageInventory") != null)
                        inventory.Elements.AddRange(((ArrayProperty)rootItem.FindChild(item.FindField<ObjectPropertyViewModel>("mStorageInventory").Str2, false).FindField<ArrayPropertyViewModel>("mInventoryStacks").Model).Elements);
                    if (item.FindField<ObjectPropertyViewModel>("mInputInventory") != null)
                        inventory.Elements.AddRange(((ArrayProperty)rootItem.FindChild(item.FindField<ObjectPropertyViewModel>("mInputInventory").Str2, false).FindField<ArrayPropertyViewModel>("mInventoryStacks").Model).Elements);
                    if (item.FindField<ObjectPropertyViewModel>("mOutputInventory") != null)
                        inventory.Elements.AddRange(((ArrayProperty)rootItem.FindChild(item.FindField<ObjectPropertyViewModel>("mOutputInventory").Str2, false).FindField<ArrayPropertyViewModel>("mInventoryStacks").Model).Elements);
                    // Unlink miners & geysers
                    if (item.Model.TypePath.StartsWith("/Game/FactoryGame/Buildable/Factory/Miner") || item.Model.TypePath.StartsWith("/Game/FactoryGame/Buildable/Factory/GeneratorGeoThermal") || item.Model.TypePath.StartsWith("/Game/FactoryGame/Buildable/Factory/OilPump"))
                    {
                        var resourceNode = item.FindField<ObjectPropertyViewModel>("mExtractResourceNode").Str2;
                        rootItem.FindChild(resourceNode, false).FindField<BoolPropertyViewModel>("mIsOccupied", property => property.Value = false);
                    }
                    var gameState = rootItem.FindChild("Persistent_Level:PersistentLevel.BP_GameState_C_*", false);
                    if (item.Model.TypePath.StartsWith("/Game/FactoryGame/Buildable/Factory/TradingPost/Build_TradingPost.Build_TradingPost_C"))
                        gameState.FindField<BoolPropertyViewModel>("mIsTradingPostBuilt", property => property.Value = false);
                    if (item.Model.TypePath.StartsWith("/Game/FactoryGame/Buildable/Factory/SpaceElevator/Build_SpaceElevator.Build_SpaceElevator_C"))
                        gameState.FindField<BoolPropertyViewModel>("mIsSpaceElevatorBuilt", property => property.Value = false);
                    rootItem.Remove(item);
                    count++;
                }
        }
        return count;
    }

    public bool Apply(SaveObjectModel rootItem, SatisfactorySave saveGame)
    {
        BuildPolygon();
        if (polygon.Length < 2)
        {
            MessageBox.Show("At least 2 points needed to mass dismantle", "Could not mass dismantle", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        var inventory = new ArrayProperty("mInventoryStacks")
        {
            Type = "StructProperty"
        };
        int countFactory = 0, countBuilding = 0, countCrate = 0;
        try
        {
            countFactory = MassDismantle(rootItem.FindChild("Buildable", true).FindChild("Factory", true).DescendantSelfViewModel, inventory, rootItem, saveGame.Header.BuildVersion);
        }
        catch (NullReferenceException) { }
        try
        {
            countBuilding = MassDismantle(rootItem.FindChild("Buildable", true).FindChild("Building", true).DescendantSelfViewModel, inventory, rootItem, saveGame.Header.BuildVersion);
        }
        catch (NullReferenceException) { }
        try
        {
            countCrate = MassDismantle(rootItem.FindChild("-Shared", true).FindChild("BP_Crate.BP_Crate_C", true).DescendantSelfViewModel, inventory, rootItem, saveGame.Header.BuildVersion);
        }
        catch (NullReferenceException) { }
        if(countFactory + countBuilding + countCrate == 0)
        {
            MessageBox.Show("Nothing was dismantled. Make sure the coordinates are correct and in clockwise order.", "Mass dismantle", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        var result = MessageBox.Show($"Dismantled {countFactory} factory buildings, {countBuilding} foundations and {countCrate} crates. Drop the items (including items in storages) in a single crate?", "Dismantled", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            CreateCrateEntityFromInventory(rootItem, inventory, saveGame.Header.BuildVersion);
        }
        return true;
    }

    public static SaveEntityModel CreateCrateEntityFromInventory(SaveObjectModel rootItem, ArrayProperty inventory, int buildVersion)
    {
        inventory = ArrangeInventory(inventory, buildVersion);
        var currentStorageID = GetNextStorageID(0, rootItem);
        var newInventory = new SaveComponent("/Script/FactoryGame.FGInventoryComponent", "Persistent_Level", $"Persistent_Level:PersistentLevel.BP_Crate_C_{currentStorageID}.inventory")
        {
            ParentEntityName = $"Persistent_Level:PersistentLevel.BP_Crate_C_{currentStorageID}",
            DataFields =
                [
                    inventory,
                    new ArrayProperty("mArbitrarySlotSizes")
                    {
                        Type = "IntProperty",
                        Elements = Enumerable.Repeat(new IntProperty("Element"){ Value = 0 }, inventory.Elements.Count).Cast<SerializedProperty>().ToList()
                    },
                    new ArrayProperty("mAllowedItemDescriptors")
                    {
                        Type = "ObjectProperty",
                        Elements = Enumerable.Repeat(new ObjectProperty("Element"){ LevelName = "", PathName = "" }, inventory.Elements.Count).Cast<SerializedProperty>().ToList()
                    },
                    new BoolProperty("mCanBeRearrange")
                    {
                        Value = false
                    }
                ]
        };
        rootItem.FindChild("FactoryGame.FGInventoryComponent", false).Items.Add(new SaveComponentModel(newInventory));
        var player = (SaveEntity)rootItem.FindChild("Char_Player.Char_Player_C", false).DescendantSelf[0];
        var newSaveObject = new SaveEntity("/Game/FactoryGame/-Shared/Crate/BP_Crate.BP_Crate_C", "Persistent_Level", $"Persistent_Level:PersistentLevel.BP_Crate_C_{currentStorageID}")
        {
            NeedTransform = true,
            Rotation = player.Rotation,
            Position = new Vector3() { X = player.Position.X, Y = player.Position.Y + 100, Z = player.Position.Z },
            Scale = new Vector3() { X = 1, Y = 1, Z = 1 },
            WasPlacedInLevel = false,
            ParentObjectName = "",
            ParentObjectRoot = "",
            DataFields =
            [
                new ObjectProperty("mInventory", 0) { LevelName = "Persistent_Level", PathName = $"Persistent_Level:PersistentLevel.BP_Crate_C_{currentStorageID}.inventory" }
            ]
        };
        if (rootItem.FindChild("Crate", false) == null)
            rootItem.FindChild("-Shared", false).Items.Add(new SaveObjectModel("Crate"));
        if (rootItem.FindChild("BP_Crate.BP_Crate_C", false) == null)
            rootItem.FindChild("Crate", false).Items.Add(new SaveObjectModel("BP_Crate.BP_Crate_C"));
        var crate = new SaveEntityModel(newSaveObject);
        rootItem.FindChild("BP_Crate.BP_Crate_C", false).Items.Add(crate);
        return crate;
    }



    public static ArrayProperty ArrangeInventory(ArrayProperty inventory, int buildVersion)
    {
        var stacks = new SortedDictionary<string, int>();
        foreach (var inventoryStruct in inventory.Elements.Cast<StructProperty>())
        {
            var inventoryStack = (DynamicStructData) inventoryStruct.Data;
            var inventoryItem = (InventoryItem)((StructProperty)inventoryStack.Fields[0]).Data;
            var itemCount = (IntProperty)inventoryStack.Fields[1];
            if (!stacks.ContainsKey(inventoryItem.ItemType))
                stacks[inventoryItem.ItemType] = 0;
            stacks[inventoryItem.ItemType] += itemCount.Value;
        }
        var newInventory = new ArrayProperty("mInventoryStacks")
        {
            Type = "StructProperty"
        };
        foreach(var itemStack in stacks)
        {
            var itemPath = itemStack.Key;
            if (string.IsNullOrWhiteSpace(itemPath))
                continue;
            var itemAmount = itemStack.Value;
            var bytes = PrepareForParse(itemPath, itemAmount);
            using (var ms = new MemoryStream(bytes))
            using (var reader = new BinaryReader(ms))
            {
                var prop = SerializedProperty.Parse(reader, buildVersion);
                newInventory.Elements.Add(prop);
            }
        }
        return newInventory;
    }
}
