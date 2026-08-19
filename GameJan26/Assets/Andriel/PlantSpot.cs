using UnityEngine;
using UnityEngine.InputSystem;

public class PlantSpot : MonoBehaviour
{
    [Header("Área de Plantio")]
    public PlantArea areaPlantio;

    [Header("Configuração")]
    public float distanciaPlantio = 5f;
    public float distanciaEntreArvores = 1.5f;
    public float tempoParaCrescer = 10f;

    [Header("Raycast")]
    public LayerMask camadaDoChao = ~0;

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

    private Transform player;
    private GameObject arvoreAtual;

    private bool foiRegada;
    private bool crescendo;

    private Shop loja;
    private Camera cam;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        loja =
            FindFirstObjectByType<Shop>();

        cam = Camera.main;
    }

    void Update()
    {
        if (player == null || cam == null)
            return;

        if (Keyboard.current == null)
            return;

        // =================================================
        // PLANTAR / REGAR
        // =================================================

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ProcessarInteracao();
        }

        // =================================================
        // SOM DE CRESCIMENTO
        // =================================================

        if (crescimentoAudioSource != null &&
            crescimentoAudioSource.isPlaying)
        {
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
    }

    // =====================================================
    // INTERAÇÃO
    // =====================================================

    void ProcessarInteracao()
    {
        Ray ray =
            cam.ScreenPointToRay(
                new Vector3(
                    Screen.width / 2f,
                    Screen.height / 2f,
                    0f
                )
            );

        RaycastHit hit;

        if (!Physics.Raycast(
                ray,
                out hit,
                distanciaPlantio,
                camadaDoChao))
        {
            return;
        }

        // =================================================
        // JÁ EXISTE ÁRVORE
        // =================================================

        if (arvoreAtual != null)
        {
            if (!foiRegada)
            {
                TentarRegar();
            }

            return;
        }

        // =================================================
        // ÁREA
        // =================================================

        if (areaPlantio == null)
        {
            Debug.LogWarning(
                "⚠️ Área de Plantio não configurada!"
            );

            return;
        }

        if (!areaPlantio.PodePlantar(hit.point))
        {
            Debug.Log(
                "❌ Você está fora da área de plantio!"
            );

            return;
        }

        // =================================================
        // VERIFICAR SEMENTE
        // =================================================

        if (loja == null)
        {
            loja =
                FindFirstObjectByType<Shop>();
        }

        if (loja == null)
        {
            Debug.LogError(
                "❌ Shop não encontrado!"
            );

            return;
        }

        if (!loja.TemSementeSelecionada())
        {
            Debug.Log(
                "🌱 Você não possui uma semente selecionada!"
            );

            return;
        }

        Plantar(hit.point);
    }

    // =====================================================
    // PLANTAR
    // =====================================================

    void Plantar(Vector3 ponto)
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
        // DISTÂNCIA ENTRE ÁRVORES
        // =================================================

        PlantSpot[] spots =
            FindObjectsByType<PlantSpot>(
                FindObjectsSortMode.None
            );

        foreach (PlantSpot spot in spots)
        {
            if (spot == null || spot == this)
                continue;

            if (!spot.TemArvore())
                continue;

            float distancia =
                Vector3.Distance(
                    ponto,
                    spot.GetPosicaoArvore()
                );

            if (distancia < distanciaEntreArvores)
            {
                Debug.Log(
                    "🌳 Já existe uma árvore muito próxima!"
                );

                return;
            }
        }

        // =================================================
        // CRIAR ÁRVORE
        // =================================================

        arvoreAtual =
            Instantiate(
                prefab,
                ponto,
                Quaternion.identity
            );

        arvoreAtual.transform.localScale =
            Vector3.one * 0.3f;

        foiRegada = false;
        crescendo = false;

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
            "🌱 " +
            loja.GetNomeSementeSelecionada() +
            " plantada!"
        );
    }

    // =====================================================
    // REGAR
    // =====================================================

    void TentarRegar()
    {
        if (loja == null)
            loja = FindFirstObjectByType<Shop>();

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

    void Regar()
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

        loja.UsarRegador();

        Debug.Log(
            "💧 Planta regada!"
        );

        if (!crescendo)
        {
            StartCoroutine(
                CrescerArvore()
            );
        }
    }

    // =====================================================
    // CRESCER
    // =====================================================

    System.Collections.IEnumerator CrescerArvore()
    {
        crescendo = true;

        Vector3 escalaInicial =
            Vector3.one * 0.3f;

        Vector3 escalaFinal =
            Vector3.one;

        float tempo = 0f;

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

        while (tempo < tempoParaCrescer)
        {
            if (arvoreAtual == null)
            {
                PararSomCrescimento();

                crescendo = false;

                yield break;
            }

            tempo += Time.deltaTime;

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

        crescendo = false;

        Debug.Log(
            "🌳 Árvore cresceu!"
        );
    }

    // =====================================================
    // SOM
    // =====================================================

    void PararSomCrescimento()
    {
        if (crescimentoAudioSource != null)
        {
            crescimentoAudioSource.Stop();
        }
    }

    // =====================================================
    // COLHEITA
    // =====================================================

    public bool PodeColher()
    {
        return arvoreAtual != null &&
               !crescendo;
    }

    public bool TemArvore()
    {
        return arvoreAtual != null;
    }

    public Vector3 GetPosicaoArvore()
    {
        if (arvoreAtual == null)
            return transform.position;

        return arvoreAtual.transform.position;
    }

    public bool ColherArvore()
    {
        if (arvoreAtual == null)
            return false;

        if (crescendo)
            return false;

        PararSomCrescimento();

        Destroy(arvoreAtual);

        arvoreAtual = null;
        foiRegada = false;
        crescendo = false;

        Debug.Log(
            "🚜 Árvore colhida!"
        );

        return true;
    }

    // =====================================================
    // UI DO PLANTIO
    // =====================================================

    void OnGUI()
    {
        if (player == null)
            return;

        string texto = "";

        if (arvoreAtual != null)
        {
            if (!foiRegada)
            {
                texto =
                    "💧 PRESSIONE E PARA REGAR";
            }
            else if (crescendo)
            {
                texto =
                    "🌱 A ÁRVORE ESTÁ CRESCENDO...";
            }
            else
            {
                texto =
                    "🌳 ÁRVORE PRONTA PARA COLHER";
            }
        }
        else
        {
            Ray ray =
                cam.ScreenPointToRay(
                    new Vector3(
                        Screen.width / 2f,
                        Screen.height / 2f,
                        0f
                    )
                );

            RaycastHit hit;

            if (Physics.Raycast(
                    ray,
                    out hit,
                    distanciaPlantio,
                    camadaDoChao))
            {
                if (areaPlantio != null &&
                    areaPlantio.PodePlantar(
                        hit.point))
                {
                    if (loja != null &&
                        loja.TemSementeSelecionada())
                    {
                        texto =
                            "🌱 PRESSIONE E PARA PLANTAR\n" +
                            loja.GetNomeSementeSelecionada();
                    }
                    else
                    {
                        texto =
                            "🌱 COMPRE UMA SEMENTE NA LOJA";
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(texto))
            return;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 18;
        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        float largura = 380f;
        float altura = 65f;

        float x =
            (Screen.width - largura) / 2f;

        float y =
            Screen.height - 120f;

        GUI.Box(
            new Rect(
                x,
                y,
                largura,
                altura
            ),
            texto,
            estilo
        );
    }
}