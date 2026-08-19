using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    // =====================================================
    // TIPO DE SEMENTE
    // =====================================================

    [System.Serializable]
    public class TipoSemente
    {
        public string nome;

        public int preco = 10;

        [Header("Prefab da árvore")]
        public GameObject prefabArvore;

        [Header("Valor da árvore")]
        public int valorBase = 50;
    }

    // =====================================================
    // SEMENTES
    // =====================================================

    [Header("Sementes")]
    public TipoSemente arvoreNivel1;
    public TipoSemente arvoreNivel2;
    public TipoSemente arvoreNivel3;
    public TipoSemente pinheiro;
    public TipoSemente macieira;

    // =====================================================
    // HOLDING POINT
    // =====================================================

    [Header("Árvore na mão")]
    public Transform holdingPoint;

    [Header("Escala da árvore na mão")]
    public float escalaArvoreNaMao = 0.15f;

    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("Configuração")]
    public float distanciaLoja = 3f;

    // =====================================================
    // SONS
    // =====================================================

    [Header("Som de compra de semente")]
    public AudioClip somCompraSemente;

    [Header("Som sem dinheiro")]
    public AudioClip somSemDinheiro;

    private AudioSource audioCompra;
    private AudioSource audioSemDinheiro;

    // =====================================================
    // PLAYER
    // =====================================================

    private Transform player;
    private PlayerMoney playerMoney;

    private bool playerPerto;

    // =====================================================
    // ÁRVORE NA MÃO
    // =====================================================

    private GameObject arvoreNaMao;

    // =====================================================
    // INVENTÁRIO
    // =====================================================

    private int quantidadeArvoreNivel1;
    private int quantidadeArvoreNivel2;
    private int quantidadeArvoreNivel3;
    private int quantidadePinheiro;
    private int quantidadeMacieira;

    // =====================================================
    // SEMENTE SELECIONADA
    // =====================================================

    private int sementeSelecionada = 0;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player =
                playerObject.transform;

            playerMoney =
                playerObject.GetComponent<PlayerMoney>();

            if (playerMoney == null)
            {
                Debug.LogError(
                    "❌ O Player não possui PlayerMoney!"
                );
            }
        }
        else
        {
            Debug.LogError(
                "❌ Player não encontrado! Confira a Tag Player."
            );
        }

        // =================================================
        // AUDIO
        // =================================================

        audioCompra =
            gameObject.AddComponent<AudioSource>();

        audioCompra.playOnAwake = false;
        audioCompra.loop = false;
        audioCompra.spatialBlend = 0f;
        audioCompra.volume = 1f;

        audioSemDinheiro =
            gameObject.AddComponent<AudioSource>();

        audioSemDinheiro.playOnAwake = false;
        audioSemDinheiro.loop = false;
        audioSemDinheiro.spatialBlend = 0f;
        audioSemDinheiro.volume = 1f;
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

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaLoja;

        // =================================================
        // COMPRAR SEMENTES
        // =================================================

        if (playerPerto)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel1,
                    1
                );
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel2,
                    2
                );
            }

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel3,
                    3
                );
            }

            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    pinheiro,
                    4
                );
            }

            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    macieira,
                    5
                );
            }
        }

        // =================================================
        // SELECIONAR SEMENTE
        // =================================================

        if (!playerPerto)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                SelecionarSemente(1);
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                SelecionarSemente(2);
            }

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                SelecionarSemente(3);
            }

            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                SelecionarSemente(4);
            }

            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                SelecionarSemente(5);
            }
        }
    }

    // =====================================================
    // COMPRAR SEMENTE
    // =====================================================

    private void ComprarSemente(
        TipoSemente semente,
        int numero
    )
    {
        if (semente == null)
        {
            Debug.LogError(
                "❌ Semente " +
                numero +
                " não configurada!"
            );

            return;
        }

        if (semente.prefabArvore == null)
        {
            Debug.LogError(
                "❌ Prefab da árvore '" +
                semente.nome +
                "' não foi configurado!"
            );

            return;
        }

        bool conseguiuPagar =
            playerMoney.RemoverDinheiro(
                semente.preco
            );

        if (!conseguiuPagar)
        {
            TocarSomSemDinheiro();

            Debug.Log(
                "❌ Dinheiro insuficiente para comprar " +
                semente.nome
            );

            return;
        }

        AdicionarSemente(numero);

        TocarSomCompraSemente();

        Debug.Log(
            "🌱 Comprou: " +
            semente.nome
        );

        Debug.Log(
            "📦 Quantidade: " +
            GetQuantidadeSemente(numero)
        );

        Debug.Log(
            "💰 Saldo: R$" +
            playerMoney.GetDinheiro()
        );
    }

    // =====================================================
    // SOM COMPRA
    // =====================================================

    private void TocarSomCompraSemente()
    {
        if (somCompraSemente == null)
            return;

        if (audioCompra == null)
        {
            audioCompra =
                gameObject.AddComponent<AudioSource>();

            audioCompra.playOnAwake = false;
            audioCompra.loop = false;
            audioCompra.spatialBlend = 0f;
        }

        audioCompra.PlayOneShot(
            somCompraSemente,
            1f
        );
    }

    // =====================================================
    // SOM SEM DINHEIRO
    // =====================================================

    private void TocarSomSemDinheiro()
    {
        if (somSemDinheiro == null)
            return;

        if (audioSemDinheiro == null)
        {
            audioSemDinheiro =
                gameObject.AddComponent<AudioSource>();

            audioSemDinheiro.playOnAwake = false;
            audioSemDinheiro.loop = false;
            audioSemDinheiro.spatialBlend = 0f;
        }

        audioSemDinheiro.PlayOneShot(
            somSemDinheiro,
            1f
        );
    }

    // =====================================================
    // ADICIONAR SEMENTE
    // =====================================================

    private void AdicionarSemente(
        int numero
    )
    {
        switch (numero)
        {
            case 1:
                quantidadeArvoreNivel1++;
                break;

            case 2:
                quantidadeArvoreNivel2++;
                break;

            case 3:
                quantidadeArvoreNivel3++;
                break;

            case 4:
                quantidadePinheiro++;
                break;

            case 5:
                quantidadeMacieira++;
                break;
        }
    }

    // =====================================================
    // SELECIONAR
    // =====================================================

    private void SelecionarSemente(
        int numero
    )
    {
        int quantidade =
            GetQuantidadeSemente(numero);

        if (quantidade <= 0)
        {
            Debug.Log(
                "❌ Você não possui essa semente."
            );

            return;
        }

        sementeSelecionada =
            numero;

        CriarArvoreNaMao();

        Debug.Log(
            "🌱 Semente selecionada: " +
            GetNomeSemente(numero)
        );

        Debug.Log(
            "📦 Quantidade: " +
            quantidade
        );
    }

    // =====================================================
    // CRIAR ÁRVORE NA MÃO
    // =====================================================

    private void CriarArvoreNaMao()
    {
        DestruirArvoreNaMao();

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ Holding Point não configurado!"
            );

            return;
        }

        GameObject prefab =
            GetPrefabSementeSelecionada();

        if (prefab == null)
        {
            Debug.LogError(
                "❌ Prefab da árvore selecionada não encontrado!"
            );

            return;
        }

        arvoreNaMao =
            Instantiate(
                prefab,
                holdingPoint
            );

        arvoreNaMao.transform.localPosition =
            Vector3.zero;

        arvoreNaMao.transform.localRotation =
            Quaternion.identity;

        arvoreNaMao.transform.localScale =
            Vector3.one *
            escalaArvoreNaMao;

        Rigidbody[] rigidbodies =
            arvoreNaMao.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] colliders =
            arvoreNaMao.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    // =====================================================
    // DESTRUIR ÁRVORE
    // =====================================================

    private void DestruirArvoreNaMao()
    {
        if (arvoreNaMao != null)
        {
            Destroy(arvoreNaMao);

            arvoreNaMao = null;
        }
    }

    // =====================================================
    // PREFAB
    // =====================================================

    public GameObject GetPrefabSementeSelecionada()
    {
        switch (sementeSelecionada)
        {
            case 1:
                return arvoreNivel1 != null
                    ? arvoreNivel1.prefabArvore
                    : null;

            case 2:
                return arvoreNivel2 != null
                    ? arvoreNivel2.prefabArvore
                    : null;

            case 3:
                return arvoreNivel3 != null
                    ? arvoreNivel3.prefabArvore
                    : null;

            case 4:
                return pinheiro != null
                    ? pinheiro.prefabArvore
                    : null;

            case 5:
                return macieira != null
                    ? macieira.prefabArvore
                    : null;
        }

        return null;
    }

    // =====================================================
    // SEMENTE SELECIONADA
    // =====================================================

    public int GetSementeSelecionada()
    {
        return sementeSelecionada;
    }

    // =====================================================
    // NOME
    // =====================================================

    public string GetNomeSementeSelecionada()
    {
        if (sementeSelecionada == 0)
            return "Nenhuma";

        return GetNomeSemente(
            sementeSelecionada
        );
    }

    private string GetNomeSemente(
        int numero
    )
    {
        switch (numero)
        {
            case 1:
                return arvoreNivel1 != null
                    ? arvoreNivel1.nome
                    : "Árvore 1";

            case 2:
                return arvoreNivel2 != null
                    ? arvoreNivel2.nome
                    : "Árvore 2";

            case 3:
                return arvoreNivel3 != null
                    ? arvoreNivel3.nome
                    : "Árvore 3";

            case 4:
                return pinheiro != null
                    ? pinheiro.nome
                    : "Pinheiro";

            case 5:
                return macieira != null
                    ? macieira.nome
                    : "Macieira";
        }

        return "Nenhuma";
    }

    // =====================================================
    // QUANTIDADE
    // =====================================================

    public int GetQuantidadeSemente(
        int numero
    )
    {
        switch (numero)
        {
            case 1:
                return quantidadeArvoreNivel1;

            case 2:
                return quantidadeArvoreNivel2;

            case 3:
                return quantidadeArvoreNivel3;

            case 4:
                return quantidadePinheiro;

            case 5:
                return quantidadeMacieira;
        }

        return 0;
    }

    // =====================================================
    // TEM SEMENTE
    // =====================================================

    public bool TemSementeSelecionada()
    {
        if (sementeSelecionada == 0)
            return false;

        return
            GetQuantidadeSemente(
                sementeSelecionada
            ) > 0;
    }

    // =====================================================
    // CONSUMIR SEMENTE
    // =====================================================

    public void ConsumirSemente()
    {
        if (sementeSelecionada == 0)
            return;

        switch (sementeSelecionada)
        {
            case 1:
                quantidadeArvoreNivel1--;
                break;

            case 2:
                quantidadeArvoreNivel2--;
                break;

            case 3:
                quantidadeArvoreNivel3--;
                break;

            case 4:
                quantidadePinheiro--;
                break;

            case 5:
                quantidadeMacieira--;
                break;
        }

        if (
            GetQuantidadeSemente(
                sementeSelecionada
            ) > 0
        )
        {
            CriarArvoreNaMao();
        }
        else
        {
            sementeSelecionada = 0;

            DestruirArvoreNaMao();
        }
    }

    // =====================================================
    // VALOR DA SEMENTE SELECIONADA
    // =====================================================

    public int GetValorSementeSelecionada()
    {
        switch (sementeSelecionada)
        {
            case 1:
                return arvoreNivel1 != null
                    ? arvoreNivel1.valorBase
                    : 0;

            case 2:
                return arvoreNivel2 != null
                    ? arvoreNivel2.valorBase
                    : 0;

            case 3:
                return arvoreNivel3 != null
                    ? arvoreNivel3.valorBase
                    : 0;

            case 4:
                return pinheiro != null
                    ? pinheiro.valorBase
                    : 0;

            case 5:
                return macieira != null
                    ? macieira.valorBase
                    : 0;
        }

        return 0;
    }

    // =====================================================
    // VALOR POR TIPO
    // =====================================================

    public int GetValorSemente(
        int numero
    )
    {
        switch (numero)
        {
            case 1:
                return arvoreNivel1 != null
                    ? arvoreNivel1.valorBase
                    : 0;

            case 2:
                return arvoreNivel2 != null
                    ? arvoreNivel2.valorBase
                    : 0;

            case 3:
                return arvoreNivel3 != null
                    ? arvoreNivel3.valorBase
                    : 0;

            case 4:
                return pinheiro != null
                    ? pinheiro.valorBase
                    : 0;

            case 5:
                return macieira != null
                    ? macieira.valorBase
                    : 0;
        }

        return 0;
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (!playerPerto ||
            playerMoney == null)
            return;

        GUIStyle estilo =
            new GUIStyle(
                GUI.skin.box
            );

        estilo.fontSize = 18;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura = 550f;
        float altura = 290f;

        float x =
            (Screen.width - largura) / 2f;

        float y =
            Screen.height - 360f;

        string selecionada =
            sementeSelecionada == 0
                ? "Nenhuma"
                : GetNomeSemente(
                    sementeSelecionada
                );

        string texto =
            "🛒 LOJA\n\n" +

            "1 - " +
            arvoreNivel1.nome +
            " - R$" +
            arvoreNivel1.preco +
            " [Qtd: " +
            quantidadeArvoreNivel1 +
            "]\n" +

            "2 - " +
            arvoreNivel2.nome +
            " - R$" +
            arvoreNivel2.preco +
            " [Qtd: " +
            quantidadeArvoreNivel2 +
            "]\n" +

            "3 - " +
            arvoreNivel3.nome +
            " - R$" +
            arvoreNivel3.preco +
            " [Qtd: " +
            quantidadeArvoreNivel3 +
            "]\n" +

            "4 - " +
            pinheiro.nome +
            " - R$" +
            pinheiro.preco +
            " [Qtd: " +
            quantidadePinheiro +
            "]\n" +

            "5 - " +
            macieira.nome +
            " - R$" +
            macieira.preco +
            " [Qtd: " +
            quantidadeMacieira +
            "]\n\n" +

            "🌱 Selecionada: " +
            selecionada +

            "\n💰 Dinheiro: R$" +
            playerMoney.GetDinheiro();

        GUI.Box(
            new Rect(
                x,
                y,
                largura,
                altura
            ),
            texto,
            estilo
        );
    }
}