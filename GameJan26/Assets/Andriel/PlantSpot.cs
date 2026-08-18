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

    [Header("Distância do Som")]
    public float distanciaMaximaSom = 15f;
    public float volumeMaximo = 1f;
    public float volumeMinimo = 0f;

    private Transform player;
    private GameObject arvoreAtual;

    private bool playerPerto;
    private bool foiRegada;
    private bool crescendo;

    private Shop loja;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player não encontrado!");
        }

        loja = FindFirstObjectByType<Shop>();

        if (loja == null)
        {
            Debug.LogWarning("Shop não encontrado!");
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaPlantio;

        if (crescimentoAudioSource != null &&
            crescimentoAudioSource.isPlaying)
        {
            float porcentagem =
                Mathf.Clamp01(
                    distancia / distanciaMaximaSom
                );

            crescimentoAudioSource.volume =
                Mathf.Lerp(
                    volumeMaximo,
                    volumeMinimo,
                    porcentagem
                );
        }

        if (!playerPerto)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (arvoreAtual == null)
            {
                Plantar();
            }
            else if (!foiRegada)
            {
                TentarRegar();
            }
        }
    }

    // =========================
    // PLANTAR
    // =========================

    void Plantar()
    {
        if (arvorePrefab == null)
        {
            Debug.LogWarning(
                "Coloque o Prefab da árvore no PlantSpot!"
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

        foiRegada = false;
        crescendo = false;

        if (audioSource != null &&
            somPlantio != null)
        {
            audioSource.PlayOneShot(somPlantio);
        }

        Debug.Log("🌱 Semente plantada!");
    }

    // =========================
    // REGAR
    // =========================

    void TentarRegar()
    {
        if (loja == null)
        {
            loja = FindFirstObjectByType<Shop>();
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

    void Regar()
    {
        if (foiRegada)
            return;

        foiRegada = true;

        if (regarAudioSource != null &&
            somRegar != null)
        {
            regarAudioSource.PlayOneShot(somRegar);
        }

        Debug.Log("💧 Planta regada!");

        loja.UsarRegador();

        if (!crescendo)
        {
            StartCoroutine(CrescerArvore());
        }
    }

    // =========================
    // CRESCER
    // =========================

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

            crescimentoAudioSource.loop = true;
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

        Debug.Log("🌳 Árvore cresceu!");
    }

    // =========================
    // PARAR SOM
    // =========================

    void PararSomCrescimento()
    {
        if (crescimentoAudioSource != null)
        {
            crescimentoAudioSource.Stop();
        }
    }

    // =========================
    // COLHER
    // =========================

    public bool ColherArvore()
    {
        if (arvoreAtual == null)
            return false;

        if (crescendo)
        {
            Debug.Log(
                "🌱 Essa árvore ainda está crescendo!"
            );

            return false;
        }

        PararSomCrescimento();

        Destroy(arvoreAtual);

        arvoreAtual = null;
        foiRegada = false;
        crescendo = false;

        Debug.Log("🚜 Árvore colhida!");

        return true;
    }

    // =========================
    // TEXTO
    // =========================

    void OnGUI()
    {
        if (!playerPerto)
            return;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 20;
        estilo.alignment =
            TextAnchor.MiddleCenter;

        float x =
            Screen.width / 2 - 180;

        float y =
            Screen.height - 150;

        if (arvoreAtual == null)
        {
            GUI.Box(
                new Rect(x, y, 360, 60),
                "🌱 PRESSIONE E PARA PLANTAR",
                estilo
            );
        }
        else if (!foiRegada)
        {
            GUI.Box(
                new Rect(x, y, 360, 60),
                "💧 PRESSIONE E PARA REGAR",
                estilo
            );
        }
        else if (crescendo)
        {
            GUI.Box(
                new Rect(x, y, 360, 60),
                "🌱 A ÁRVORE ESTÁ CRESCENDO...",
                estilo
            );
        }
        else
        {
            GUI.Box(
                new Rect(x, y, 360, 60),
                "🌳 ÁRVORE PRONTA PARA COLHER!",
                estilo
            );
        }
    }
}