using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    [Header("Preços")]
    public int precoSemente = 10;
    public int precoRegador = 15;

    [Header("Regador")]
    public GameObject regadorPrefab;
    public Transform holdingPoint;

    [Header("Configuração")]
    public float distanciaLoja = 3f;

    [Header("Sons da Loja")]
    public AudioSource audioSource;
    public AudioClip somCompraSemente;
    public AudioClip somCompraRegador;

    private Transform player;
    private PlayerMoney playerMoney;

    private bool playerPerto = false;

    private GameObject regadorNaMao;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            playerMoney =
                playerObject.GetComponent<PlayerMoney>();

            if (playerMoney == null)
            {
                Debug.LogError(
                    "❌ O Player não possui o script PlayerMoney!"
                );
            }
        }
        else
        {
            Debug.LogError(
                "❌ Player não encontrado! Confira a Tag 'Player'."
            );
        }
    }

    void Update()
    {
        if (player == null || playerMoney == null)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaLoja;

        if (Keyboard.current == null)
            return;

        // =========================
        // COMPRAS
        // =========================

        if (playerPerto)
        {
            // E = Semente
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ComprarSemente();
            }

            // Q = Regador
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ComprarRegador();
            }
        }

        // =========================
        // SOLTAR REGADOR
        // =========================

        if (!playerPerto &&
            regadorNaMao != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            SoltarRegador();
        }
    }

    // =====================================================
    // COMPRAR SEMENTE
    // =====================================================

    void ComprarSemente()
    {
        if (playerMoney.RemoverDinheiro(precoSemente))
        {
            // SOM DE COMPRA DA SEMENTE
            if (audioSource != null && somCompraSemente != null)
            {
                audioSource.PlayOneShot(somCompraSemente);
            }

            Debug.Log("🌱 Semente comprada!");

            Debug.Log(
                "💰 Saldo: R$" +
                playerMoney.GetDinheiro()
            );
        }
    }

    // =====================================================
    // COMPRAR REGADOR
    // =====================================================

    void ComprarRegador()
    {
        if (regadorNaMao != null)
        {
            Debug.Log(
                "💧 Você já está segurando um regador!"
            );

            return;
        }

        if (regadorPrefab == null)
        {
            Debug.LogError(
                "❌ Regador Prefab não foi colocado no Shop!"
            );

            return;
        }

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ Holding Point não foi colocado no Shop!"
            );

            return;
        }

        // Tenta tirar o dinheiro
        if (!playerMoney.RemoverDinheiro(precoRegador))
        {
            return;
        }

        // Cria o regador
        regadorNaMao = Instantiate(
            regadorPrefab,
            holdingPoint.position,
            holdingPoint.rotation
        );

        // Coloca na mão
        regadorNaMao.transform.SetParent(
            holdingPoint
        );

        regadorNaMao.transform.localPosition =
            Vector3.zero;

        regadorNaMao.transform.localRotation =
            Quaternion.identity;

        // Desliga física
        Rigidbody rb =
            regadorNaMao.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Desliga colisão
        Collider col =
            regadorNaMao.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        // SOM DE COMPRA DO REGADOR
        if (audioSource != null && somCompraRegador != null)
        {
            audioSource.PlayOneShot(somCompraRegador);
        }

        Debug.Log("💧 Regador comprado!");

        Debug.Log(
            "💰 Saldo: R$" +
            playerMoney.GetDinheiro()
        );
    }

    // =====================================================
    // SOLTAR REGADOR
    // =====================================================

    void SoltarRegador()
    {
        if (regadorNaMao == null)
            return;

        regadorNaMao.transform.SetParent(null);

        regadorNaMao.transform.position =
            player.position +
            player.forward * 1.5f;

        Rigidbody rb =
            regadorNaMao.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col =
            regadorNaMao.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        regadorNaMao = null;

        Debug.Log("💧 Regador solto!");
    }

    // =====================================================
    // VERIFICAR REGADOR
    // =====================================================

    public bool EstaSegurandoRegador()
    {
        return regadorNaMao != null;
    }

    // =====================================================
    // USAR REGADOR
    // =====================================================

    public void UsarRegador()
    {
        if (regadorNaMao == null)
            return;

        Destroy(regadorNaMao);

        regadorNaMao = null;

        Debug.Log(
            "💧 Regador usado e consumido!"
        );
    }

    // =====================================================
    // TEXTO DA LOJA
    // =====================================================

    void OnGUI()
    {
        if (!playerPerto || playerMoney == null)
            return;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 20;
        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura = 450;
        float altura = 150;

        float x =
            (Screen.width - largura) / 2;

        float y =
            Screen.height - 220;

        GUI.Box(
            new Rect(
                x,
                y,
                largura,
                altura
            ),

            "🛒 LOJA\n\n" +

            "E - Comprar Semente - R$" +
            precoSemente +

            "\nQ - Comprar Regador - R$" +
            precoRegador +

            "\n\nDinheiro: R$" +
            playerMoney.GetDinheiro(),

            estilo
        );
    }
}