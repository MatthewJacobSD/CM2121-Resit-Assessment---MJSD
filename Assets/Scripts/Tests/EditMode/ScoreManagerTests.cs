using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// EditMode tests for ScoreManager: accumulation, penalty clamping, reset,
/// high-score tracking and PlayerPrefs persistence.
/// </summary>
public class ScoreManagerTests
{
    private const string HighScoreKey = "HighScore_Recycling";

    private readonly List<GameObject> created = new List<GameObject>();

    private ScoreManager CreateScoreManager()
    {
        GameObject go = new GameObject("TestScoreManager");
        created.Add(go);
        return go.AddComponent<ScoreManager>();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        foreach (GameObject go in created)
        {
            if (go != null) Object.DestroyImmediate(go);
        }
        created.Clear();
    }

    [Test]
    public void AddScore_Accumulates()
    {
        ScoreManager sm = CreateScoreManager();
        sm.AddScore(50);
        sm.AddScore(30);
        Assert.AreEqual(80, sm.CurrentScore);
    }

    [Test]
    public void AddScore_NegativeReducesScore()
    {
        ScoreManager sm = CreateScoreManager();
        sm.AddScore(60);
        sm.AddScore(-30);
        Assert.AreEqual(30, sm.CurrentScore);
    }

    [Test]
    public void AddPenalty_ClampsAtZero()
    {
        ScoreManager sm = CreateScoreManager();
        sm.AddScore(10);
        sm.AddPenalty(15);
        Assert.AreEqual(0, sm.CurrentScore);
    }

    [Test]
    public void AddPenalty_NeverGoesBelowZero()
    {
        ScoreManager sm = CreateScoreManager();
        sm.AddPenalty(100);
        Assert.AreEqual(0, sm.CurrentScore);
    }

    [Test]
    public void ResetScore_Zeroes()
    {
        ScoreManager sm = CreateScoreManager();
        sm.AddScore(99);
        sm.ResetScore();
        Assert.AreEqual(0, sm.CurrentScore);
    }

    [Test]
    public void HighScore_TracksPeakNotCurrent()
    {
        ScoreManager sm = CreateScoreManager();
        sm.AddScore(60);
        sm.AddScore(-30);
        Assert.AreEqual(30, sm.CurrentScore);
        Assert.AreEqual(60, sm.HighScore);
    }

    [Test]
    public void SaveHighScore_PersistsToPlayerPrefs()
    {
        ScoreManager sm = CreateScoreManager();
        sm.AddScore(100);
        sm.SaveHighScore();
        Assert.AreEqual(100, PlayerPrefs.GetInt(HighScoreKey));
    }
}
