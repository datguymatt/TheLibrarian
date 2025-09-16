using UnityEngine;
using DG.Tweening;

public abstract class SizeGrowEffect : ConsumableEffect
{
    public Vector3 playerDefaultSize = Vector3.zero;
    public Vector3 growthAmmount = new Vector3(0.2f, 0.2f, 0.2f);
    //how long it will take for the size increase to take place
    public float growthChangeTimer = 3f;
    //how long to wait before the size increase starts
    public float delay = 5f;

    public override void ApplyEffects(GameObject consumer)
    {

        //start the first effect - grow player


    }

    private System.Collections.IEnumerator GrowPlayer(GameObject player)
    {
        yield return new WaitForSeconds(delay);
        //move the camera to 3rd person for a quick visual of the growth

        //grow the player in size
    }

}
