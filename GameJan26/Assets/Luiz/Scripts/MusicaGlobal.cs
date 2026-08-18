using UnityEngine;

public class MusicaGlobal : MonoBehaviour
{
    private static MusicaGlobal instancia;

    void Awake()
    {
       
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;

       
        DontDestroyOnLoad(gameObject);
    }
}