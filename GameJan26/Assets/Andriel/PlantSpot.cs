using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlantSpot : MonoBehaviour
{
    [Header("Área de Plantio")]
    public PlantArea areaPlantio;

    [Header("Configuração")]
    public float distanciaPlantio = 5f;

    [Tooltip("Distância mínima entre árvores.")]
    public float distanciaEntreArvores = 0.35f;

    [Tooltip("Tempo para a árvore crescer depois de ser regada.")]
    public float tempoParaCrescer = 10f;

    [Header("Controlador")]
    [Tooltip("ATIVADO somente no PlantSpot que está na cena.")]
    public bool controladorDePlantio = true;

    [Header("Raycast do Chão")]
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
    private Shop loja;
    private Camera cam;

    [HideInInspector]
    public GameObject arvoreAtual;

    [HideInInspector]
    public bool foiRegada;

    [HideInInspector]
    public bool crescendo;

    [HideInInspector]
    public bool terminouDeCrescer;


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
        else
        {
            Debug.LogError(
                "❌ Player não encontrado! Coloque a Tag Player."
            );
        }

        loja =
            FindFirstObjectByType<Shop>();

        cam =
            Camera.main;

        // Se esse PlantSpot é o controlador da cena,
        // ele não representa uma árvore.
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
        // PlantSpot das árvores NÃO processa E.
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
    // PROCESSAR INTERAÇÃO
    // =====================================================

    private void ProcessarInteracao()
    {
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


        // =================================================
        // PRIMEIRO: VERIFICAR ÁRVORE
        // =================================================

        RaycastHit[] hitsArvore =
            Physics.RaycastAll(
                ray,
                distanciaPlantio,
                ~0,
                QueryTriggerInteraction.Collide
            );

        System.Array.Sort(
            hitsArvore,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );


        foreach (RaycastHit hit in hitsArvore)
        {
            PlantSpot plantaMirada =
                EncontrarPlantSpot(
                    hit.collider
                );

            if (plantaMirada == null)
                continue;

            if (plantaMirada == this)
                continue;

            if (plantaMirada.controladorDePlantio)
                continue;

            if (!plantaMirada.TemArvore())
                continue;


            Debug.Log(
                "🌳 Você está mirando em uma árvore."
            );


            if (!plantaMirada.foiRegada)
            {
                plantaMirada.TentarRegar();
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
        // DEPOIS: PROCURAR O CHÃO
        // =================================================

        RaycastHit hitChao;

        if (!Physics.Raycast(
                ray,
                out hitChao,
                distanciaPlantio,
                camadaDoChao,
                QueryTriggerInteraction.Ignore))
        {
            Debug.Log(
                "❌ Não encontrou o chão."
            );

            return;
        }


        // =================================================
        // ÁREA DE PLANTIO
        // =================================================

        if (areaPlantio == null)
        {
            areaPlantio =
                FindFirstObjectByType<PlantArea>();
        }

        if (areaPlantio == null)
        {
            Debug.LogError(
                "❌ Nenhum PlantArea foi encontrado na cena!"
            );

            return;
        }


        if (!areaPlantio.PodePlantar(
                hitChao.point))
        {
            Debug.Log(
                "❌ Esse ponto está fora da área de plantio."
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

        if (!PodePlantarAqui(
                hitChao.point))
        {
            return;
        }


        // =================================================
        // PLANTAR
        // =================================================

        PlantarNovaArvore(
            hitChao.point
        );
    }


    // =====================================================
    // ENCONTRAR PLANTSPOT
    // =====================================================

    private PlantSpot EncontrarPlantSpot(
        Collider collider
    )
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
    // VERIFICAR ESPAÇO ENTRE ÁRVORES
    // =====================================================

    private bool PodePlantarAqui(
        Vector3 ponto
    )
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
    // PLANTAR NOVA ÁRVORE
    // =====================================================

    private void PlantarNovaArvore(
        Vector3 ponto
    )
    {
        GameObject prefab =
            loja.GetPrefabSementeSelecionada();


        if (prefab == null)
        {
            Debug.LogError(
                "❌ O prefab da árvore não foi encontrado!"
            );

            return;
        }


        // =================================================
        // CRIAR ÁRVORE
        // =================================================

        GameObject novaArvore =
            Instantiate(
                prefab,
                ponto,
                Quaternion.identity
            );


        novaArvore.transform.localScale =
            Vector3.one * 0.3f;


        // =================================================
        // CRIAR PLANTSPOT AUTOMATICAMENTE
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
        // CONFIGURAÇÃO DO NOVO PLANTSPOT
        // =================================================

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
        // ESTADO INICIAL
        // =================================================

        novoSpot.arvoreAtual =
            novaArvore;

        novoSpot.foiRegada =
            false;

        novoSpot.crescendo =
            false;

        novoSpot.terminouDeCrescer =
            false;


        // =================================================
        // CONSUMIR SEMENTE
        // =================================================

        loja.ConsumirSemente();


        // =================================================
        // SOM DE PLANTIO
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
    }


    // =====================================================
    // TENTAR REGAR
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
        {
            Debug.LogError(
                "❌ Shop não encontrada!"
            );

            return;
        }


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


        // =================================================
        // SOM DE REGAR
        // =================================================

        if (regarAudioSource != null &&
            somRegar != null)
        {
            regarAudioSource.PlayOneShot(
                somRegar
            );
        }


        // =================================================
        // USAR REGADOR
        // =================================================

        loja.UsarRegador();


        Debug.Log(
            "💧 ÁRVORE REGADA!"
        );


        // =================================================
        // COMEÇAR CRESCIMENTO
        // =================================================

        StartCoroutine(
            CrescerArvore()
        );
    }


    // =====================================================
    // CRESCER ÁRVORE
    // =====================================================

    private IEnumerator CrescerArvore()
    {
        crescendo =
            true;

        terminouDeCrescer =
            false;


        Vector3 escalaInicial =
            Vector3.one * 0.3f;

        Vector3 escalaFinal =
            Vector3.one;


        float tempo =
            0f;


        // =================================================
        // SOM
        // =================================================

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


        // =================================================
        // CRESCER
        // =================================================

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


            arvoreAtual.transform.localScale =
                Vector3.Lerp(
                    escalaInicial,
                    escalaFinal,
                    porcentagem
                );


            yield return null;
        }


        // =================================================
        // FINAL
        // =================================================

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
            "🌳 ÁRVORE CRESCEU E ESTÁ PRONTA PARA COLHER!"
        );
    }


    // =====================================================
    // SOM DE CRESCIMENTO
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
    // POSIÇÃO DA ÁRVORE
    // =====================================================

    public Vector3 GetPosicaoArvore()
    {
        if (arvoreAtual == null)
            return transform.position;


        return arvoreAtual.transform.position;
    }


    // =====================================================
    // COLHER
    // =====================================================

    public bool ColherArvore()
    {
        if (!PodeColher())
        {
            Debug.Log(
                "❌ Essa árvore ainda não está pronta para colher!"
            );

            return false;
        }


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


        Destroy(
            arvore
        );


        Debug.Log(
            "🚜 ÁRVORE COLHIDA!"
        );


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
                a.distance.CompareTo(b.distance)
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
                        "🌳 ÁRVORE PRONTA PARA COLHER"
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
        string texto
    )
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