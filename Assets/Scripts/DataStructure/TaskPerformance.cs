using System;

[System.Serializable]
public class TaskPerformance
{
    public TaskCategory category;
    public float completionTime; // seconds
    public float accuracy;       // 0–100
    public float responseTimeMs; // instinct only
    public int healthRemaining;  // instinct only
}

public enum TaskCategory { Intellect, Physical, Instinct }