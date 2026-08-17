using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    [Header("Preços")]
    public int precoSemente = 10;
    public int precoRegador = 15;

    [Header("Configuração")]
    public float distanciaCompra = 3f;

    private Transform player;
    private bool pertoDaLoja = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Não encontrei um Player com a Tag 'Player'.");
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distancia = Vector3.Distance(transform.position, player.position);

        pertoDaLoja = distancia <= distanciaCompra;

        if (pertoDaLoja && Keyboard.current != null)
        {
            // E = comprar semente
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ComprarSemente();
            }

            // Q = comprar regador
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ComprarRegador();
            }
        }
    }

    void ComprarSemente()
    {
        Debug.Log("🌱 Semente comprada por R$" + precoSemente);
    }

    void ComprarRegador()
    {
        Debug.Log("💧 Regador comprado por R$" + precoRegador);
    }

    void OnGUI()
    {
        if (!pertoDaLoja)
            return;

        GUIStyle estilo = new GUIStyle(GUI.skin.box);

        estilo.fontSize = 22;
        estilo.alignment = TextAnchor.MiddleCenter;

        float largura = 400;
        float altura = 130;

        float x = (Screen.width - largura) / 2;
        float y = Screen.height - 180;

        GUI.Box(
            new Rect(x, y, largura, altura),
            "🛒 LOJA\n\n" +
            "E - Comprar Semente - R$" + precoSemente +
            "\nQ - Comprar Regador - R$" + precoRegador,
            estilo
        );
    }
}