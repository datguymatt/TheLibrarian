using UnityEngine;

public abstract class ConsumableEffect : ScriptableObject
{
    public abstract void ApplyEffects(GameObject consumer);

    public abstract void ApplySecondaryEffect(GameObject consumer);

}
