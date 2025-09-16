using UnityEngine;

//this class handles triggers, interaction with player and checks for permissions to proceed with logic during an event
public abstract class ConsumableBase : MonoBehaviour, IConsumable
{
    [Header("Consumable Settings")]
    [SerializeField] private bool canBeConsumed = true;
    [SerializeField] private float interactionRange = 2f;

    //the vars are read-only, when accessed it returns the value of the private field 
    public bool CanBeConsumed => canBeConsumed; 
    public float InteractionRange => interactionRange;

    protected GameObject player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(CanBeConsumed && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= interactionRange && Input.GetKey(KeyCode.E))
            {
                Consume(player);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(CanBeConsumed && other.CompareTag("Player"))
        {
            Consume(other.gameObject);
        }
    }
    public abstract void Consume(GameObject consumer);

}
