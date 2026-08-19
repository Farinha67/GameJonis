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
        public int preco;
        public GameObject prefabArvore;
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
    // REGADOR
    // =====================================================

    [Header("Regador")]
    public int precoRegador = 15;
    public GameObject regadorPrefab;
    public Transform holdingPoint;

    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("Configuração")]
    public float distanciaLoja = 3f;

    // =====================================================
    // SONS
    // =====================================================

    [Header("Sons da Loja")]
    public AudioSource audioSource;
    public AudioClip somCompraSemente;
    public AudioClip somCompraRegador;

    [Header("Som - Sem Dinheiro")]
    public AudioSource audioSourceSemDinheiro;
    public AudioClip somSemDinheiro;

    // =====================================================
    // PLAYER
    // =====================================================

    private Transform player;
    private PlayerMoney playerMoney;

    private bool playerPerto;

    // =====================================================
    // REGADOR
    // =====================================================

    private GameObject regadorNaMao;

    // =====================================================
    // INVENTÁRIO DE SEMENTES
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

    // 0 = nenhuma
    // 1 = árvore nível 1
    // 2 = árvore nível 2
    // 3 = árvore nível 3
    // 4 = pinheiro
    // 5 = macieira

    // =====================================================
    // START
    // =====================================================

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

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaLoja;

        // =================================================
        // COMPRAR NA LOJA
        // =================================================

        if (playerPerto)
        {
            // 1 = comprar árvore nível 1
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel1,
                    1
                );
            }

            // 2 = comprar árvore nível 2
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel2,
                    2
                );
            }

            // 3 = comprar árvore nível 3
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    arvoreNivel3,
                    3
                );
            }

            // 4 = comprar pinheiro
            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    pinheiro,
                    4
                );
            }

            // 5 = comprar macieira
            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                ComprarSemente(
                    macieira,
                    5
                );
            }

            // Q = regador
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ComprarRegador();
            }
        }

        // =================================================
        // SELECIONAR SEMENTE FORA DA LOJA
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

        // =================================================
        // SOLTAR REGADOR
        // =================================================

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

    void ComprarSemente(
        TipoSemente semente,
        int numero
    )
    {
        if (semente == null)
            return;

        if (semente.prefabArvore == null)
        {
            Debug.LogError(
                "❌ O prefab da semente '" +
                semente.nome +
                "' não foi configurado!"
            );

            return;
        }

        if (!playerMoney.RemoverDinheiro(
                semente.preco))
        {
            TocarSomSemDinheiro();

            Debug.Log(
                "❌ Dinheiro insuficiente para comprar " +
                semente.nome
            );

            Debug.Log(
                "💰 Você tem: R$" +
                playerMoney.GetDinheiro()
            );

            return;
        }

        // =================================================
        // ADICIONAR AO INVENTÁRIO
        // =================================================

        AdicionarSemente(numero);

        // =================================================
        // SOM
        // =================================================

        if (audioSource != null &&
            somCompraSemente != null)
        {
            audioSource.PlayOneShot(
                somCompraSemente
            );
        }

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
    // ADICIONAR SEMENTE
    // =====================================================

    void AdicionarSemente(int numero)
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
    // SELECIONAR SEMENTE
    // =====================================================

    void SelecionarSemente(int numero)
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
    // PEGAR PREFAB DA SEMENTE SELECIONADA
    // =====================================================

    public GameObject GetPrefabSementeSelecionada()
    {
        switch (sementeSelecionada)
        {
            case 1:
                return arvoreNivel1.prefabArvore;

            case 2:
                return arvoreNivel2.prefabArvore;

            case 3:
                return arvoreNivel3.prefabArvore;

            case 4:
                return pinheiro.prefabArvore;

            case 5:
                return macieira.prefabArvore;
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
    // NOME DA SEMENTE
    // =====================================================

    public string GetNomeSementeSelecionada()
    {
        if (sementeSelecionada == 0)
            return "Nenhuma";

        return GetNomeSemente(
            sementeSelecionada
        );
    }

    string GetNomeSemente(int numero)
    {
        switch (numero)
        {
            case 1:
                return arvoreNivel1.nome;

            case 2:
                return arvoreNivel2.nome;

            case 3:
                return arvoreNivel3.nome;

            case 4:
                return pinheiro.nome;

            case 5:
                return macieira.nome;
        }

        return "Nenhuma";
    }

    // =====================================================
    // QUANTIDADE
    // =====================================================

    public int GetQuantidadeSemente(int numero)
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
    // TEM SEMENTE SELECIONADA
    // =====================================================

    public bool TemSementeSelecionada()
    {
        if (sementeSelecionada == 0)
            return false;

        return GetQuantidadeSemente(
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

        if (GetQuantidadeSemente(
                sementeSelecionada) <= 0)
        {
            sementeSelecionada = 0;
        }
    }

    // =====================================================
    // REGADOR
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

        if (!playerMoney.RemoverDinheiro(
                precoRegador))
        {
            TocarSomSemDinheiro();

            Debug.Log(
                "❌ Você não tem dinheiro suficiente para comprar o regador!"
            );

            return;
        }

        regadorNaMao =
            Instantiate(
                regadorPrefab,
                holdingPoint.position,
                holdingPoint.rotation
            );

        regadorNaMao.transform.SetParent(
            holdingPoint
        );

        regadorNaMao.transform.localPosition =
            Vector3.zero;

        regadorNaMao.transform.localRotation =
            Quaternion.identity;

        Rigidbody rb =
            regadorNaMao.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col =
            regadorNaMao.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        if (audioSource != null &&
            somCompraRegador != null)
        {
            audioSource.PlayOneShot(
                somCompraRegador
            );
        }

        Debug.Log(
            "💧 Regador comprado!"
        );
    }

    // =====================================================
    // SOM SEM DINHEIRO
    // =====================================================

    void TocarSomSemDinheiro()
    {
        if (audioSourceSemDinheiro != null &&
            somSemDinheiro != null)
        {
            audioSourceSemDinheiro.PlayOneShot(
                somSemDinheiro
            );
        }
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
    }

    // =====================================================
    // REGADOR
    // =====================================================

    public bool EstaSegurandoRegador()
    {
        return regadorNaMao != null;
    }

    public void UsarRegador()
    {
        if (regadorNaMao == null)
            return;

        Destroy(regadorNaMao);

        regadorNaMao = null;
    }

    // =====================================================
    // UI DA LOJA
    // =====================================================

    void OnGUI()
    {
        if (!playerPerto ||
            playerMoney == null)
            return;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 18;
        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura = 550;
        float altura = 330;

        float x =
            (Screen.width - largura) / 2;

        float y =
            Screen.height - 400;

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
            "  [Qtd: " +
            quantidadeArvoreNivel1 +
            "]\n" +

            "2 - " +
            arvoreNivel2.nome +
            " - R$" +
            arvoreNivel2.preco +
            "  [Qtd: " +
            quantidadeArvoreNivel2 +
            "]\n" +

            "3 - " +
            arvoreNivel3.nome +
            " - R$" +
            arvoreNivel3.preco +
            "  [Qtd: " +
            quantidadeArvoreNivel3 +
            "]\n" +

            "4 - " +
            pinheiro.nome +
            " - R$" +
            pinheiro.preco +
            "  [Qtd: " +
            quantidadePinheiro +
            "]\n" +

            "5 - " +
            macieira.nome +
            " - R$" +
            macieira.preco +
            "  [Qtd: " +
            quantidadeMacieira +
            "]\n\n" +

            "Q - Regador - R$" +
            precoRegador +

            "\n\n🌱 Selecionada: " +
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