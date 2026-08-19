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

        if (shop == null)
        {
            shop =
                FindFirstObjectByType<Shop>();
        }

        if (regadorPickup == null)
        {
            regadorPickup =
                playerObject.GetComponent<RegadorPickup>();
        }

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

        quantidadeInicialArvore1 =
            shop != null
                ? shop.GetQuantidadeSemente(1)
                : 0;

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
    // 6 - ENCHER
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

        // Se o regador já gastou água,
        // significa que a rega aconteceu.
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

        if (arvorePlantada.TemArvore())
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

        // Depois da colheita o PlantSpot
        // normalmente deixa de ter árvore.
        if (!arvorePlantada.TemArvore())
        {
            Avancar(
                Etapa.Finalizado
            );
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
                return "Tutorial concluído!";

            default:
                return "";
        }
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (!tutorialAtivo)
            return;

        if (etapaAtual ==
            Etapa.Finalizado)
        {
            GUIStyle finalStyle =
                CriarEstilo();

            GUI.Box(
                new Rect(
                    (Screen.width - 500f) / 2f,
                    35f,
                    500f,
                    70f
                ),
                "🎉 TUTORIAL CONCLUÍDO!\n" +
                "Você aprendeu a plantar e colher.",
                finalStyle
            );

            return;
        }

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

        estilo.fontSize = 20;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        estilo.wordWrap = true;

        return estilo;
    }

    // =====================================================
    // ATIVAR / DESATIVAR
    // =====================================================

    public void IniciarTutorial()
    {
        tutorialAtivo = true;

        etapaAtual =
            Etapa.ComprarArvoreNivel1;

        quantidadeInicialArvore1 =
            shop != null
                ? shop.GetQuantidadeSemente(1)
                : 0;
    }

    public void PularTutorial()
    {
        tutorialAtivo = false;

        etapaAtual =
            Etapa.Finalizado;
    }

    public bool TutorialAtivo()
    {
        return tutorialAtivo;
    }

    public string GetEtapaAtual()
    {
        return ObterTextoEtapa();
    }
}