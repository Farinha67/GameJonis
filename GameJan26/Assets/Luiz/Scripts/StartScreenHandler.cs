using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class StartScreenHandler : MonoBehaviour
{
    [Header("Cena do Menu")]
    public string menuSceneName; 

    private bool hasStarted = false;

    private void Update()
    {
        if (hasStarted) return;

        
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            hasStarted = true;
            LoadMenu();
        }

        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            hasStarted = true;
            LoadMenu();
        }
    }

    private void LoadMenu()
    {
        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
