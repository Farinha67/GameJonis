using UnityEngine;

public class SomCliqueBotao : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip somClique;

    public void Clicar()
    {
        if (audioSource == null)
        {
            Debug.LogError("❌ AudioSource não configurado!");
            return;
        }

        if (somClique == null)
        {
            Debug.LogError("❌ Som de clique não configurado!");
            return;
        }

        audioSource.PlayOneShot(somClique);

        Debug.Log("🔊 Som do botão tocado!");
    }
}