using UnityEngine;

public interface IConsumable
{
    //required
    bool CanBeConsumed { get; }
    float InteractionRange { get; }

    void Consume(GameObject consumer);
}