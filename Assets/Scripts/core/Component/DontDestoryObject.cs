using UnityEngine;

public class DontDestoryObject : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
