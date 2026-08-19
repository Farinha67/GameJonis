using UnityEngine;

public class TrashTaskManager : MonoBehaviour
{
    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("Configuração da Tarefa")]
    public bool tarefaAtiva = true;

    [Header("Recompensa Final")]
    public int recompensaFinal = 100;

    // =====================================================
    // DADOS
    // =====================================================

    private int totalLixos;
    private int lixosColetados;

    private bool tarefaConcluida;

    // =====================================================
    // PLAYER
    // =====================================================

    private PlayerMoney playerMoney;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerMoney =
                player.GetComponent<PlayerMoney>();
        }

        AtualizarQuantidadeLixos();

        Debug.Log(
            "🗑️ TAREFA INICIADA!"
        );

        Debug.Log(
            "🗑️ Total de lixos: " +
            totalLixos
        );
    }

    // =====================================================
    // CONTAR LIXOS DA CENA
    // =====================================================

    public void AtualizarQuantidadeLixos()
    {
        TrashValue[] lixos =
            FindObjectsByType<TrashValue>(
                FindObjectsSortMode.None
            );

        totalLixos =
            lixos.Length;
    }

    // =====================================================
    // LIXO COLOCADO NA LIXEIRA
    // =====================================================

    public void LixoColocadoNaLixeira(
        TrashValue lixo)
    {
        if (!tarefaAtiva)
            return;

        if (tarefaConcluida)
            return;

        if (lixo == null)
            return;

        lixosColetados++;

        Debug.Log(
            "🗑️ Lixo descartado corretamente!"
        );

        Debug.Log(
            "📊 Progresso: " +
            lixosColetados +
            " / " +
            totalLixos
        );

        // =================================================
        // VERIFICAR SE TERMINOU
        // =================================================

        if (lixosColetados >= totalLixos)
        {
            ConcluirTarefa();
        }
    }

    // =====================================================
    // CONCLUIR TAREFA
    // =====================================================

    private void ConcluirTarefa()
    {
        if (tarefaConcluida)
            return;

        tarefaConcluida =
            true;

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "🎉 TAREFA CONCLUÍDA!"
        );

        Debug.Log(
            "🗑️ Todos os lixos foram colocados nas lixeiras!"
        );

        Debug.Log(
            "💰 Recompensa final: R$" +
            recompensaFinal
        );

        Debug.Log(
            "=============================="
        );

        if (playerMoney != null)
        {
            playerMoney.AdicionarDinheiro(
                recompensaFinal
            );
        }
    }

    // =====================================================
    // GUI
    // =====================================================

    private void OnGUI()
    {
        if (!tarefaAtiva)
            return;

        // =================================================
        // ESTILO
        // =================================================

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 18;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.UpperLeft;

        estilo.wordWrap =
            true;

        estilo.padding =
            new RectOffset(
                15,
                15,
                12,
                12
            );

        // =================================================
        // CONCLUÍDA
        // =================================================

        if (tarefaConcluida)
        {
            GUI.Box(
                new Rect(
                    20f,
                    20f,
                    380f,
                    130f
                ),

                "🎉 TAREFA CONCLUÍDA!\n\n" +
                "🗑️ Todos os lixos foram descartados!\n\n" +
                "💰 Recompensa: R$" +
                recompensaFinal,

                estilo
            );

            return;
        }

        // =================================================
        // TAREFA
        // =================================================

        string texto =
            "🗑️ TAREFA\n\n" +
            "Coloque todos os lixos nas lixeiras\n\n" +
            "Lixos: " +
            lixosColetados +
            " / " +
            totalLixos +
            "\n\n" +
            "💰 Cada lixo dá dinheiro";

        GUI.Box(
            new Rect(
                20f,
                20f,
                380f,
                190f
            ),

            texto,

            estilo
        );
    }

    // =====================================================
    // GETTERS
    // =====================================================

    public int GetTotalLixos()
    {
        return totalLixos;
    }

    public int GetLixosColetados()
    {
        return lixosColetados;
    }

    public bool TarefaConcluida()
    {
        return tarefaConcluida;
    }
}