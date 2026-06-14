using UnityEngine;

public class BGM : MonoBehaviour
{
    private static BGM instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
      
        else
        {
            Destroy(gameObject); 
        }
    }
}