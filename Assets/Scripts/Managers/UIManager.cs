using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //singleton
    public static UIManager Instance;

    //text objects
    [SerializeField] TextMeshProUGUI narrationText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateNarrationText(string text)
    {
        //start a co-routine
        narrationText.text = text;
    }


}
