using UnityEngine;
using UnityEngine.InputSystem;

public class PlantSpot : MonoBehaviour
{
    [Header("Árvore")]
    public GameObject arvorePrefab;

    [Header("Configuração")]
    public float distanciaPlantio = 2f;
    public float tempoParaCrescer = 10f;

    [Header("Som de Plantio")]
    public AudioSource audioSource;
    public AudioClip somPlantio;

    [Header("Som de Regar")]
    public AudioSource regarAudioSource;
    public AudioClip somRegar;

    [Header("Som de Crescimento")]
    public AudioSource crescimentoAudioSource;
    public AudioClip somCrescimento;

    [Header("Distância do Som de Crescimento")]
    public float distanciaMaximaSom = 15f;
    public float volumeMaximo = 1f;
    public float volumeMinimo = 0f;

    private Transform player;
    private GameObject arvoreAtual;

    private bool playerPerto = false;
    private bool foiRegada = false;
    private bool crescendo = false;

    private Shop loja;

    void Start()
    {
        // =========================
        // PLAYER
        // =========================

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "❌ Player não encontrado!"
            );
        }

        // =========================
        // SHOP
        // =========================

        loja = FindFirstObjectByType<Shop>();

        if (loja == null)
        {
            Debug.LogWarning(
                "❌ Shop não encontrado!"
            );
        }
    }

    void Update()
    {
        if (player == null)
            return;

        // =========================
        // DISTÂNCIA
        // =========================

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaPlantio;

        // =========================
        // SOM DE CRESCIMENTO
        // =========================

        if (crescimentoAudioSource != null &&
            crescimentoAudioSource.isPlaying)
        {
            float porcentagemDistancia =
                Mathf.Clamp01(
                    distancia / distanciaMaximaSom
                );

            crescimentoAudioSource.volume =
                Mathf.Lerp(
                    volumeMaximo,
                    volumeMinimo,
                    porcentagemDistancia
                );
        }

        if (!playerPerto)
            return;

        if (Keyboard.current == null)
            return;

        // =========================
        // E
        // =========================

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Se não existe árvore
            if (arvoreAtual == null)
            {
                Plantar();
                return;
            }

            // Se existe árvore e ainda não foi regada
            if (!foiRegada)
            {
                TentarRegar();
            }
        }
    }

    // =====================================================
    // PLANTAR
    // =====================================================

    void Plantar()
    {
        if (arvorePrefab == null)
        {
            Debug.LogWarning(
                "⚠️ Coloque o Prefab da árvore!"
            );

            return;
        }

        arvoreAtual = Instantiate(
            arvorePrefab,
            transform.position,
            Quaternion.identity
        );

        arvoreAtual.transform.localScale =
            Vector3.one * 0.3f;

        // Garante a Tag
        arvoreAtual.tag = "Arvore";

        // =========================
        // RESET DOS ESTADOS
        // =========================

        foiRegada = false;
        crescendo = false;

        // =========================
        // SOM
        // =========================

        if (audioSource != null &&
            somPlantio != null)
        {
            audioSource.PlayOneShot(
                somPlantio
            );
        }

        Debug.Log(
            "🌱 Semente plantada!"
        );
    }

    // =====================================================
    // TENTAR REGAR
    // =====================================================

    void TentarRegar()
    {
        if (loja == null)
        {
            loja =
                FindFirstObjectByType<Shop>();
        }

        if (loja == null)
        {
            Debug.LogWarning(
                "❌ Shop não encontrado!"
            );

            return;
        }

        // =========================
        // VERIFICAR REGADOR
        // =========================

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

    void Regar()
    {
        // Impede regar duas vezes
        if (foiRegada)
            return;

        foiRegada = true;

        // =========================
        // SOM
        // =========================

        if (regarAudioSource != null &&
            somRegar != null)
        {
            regarAudioSource.PlayOneShot(
                somRegar
            );
        }

        Debug.Log(
            "💧 Planta regada!"
        );

        // =========================
        // CONSUME REGADOR
        // =========================

        loja.UsarRegador();

        // =========================
        // CRESCIMENTO
        // =========================

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

        // =========================
        // SOM
        // =========================

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

        // =========================
        // CRESCIMENTO
        // =========================

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
                tempo / tempoParaCrescer;

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
    // PARAR SOM
    // =====================================================

    void PararSomCrescimento()
    {
        if (crescimentoAudioSource != null)
        {
            crescimentoAudioSource.Stop();
        }
    }

    // =====================================================
    // COLHER
    // =====================================================

    public bool ColherArvore()
    {
        // Não existe árvore
        if (arvoreAtual == null)
            return false;

        // Se ainda está crescendo,
        // não permite colher
        if (crescendo)
        {
            Debug.Log(
                "🌱 Essa árvore ainda está crescendo!"
            );

            return false;
        }

        // Para o som
        PararSomCrescimento();

        // Destrói árvore
        Destroy(arvoreAtual);

        // =========================
        // RESET COMPLETO
        // =========================

        arvoreAtual = null;
        foiRegada = false;
        crescendo = false;

        Debug.Log(
            "🚜 Árvore colhida!"
        );

        return true;
    }

    // =====================================================
    // TEXTO
    // =====================================================

    void OnGUI()
    {
        if (!playerPerto)
            return;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 20;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        // =========================
        // PLANTAR
        // =========================

        if (arvoreAtual == null)
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2 - 180,
                    Screen.height - 150,
                    360,
                    60
                ),
                "🌱 PRESSIONE E PARA PLANTAR",
                estilo
            );
        }

        // =========================
        // REGAR
        // =========================

        else if (!foiRegada)
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2 - 180,
                    Screen.height - 150,
                    360,
                    60
                ),
                "💧 PRESSIONE E PARA REGAR",
                estilo
            );
        }

        // =========================
        // CRESCENDO
        // =========================

        else if (crescendo)
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2 - 180,
                    Screen.height - 150,
                    360,
                    60
                ),
                "🌱 A ÁRVORE ESTÁ CRESCENDO...",
                estilo
            );
        }

        // =========================
        // PRONTA
        // =========================

        else
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2 - 180,
                    Screen.height - 150,
                    360,
                    60
                ),
                "🌳 ÁRVORE PRONTA PARA COLHER!",
                estilo
            );
        }
    }
}