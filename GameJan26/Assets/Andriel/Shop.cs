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
    // REGADOR
    // =====================================================

    [Header("Regador")]
    public int precoRegador = 15;
    public GameObject regadorPrefab;

    [Tooltip("Ponto onde a árvore aparece na mão.")]
    public Transform holdingPoint;

    [Tooltip("Ponto onde o regador aparece na mão.")]
    public Transform regadorHoldingPoint;

    // =====================================================
    // ESCALA DA ÁRVORE NA MÃO
    // =====================================================

    [Header("Árvore na mão")]
    public float escalaArvoreNaMao = 0.15f;

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

    /*
        0 = nenhuma
        1 = árvore nível 1
        2 = árvore nível 2
        3 = árvore nível 3
        4 = pinheiro
        5 = macieira
    */

    // =====================================================
    // START
    // =====================================================

    private void Start()
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

        // Se não tiver um ponto específico para o regador,
        // usa o holdingPoint.
        if (regadorHoldingPoint == null)
        {
            regadorHoldingPoint = holdingPoint;
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

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaLoja;

        // =================================================
        // COMPRAS
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

            // R = comprar/pegar regador
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ComprarRegador();
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
                "❌ Semente " + numero + " não configurada!"
            );

            return;
        }

        if (semente.prefabArvore == null)
        {
            Debug.LogError(
                "❌ O prefab da árvore '" +
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

            return;
        }

        AdicionarSemente(numero);

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

    private void AdicionarSemente(int numero)
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

    private void SelecionarSemente(int numero)
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

        // Ao selecionar árvore,
        // o regador sai da mão.
        GuardarRegador();

        // Mostra a árvore na mão.
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
            Vector3.one * escalaArvoreNaMao;

        // Desliga física da árvore na mão.
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

        Debug.Log(
            "🌳 Árvore apareceu na mão!"
        );
    }

    // =====================================================
    // DESTRUIR ÁRVORE DA MÃO
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
    // PEGAR PREFAB
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

    private string GetNomeSemente(int numero)
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

        // Atualiza árvore da mão.
        if (GetQuantidadeSemente(
            sementeSelecionada) > 0)
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
    // REGADOR
    // =====================================================

    private void ComprarRegador()
    {
        // Se já está segurando,
        // não compra outro.
        if (regadorNaMao != null)
        {
            Debug.Log(
                "💧 Você já está segurando o regador!"
            );

            return;
        }

        if (regadorPrefab == null)
        {
            Debug.LogError(
                "❌ Regador Prefab não configurado!"
            );

            return;
        }

        if (regadorHoldingPoint == null)
        {
            Debug.LogError(
                "❌ Regador Holding Point não configurado!"
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

        // Árvore sai da mão.
        DestruirArvoreNaMao();

        regadorNaMao =
            Instantiate(
                regadorPrefab,
                regadorHoldingPoint
            );

        regadorNaMao.transform.localPosition =
            Vector3.zero;

        regadorNaMao.transform.localRotation =
            Quaternion.identity;

        regadorNaMao.transform.localScale =
            Vector3.one;

        Rigidbody[] rigidbodies =
            regadorNaMao.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] colliders =
            regadorNaMao.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
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
            "💧 Regador comprado e equipado!"
        );
    }

    // =====================================================
    // GUARDAR REGADOR
    // =====================================================

    public void GuardarRegador()
    {
        if (regadorNaMao == null)
            return;

        Destroy(
            regadorNaMao
        );

        regadorNaMao = null;

        Debug.Log(
            "💧 Regador saiu da mão."
        );
    }

    // =====================================================
    // ESTÁ SEGURANDO REGADOR
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
        {
            Debug.Log(
                "❌ Você não está segurando o regador!"
            );

            return;
        }

        // Não destrói o regador.
        // Apenas continua equipado.
        Debug.Log(
            "💧 Regador usado."
        );
    }

    // =====================================================
    // VALOR DA SEMENTE
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

    public int GetValorSemente(int numero)
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
    // SOM SEM DINHEIRO
    // =====================================================

    private void TocarSomSemDinheiro()
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
        float altura = 330f;

        float x =
            (Screen.width - largura) / 2f;

        float y =
            Screen.height - 400f;

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

            "R - Regador - R$" +
            precoRegador +

            "\n\n🌱 Selecionada: " +
            selecionada +

            "\n💧 Regador: " +
            (
                regadorNaMao != null
                    ? "NA MÃO"
                    : "GUARDADO"
            ) +

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