using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDefinition", menuName = "Game/Level Definition")]
public class LevelDefinition : ScriptableObject
{
    public string levelID;
    public string displayName;
    public float maxAllowedTime = 120f;
    public float maxResponseTime = 500f;
    public int baseReward = 50;
    public List<TaskDefinition> tasks;
}

[System.Serializable]
public class TaskDefinition
{
    public string taskID;
    public TaskCategory category;
    public string description;
}