using UnityEngine;
using System.Linq;

public class Database : MonoBehaviour
{
    public static Database Instance;
    public LevelDefinition[] allLevels;
    public UpgradeDefinition[] allUpgrades;

    private void Awake() { Instance = this; }

    public LevelDefinition GetLevelByID(string id) =>
        allLevels.FirstOrDefault(l => l.levelID == id);

    public UpgradeDefinition GetUpgradeByID(string id) =>
        allUpgrades.FirstOrDefault(u => u.upgradeID == id);
}