using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //state and global properties
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //sub to any events

        // Check if an instance already exists
        if (instance != null && instance != this)
        {
            // If a duplicate exists, destroy this new instance
            Destroy(gameObject);
        }
        else
        {
            // If no instance exists, set this as the instance
            instance = this;
            // Optionally, make the object persist across scene loads
            DontDestroyOnLoad(gameObject);

        }

        //queue start game sequence
        StartCoroutine(StartScene());
    }

    IEnumerator StartScene()
    {
        //load saved data for player - transfer it to the next scene
        //declare the start of current scene as an event
        yield return new WaitForSeconds(2);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    
}
