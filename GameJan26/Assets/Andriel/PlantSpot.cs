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
    // TAMANHO DA ÁRVORE
    // =====================================================

    [Header("Tamanho das Árvores")]

    [Tooltip("Tamanho mínimo.")]
    public float tamanhoMinimo = 0.8f;

    [Tooltip("Tamanho máximo de uma árvore gigante.")]
    public float tamanhoMaximo = 2.5f;

    [Tooltip("Chance de nascer uma árvore colossal.")]
    [Range(0f, 1f)]
    public float chanceColossal = 0.03f;

    [Tooltip("Tamanho mínimo da colossal.")]
    public float tamanhoMinimoColossal = 3.5f;

    [Tooltip("Tamanho máximo da colossal.")]
    public float tamanhoMaximoColossal = 5f;

    // =====================================================
    // MULTIPLICADOR DE DINHEIRO
    // =====================================================

    [Header("Dinheiro por Tamanho")]

    public float multiplicadorMinimo = 0.8f;

    public float multiplicadorMaximo = 2.5f;

    public float multiplicadorColossal = 5f;

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
    // PLAYER / SHOP / CAMERA
    // =====================================================

    private Transform player;
    private Shop loja;
    private Camera cam;

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
    // DADOS DA ÁRVORE
    // =====================================================

    private float tamanhoArvore = 1f;
    private float multiplicadorDinheiro = 1f;

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
        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(
                    0.5f,
                    0.5f,
                    0f
                )
            );

        // =================================================
        // PRIMEIRO: ÁRVORE
        // =================================================

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                distanciaPlantio,
                ~0,
                QueryTriggerInteraction.Collide
            );

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        foreach (RaycastHit hit in hits)
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

            Debug.Log(
                "🌳 Mirando na árvore!"
            );

            if (!spot.foiRegada)
            {
                spot.TentarRegar();
            }
            else
            {
                Debug.Log(
                    "💧 Essa árvore já foi regada."
                );
            }

            return;
        }

        // =================================================
        // CHÃO
        // =================================================

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

        return collider.GetComponentInChildren<PlantSpot>();
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
        // CRIAR
        // =================================================

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
        // SPOT
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

        novoSpot.controladorDePlantio =
            false;

        novoSpot.areaPlantio =
            areaPlantio;

        novoSpot.distanciaPlantio =
            distanciaPlantio;

        novoSpot.distanciaEntreArvores =
            distanciaEntreArvores;

        novoSpot.tempoParaCrescer =
            tempoParaCrescer;

        novoSpot.camadaDoChao =
            camadaDoChao;

        // Copiar configurações.

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

        Debug.Log(
            "🌱 ÁRVORE PLANTADA!"
        );

        Debug.Log(
            "📏 Tamanho: " +
            tamanhoArvore
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

        // =================================================
        // COLOSSAL
        // =================================================

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

        arvore.transform.localScale =
            Vector3.one *
            tamanhoArvore;
    }

    // =====================================================
    // REGAR
    // =====================================================

    public void TentarRegar()
    {
        if (foiRegada)
            return;

        if (loja == null)
        {
            loja =
                FindFirstObjectByType<Shop>();
        }

        if (loja == null)
            return;

        if (!loja.EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você precisa estar segurando o regador!"
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

        loja.UsarRegador();

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
            (tamanhoArvore * 0.3f);

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

            arvoreAtual.transform.localScale =
                Vector3.Lerp(
                    escalaInicial,
                    escalaFinal,
                    porcentagem
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
    }

    // =====================================================
    // SOM
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
    // VALOR FINAL
    // =====================================================

    public int GetValorColheita()
    {
        if (loja == null)
        {
            loja =
                FindFirstObjectByType<Shop>();
        }

        if (loja == null)
            return 0;

        int valorBase =
            loja.GetValorSementeSelecionada();

        return Mathf.RoundToInt(
            valorBase *
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
            "💰 Valor: R$" +
            valor
        );

        PararSomCrescimento();

        GameObject arvore =
            arvoreAtual;

        arvoreAtual =
            null;

        foiRegada =
            false;

        crescendo =
            false;

        terminouDeCrescer =
            false;

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
                distanciaPlantio,
                ~0,
                QueryTriggerInteraction.Collide
            );

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(
                    b.distance
                )
        );

        foreach (RaycastHit hit in hits)
        {
            PlantSpot spot =
                EncontrarPlantSpot(
                    hit.collider
                );

            if (spot == null)
                continue;

            if (!spot.controladorDePlantio &&
                spot.TemArvore())
            {
                if (!spot.foiRegada)
                {
                    MostrarTexto(
                        "💧 PRESSIONE E PARA REGAR"
                    );
                }
                else if (spot.crescendo)
                {
                    MostrarTexto(
                        "🌱 ÁRVORE CRESCENDO..."
                    );
                }
                else if (spot.terminouDeCrescer)
                {
                    MostrarTexto(
                        "🌳 PRONTA PARA COLHER\n" +
                        "💰 R$" +
                        spot.GetValorColheita()
                    );
                }

                return;
            }
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
}