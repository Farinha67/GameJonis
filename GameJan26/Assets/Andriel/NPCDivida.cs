using UnityEngine;
using UnityEngine.InputSystem;

public class NPCDivida : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Movimento")]
    public float velocidade = 2.5f;
    public float distanciaParar = 2f;

    [Header("Distância para cobrar")]
    public float distanciaParaCobrar = 2.5f;

    [Header("Player Money")]
    private PlayerMoney playerMoney;

    [Header("Parcelas")]
    public int parcela1 = 100;
    public int parcela2 = 150;
    public int parcela3 = 200;

    [Header("Tempo entre cobranças")]
    public float tempoEntreCobrancas = 60f;

    private int parcelaAtual = 0;

    private bool indoAtePlayer = true;
    private bool esperando = false;
    private bool cobrando = false;
    private bool dividaQuitada = false;

    private float tempoEspera = 0f;

    void Start()
    {
        // =========================
        // ENCONTRAR PLAYER
        // =========================

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
                Debug.LogWarning(
                    "❌ Player não encontrado! " +
                    "Coloque a Tag Player no seu Player."
                );
            }
        }

        // =========================
        // ENCONTRAR PLAYER MONEY
        // =========================

        if (player != null)
        {
            playerMoney =
                player.GetComponent<PlayerMoney>();
        }

        if (playerMoney == null)
        {
            Debug.LogError(
                "❌ PlayerMoney não encontrado no Player!"
            );
        }
    }

    void Update()
    {
        if (player == null ||
            playerMoney == null ||
            dividaQuitada)
            return;

        // =========================
        // ESPERANDO 1 MINUTO
        // =========================

        if (esperando)
        {
            tempoEspera -= Time.deltaTime;

            if (tempoEspera <= 0)
            {
                esperando = false;
                indoAtePlayer = true;

                // NPC volta a aparecer
                gameObject.SetActive(true);

                Debug.Log(
                    "💰 O cobrador está voltando!"
                );
            }

            return;
        }

        // =========================
        // INDO ATÉ O PLAYER
        // =========================

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

                Debug.Log(
                    "💰 O cobrador chegou!"
                );
            }
        }

        // =========================
        // COBRANDO
        // =========================

        if (cobrando)
        {
            OlharParaPlayer();

            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                PagarParcela();
            }
        }
    }

    // =====================================================
    // MOVIMENTO
    // =====================================================

    void IrAtePlayer()
    {
        Vector3 direcao =
            player.position - transform.position;

        direcao.y = 0;

        if (direcao.magnitude > distanciaParar)
        {
            transform.position +=
                direcao.normalized *
                velocidade *
                Time.deltaTime;

            if (direcao != Vector3.zero)
            {
                Quaternion rotacao =
                    Quaternion.LookRotation(direcao);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        rotacao,
                        5f * Time.deltaTime
                    );
            }
        }
    }

    // =====================================================
    // OLHAR PARA PLAYER
    // =====================================================

    void OlharParaPlayer()
    {
        Vector3 direcao =
            player.position - transform.position;

        direcao.y = 0;

        if (direcao != Vector3.zero)
        {
            Quaternion rotacao =
                Quaternion.LookRotation(direcao);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    rotacao,
                    5f * Time.deltaTime
                );
        }
    }

    // =====================================================
    // PAGAR PARCELA
    // =====================================================

    void PagarParcela()
    {
        int valor = 0;

        if (parcelaAtual == 0)
            valor = parcela1;

        else if (parcelaAtual == 1)
            valor = parcela2;

        else if (parcelaAtual == 2)
            valor = parcela3;

        // =========================
        // VERIFICAR DINHEIRO
        // =========================

        if (!playerMoney.TemDinheiro(valor))
        {
            Debug.Log(
                "❌ Você não tem dinheiro suficiente!"
            );

            Debug.Log(
                "💬 O cobrador diz: " +
                "\"Você não tem dinheiro? " +
                "Tudo bem, volto mais tarde.\""
            );

            // NPC vai embora
            IrEmbora();

            // Espera 1 minuto
            esperando = true;
            tempoEspera = tempoEntreCobrancas;

            return;
        }

        // =========================
        // PAGAR
        // =========================

        bool pagamento =
            playerMoney.RemoverDinheiro(valor);

        if (!pagamento)
            return;

        Debug.Log(
            "💰 Parcela paga: R$" + valor
        );

        Debug.Log(
            "💵 Dinheiro restante: R$" +
            playerMoney.GetDinheiro()
        );

        // Próxima parcela
        parcelaAtual++;

        cobrando = false;

        // =========================
        // TERMINOU A DÍVIDA
        // =========================

        if (parcelaAtual >= 3)
        {
            QuitarDivida();
            return;
        }

        // =========================
        // ESPERAR 1 MINUTO
        // =========================

        Debug.Log(
            "✅ Parcela paga!"
        );

        Debug.Log(
            "⏰ Próxima cobrança em 1 minuto."
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

        Vector3 direcao =
            transform.position -
            player.position;

        direcao.y = 0;

        if (direcao != Vector3.zero)
        {
            transform.position +=
                direcao.normalized * 8f;
        }

        Debug.Log(
            "🚶 O cobrador foi embora."
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

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "🏡 FAZENDA QUITADA!"
        );

        Debug.Log(
            "💰 Todas as parcelas foram pagas!"
        );

        Debug.Log(
            "🌾 AS TERRAS AGORA SÃO DO PLAYER!"
        );

        Debug.Log(
            "=============================="
        );

        // NPC vai embora
        Vector3 direcao =
            transform.position -
            player.position;

        direcao.y = 0;

        transform.position +=
            direcao.normalized * 10f;

        // Desaparece
        Destroy(gameObject);
    }

    // =====================================================
    // TEXTO DA COBRANÇA
    // =====================================================

    void OnGUI()
    {
        if (!cobrando ||
            dividaQuitada)
            return;

        int valor = 0;

        if (parcelaAtual == 0)
            valor = parcela1;

        else if (parcelaAtual == 1)
            valor = parcela2;

        else if (parcelaAtual == 2)
            valor = parcela3;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 22;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura = 500;
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