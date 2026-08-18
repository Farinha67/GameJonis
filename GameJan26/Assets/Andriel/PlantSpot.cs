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

    private Transform player;
    private GameObject arvoreAtual;

    private bool playerPerto = false;
    private bool foiRegada = false;
    private bool crescendo = false;

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
            Debug.LogWarning(
                "Player não encontrado! Coloque a Tag 'Player' no Player."
            );
        }

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

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaPlantio;

        if (!playerPerto)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // PLANTAR
            if (arvoreAtual == null)
            {
                Plantar();
                return;
            }

            // REGAR
            if (arvoreAtual != null &&
                !foiRegada)
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
                "⚠️ Coloque o Prefab da árvore no PlantSpot!"
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

        // SOM DE PLANTIO
        if (audioSource != null && somPlantio != null)
        {
            audioSource.PlayOneShot(somPlantio);
        }

        Debug.Log("🌱 Semente plantada!");
    }

    // =========================
    // TENTAR REGAR
    // =========================

    void TentarRegar()
    {
        if (loja == null)
        {
            Debug.LogWarning(
                "Shop não encontrado!"
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

    // =========================
    // REGAR
    // =========================

    void Regar()
    {
        foiRegada = true;

        Debug.Log("💧 Planta regada!");

        loja.UsarRegador();

        if (!crescendo)
        {
            StartCoroutine(
                CrescerArvore()
            );
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

        while (tempo < tempoParaCrescer)
        {
            if (arvoreAtual == null)
                yield break;

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

        Debug.Log("🌳 Árvore cresceu!");
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