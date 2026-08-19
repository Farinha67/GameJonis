using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        StartCoroutine(TrocarCenaComDelay());
    }

    private IEnumerator TrocarCenaComDelay()
    {
        // Espera 1 segundo para o som tocar
        yield return new WaitForSeconds(1f);

        // Troca de cena
        SceneManager.LoadScene(sceneName);
    }
}