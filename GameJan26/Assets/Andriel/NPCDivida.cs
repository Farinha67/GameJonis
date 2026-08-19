using UnityEngine;
using UnityEngine.InputSystem;

public class NPCDivida : MonoBehaviour
{
    // =====================================================
    // PLAYER
    // =====================================================

    [Header("Player")]
    public Transform player;

    // =====================================================
    // MOVIMENTO
    // =====================================================

    [Header("Movimento")]
    public float velocidade = 1.5f;
    public float distanciaParar = 2f;

    // =====================================================
    // DISTÂNCIA PARA COBRAR
    // =====================================================

    [Header("Distância para cobrar")]
    public float distanciaParaCobrar = 2.5f;

    // =====================================================
    // DINHEIRO DO PLAYER
    // =====================================================

    private PlayerMoney playerMoney;

    // =====================================================
    // ANIMAÇÃO
    // =====================================================

    [Header("Animação")]
    public Animator animator;

    // =====================================================
    // PARCELAS
    // =====================================================

    [Header("Parcelas")]
    public int parcela1 = 150;
    public int parcela2 = 250;
    public int parcela3 = 350;

    // =====================================================
    // TEMPO
    // =====================================================

    [Header("Tempo das cobranças")]
    public float primeiraCobranca = 30f;
    public float tempoEntreCobrancas = 30f;

    // =====================================================
    // SOM
    // =====================================================

    [Header("Som de Pagamento")]
    public AudioSource audioPagamento;
    public AudioClip somPagamento;
    public float volumePagamento = 1f;

    // =====================================================
    // ESTADO
    // =====================================================

    private int parcelaAtual = 0;

    private bool indoAtePlayer = false;
    private bool esperando = false;
    private bool cobrando = false;
    private bool dividaQuitada = false;

    private float tempoEspera = 0f;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        // -------------------------------------------------
        // ENCONTRAR PLAYER
        // -------------------------------------------------

        if (player == null)
        {
            GameObject objPlayer =
                GameObject.FindGameObjectWithTag("Player");

            if (objPlayer != null)
            {
                player = objPlayer.transform;
            }
            else
            {
                Debug.LogError(
                    "❌ NPCDivida: Player não encontrado! " +
                    "Coloque a Tag 'Player' no Player."
                );

                return;
            }
        }

        // -------------------------------------------------
        // ENCONTRAR PLAYER MONEY
        // -------------------------------------------------

