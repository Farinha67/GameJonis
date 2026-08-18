using UnityEngine;

public class CameraEntrandoNoCeleiro : MonoBehaviour
{
    [Header("Configurações da Câmera")]
    public Transform mainCamera;
    public Transform insideBarnPosition; // Aquele Ponto B escuro lá dentro da casinha
    public float transitionSpeed = 1.5f; // Diminua esse número se quiser que a câmera ande mais devagar!

    [Header("Telas de Interface (UI)")]
    public GameObject mainMenuCanvas;    // O seu Canvas com os botões (Jogar, Opções, Sair)

    private bool isMoving = false;

    void Start()
    {
        // Assim que a cena carregar, esconde os botões e avisa a câmera para começar a andar
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            // O comando Lerp faz a câmera DESLIZAR suavemente até lá dentro, sem teleportar
            mainCamera.position = Vector3.Lerp(mainCamera.position, insideBarnPosition.position, transitionSpeed * Time.deltaTime);
            mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, insideBarnPosition.rotation, transitionSpeed * Time.deltaTime);

            // Quando a câmera chegar bem pertinho do Ponto B, ela para
            if (Vector3.Distance(mainCamera.position, insideBarnPosition.position) < 0.1f)
            {
                isMoving = false; // Para a câmera
                if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true); // Mostra os seus botões
            }
        }
    }
}