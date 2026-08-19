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

    // =====================================================
    // START
    // =====================================================

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
                "❌ Player não encontrado! Verifique a Tag Player."
            );
        }

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ PlayerMoney não encontrado no Player!"
            );
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (player == null ||
            playerMoney == null)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            TentarColher();
        }
    }

    // =====================================================
    // TENTAR COLHER
    // =====================================================

    void TentarColher()
    {
        float distanciaPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distanciaPlayer > distanciaParaColher)
        {
            Debug.Log(
                "🚜 Você está muito longe da colheitadeira!"
            );

            return;
        }

        PlantSpot[] spots =
            FindObjectsByType<PlantSpot>(
                FindObjectsSortMode.None
            );

        int quantidadeColhida = 0;

        foreach (PlantSpot spot in spots)
        {
            if (spot == null)
                continue;

            if (!spot.PodeColher())
                continue;

            // IMPORTANTE:
            // usa a posição REAL da árvore
            Vector3 posicaoArvore =
                spot.GetPosicaoArvore();

            float distanciaArvore =
                Vector3.Distance(
                    transform.position,
                    posicaoArvore
                );

            if (distanciaArvore >
                distanciaParaColher)
            {
                continue;
            }

            if (spot.ColherArvore())
            {
                quantidadeColhida++;

                Debug.Log(
                    "🌳 Árvore colhida!"
                );
            }
        }

        if (quantidadeColhida == 0)
        {
            Debug.Log(
                "❌ Nenhuma árvore pronta dentro do alcance da colheitadeira."
            );

            return;
        }

        // =================================================
        // PAGAMENTO
        // =================================================

        int dinheiroGanho =
            quantidadeColhida *
            dinheiroPorArvore;

        playerMoney.AdicionarDinheiro(
            dinheiroGanho
        );

        Debug.Log("==============================");
        Debug.Log("🚜 COLHEITA CONCLUÍDA!");
        Debug.Log(
            "🌳 Árvores colhidas: " +
            quantidadeColhida
        );
        Debug.Log(
            "💰 Dinheiro ganho: R$" +
            dinheiroGanho
        );
        Debug.Log(
            "💵 Saldo atual: R$" +
            playerMoney.GetDinheiro()
        );
        Debug.Log("==============================");
    }
}