using UnityEngine;

public class GameControlsUI : MonoBehaviour
{
    [Header("UI")]
    public bool comandosAtivos = true;

    [Header("Tecla para abrir/fechar")]
    public KeyCode teclaComandos = KeyCode.F;

    private GUIStyle tituloStyle;
    private GUIStyle comandoStyle;
    private GUIStyle fundoStyle;

    private bool estilosCriados = false;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // Não criar GUIStyle aqui.
        // Os estilos são criados dentro do OnGUI.
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Input.GetKeyDown(teclaComandos))
        {
            comandosAtivos =
                !comandosAtivos;

            Debug.Log(
                comandosAtivos
                    ? "📖 Comandos ativados."
                    : "📖 Comandos desativados."
            );
        }
    }

    // =====================================================
    // ON GUI
    // =====================================================

    private void OnGUI()
    {
        if (!comandosAtivos)
            return;

        CriarEstilos();

        float largura = 350f;
        float altura = 300f;

        float x =
            Screen.width -
            largura -
            20f;

        float y =
            Screen.height -
            altura -
            20f;

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
            GUIContent.none,
            fundoStyle
        );

        // =================================================
        // TÍTULO
        // =================================================

        GUI.Label(
            new Rect(
                x + 15f,
                y + 10f,
                largura - 30f,
                35f
            ),
            "🎮 COMANDOS",
            tituloStyle
        );

        // =================================================
        // COMANDOS
        // =================================================

        string comandos =
            "E  →  Pegar / soltar lixo\n" +
            "Q  →  Colocar lixo na lixeira\n" +
            "E  →  Regar / colher árvore\n" +
            "\n" +

            "1  →  Selecionar Árvore Nível 1\n" +
            "2  →  Selecionar Árvore Nível 2\n" +
            "3  →  Selecionar Árvore Nível 3\n" +
            "4  →  Selecionar Pinheiro\n" +
            "5  →  Selecionar Macieira\n" +
            "\n" +

            "F  →  Mostrar / esconder comandos\n" +
            "Mouse  →  Olhar ao redor";

        GUI.Label(
            new Rect(
                x + 15f,
                y + 50f,
                largura - 30f,
                altura - 60f
            ),
            comandos,
            comandoStyle
        );
    }

    // =====================================================
    // CRIAR ESTILOS
    // =====================================================

    private void CriarEstilos()
    {
        if (estilosCriados)
            return;

        // =================================================
        // TÍTULO
        // =================================================

        tituloStyle =
            new GUIStyle(
                GUI.skin.label
            );

        tituloStyle.fontSize =
            22;

        tituloStyle.fontStyle =
            FontStyle.Bold;

        tituloStyle.alignment =
            TextAnchor.MiddleCenter;

        // =================================================
        // COMANDOS
        // =================================================

        comandoStyle =
            new GUIStyle(
                GUI.skin.label
            );

        comandoStyle.fontSize =
            16;

        comandoStyle.fontStyle =
            FontStyle.Bold;

        comandoStyle.alignment =
            TextAnchor.UpperLeft;

        comandoStyle.wordWrap =
            true;

        // =================================================
        // FUNDO
        // =================================================

        fundoStyle =
            new GUIStyle(
                GUI.skin.box
            );

        estilosCriados =
            true;
    }
}