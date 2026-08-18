using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SomBotao : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip somBotao;

    public string nomeDaCena;

    public void Clicar()
    {
        StartCoroutine(TocarESeguir());
    }

    IEnumerator TocarESeguir()
    {
        
        audioSource.PlayOneShot(somBotao);

        
        yield return new WaitForSeconds(somBotao.length);

        
        SceneManager.LoadScene(nomeDaCena);
    }
}