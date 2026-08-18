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
        // ENCONTRAR PLAYER
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
                "Player não encontrado! Coloque a Tag 'Player' no Player."
            );
        }

        // =========================
        // ENCONTRAR LOJA
        // =========================

        loja = FindFirstObjectByType<Shop>();

        if (loja == null)
        {
            Debug.LogWarning(
                "Shop não encontrado na cena!"
            );
        }
    }

    void Update()
    {
        if (player == null)
            return;

        // =========================
        // DISTÂNCIA DO PLAYER
        // =========================

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaPlantio;

        // =========================
        // VOLUME DO SOM DE CRESCIMENTO
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

        // =========================
        // INTERAÇÃO
        // =========================

        if (!playerPerto)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // =========================
            // PLANTAR
            // =========================

            if (arvoreAtual == null)
            {
                Plantar();
                return;
            }

            // =========================
            // REGAR
            // =========================

            if (arvoreAtual != null &&
                !foiRegada)
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
                "⚠️ Coloque o Prefab da árvore no PlantSpot!"
            );

            return;
        }

        arvoreAtual = Instantiate(
            arvorePrefab,
            transform.position,
            Quaternion.identity
        );

        // Começa com 30% do tamanho
        arvoreAtual.transform.localScale =
            Vector3.one * 0.3f;

        // =========================
        // SOM DE PLANTIO
        // =========================

        if (audioSource != null &&
            somPlantio != null)
        {
            audioSource.PlayOneShot(somPlantio);
        }

        Debug.Log("🌱 Semente plantada!");
    }

    // =====================================================
    // TENTAR REGAR
    // =====================================================

    void TentarRegar()
    {
        if (loja == null)
        {
            Debug.LogWarning(
                "Shop não encontrado!"
            );

            return;
        }

        // Verifica se está segurando o regador
        if (!loja.EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você precisa estar segurando o regador!"
            );

            return;
        }

        // Tem regador
        Regar();
    }

    // =====================================================
    // REGAR
    // =====================================================

    void Regar()
    {
        foiRegada = true;

        // =========================
        // SOM DE REGAR
        // =========================

        if (regarAudioSource != null &&
            somRegar != null)
        {
            regarAudioSource.PlayOneShot(somRegar);
        }

        Debug.Log("💧 Planta regada!");

        // Consome o regador
        loja.UsarRegador();

        // =========================
        // COMEÇA O CRESCIMENTO
        // =========================

        if (!crescendo)
        {
            StartCoroutine(
                CrescerArvore()
            );
        }
    }

    // =====================================================
    // CRESCER ÁRVORE
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
        // COMEÇA SOM DE CRESCIMENTO
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
                if (crescimentoAudioSource != null)
                {
                    crescimentoAudioSource.Stop();
                }

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

        // =========================
        // TAMANHO FINAL
        // =========================

        if (arvoreAtual != null)
        {
            arvoreAtual.transform.localScale =
                escalaFinal;
        }

        // =========================
        // PARA O SOM
        // =========================

        if (crescimentoAudioSource != null)
        {
            crescimentoAudioSource.Stop();
        }

        crescendo = false;

        Debug.Log("🌳 Árvore cresceu!");
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
        // AINDA NÃO PLANTOU
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
        // PLANTOU, MAS NÃO REGOU
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
        // ESTÁ CRESCENDO
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
    }
}