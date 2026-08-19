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
    // PLAYER
    // =====================================================

    private Transform player;
    private PlayerMoney playerMoney;

    // =====================================================
    // CÂMERA
    // =====================================================

    [Header("Câmera")]
    public Camera cam;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        GameObject objPlayer =
            GameObject.FindGameObjectWithTag("Player");

        if (objPlayer != null)
        {
            player =
                objPlayer.transform;

            playerMoney =
                objPlayer.GetComponent<PlayerMoney>();
        }

        if (cam == null)
        {
            cam = Camera.main;
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

        if (cam == null)
        {
            Debug.LogError(
                "❌ Câmera não encontrada!"
            );
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // =================================================
        // F = COLHER
        // =================================================

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log(
                "⌨️ F pressionado!"
            );

            TentarColher();
        }
    }

    // =====================================================
    // TENTAR COLHER
    // =====================================================

    private void TentarColher()
    {
        // =================================================
        // VERIFICAR PLAYER
        // =================================================

        if (player == null)
        {
            Debug.LogError(
                "❌ Player está NULL!"
            );

            return;
        }

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ PlayerMoney está NULL!"
            );

            return;
        }

        // =================================================
        // VERIFICAR CÂMERA
        // =================================================

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogError(
                "❌ Camera está NULL!"
            );

            return;
        }

        // =================================================
        // DISTÂNCIA DA COLHEITADEIRA
        // =================================================

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

        // =================================================
        // RAYCAST PELO CENTRO DA TELA
        // =================================================

        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(
                    0.5f,
                    0.5f,
                    0f
                )
            );

        Debug.DrawRay(
            ray.origin,
            ray.direction * distanciaParaColher,
            Color.green,
            2f
        );

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                distanciaParaColher,
                ~0,
                QueryTriggerInteraction.Collide
            );

        // =================================================
        // NENHUM COLLIDER
        // =================================================

        if (hits == null ||
            hits.Length == 0)
        {
            Debug.Log(
                "❌ O Raycast não acertou nenhum objeto."
            );

            return;
        }

        // =================================================
        // ORDENAR
        // =================================================

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        // =================================================
        // PROCURAR PLANTSPOT
        // =================================================

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(
                "🎯 Raycast acertou: " +
                hit.collider.name
            );

            PlantSpot spot =
                EncontrarPlantSpot(
                    hit.collider
                );

            if (spot == null)
            {
                Debug.Log(
                    "⚠️ Esse objeto não possui PlantSpot."
                );

                continue;
            }

            Debug.Log(
                "🌳 PlantSpot encontrado!"
            );

            // =================================================
            // VERIFICAR ÁRVORE
            // =================================================

            if (!spot.TemArvore())
            {
                Debug.Log(
                    "❌ Esse PlantSpot não possui árvore."
                );

                continue;
            }

            // =================================================
            // DISTÂNCIA DA ÁRVORE
            // =================================================

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
                Debug.Log(
                    "❌ Árvore está muito longe da colheitadeira."
                );

                return;
            }

            // =================================================
            // VERIFICAR SE ESTÁ PRONTA
            // =================================================

            if (!spot.PodeColher())
            {
                Debug.Log(
                    "🌱 Essa árvore ainda não está pronta para colher."
                );

                return;
            }

            // =================================================
            // PEGAR VALOR
            // =================================================

            int valor =
                spot.GetValorColheita();

            Debug.Log(
                "💰 Valor da árvore: R$" +
                valor
            );

            // =================================================
            // COLHER
            // =================================================

            bool colheu =
                spot.ColherArvore();

            if (!colheu)
            {
                Debug.Log(
                    "❌ Não foi possível colher a árvore."
                );

                return;
            }

            // =================================================
            // PAGAR
            // =================================================

            playerMoney.AdicionarDinheiro(
                valor
            );

            Debug.Log(
                "=============================="
            );

            Debug.Log(
                "🚜 ÁRVORE COLHIDA!"
            );

            Debug.Log(
                "💰 Dinheiro ganho: R$" +
                valor
            );

            Debug.Log(
                "💵 Saldo atual: R$" +
                playerMoney.GetDinheiro()
            );

            Debug.Log(
                "=============================="
            );

            // =================================================
            // IMPORTANTE:
            // PARA AQUI.
            // NÃO COLHE OUTRAS ÁRVORES.
            // =================================================

            return;
        }

        Debug.Log(
            "❌ Nenhuma árvore válida foi encontrada na mira."
        );
    }

    // =====================================================
    // ENCONTRAR PLANTSPOT
    // =====================================================

    private PlantSpot EncontrarPlantSpot(
        Collider collider)
    {
        if (collider == null)
            return null;

        // =================================================
        // 1 - PRÓPRIO OBJETO
        // =================================================

        PlantSpot spot =
            collider.GetComponent<PlantSpot>();

        if (spot != null)
            return spot;

        // =================================================
        // 2 - PAI
        // =================================================

        spot =
            collider.GetComponentInParent<PlantSpot>();

        if (spot != null)
            return spot;

        // =================================================
        // 3 - FILHOS
        // =================================================

        spot =
            collider.GetComponentInChildren<PlantSpot>();

        if (spot != null)
            return spot;

        // =================================================
        // 4 - PROCURAR NA ÁRVORE
        // =================================================

        Transform raiz =
            collider.transform.root;

        if (raiz != null)
        {
            spot =
                raiz.GetComponentInChildren<PlantSpot>();

            if (spot != null)
                return spot;
        }

        return null;
    }
}