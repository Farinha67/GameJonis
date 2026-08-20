using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlantSpot : MonoBehaviour
{
    // =====================================================
    // ÁREA DE PLANTIO
    // =====================================================

    [Header("Área de Plantio")]
    public PlantArea areaPlantio;

    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("Configuração")]
    public float distanciaPlantio = 5f;

    [Tooltip("Distância máxima para regar e colher a árvore.")]
    public float distanciaInteracaoArvore = 2f;

    public float distanciaEntreArvores = 0.35f;

    // =====================================================
    // CRESCIMENTO
    // =====================================================

    [Header("Crescimento")]
    public float tempoParaCrescer = 10f;

    // =====================================================
    // CONTROLADOR
    // =====================================================

    [Header("Controlador")]
    public bool controladorDePlantio = true;

    // =====================================================
    // RAYCAST
    // =====================================================

    [Header("Raycast")]
    public LayerMask camadaDoChao = ~0;

    // =====================================================
    // TAMANHO
    // =====================================================

    [Header("Tamanho das Árvores")]
    public float tamanhoMinimo = 0.8f;
    public float tamanhoMaximo = 2.5f;

    [Range(0f, 1f)]
    public float chanceColossal = 0.03f;

    public float tamanhoMinimoColossal = 3.5f;
    public float tamanhoMaximoColossal = 5f;

    // =====================================================
    // DINHEIRO
    // =====================================================

    [Header("Dinheiro por Tamanho")]
    public float multiplicadorMinimo = 0.8f;
    public float multiplicadorMaximo = 2.5f;
    public float multiplicadorColossal = 5f;

    // =====================================================
    // TAMANHO INICIAL
    // =====================================================

    [Header("Tamanho Inicial")]
    [Range(0.01f, 1f)]
    public float porcentagemTamanhoInicial = 0.3f;

    // =====================================================
    // SONS
    // =====================================================

    [Header("Som de Plantio")]
    public AudioSource audioSource;
    public AudioClip somPlantio;

    [Header("Som de Regar")]
    public AudioSource regarAudioSource;
    public AudioClip somRegar;

    [Header("Som de Crescimento")]
    public AudioSource crescimentoAudioSource;
    public AudioClip somCrescimento;

    [Header("Distância do Som")]
    public float distanciaMaximaSom = 15f;
    public float volumeMaximo = 1f;
    public float volumeMinimo = 0f;

    // =====================================================
    // PLAYER
    // =====================================================

    private Transform player;
    private Shop loja;
    private Camera cam;
    private RegadorPickup regadorPickup;

    // =====================================================
    // ÁRVORE
    // =====================================================

    [HideInInspector]
    public GameObject arvoreAtual;

    [HideInInspector]
    public bool foiRegada;

    [HideInInspector]
    public bool crescendo;

    [HideInInspector]
    public bool terminouDeCrescer;

    // =====================================================
    // DADOS
    // =====================================================

    private float tamanhoArvore = 1f;
    private float multiplicadorDinheiro = 1f;

    // Valor base da árvore.
    private int valorBaseArvore = 0;

    // Valor guardado da colheita.
    // Isso impede que o valor seja perdido quando a árvore
    // for destruída.
    private int valorColheitaGuardado = 0;

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

            regadorPickup =
                playerObject.GetComponent<RegadorPickup>();
        }

        loja =
            FindFirstObjectByType<Shop>();

        cam =
            Camera.main;

        if (controladorDePlantio)
        {
            arvoreAtual = null;

            foiRegada = false;

            crescendo = false;

            terminouDeCrescer = false;

            valorBaseArvore = 0;

            valorColheitaGuardado = 0;
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        AtualizarSomCrescimento();

        if (player == null)
            return;

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return;

        if (Keyboard.current == null)
            return;

        // =================================================
        // E = INTERAGIR
        // =================================================

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ProcessarInteracao();
        }
    }

    // =====================================================
    // INTERAÇÃO
    // =====================================================

    private void ProcessarInteracao()
    {
        // =================================================
        // PROCURAR ÁRVORE NA MIRA
        // =================================================

        PlantSpot spotMirado =
            EncontrarArvoreMirada();

        // =================================================
        // ÁRVORE ENCONTRADA
        // =================================================

        if (spotMirado != null)
        {
            Debug.Log(
                "🌳 Árvore encontrada na mira!"
            );

            // =============================================
            // REGAR
            // =============================================

            if (!spotMirado.foiRegada)
            {
                spotMirado.TentarRegar();

                return;
            }

            // =============================================
            // COLHER
            // =============================================

            if (spotMirado.terminouDeCrescer)
            {
                PlayerMoney dinheiro =
                    player != null
                        ? player.GetComponent<PlayerMoney>()
                        : null;

                // =================================================
                // IMPORTANTE:
                // PEGAR O VALOR ANTES DE COLHER.
                // =================================================

                int valor =
                    spotMirado.GetValorColheita();

                Debug.Log(
                    "💰 Valor calculado ANTES da colheita: R$" +
                    valor
                );

                // =================================================
                // COLHER
                // =================================================

                bool colheu =
                    spotMirado.ColherArvore();

                // =================================================
                // DAR DINHEIRO
                // =================================================

                if (colheu && dinheiro != null)
                {
                    if (valor > 0)
                    {
                        dinheiro.AdicionarDinheiro(
                            valor
                        );

                        Debug.Log(
                            "💵 DINHEIRO ADICIONADO: R$" +
                            valor
                        );

                        Debug.Log(
                            "💰 SALDO ATUAL: R$" +
                            dinheiro.GetDinheiro()
                        );
                    }
                    else
                    {
                        Debug.LogError(
                            "❌ A árvore foi colhida, mas o valor calculado foi R$0!"
                        );
                    }
                }

                return;
            }

            // =============================================
            // AINDA CRESCENDO
            // =============================================

            Debug.Log(
                "🌱 A árvore ainda está crescendo."
            );

            return;
        }

        // =================================================
        // SE NÃO FOR CONTROLADOR, NÃO PLANTA
        // =================================================

        if (!controladorDePlantio)
            return;

        // =================================================
        // RAYCAST PARA O CHÃO
        // =================================================

        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(
                    0.5f,
                    0.5f,
                    0f
                )
            );

        if (!Physics.Raycast(
            ray,
            out RaycastHit hitChao,
            distanciaPlantio,
            camadaDoChao,
            QueryTriggerInteraction.Collide))
        {
            Debug.Log(
                "❌ Nenhum chão válido encontrado."
            );

            return;
        }

        Vector3 ponto =
            hitChao.point;

        // =================================================
        // ÁREA
        // =================================================

        if (areaPlantio == null)
        {
            areaPlantio =
                FindFirstObjectByType<PlantArea>();
        }

        if (areaPlantio == null)
        {
            Debug.LogError(
                "❌ Nenhum PlantArea encontrado!"
            );

            return;
        }

        if (!areaPlantio.PodePlantar(ponto))
        {
            Debug.Log(
                "❌ Fora da área de plantio."
            );

            return;
        }

        // =================================================
        // SHOP
        // =================================================

        if (loja == null)
        {
            loja =
                FindFirstObjectByType<Shop>();
        }

        if (loja == null)
        {
            Debug.LogError(
                "❌ Shop não encontrada!"
            );

            return;
        }

        // =================================================
        // SEMENTE
        // =================================================

        if (!loja.TemSementeSelecionada())
        {
            Debug.Log(
                "❌ Você não possui uma semente selecionada!"
            );

            return;
        }

        // =================================================
        // DISTÂNCIA ENTRE ÁRVORES
        // =================================================

        if (!PodePlantarAqui(ponto))
            return;

        // =================================================
        // PLANTAR
        // =================================================

        PlantarNovaArvore(ponto);
    }

    // =====================================================
    // ENCONTRAR ÁRVORE MIRADA
    // =====================================================

    private PlantSpot EncontrarArvoreMirada()
    {
        if (cam == null)
            return null;

        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(
                    0.5f,
                    0.5f,
                    0f
                )
            );

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                distanciaInteracaoArvore,
                ~0,
                QueryTriggerInteraction.Collide
            );

        if (hits == null ||
            hits.Length == 0)
        {
            return null;
        }

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            PlantSpot spot =
                EncontrarPlantSpot(
                    hit.collider
                );

            if (spot == null)
                continue;

            if (spot.controladorDePlantio)
                continue;

            if (!spot.TemArvore())
                continue;

            // =================================================
            // DISTÂNCIA REAL DO PLAYER
            // =================================================

            if (player != null)
            {
                float distancia =
                    Vector3.Distance(
                        player.position,
                        spot.GetPosicaoArvore()
                    );

                if (distancia >
                    distanciaInteracaoArvore)
                {
                    continue;
                }
            }

            return spot;
        }

        return null;
    }

    // =====================================================
    // ENCONTRAR PLANTSPOT
    // =====================================================

    private PlantSpot EncontrarPlantSpot(
        Collider collider)
    {
        if (collider == null)
            return null;

        PlantSpot spot =
            collider.GetComponent<PlantSpot>();

        if (spot != null)
            return spot;

        spot =
            collider.GetComponentInParent<PlantSpot>();

        if (spot != null)
            return spot;

        spot =
            collider.GetComponentInChildren<PlantSpot>(
                true
            );

        if (spot != null)
            return spot;

        Transform root =
            collider.transform.root;

        if (root != null)
        {
            spot =
                root.GetComponentInChildren<PlantSpot>(
                    true
                );

            if (spot != null)
                return spot;
        }

        return null;
    }

    // =====================================================
    // PODE PLANTAR
    // =====================================================

    private bool PodePlantarAqui(
        Vector3 ponto)
    {
        PlantSpot[] spots =
            FindObjectsByType<PlantSpot>(
                FindObjectsSortMode.None
            );

        foreach (PlantSpot spot in spots)
        {
            if (spot == null)
                continue;

            if (spot.controladorDePlantio)
                continue;

            if (!spot.TemArvore())
                continue;

            float distancia =
                Vector3.Distance(
                    ponto,
                    spot.GetPosicaoArvore()
                );

            if (distancia <
                distanciaEntreArvores)
            {
                Debug.Log(
                    "🌳 Muito perto de outra árvore!"
                );

                return false;
            }
        }

        return true;
    }

    // =====================================================
    // PLANTAR ÁRVORE
    // =====================================================

    private void PlantarNovaArvore(
        Vector3 ponto)
    {
        GameObject prefab =
            loja.GetPrefabSementeSelecionada();

        if (prefab == null)
        {
            Debug.LogError(
                "❌ Prefab da árvore não encontrado!"
            );

            return;
        }

        // =================================================
        // PEGAR VALOR ANTES DE CONSUMIR A SEMENTE
        // =================================================

        int valorSemente =
            loja.GetValorSementeSelecionada();

        if (valorSemente <= 0)
        {
            Debug.LogError(
                "❌ O valor base da árvore é R$0!"
            );

            return;
        }

        GameObject novaArvore =
            Instantiate(
                prefab,
                ponto,
                Quaternion.identity
            );

        // =================================================
        // TAMANHO
        // =================================================

        SortearTamanho(
            novaArvore
        );

        // =================================================
        // CRIAR PLANTSPOT
        // =================================================

        GameObject objetoSpot =
            new GameObject(
                "PlantSpot"
            );

        objetoSpot.transform.SetParent(
            novaArvore.transform
        );

        objetoSpot.transform.localPosition =
            Vector3.zero;

        objetoSpot.transform.localRotation =
            Quaternion.identity;

        PlantSpot novoSpot =
            objetoSpot.AddComponent<PlantSpot>();

        // =================================================
        // CONFIGURAÇÃO
        // =================================================

        novoSpot.controladorDePlantio =
            false;

        novoSpot.areaPlantio =
            areaPlantio;

        novoSpot.distanciaPlantio =
            distanciaPlantio;

        novoSpot.distanciaInteracaoArvore =
            distanciaInteracaoArvore;

        novoSpot.distanciaEntreArvores =
            distanciaEntreArvores;

        novoSpot.tempoParaCrescer =
            tempoParaCrescer;

        novoSpot.camadaDoChao =
            camadaDoChao;

        // =================================================
        // TAMANHOS
        // =================================================

        novoSpot.tamanhoMinimo =
            tamanhoMinimo;

        novoSpot.tamanhoMaximo =
            tamanhoMaximo;

        novoSpot.chanceColossal =
            chanceColossal;

        novoSpot.tamanhoMinimoColossal =
            tamanhoMinimoColossal;

        novoSpot.tamanhoMaximoColossal =
            tamanhoMaximoColossal;

        // =================================================
        // DINHEIRO
        // =================================================

        novoSpot.multiplicadorMinimo =
            multiplicadorMinimo;

        novoSpot.multiplicadorMaximo =
            multiplicadorMaximo;

        novoSpot.multiplicadorColossal =
            multiplicadorColossal;

        // =================================================
        // TAMANHO INICIAL
        // =================================================

        novoSpot.porcentagemTamanhoInicial =
            porcentagemTamanhoInicial;

        // =================================================
        // VALOR
        // =================================================

        novoSpot.valorBaseArvore =
            valorSemente;

        novoSpot.valorColheitaGuardado =
            0;

        // =================================================
        // SONS
        // =================================================

        novoSpot.audioSource =
            audioSource;

        novoSpot.somPlantio =
            somPlantio;

        novoSpot.regarAudioSource =
            regarAudioSource;

        novoSpot.somRegar =
            somRegar;

        novoSpot.crescimentoAudioSource =
            crescimentoAudioSource;

        novoSpot.somCrescimento =
            somCrescimento;

        novoSpot.distanciaMaximaSom =
            distanciaMaximaSom;

        novoSpot.volumeMaximo =
            volumeMaximo;

        novoSpot.volumeMinimo =
            volumeMinimo;

        // =================================================
        // ESTADO
        // =================================================

        novoSpot.arvoreAtual =
            novaArvore;

        novoSpot.foiRegada =
            false;

        novoSpot.crescendo =
            false;

        novoSpot.terminouDeCrescer =
            false;

        novoSpot.tamanhoArvore =
            tamanhoArvore;

        novoSpot.multiplicadorDinheiro =
            multiplicadorDinheiro;

        // =================================================
        // CONSUMIR SEMENTE
        // =================================================

        loja.ConsumirSemente();

        // =================================================
        // SOM
        // =================================================

        if (audioSource != null &&
            somPlantio != null)
        {
            audioSource.PlayOneShot(
                somPlantio
            );
        }

        // =================================================
        // DEBUG
        // =================================================

        Debug.Log(
            "🌱 ÁRVORE PLANTADA!"
        );

        Debug.Log(
            "📏 Tamanho final: " +
            tamanhoArvore
        );

        Debug.Log(
            "💰 Valor base: R$" +
            valorSemente
        );

        Debug.Log(
            "💰 Multiplicador: x" +
            multiplicadorDinheiro
        );

        Debug.Log(
            "💵 Valor final: R$" +
            novoSpot.GetValorColheita()
        );
    }

    // =====================================================
    // SORTEAR TAMANHO
    // =====================================================

    private void SortearTamanho(
        GameObject arvore)
    {
        float sorteio =
            Random.value;

        if (sorteio < chanceColossal)
        {
            tamanhoArvore =
                Random.Range(
                    tamanhoMinimoColossal,
                    tamanhoMaximoColossal
                );

            multiplicadorDinheiro =
                multiplicadorColossal;
        }
        else
        {
            tamanhoArvore =
                Random.Range(
                    tamanhoMinimo,
                    tamanhoMaximo
                );

            float porcentagem =
                Mathf.InverseLerp(
                    tamanhoMinimo,
                    tamanhoMaximo,
                    tamanhoArvore
                );

            multiplicadorDinheiro =
                Mathf.Lerp(
                    multiplicadorMinimo,
                    multiplicadorMaximo,
                    porcentagem
                );
        }

        float tamanhoInicial =
            tamanhoArvore *
            porcentagemTamanhoInicial;

        arvore.transform.localScale =
            Vector3.one *
            tamanhoInicial;
    }

    // =====================================================
    // TENTAR REGAR
    // =====================================================

    public void TentarRegar()
    {
        if (foiRegada)
            return;

        if (regadorPickup == null &&
            player != null)
        {
            regadorPickup =
                player.GetComponent<RegadorPickup>();
        }

        if (regadorPickup == null)
        {
            Debug.LogError(
                "❌ RegadorPickup não encontrado!"
            );

            return;
        }

        if (!regadorPickup.EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você precisa estar segurando o regador!"
            );

            return;
        }

        if (!regadorPickup.TemAgua())
        {
            Debug.Log(
                "❌ O regador está vazio! Vá até o poço."
            );

            return;
        }

        bool usouAgua =
            regadorPickup.UsarAgua();

        if (!usouAgua)
        {
            Debug.Log(
                "❌ Não foi possível usar a água."
            );

            return;
        }

        Regar();
    }

    // =====================================================
    // REGAR
    // =====================================================

    private void Regar()
    {
        if (foiRegada)
            return;

        foiRegada =
            true;

        if (regarAudioSource != null &&
            somRegar != null)
        {
            regarAudioSource.PlayOneShot(
                somRegar
            );
        }

        Debug.Log(
            "💧 ÁRVORE REGADA!"
        );

        StartCoroutine(
            CrescerArvore()
        );
    }

    // =====================================================
    // CRESCER
    // =====================================================

    private IEnumerator CrescerArvore()
    {
        crescendo =
            true;

        terminouDeCrescer =
            false;

        Vector3 escalaInicial =
            Vector3.one *
            (
                tamanhoArvore *
                porcentagemTamanhoInicial
            );

        Vector3 escalaFinal =
            Vector3.one *
            tamanhoArvore;

        if (arvoreAtual != null)
        {
            arvoreAtual.transform.localScale =
                escalaInicial;
        }

        float tempo =
            0f;

        if (crescimentoAudioSource != null &&
            somCrescimento != null)
        {
            crescimentoAudioSource.clip =
                somCrescimento;

            crescimentoAudioSource.loop =
                true;

            crescimentoAudioSource.volume =
                volumeMaximo;

            crescimentoAudioSource.Play();
        }

        while (
            tempo <
            tempoParaCrescer)
        {
            if (arvoreAtual == null)
            {
                PararSomCrescimento();

                crescendo =
                    false;

                terminouDeCrescer =
                    false;

                yield break;
            }

            tempo +=
                Time.deltaTime;

            float porcentagem =
                Mathf.Clamp01(
                    tempo /
                    tempoParaCrescer
                );

            float crescimentoSuave =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    porcentagem
                );

            arvoreAtual.transform.localScale =
                Vector3.Lerp(
                    escalaInicial,
                    escalaFinal,
                    crescimentoSuave
                );

            yield return null;
        }

        if (arvoreAtual != null)
        {
            arvoreAtual.transform.localScale =
                escalaFinal;
        }

        PararSomCrescimento();

        crescendo =
            false;

        terminouDeCrescer =
            true;

        // =================================================
        // GUARDAR O VALOR DA COLHEITA
        // =================================================

        valorColheitaGuardado =
            CalcularValorColheita();

        Debug.Log(
            "🌳 ÁRVORE CRESCEU!"
        );

        Debug.Log(
            "📏 Tamanho final: " +
            tamanhoArvore
        );

        Debug.Log(
            "💰 Multiplicador: x" +
            multiplicadorDinheiro
        );

        Debug.Log(
            "💵 VALOR DA COLHEITA: R$" +
            valorColheitaGuardado
        );
    }

    // =====================================================
    // CALCULAR VALOR
    // =====================================================

    private int CalcularValorColheita()
    {
        if (valorBaseArvore <= 0)
            return 0;

        return Mathf.Max(
            1,
            Mathf.RoundToInt(
                valorBaseArvore *
                multiplicadorDinheiro
            )
        );
    }

    // =====================================================
    // SOM DO CRESCIMENTO
    // =====================================================

    private void AtualizarSomCrescimento()
    {
        if (crescimentoAudioSource == null)
            return;

        if (!crescimentoAudioSource.isPlaying)
            return;

        if (player == null)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        float porcentagem =
            Mathf.Clamp01(
                distancia /
                distanciaMaximaSom
            );

        crescimentoAudioSource.volume =
            Mathf.Lerp(
                volumeMaximo,
                volumeMinimo,
                porcentagem
            );
    }

    // =====================================================
    // PARAR SOM
    // =====================================================

    private void PararSomCrescimento()
    {
        if (crescimentoAudioSource != null)
        {
            crescimentoAudioSource.Stop();
        }
    }

    // =====================================================
    // PODE COLHER
    // =====================================================

    public bool PodeColher()
    {
        return
            !controladorDePlantio &&
            arvoreAtual != null &&
            foiRegada &&
            terminouDeCrescer;
    }

    // =====================================================
    // TEM ÁRVORE
    // =====================================================

    public bool TemArvore()
    {
        return
            !controladorDePlantio &&
            arvoreAtual != null;
    }

    // =====================================================
    // POSIÇÃO
    // =====================================================

    public Vector3 GetPosicaoArvore()
    {
        if (arvoreAtual == null)
            return transform.position;

        return
            arvoreAtual.transform.position;
    }

    // =====================================================
    // MULTIPLICADOR
    // =====================================================

    public float GetMultiplicadorDinheiro()
    {
        return multiplicadorDinheiro;
    }

    // =====================================================
    // TAMANHO
    // =====================================================

    public float GetTamanhoArvore()
    {
        return tamanhoArvore;
    }

    // =====================================================
    // VALOR DA COLHEITA
    // =====================================================

    public int GetValorColheita()
    {
        // Se já temos um valor guardado porque a árvore
        // terminou de crescer, usamos ele.
        if (valorColheitaGuardado > 0)
            return valorColheitaGuardado;

        return CalcularValorColheita();
    }

    // =====================================================
    // COLHER
    // =====================================================

    public bool ColherArvore()
    {
        if (!PodeColher())
        {
            Debug.Log(
                "❌ Essa árvore ainda não está pronta!"
            );

            return false;
        }

        // =================================================
        // PEGAR VALOR ANTES DE RESETAR OS DADOS
        // =================================================

        int valor =
            GetValorColheita();

        Debug.Log(
            "🌳 ÁRVORE COLHIDA!"
        );

        Debug.Log(
            "📏 Tamanho: " +
            tamanhoArvore
        );

        Debug.Log(
            "💰 Multiplicador: x" +
            multiplicadorDinheiro
        );

        Debug.Log(
            "💵 VALOR PAGO: R$" +
            valor
        );

        PararSomCrescimento();

        GameObject arvore =
            arvoreAtual;

        // =================================================
        // RESET
        // =================================================

        arvoreAtual =
            null;

        foiRegada =
            false;

        crescendo =
            false;

        terminouDeCrescer =
            false;

        valorBaseArvore =
            0;

        // =================================================
        // NÃO APAGAR O VALOR GUARDADO AQUI
        // =================================================

        valorColheitaGuardado =
            valor;

        // =================================================
        // DESTRUIR
        // =================================================

        if (arvore != null)
        {
            Destroy(
                arvore
            );
        }

        return true;
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (!controladorDePlantio)
            return;

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return;

        PlantSpot spot =
            EncontrarArvoreMirada();

        if (spot == null)
            return;

        if (!spot.TemArvore())
            return;

        // =================================================
        // REGAR
        // =================================================

        if (!spot.foiRegada)
        {
            if (
                regadorPickup != null &&
                regadorPickup.EstaSegurandoRegador() &&
                regadorPickup.TemAgua()
            )
            {
                MostrarTexto(
                    "💧 PRESSIONE E PARA REGAR"
                );
            }
            else
            {
                MostrarTexto(
                    "💧 PEGUE ÁGUA NO POÇO"
                );
            }

            return;
        }

        // =================================================
        // CRESCENDO
        // =================================================

        if (spot.crescendo)
        {
            MostrarTexto(
                "🌱 ÁRVORE CRESCENDO..."
            );

            return;
        }

        // =================================================
        // PRONTA
        // =================================================

        if (spot.terminouDeCrescer)
        {
            MostrarTexto(
                "🌳 PRESSIONE E PARA COLHER\n" +
                "💰 R$" +
                spot.GetValorColheita()
            );
        }
    }

    // =====================================================
    // TEXTO
    // =====================================================

    private void MostrarTexto(
        string texto)
    {
        GUIStyle estilo =
            new GUIStyle(
                GUI.skin.box
            );

        estilo.fontSize =
            18;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        GUI.Box(
            new Rect(
                (Screen.width - 380f) / 2f,
                Screen.height - 120f,
                380f,
                65f
            ),
            texto,
            estilo
        );
    }

    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        if (!controladorDePlantio)
            return;

        Gizmos.color =
            Color.yellow;

        if (cam != null)
        {
            Ray ray =
                cam.ViewportPointToRay(
                    new Vector3(
                        0.5f,
                        0.5f,
                        0f
                    )
                );

            Gizmos.DrawRay(
                ray.origin,
                ray.direction *
                distanciaPlantio
            );
        }
    }
}