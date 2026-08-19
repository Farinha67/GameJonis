using UnityEngine;
using UnityEngine.InputSystem;

public class GameControlsUI : MonoBehaviour
{
    [Header("Posição")]
    public float margemDireita = 20f;
    public float margemBaixo = 20f;

    [Header("Tamanho")]
    public float largura = 320f;
    public float altura = 330f;

    [Header("Texto")]
    public int tamanhoTexto = 17;

    [Header("Painel")]
    [Range(0f, 1f)]
    public float transparenciaFundo = 0.75f;

    // =====================================================
    // ESTADO
    // =====================================================

    private bool comandosVisiveis = true;

    // =====================================================
    // ESTILOS
    // =====================================================

    private GUIStyle estiloFundo;
    private GUIStyle estiloTexto;
    private GUIStyle estiloTitulo;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        comandosVisiveis = true;

        CriarEstilos();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // =================================================
        // F = ABRIR / FECHAR COMANDOS
        // =================================================

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            comandosVisiveis =
                !comandosVisiveis;
        }
    }

    // =====================================================
    // CRIAR ESTILOS
    // =====================================================

    private void CriarEstilos()
    {
        // =================================================
        // FUNDO
        // =================================================

        estiloFundo =
            new GUIStyle(
                GUI.skin.box
            );

        estiloFundo.alignment =
            TextAnchor.UpperLeft;

        estiloFundo.padding =
            new RectOffset(
                15,
                15,
                12,
                12
            );

        // =================================================
        // TEXTO
        // =================================================

        estiloTexto =
            new GUIStyle(
                GUI.skin.label
            );

        estiloTexto.fontSize =
            tamanhoTexto;

        estiloTexto.fontStyle =
            FontStyle.Normal;

        estiloTexto.alignment =
            TextAnchor.UpperLeft;

        estiloTexto.wordWrap =
            true;

        // =================================================
        // TÍTULO
        // =================================================

        estiloTitulo =
            new GUIStyle(
                GUI.skin.label
            );

        estiloTitulo.fontSize =
            tamanhoTexto + 4;

        estiloTitulo.fontStyle =
            FontStyle.Bold;

        estiloTitulo.alignment =
            TextAnchor.MiddleCenter;
    }

    // =====================================================
    // GUI
    // =====================================================

    private void OnGUI()
    {
        // =================================================
        // SE ESTIVER FECHADO
        // =================================================

        if (!comandosVisiveis)
            return;

        // =================================================
        // GARANTIR ESTILOS
        // =================================================

        if (estiloFundo == null ||
            estiloTexto == null ||
            estiloTitulo == null)
        {
            CriarEstilos();
        }

        // =================================================
        // POSIÇÃO
        // =================================================

        float x =
            Screen.width -
            largura -
            margemDireita;

        float y =
            Screen.height -
            altura -
            margemBaixo;

        // =================================================
        // FUNDO
        // =================================================

        GUI.Box(
            new Rect(
                x,
                y,
                largura,
                altura
            ),
            "",
            estiloFundo
        );

        // =================================================
        // TÍTULO
        // =================================================

        GUI.Label(
            new Rect(
                x + 10f,
                y + 8f,
                largura - 20f,
                35f
            ),
            "COMANDOS",
            estiloTitulo
        );

        // =================================================
        // COMANDOS
        // =================================================

        string comandos =
            "[E]  Interagir\n\n" +

            "[1]  Árvore nível 1\n" +
            "[2]  Árvore nível 2\n" +
            "[3]  Árvore nível 3\n" +
            "[4]  Pinheiro\n" +
            "[5]  Macieira\n\n" +

            "[E]  Plantar árvore\n" +
            "[E]  Regar árvore\n" +
            "[E]  Colher árvore\n" +
            "[E]  Pegar regador\n" +
            "[E]  Soltar regador\n\n" +

            "[F]  Fechar comandos";

        GUI.Label(
            new Rect(
                x + 15f,
                y + 48f,
                largura - 30f,
                altura - 60f
            ),
            comandos,
            estiloTexto
        );
    }
}