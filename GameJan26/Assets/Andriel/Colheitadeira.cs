using UnityEngine;
using UnityEngine.InputSystem;

public class Colheitadeira : MonoBehaviour
{
    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("Configuração")]
    public float distanciaParaColher = 4f;

    // =====================================================
    // PAGAMENTO
    // =====================================================

    [Header("Pagamento")]
    public int dinheiroPorArvore = 50;

    // =====================================================
    // PLAYER
    // =====================================================

    private Transform player;
    private PlayerMoney playerMoney;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        GameObject objPlayer =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (objPlayer != null)
        {
            player =
                objPlayer.transform;

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

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (player == null ||
            playerMoney == null)
            return;

        if (Keyboard.current == null)
            return;

        // =================================================
        // F = COLHER
        // =================================================

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            TentarColher();
        }
    }

    // =====================================================
    // TENTAR COLHER
    // =====================================================

    private void TentarColher()
    {
        float distanciaPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distanciaPlayer >
            distanciaParaColher)
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

            if (!spot.TemArvore())
                continue;

            if (!spot.PodeColher())
                continue;

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

        // =================================================
        // NENHUMA
        // =================================================

        if (quantidadeColhida == 0)
        {
            Debug.Log(
                "❌ Nenhuma árvore pronta dentro do alcance."
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
            "💰 Dinheiro ganho: R$" +
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
}