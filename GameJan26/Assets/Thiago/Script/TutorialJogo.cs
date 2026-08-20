using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialJogo : MonoBehaviour
{
    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("Tutorial")]
    public bool iniciarAutomaticamente = true;

    [Header("Referências")]
    public Shop shop;
    public RegadorPickup regadorPickup;

    [Header("Distância para detectar")]
    public float distanciaDeteccao = 5f;

    // =====================================================
    // PLAYER
    // =====================================================

    private Transform player;

    // =====================================================
    // ESTADOS
    // =====================================================

    private enum Etapa
    {
        ComprarArvoreNivel1,
        SelecionarArvore,
        Plantar,
        PegarRegador,
        IrAoPoco,
        EncherRegador,
        RegarArvore,
        EsperarCrescer,
        Colher,
        Finalizado
    }

    private Etapa etapaAtual;

    // =====================================================
    // CONTROLE
    // =====================================================

    private bool tutorialAtivo;

    private int quantidadeInicialArvore1;

    private PlantSpot arvorePlantada;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError(
                "❌ TutorialJogo: Player não encontrado!"
            );

            return;
        }

        player =
            playerObject.transform;

        // =================================================
        // SHOP
        // =================================================

        if (shop == null)
        {
            shop =
                FindFirstObjectByType<Shop>();
        }

        // =================================================
        // REGADOR
        // =================================================

        if (regadorPickup == null)
        {
            regadorPickup =
                playerObject.GetComponent<RegadorPickup>();
        }

        // =================================================
        // VERIFICAÇÕES
        // =================================================

        if (shop == null)
        {
            Debug.LogError(
                "❌ TutorialJogo: Shop não encontrado!"
            );
        }

        if (regadorPickup == null)
        {
            Debug.LogError(
                "❌ TutorialJogo: RegadorPickup não encontrado no Player!"
            );
        }

        // =================================================
        // QUANTIDADE INICIAL
        // =================================================

        quantidadeInicialArvore1 =
            shop != null
                ? shop.GetQuantidadeSemente(1)
                : 0;

        // =================================================
        // INICIAR
        // =================================================

        etapaAtual =
            Etapa.ComprarArvoreNivel1;

        tutorialAtivo =
            iniciarAutomaticamente;

        Debug.Log(
            "🎓 Tutorial iniciado!"
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!tutorialAtivo)
            return;

        if (player == null)
            return;

        AtualizarTutorial();
    }

    // =====================================================
    // ATUALIZAR TUTORIAL
    // =====================================================

    private void AtualizarTutorial()
    {
        if (shop == null)
            return;

        switch (etapaAtual)
        {
            case Etapa.ComprarArvoreNivel1:
                VerificarCompra();
                break;

            case Etapa.SelecionarArvore:
                VerificarSelecao();
                break;

            case Etapa.Plantar:
                VerificarPlantio();
                break;

            case Etapa.PegarRegador:
                VerificarRegador();
                break;

            case Etapa.IrAoPoco:
                VerificarPoco();
                break;

            case Etapa.EncherRegador:
                VerificarRegadorCheio();
                break;

            case Etapa.RegarArvore:
                VerificarRega();
                break;

            case Etapa.EsperarCrescer:
                VerificarCrescimento();
                break;

            case Etapa.Colher:
                VerificarColheita();
                break;

            case Etapa.Finalizado:
                FinalizarTutorial();
                break;
        }
    }

    // =====================================================
    // 1 - COMPRAR
    // =====================================================

    private void VerificarCompra()
    {
        int quantidadeAtual =
            shop.GetQuantidadeSemente(1);

        if (quantidadeAtual >
            quantidadeInicialArvore1)
        {
            Avancar(
                Etapa.SelecionarArvore
            );
        }
    }

    // =====================================================
    // 2 - SELECIONAR
    // =====================================================

    private void VerificarSelecao()
    {
        if (shop.GetSementeSelecionada() == 1 &&
            shop.TemSementeSelecionada())
        {
            Avancar(
                Etapa.Plantar
            );
        }
    }

    // =====================================================
    // 3 - PLANTAR
    // =====================================================

    private void VerificarPlantio()
    {
        PlantSpot[] spots =
            FindObjectsByType<PlantSpot>(
                FindObjectsSortMode.None
            );

        foreach (PlantSpot spot in spots)
        {
            if (spot == null)
                continue;

            if (spot.TemArvore())
            {
                arvorePlantada =
                    spot;

                Avancar(
                    Etapa.PegarRegador
                );

                return;
            }
        }
    }

    // =====================================================
    // 4 - PEGAR REGADOR
    // =====================================================

    private void VerificarRegador()
    {
        if (regadorPickup == null)
            return;

        if (regadorPickup.EstaSegurandoRegador())
        {
            Avancar(
                Etapa.IrAoPoco
            );
        }
    }

    // =====================================================
    // 5 - IR AO POÇO
    // =====================================================

    private void VerificarPoco()
    {
        PocoAgua[] pocos =
            FindObjectsByType<PocoAgua>(
                FindObjectsSortMode.None
            );

        foreach (PocoAgua poco in pocos)
        {
            if (poco == null)
                continue;

            float distancia =
                Vector3.Distance(
                    player.position,
                    poco.transform.position
                );

            if (distancia <=
                distanciaDeteccao)
            {
                Avancar(
                    Etapa.EncherRegador
                );

                return;
            }
        }
    }

    // =====================================================
    // 6 - ENCHER REGADOR
    // =====================================================

    private void VerificarRegadorCheio()
    {
        if (regadorPickup == null)
            return;

        if (!regadorPickup.EstaSegurandoRegador())
            return;

        if (regadorPickup.EstaCheio())
        {
            Avancar(
                Etapa.RegarArvore
            );
        }
    }

    // =====================================================
    // 7 - REGAR
    // =====================================================

    private void VerificarRega()
    {
        if (arvorePlantada == null)
        {
            ProcurarArvoreNovamente();

            if (arvorePlantada == null)
                return;
        }

        if (regadorPickup == null)
            return;

        // Se o regador gastou água,
        // significa que a árvore foi regada.
        if (!regadorPickup.EstaCheio())
        {
            Avancar(
                Etapa.EsperarCrescer
            );
        }
    }

    // =====================================================
    // 8 - ESPERAR CRESCER
    // =====================================================

    private void VerificarCrescimento()
    {
        if (arvorePlantada == null)
        {
            ProcurarArvoreNovamente();

            if (arvorePlantada == null)
                return;
        }

        // =================================================
        // IMPORTANTE:
        // Só avança quando a árvore realmente terminou
        // de crescer.
        // =================================================

        if (arvorePlantada.terminouDeCrescer)
        {
            Avancar(
                Etapa.Colher
            );
        }
    }

    // =====================================================
    // 9 - COLHER
    // =====================================================

    private void VerificarColheita()
    {
        if (arvorePlantada == null)
            return;

        // =================================================
        // A árvore foi colhida
        // =================================================

        if (!arvorePlantada.TemArvore())
        {
            FinalizarTutorial();

            return;
        }
    }

    // =====================================================
    // PROCURAR ÁRVORE
    // =====================================================

    private void ProcurarArvoreNovamente()
    {
        PlantSpot[] spots =
            FindObjectsByType<PlantSpot>(
                FindObjectsSortMode.None
            );

        foreach (PlantSpot spot in spots)
        {
            if (spot == null)
                continue;

            if (spot.TemArvore())
            {
                arvorePlantada =
                    spot;

                return;
            }
        }
    }

    // =====================================================
    // AVANÇAR
    // =====================================================

    private void Avancar(
        Etapa novaEtapa
    )
    {
        etapaAtual =
            novaEtapa;

        Debug.Log(
            "🎓 Tutorial: " +
            ObterTextoEtapa()
        );
    }

    // =====================================================
    // FINALIZAR TUTORIAL
    // =====================================================

    private void FinalizarTutorial()
    {
        // =================================================
        // EVITAR EXECUTAR VÁRIAS VEZES
        // =================================================

        if (!tutorialAtivo)
            return;

        etapaAtual =
            Etapa.Finalizado;

        // =================================================
        // DESATIVAR IMEDIATAMENTE
        // =================================================

        tutorialAtivo =
            false;

        // =================================================
        // LIMPAR REFERÊNCIA
        // =================================================

        arvorePlantada =
            null;

        Debug.Log(
            "🎉 TUTORIAL CONCLUÍDO!"
        );

        Debug.Log(
            "🎓 Mensagem do tutorial removida."
        );
    }

    // =====================================================
    // TEXTO
    // =====================================================

    private string ObterTextoEtapa()
    {
        switch (etapaAtual)
        {
            case Etapa.ComprarArvoreNivel1:
                return "Compre a Árvore Nível 1 na loja.";

            case Etapa.SelecionarArvore:
                return "Selecione a Árvore Nível 1.";

            case Etapa.Plantar:
                return "Vá até uma área de plantio e plante a árvore.";

            case Etapa.PegarRegador:
                return "Pegue o regador.";

            case Etapa.IrAoPoco:
                return "Vá até o poço de água.";

            case Etapa.EncherRegador:
                return "Encha o regador no poço.";

            case Etapa.RegarArvore:
                return "Regue a árvore.";

            case Etapa.EsperarCrescer:
                return "Espere a árvore crescer.";

            case Etapa.Colher:
                return "Colete a árvore quando ela estiver pronta.";

            case Etapa.Finalizado:
                return "";

            default:
                return "";
        }
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        // =================================================
        // SE O TUTORIAL ESTIVER DESATIVADO,
        // NÃO MOSTRA ABSOLUTAMENTE NADA
        // =================================================

        if (!tutorialAtivo)
            return;

        // =================================================
        // SEGURANÇA
        // =================================================

        if (etapaAtual == Etapa.Finalizado)
            return;

        GUIStyle estilo =
            CriarEstilo();

        GUI.Box(
            new Rect(
                (Screen.width - 600f) / 2f,
                35f,
                600f,
                85f
            ),
            "TUTORIAL\n\n" +
            ObterTextoEtapa(),
            estilo
        );
    }

    // =====================================================
    // ESTILO
    // =====================================================

    private GUIStyle CriarEstilo()
    {
        GUIStyle estilo =
            new GUIStyle(
                GUI.skin.box
            );

        estilo.fontSize =
            20;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        estilo.wordWrap =
            true;

        return estilo;
    }

    // =====================================================
    // ATIVAR TUTORIAL
    // =====================================================

    public void IniciarTutorial()
    {
        tutorialAtivo =
            true;

        etapaAtual =
            Etapa.ComprarArvoreNivel1;

        arvorePlantada =
            null;

        quantidadeInicialArvore1 =
            shop != null
                ? shop.GetQuantidadeSemente(1)
                : 0;

        Debug.Log(
            "🎓 Tutorial iniciado novamente."
        );
    }

    // =====================================================
    // PULAR TUTORIAL
    // =====================================================

    public void PularTutorial()
    {
        FinalizarTutorial();
    }

    // =====================================================
    // ESTADO
    // =====================================================

    public bool TutorialAtivo()
    {
        return tutorialAtivo;
    }

    // =====================================================
    // ETAPA ATUAL
    // =====================================================

    public string GetEtapaAtual()
    {
        if (!tutorialAtivo)
            return "";

        return ObterTextoEtapa();
    }
}