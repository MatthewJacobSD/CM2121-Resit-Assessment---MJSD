using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// PlayMode tests for GameManager: category objectives, signed scoring,
/// win/lose conditions, plant chain bonus, pause/resume and timer expiry.
/// Each test builds its own manager instance, independent of the Florance scene.
/// </summary>
public class GameManagerPlayTests
{
    private GameManager manager;
    private ScoreManager score;

    [SetUp]
    public void SetUp()
    {
        ClearSingleton<GameManager>(default);
        ClearSingleton<ScoreManager>(default);

        manager = new GameObject("TestGameManager").AddComponent<GameManager>();
        score = new GameObject("TestScoreManager").AddComponent<ScoreManager>();
    }

    [TearDown]
    public void TearDown()
    {
        ClearSingleton<GameManager>(default);
        ClearSingleton<ScoreManager>(default);

        if (manager != null) Object.Destroy(manager.gameObject);
        if (score != null) Object.Destroy(score.gameObject);
    }

    private static void ClearSingleton<T>(T unused)
    {
        typeof(T).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)
            .SetValue(null, null);
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(obj, value);
    }

    private static GameObject CreateItem(ItemType type)
    {
        GameObject go = new GameObject("Item_" + type);
        go.AddComponent<Rigidbody>();
        PickupItem item = go.AddComponent<PickupItem>();
        typeof(PickupItem).GetField("itemType", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(item, type);
        return go;
    }

    [Test]
    public void StartGame_ResetsAllCountersAndRefillsLives()
    {
        manager.StartGame();
        manager.ReportRecycled(CreateItem(ItemType.Plant), 20);
        manager.StartGame();

        Assert.IsTrue(manager.IsPlaying);
        Assert.AreEqual(manager.MaxLives, manager.Lives);
        Assert.AreEqual(0, manager.PlantsRecycled);
        Assert.AreEqual(0, manager.ItemsRecycled);
        Assert.AreEqual(0, score.CurrentScore);
    }

    [Test]
    public void CorrectRecycles_AdvanceOnlyMatchingCategory()
    {
        manager.StartGame();
        manager.ReportRecycled(CreateItem(ItemType.Plant), 20);
        manager.ReportRecycled(CreateItem(ItemType.Toy), 25);
        manager.ReportRecycled(CreateItem(ItemType.Bottle), 20);

        Assert.AreEqual(1, manager.PlantsRecycled);
        Assert.AreEqual(1, manager.ToysRecycled);
        Assert.AreEqual(1, manager.BottlesRecycled);
        Assert.AreEqual(3, manager.ItemsRecycled);
        Assert.IsTrue(manager.IsPlaying);
    }

    [Test]
    public void WrongRecycle_CostsLife_NoProgress_AndNegativeScore()
    {
        manager.StartGame();
        int livesBefore = manager.Lives;
        manager.ReportRecycled(CreateItem(ItemType.Plant), -45);

        Assert.AreEqual(livesBefore - 1, manager.Lives);
        Assert.AreEqual(0, manager.PlantsRecycled);
        Assert.AreEqual(-45, score.CurrentScore);
    }

    [Test]
    public void WrongRecycle_ImmediatelyEndsGame_WhenScoreGoesNegative()
    {
        manager.StartGame();
        manager.ReportRecycled(CreateItem(ItemType.Bottle), -15);

        Assert.AreEqual(GameResult.Failure, manager.Result);
        Assert.IsFalse(manager.IsPlaying);
    }

    [Test]
    public void PerfectWin_RequiresAllCategoryCounts()
    {
        manager.StartGame();
        for (int i = 0; i < manager.PlantsRequired; i++)
            manager.ReportRecycled(CreateItem(ItemType.Plant), 20);
        for (int i = 0; i < manager.ToysRequired; i++)
            manager.ReportRecycled(CreateItem(ItemType.Toy), 25);
        for (int i = 0; i < manager.BottlesRequired; i++)
            manager.ReportRecycled(CreateItem(ItemType.Bottle), 20);

        Assert.AreEqual(manager.PlantsRequired, manager.PlantsRecycled);
        Assert.AreEqual(manager.ToysRequired, manager.ToysRecycled);
        Assert.AreEqual(manager.BottlesRequired, manager.BottlesRecycled);
        Assert.AreEqual(GameResult.Perfect, manager.Result);
        Assert.IsFalse(manager.IsPlaying);
    }

    [Test]
    public void ZeroLives_EndsGameFailure_WhenScoreStaysPositive()
    {
        manager.StartGame();
        for (int i = 0; i < 6; i++)
            manager.ReportRecycled(CreateItem(ItemType.Plant), 20);

        for (int i = 0; i < manager.MaxLives; i++)
            manager.ReportRecycled(CreateItem(ItemType.Bottle), -15);

        Assert.AreEqual(0, manager.Lives);
        Assert.AreEqual(GameResult.Failure, manager.Result);
        Assert.IsFalse(manager.IsPlaying);
    }

    [Test]
    public void PlantChainBonus_GrantedAtThreshold()
    {
        manager.StartGame();
        manager.ReportRecycled(CreateItem(ItemType.Plant), 20);
        manager.ReportRecycled(CreateItem(ItemType.Plant), 20);

        // 20 + 20 base + 40 chain bonus at the 2-plant threshold.
        Assert.AreEqual(80, score.CurrentScore);
    }

    [UnityTest]
    public IEnumerator TimeoutWithPositiveScore_IsDefault()
    {
        manager.StartGame();
        manager.ReportRecycled(CreateItem(ItemType.Plant), 20);

        SetPrivateField(manager, "timeRemaining", 0.0001f);
        int guard = 0;
        while (manager.IsPlaying && guard++ < 30)
            yield return null;

        Assert.IsFalse(manager.IsPlaying);
        Assert.AreEqual(GameResult.Default, manager.Result);
    }

    [UnityTest]
    public IEnumerator TimeoutWithZeroScore_IsFailure()
    {
        manager.StartGame();

        SetPrivateField(manager, "timeRemaining", 0.0001f);
        int guard = 0;
        while (manager.IsPlaying && guard++ < 30)
            yield return null;

        Assert.IsFalse(manager.IsPlaying);
        Assert.AreEqual(GameResult.Failure, manager.Result);
    }

    [UnityTest]
    public IEnumerator Pause_StopsTimer_ResumeContinues()
    {
        manager.StartGame();
        float t0 = manager.TimeRemaining;
        yield return null;
        Assert.Less(manager.TimeRemaining, t0, "Timer should count down while playing.");

        manager.PauseGame();
        float paused = manager.TimeRemaining;
        yield return null;
        yield return null;
        Assert.AreEqual(paused, manager.TimeRemaining, 0.0001f, "Timer should freeze while paused.");

        manager.ResumeGame();
        yield return null;
        Assert.Less(manager.TimeRemaining, paused, "Timer should count down again after resume.");
    }
}
