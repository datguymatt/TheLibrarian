using UnityEngine;
using System.Collections.Generic;

public enum UpgradeType { SpeedBoost, AccuracyBoost, HealthBoost, ResponseBoost }

[CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "Game/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    public string upgradeID;
    public string displayName;
    public string description;
    public int baseCost;
    public Sprite icon;
    public UpgradeType type;
    public float baseEffectValue = 0.1f;
    public float perLevelIncrease = 0.05f;
}

[System.Serializable]
public class Upgrade
{
    public string upgradeID;
    public bool isUnlocked;
    public int level;
}

[System.Serializable]
public class UpgradeCollection
{
    public List<Upgrade> upgrades = new List<Upgrade>();
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private void Awake() { Instance = this; }

    public float GetMultiplier(UpgradeType type)
    {
        float total = 1f;
        foreach (var upgrade in GameManager.Instance.playerData.upgrades.upgrades)
        {
            if (!upgrade.isUnlocked) continue;
            var def = Database.Instance.GetUpgradeByID(upgrade.upgradeID);
            if (def.type == type)
            {
                float effect = def.baseEffectValue + (def.perLevelIncrease * (upgrade.level - 1));
                total += effect;
            }
        }
        return total;
    }
}