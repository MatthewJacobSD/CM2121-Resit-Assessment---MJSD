using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// EditMode tests for the bin acceptance matrix (source of truth:
/// RecycleBinInteractable.CalculateScore). These assert the documented
/// design: Nature accepts Plants, Plastic accepts Bottles, and General Waste
/// rewards both Toys and Bottles.
/// </summary>
public class RecycleBinMatrixTests
{
    private readonly List<GameObject> created = new List<GameObject>();

    private RecycleBinInteractable CreateBin(BinType binType)
    {
        GameObject go = new GameObject("Bin_" + binType);
        created.Add(go);
        RecycleBinInteractable bin = go.AddComponent<RecycleBinInteractable>();
        typeof(RecycleBinInteractable)
            .GetField("binType", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(bin, binType);
        return bin;
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in created)
        {
            if (go != null) Object.DestroyImmediate(go);
        }
        created.Clear();
    }

    [Test]
    public void NatureBin_AcceptsOnlyPlants()
    {
        RecycleBinInteractable bin = CreateBin(BinType.NatureRecycling);
        Assert.IsTrue(bin.AcceptsItem(ItemType.Plant));
        Assert.IsFalse(bin.AcceptsItem(ItemType.Toy));
        Assert.IsFalse(bin.AcceptsItem(ItemType.Bottle));
    }

    [Test]
    public void PlasticBin_AcceptsOnlyBottles()
    {
        RecycleBinInteractable bin = CreateBin(BinType.PlasticRecycling);
        Assert.IsTrue(bin.AcceptsItem(ItemType.Bottle));
        Assert.IsFalse(bin.AcceptsItem(ItemType.Plant));
        Assert.IsFalse(bin.AcceptsItem(ItemType.Toy));
    }

    [Test]
    public void GeneralWaste_AcceptsToysAndBottles_ButNotPlants()
    {
        // Author's intended design: General Waste rewards Bottles (+15) and Toys (+25).
        RecycleBinInteractable bin = CreateBin(BinType.GeneralWaste);
        Assert.IsTrue(bin.AcceptsItem(ItemType.Toy));
        Assert.IsTrue(bin.AcceptsItem(ItemType.Bottle));
        Assert.IsFalse(bin.AcceptsItem(ItemType.Plant));
    }

    [Test]
    public void EveryItemType_IsAcceptedByAtLeastOneBin()
    {
        Assert.IsTrue(CreateBin(BinType.NatureRecycling).AcceptsItem(ItemType.Plant));
        Assert.IsTrue(CreateBin(BinType.PlasticRecycling).AcceptsItem(ItemType.Bottle));
        Assert.IsTrue(CreateBin(BinType.GeneralWaste).AcceptsItem(ItemType.Toy));
    }
}
