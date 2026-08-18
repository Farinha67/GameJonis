using UnityEngine;
using UnityEngine.InputSystem;

public class Colheitadeira : MonoBehaviour
{
    [Header("Configuração")]
    public float distanciaParaColher = 4f;

    [Header("Pagamento")]
    public int dinheiroPorArvore = 50;

    private Transform player;
    private PlayerMoney playerMoney;

    private bool playerPerto = false;

    void Start()
    {
        GameObject objPlayer =
            GameObject.FindGameObjectWithTag("Player");

        if (objPlayer != null)
        {
            player = objPlayer.transform;

            playerMoney =
                objPlayer.GetComponent<PlayerMoney>();
        }

        if (player == null)
        {
            Debug.LogError(
                "❌ Player não encontrado!"
            );
        }

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ PlayerMoney não encontrado!"
            );
        }
    }

    void Update()
    {
        if (player == null ||
            playerMoney == null)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaParaColher;

        if (!playerPerto)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ColherTudo();
        }
    }

    // =====================================================
    // COLHER TUDO
    // =====================================================

    void ColherTudo()
    {
        PlantSpot[] plantacoes =
            FindObjectsByType<PlantSpot>(
                FindObjectsSortMode.None
            );

        int quantidadeColhida = 0;

        foreach (PlantSpot plantacao in plantacoes)
        {
            if (plantacao.ColherArvore())
            {
                quantidadeColhida++;
            }
        }

        // =========================
        // NADA PARA COLHER
        // =========================

        if (quantidadeColhida == 0)
        {
            Debug.Log(
                "🚜 Não existem árvores prontas para colher!"
            );

            return;
        }

        // =========================
        // CALCULAR PAGAMENTO
        // =========================

        int dinheiroGanho =
            quantidadeColhida *
            dinheiroPorArvore;

        playerMoney.AdicionarDinheiro(
            dinheiroGanho
        );

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "🚜 COLHEITA CONCLUÍDA!"
        );

        Debug.Log(
            "🌳 Árvores colhidas: " +
            quantidadeColhida
        );

        Debug.Log(
            "💰 Dinheiro recebido: R$" +
            dinheiroGanho
        );

        Debug.Log(
            "💵 Saldo atual: R$" +
            playerMoney.GetDinheiro()
        );

        Debug.Log(
            "=============================="
        );
    }

    // =====================================================
    // TEXTO
    // =====================================================

    void OnGUI()
    {
        if (!playerPerto)
            return;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 22;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura = 420;
        float altura = 70;

        float x =
            (Screen.width - largura) / 2;

        float y =
            Screen.height - 150;

        GUI.Box(
            new Rect(
                x,
                y,
                largura,
                altura
            ),
            "🚜 PRESSIONE E PARA COLHER",
            estilo
        );
    }
}