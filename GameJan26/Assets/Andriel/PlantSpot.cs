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
    public float distanciaEntreArvores = 0.35f;

    [Header("Detecção da Árvore")]
    [Tooltip("Aumente se quiser facilitar a mira nas árvores.")]
    public float raioDeteccaoArvore = 0.45f;

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
    private int valorBaseArvore = 0;

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
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!controladorDePlantio)
        {
            AtualizarSomCrescimento();
            return;
        }

        if (player == null)
            return;

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return;

        if (Keyboard.current == null)
            return;

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
        PlantSpot spotMirado =
            EncontrarArvoreMirada();

        // =================================================
        // ÁRVORE
        // =================================================

        if (spotMirado != null)
        {
            Debug.Log("🌳 Árvore encontrada!");

            if (!spotMirado.foiRegada)
            {
                spotMirado.TentarRegar();
            }
            else if (spotMirado.terminouDeCrescer)
            {
                PlayerMoney dinheiro =
                    player != null
                        ? player.GetComponent<PlayerMoney>()
                        : null;

                bool colheu =
                    spotMirado.ColherArvore();

                if (colheu && dinheiro != null)
                {
                    // O dinheiro é adicionado aqui.
                    // Se o seu sistema já adiciona em outro lugar,
                    // remova esta parte para não pagar duas vezes.
                    int valor =
                        spotMirado.GetValorColheita();

                    if (valor > 0)
                        dinheiro.AdicionarDinheiro(valor);
                }
            }

            return;
        }

        // =================================================
        // CHÃO
        // =================================================

        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
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
        // DISTÂNCIA
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

        // =================================================
        // 1. SPHERECAST
        // =================================================

        RaycastHit[] sphereHits =
            Physics.SphereCastAll(
                ray,
                raioDeteccaoArvore,
                distanciaPlantio,
                ~0,
                QueryTriggerInteraction.Collide
            );

        System.Array.Sort(
            sphereHits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        foreach (RaycastHit hit in sphereHits)
        {
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

            return spot;
        }

        // =================================================
        // 2. RAYCAST NORMAL
        // =================================================

        if (Physics.Raycast(
            ray,
            out RaycastHit rayHit,
            distanciaPlantio,
            ~0,
            QueryTriggerInteraction.Collide))
        {
            PlantSpot spot =
                EncontrarPlantSpot(
                    rayHit.collider
                );

            if (spot != null &&
                !spot.controladorDePlantio &&
                spot.TemArvore())
            {
                return spot;
            }
        }

        // =================================================
        // 3. DETECÇÃO PRÓXIMA
        // =================================================
        // Resolve principalmente o problema de a câmera
        // ficar dentro do Collider quando o jogador chega
        // muito perto da árvore.
        // =================================================

        Vector3 pontoFrente =
            ray.origin +
            ray.direction * 1.5f;

        Collider[] collidersProximos =
            Physics.OverlapSphere(
                pontoFrente,
                1.2f,
                ~0,
                QueryTriggerInteraction.Collide
            );

        float menorDistancia =
            float.MaxValue;

        PlantSpot melhorSpot =
            null;

        foreach (Collider col in collidersProximos)
        {
            PlantSpot spot =
                EncontrarPlantSpot(col);

            if (spot == null)
                continue;

            if (spot.controladorDePlantio)
                continue;

            if (!spot.TemArvore())
                continue;

            float distancia =
                Vector3.Distance(
                    ray.origin,
                    spot.GetPosicaoArvore()
                );

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                melhorSpot = spot;
            }
        }

        return melhorSpot;
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
            collider.GetComponentInChildren<PlantSpot>();

        return spot;
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

        int valorSemente =
            loja.GetValorSementeSelecionada();

        GameObject novaArvore =
            Instantiate(
                prefab,
                ponto,
                Quaternion.identity
            );

        // =================================================
        // TAMANHO
        // =================================================

        SortearTamanho(novaArvore);

        // =================================================
        // CRIAR SPOT
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

        novoSpot.controladorDePlantio = false;
        novoSpot.areaPlantio = areaPlantio;
        novoSpot.distanciaPlantio = distanciaPlantio;
        novoSpot.distanciaEntreArvores = distanciaEntreArvores;
        novoSpot.raioDeteccaoArvore = raioDeteccaoArvore;
        novoSpot.tempoParaCrescer = tempoParaCrescer;
        novoSpot.camadaDoChao = camadaDoChao;

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

        novoSpot.multiplicadorMinimo =
            multiplicadorMinimo;

        novoSpot.multiplicadorMaximo =
            multiplicadorMaximo;

        novoSpot.multiplicadorColossal =
            multiplicadorColossal;

        novoSpot.porcentagemTamanhoInicial =
            porcentagemTamanhoInicial;

        // =================================================
        // VALOR
        // =================================================

        novoSpot.valorBaseArvore =
            valorSemente;

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

        novoSpot.foiRegada = false;
        novoSpot.crescendo = false;
        novoSpot.terminouDeCrescer = false;

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

        Debug.Log(
            "🌱 ÁRVORE PLANTADA!"
        );

        Debug.Log(
            "📏 Tamanho final sorteado: " +
            tamanhoArvore
        );

        Debug.Log(
            "🌱 Tamanho inicial: " +
            tamanhoArvore *
            porcentagemTamanhoInicial
        );

        Debug.Log(
            "🌱 Valor da semente: R$" +
            valorSemente
        );

        Debug.Log(
            "💰 Multiplicador: x" +
            multiplicadorDinheiro
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

        if (regadorPickup == null)
        {
            if (player != null)
            {
                regadorPickup =
                    player.GetComponent<RegadorPickup>();
            }
        }

        if (regadorPickup == null)
        {
            Debug.LogError(
                "❌ RegadorPickup não encontrado no Player!"
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

        foiRegada = true;

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
        crescendo = true;
        terminouDeCrescer = false;

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

        float tempo = 0f;

        if (crescimentoAudioSource != null &&
            somCrescimento != null)
        {
            crescimentoAudioSource.clip =
                somCrescimento;

            crescimentoAudioSource.loop = true;
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

                crescendo = false;
                terminouDeCrescer = false;

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

        crescendo = false;
        terminouDeCrescer = true;

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
            "💵 Valor: R$" +
            GetValorColheita()
        );
    }

    // =====================================================
    // SOM CRESCIMENTO
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
    // VALOR
    // =====================================================

    public int GetValorColheita()
    {
        if (valorBaseArvore <= 0)
            return 0;

        return Mathf.RoundToInt(
            valorBaseArvore *
            multiplicadorDinheiro
        );
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

        int valor =
            GetValorColheita();

        Debug.Log(
            "🌳 Árvore colhida!"
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
            "💰 Valor: R$" +
            valor
        );

        PararSomCrescimento();

        GameObject arvore =
            arvoreAtual;

        arvoreAtual = null;
        foiRegada = false;
        crescendo = false;
        terminouDeCrescer = false;

        valorBaseArvore = 0;

        Destroy(arvore);

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

        if (spot.controladorDePlantio)
            return;

        if (!spot.TemArvore())
            return;

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

        if (spot.crescendo)
        {
            MostrarTexto(
                "🌱 ÁRVORE CRESCENDO..."
            );

            return;
        }

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

        estilo.fontSize = 18;
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