        playerMoney =
            player.GetComponent<PlayerMoney>();

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ NPCDivida: PlayerMoney não encontrado no Player!"
            );
        }

        // -------------------------------------------------
        // ENCONTRAR ANIMATOR
        // -------------------------------------------------

        if (animator == null)
        {
            animator =
                GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning(
                "⚠️ NPCDivida: Animator não encontrado no NPC."
            );
        }

        // -------------------------------------------------
        // CONFIGURAR SOM
        // -------------------------------------------------

        if (audioPagamento != null)
        {
            audioPagamento.playOnAwake = false;
            audioPagamento.loop = false;
            audioPagamento.volume = volumePagamento;
        }

        // -------------------------------------------------
        // COMEÇA PARADO
        // -------------------------------------------------

        AtualizarAnimacao(false);

        // -------------------------------------------------
        // PRIMEIRA COBRANÇA
        // -------------------------------------------------

        esperando = true;
        tempoEspera = primeiraCobranca;

        Debug.Log(
            "⏰ Primeira cobrança acontecerá em " +
            primeiraCobranca +
            " segundos."
        );

        Debug.Log(
            "💰 Dívida total: R$" +
            (parcela1 + parcela2 + parcela3)
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (player == null ||
            playerMoney == null ||
            dividaQuitada)
        {
            return;
        }

        // =================================================
        // ESPERANDO PRÓXIMA COBRANÇA
        // =================================================

        if (esperando)
        {
            tempoEspera -= Time.deltaTime;

            if (tempoEspera <= 0f)
            {
                esperando = false;
                indoAtePlayer = true;

                AtualizarAnimacao(true);

                Debug.Log(
                    "💰 COBRADOR ESTÁ VINDO!"
                );
            }

            return;
        }

        // =================================================
        // INDO ATÉ O PLAYER
        // =================================================

        if (indoAtePlayer)
        {
            IrAtePlayer();

            float distancia =
                Vector3.Distance(
                    transform.position,
                    player.position
                );

            if (distancia <= distanciaParaCobrar)
            {
                indoAtePlayer = false;
                cobrando = true;

                AtualizarAnimacao(false);

                OlharParaPlayer();

                Debug.Log(
                    "💰 COBRADOR CHEGOU NO PLAYER!"
                );
            }

            return;
        }

        // =================================================
        // COBRANDO
        // =================================================

        if (cobrando)
        {
            AtualizarAnimacao(false);

            OlharParaPlayer();

            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                PagarParcela();
            }
        }
    }

    // =====================================================
    // MOVIMENTO ATÉ O PLAYER
    // =====================================================

    void IrAtePlayer()
    {
        if (player == null)
            return;

        Vector3 direcao =
            player.position - transform.position;

        // Ignora altura
        direcao.y = 0f;

        float distancia =
            direcao.magnitude;

        // -------------------------------------------------
        // AINDA PRECISA ANDAR
        // -------------------------------------------------

        if (distancia > distanciaParar)
        {
            AtualizarAnimacao(true);

            Vector3 movimento =
                direcao.normalized *
                velocidade *
                Time.deltaTime;

            transform.position += movimento;

            // ---------------------------------------------
            // OLHAR PARA O PLAYER
            // ---------------------------------------------

            if (direcao != Vector3.zero)
            {
                Quaternion rotacao =
                    Quaternion.LookRotation(direcao);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        rotacao,
                        8f * Time.deltaTime
                    );
            }
        }
        else
        {
            AtualizarAnimacao(false);
        }
    }

    // =====================================================
    // OLHAR PARA PLAYER
    // =====================================================

    void OlharParaPlayer()
    {
        if (player == null)
            return;

        Vector3 direcao =
            player.position - transform.position;

        direcao.y = 0f;

        if (direcao.sqrMagnitude > 0.01f)
        {
            Quaternion rotacao =
                Quaternion.LookRotation(direcao);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    rotacao,
                    8f * Time.deltaTime
                );
        }
    }

    // =====================================================
    // ANIMAÇÃO
    // =====================================================

    void AtualizarAnimacao(bool andando)
    {
        if (animator == null)
            return;

        animator.SetBool(
            "Andando",
            andando
        );
    }

    // =====================================================
    // PAGAR PARCELA
    // =====================================================

    void PagarParcela()
    {
        int valor = 0;

        // -------------------------------------------------
        // DEFINIR VALOR
        // -------------------------------------------------

        if (parcelaAtual == 0)
        {
            valor = parcela1;
        }
        else if (parcelaAtual == 1)
        {
            valor = parcela2;
        }
        else if (parcelaAtual == 2)
        {
            valor = parcela3;
        }
        else
        {
            return;
        }

        // -------------------------------------------------
        // VERIFICAR DINHEIRO
        // -------------------------------------------------

        if (!playerMoney.TemDinheiro(valor))
        {
            Debug.Log(
                "❌ VOCÊ NÃO TEM DINHEIRO!"
            );

            Debug.Log(
                "💬 Cobrador: " +
                "\"Você não tem dinheiro? " +
                "Tudo bem, volto mais tarde.\""
            );

            IrEmbora();

            esperando = true;
            tempoEspera = tempoEntreCobrancas;

            return;
        }

        // -------------------------------------------------
        // REMOVER DINHEIRO
        // -------------------------------------------------

        bool pagamento =
            playerMoney.RemoverDinheiro(valor);

        if (!pagamento)
        {
            Debug.LogWarning(
                "⚠️ Não foi possível remover o dinheiro."
            );

            return;
        }

        // -------------------------------------------------
        // SOM
        // -------------------------------------------------

        if (audioPagamento != null &&
            somPagamento != null)
        {
            audioPagamento.PlayOneShot(
                somPagamento,
                volumePagamento
            );
        }

        // -------------------------------------------------
        // DEBUG
        // -------------------------------------------------

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "💰 PARCELA PAGA!"
        );

        Debug.Log(
            "💵 Valor: R$" +
            valor
        );

        Debug.Log(
            "💵 Dinheiro restante: R$" +
            playerMoney.GetDinheiro()
        );

        Debug.Log(
            "================================"
        );

        // -------------------------------------------------
        // PRÓXIMA PARCELA
        // -------------------------------------------------

        parcelaAtual++;

        cobrando = false;

        // -------------------------------------------------
        // TERMINOU A DÍVIDA?
        // -------------------------------------------------

        if (parcelaAtual >= 3)
        {
            QuitarDivida();
            return;
        }

        // -------------------------------------------------
        // IR EMBORA
        // -------------------------------------------------

        Debug.Log(
            "✅ Parcela paga!"
        );

        Debug.Log(
            "⏰ Próxima cobrança em " +
            tempoEntreCobrancas +
            " segundos."
        );

        IrEmbora();

        esperando = true;
        tempoEspera = tempoEntreCobrancas;
    }

    // =====================================================
    // NPC VAI EMBORA
    // =====================================================

    void IrEmbora()
    {
        cobrando = false;
        indoAtePlayer = false;

        AtualizarAnimacao(false);

        if (player == null)
            return;

        Vector3 direcao =
            transform.position -
            player.position;

        direcao.y = 0f;

        if (direcao.sqrMagnitude > 0.01f)
        {
            // Afasta o NPC do player
            transform.position +=
                direcao.normalized * 8f;
        }

        Debug.Log(
            "🚶 COBRADOR FOI EMBORA."
        );
    }

    // =====================================================
    // QUITAR DÍVIDA
    // =====================================================

    void QuitarDivida()
    {
        dividaQuitada = true;

        cobrando = false;
        esperando = false;
        indoAtePlayer = false;

        AtualizarAnimacao(false);

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "🏡 DÍVIDA QUITADA!"
        );

        Debug.Log(
            "💰 Todas as 3 parcelas foram pagas!"
        );

        Debug.Log(
            "💵 Total pago: R$" +
            (parcela1 + parcela2 + parcela3)
        );

        Debug.Log(
            "🌾 AS TERRAS AGORA SÃO DO PLAYER!"
        );

        Debug.Log(
            "================================"
        );

        // -------------------------------------------------
        // AFASTAR DO PLAYER
        // -------------------------------------------------

        if (player != null)
        {
            Vector3 direcao =
                transform.position -
                player.position;

            direcao.y = 0f;

            if (direcao.sqrMagnitude > 0.01f)
            {
                transform.position +=
                    direcao.normalized * 10f;
            }
        }

        // -------------------------------------------------
        // DESTRUIR NPC
        // -------------------------------------------------

        Destroy(gameObject);
    }

    // =====================================================
    // TEXTO NA TELA
    // =====================================================

    void OnGUI()
    {
        if (!cobrando ||
            dividaQuitada)
        {
            return;
        }

        int valor = 0;

        if (parcelaAtual == 0)
        {
            valor = parcela1;
        }
        else if (parcelaAtual == 1)
        {
            valor = parcela2;
        }
        else if (parcelaAtual == 2)
        {
            valor = parcela3;
        }

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 22;
        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura = 500f;
        float altura = 150f;

        float x =
            (Screen.width - largura) / 2f;

        float y =
            Screen.height - 220f;

        GUI.Box(
            new Rect(
                x,
                y,
                largura,
                altura
            ),

            "💰 COBRADOR\n\n" +

            "Parcela " +
            (parcelaAtual + 1) +
            "/3\n" +

            "Valor: R$" +
            valor +

            "\n\nPRESSIONE E PARA PAGAR",

            estilo
        );
    }
}