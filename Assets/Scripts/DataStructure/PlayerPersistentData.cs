using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerPersistentData
{
    public List<LevelData> levelPerformances = new List<LevelData>();
    public PerformanceMetrics performance = new PerformanceMetrics();
    public LoyaltyRewards loyalty = new LoyaltyRewards();
    public UpgradeCollection upgrades = new UpgradeCollection();
}

[System.Serializable]
public class PerformanceMetrics
{
    public float intellectScore;
    public float physicalScore;
    public float instinctScore;
}

[System.Serializable]
public class LoyaltyRewards
{
    public int currentPoints;
    public int totalEarned;
}