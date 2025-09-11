using System;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public string levelID;
    public float bestCompletionTime;
    public List<TaskPerformanceData> tasks;
}

[System.Serializable]
public class TaskPerformanceData
{
    public string taskID;
    public TaskCategory category;
    public float bestCompletionTime;
    public float bestAccuracy;
    public float bestResponseTimeMs;
    public int bestHealthRemaining;
}