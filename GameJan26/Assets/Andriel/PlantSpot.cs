using UnityEngine;
using UnityEngine.InputSystem;

public class PlantSpot : MonoBehaviour
{
    [Header("Árvore")]
    public GameObject arvorePrefab;

    [Header("Configuração")]
    public float distanciaPlantio = 2f;
    public float tempoParaCrescer = 10f;

    private Transform player;
    private GameObject arvoreAtual;

    private bool playerPerto = false;
    private bool foiRegada = false;
    private bool crescendo = false;

    private Shop loja;

    void Start()
    {
        // Procura o Player
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

        // Procura a loja
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

        // E
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // =========================
            // PLANTAR
            // =========================

            if (arvoreAtual == null)
            {
                Plantar();

                // Impede o mesmo E de plantar
                // e regar ao mesmo tempo
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

        // NÃO TEM REGADOR
        if (!loja.EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você precisa estar segurando o regador!"
            );

            return;
        }

        // Tem regador → pode regar
        Regar();
    }

    // =========================
    // REGAR
    // =========================

    void Regar()
    {
        foiRegada = true;

        Debug.Log("💧 Planta regada!");

        // CONSOME O REGADOR
        loja.UsarRegador();

        // Começa o crescimento
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

        // Ainda não plantou
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

        // Plantou, mas não regou
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

        // Já foi regada
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