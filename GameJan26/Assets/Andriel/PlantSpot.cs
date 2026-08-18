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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // PLAYER
=======
        // ENCONTRAR PLAYER
>>>>>>> Stashed changes
=======
        // ENCONTRAR PLAYER
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                "❌ Player não encontrado!"
=======
                "Player não encontrado! Coloque a Tag 'Player' no Player."
>>>>>>> Stashed changes
=======
                "Player não encontrado! Coloque a Tag 'Player' no Player."
>>>>>>> Stashed changes
            );
        }

        // =========================
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // SHOP
=======
        // ENCONTRAR LOJA
>>>>>>> Stashed changes
=======
        // ENCONTRAR LOJA
>>>>>>> Stashed changes
        // =========================

        loja = FindFirstObjectByType<Shop>();

        if (loja == null)
        {
            Debug.LogWarning(
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                "❌ Shop não encontrado!"
=======
                "Shop não encontrado na cena!"
>>>>>>> Stashed changes
=======
                "Shop não encontrado na cena!"
>>>>>>> Stashed changes
            );
        }
    }

    void Update()
    {
        if (player == null)
            return;

        // =========================
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // DISTÂNCIA
=======
        // DISTÂNCIA DO PLAYER
>>>>>>> Stashed changes
=======
        // DISTÂNCIA DO PLAYER
>>>>>>> Stashed changes
        // =========================

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerPerto =
            distancia <= distanciaPlantio;

        // =========================
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // SOM DE CRESCIMENTO
=======
        // VOLUME DO SOM DE CRESCIMENTO
>>>>>>> Stashed changes
=======
        // VOLUME DO SOM DE CRESCIMENTO
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
        // =========================
        // INTERAÇÃO
        // =========================

<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        if (!playerPerto)
            return;

        if (Keyboard.current == null)
            return;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // =========================
        // E
        // =========================

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Se não existe árvore
=======
=======
>>>>>>> Stashed changes
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // =========================
            // PLANTAR
            // =========================

<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            if (arvoreAtual == null)
            {
                Plantar();
                return;
            }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
            // Se existe árvore e ainda não foi regada
            if (!foiRegada)
=======
=======
>>>>>>> Stashed changes
            // =========================
            // REGAR
            // =========================

            if (arvoreAtual != null &&
                !foiRegada)
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                "⚠️ Coloque o Prefab da árvore!"
=======
                "⚠️ Coloque o Prefab da árvore no PlantSpot!"
>>>>>>> Stashed changes
=======
                "⚠️ Coloque o Prefab da árvore no PlantSpot!"
>>>>>>> Stashed changes
            );

            return;
        }

        arvoreAtual = Instantiate(
            arvorePrefab,
            transform.position,
            Quaternion.identity
        );

<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
=======
>>>>>>> Stashed changes
        // Começa com 30% do tamanho
        arvoreAtual.transform.localScale =
            Vector3.one * 0.3f;

        // =========================
        // SOM DE PLANTIO
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        // =========================

        if (audioSource != null &&
            somPlantio != null)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            audioSource.PlayOneShot(
                somPlantio
            );
        }

        Debug.Log(
            "🌱 Semente plantada!"
        );
=======
=======
>>>>>>> Stashed changes
            audioSource.PlayOneShot(somPlantio);
        }

        Debug.Log("🌱 Semente plantada!");
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }

    // =====================================================
    // TENTAR REGAR
    // =====================================================

    void TentarRegar()
    {
        if (loja == null)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            loja =
                FindFirstObjectByType<Shop>();
        }

        if (loja == null)
        {
            Debug.LogWarning(
                "❌ Shop não encontrado!"
=======
            Debug.LogWarning(
                "Shop não encontrado!"
>>>>>>> Stashed changes
=======
            Debug.LogWarning(
                "Shop não encontrado!"
>>>>>>> Stashed changes
            );

            return;
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // =========================
        // VERIFICAR REGADOR
        // =========================

=======
        // Verifica se está segurando o regador
>>>>>>> Stashed changes
=======
        // Verifica se está segurando o regador
>>>>>>> Stashed changes
        if (!loja.EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você precisa estar segurando o regador!"
            );

            return;
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
        // Tem regador
>>>>>>> Stashed changes
=======
        // Tem regador
>>>>>>> Stashed changes
        Regar();
    }

    // =====================================================
    // REGAR
    // =====================================================

    void Regar()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // Impede regar duas vezes
        if (foiRegada)
            return;

        foiRegada = true;

        // =========================
        // SOM
=======
=======
>>>>>>> Stashed changes
        foiRegada = true;

        // =========================
        // SOM DE REGAR
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        // =========================

        if (regarAudioSource != null &&
            somRegar != null)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
=======
>>>>>>> Stashed changes
            regarAudioSource.PlayOneShot(somRegar);
        }

        Debug.Log("💧 Planta regada!");

        // Consome o regador
        loja.UsarRegador();

        // =========================
        // COMEÇA O CRESCIMENTO
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        // =========================

        if (!crescendo)
        {
            StartCoroutine(
                CrescerArvore()
            );
        }
    }

    // =====================================================
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    // CRESCER
=======
    // CRESCER ÁRVORE
>>>>>>> Stashed changes
=======
    // CRESCER ÁRVORE
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // SOM
=======
        // COMEÇA SOM DE CRESCIMENTO
>>>>>>> Stashed changes
=======
        // COMEÇA SOM DE CRESCIMENTO
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                PararSomCrescimento();

                crescendo = false;
=======
=======
>>>>>>> Stashed changes
                if (crescimentoAudioSource != null)
                {
                    crescimentoAudioSource.Stop();
                }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
        // =========================
        // TAMANHO FINAL
        // =========================

<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        if (arvoreAtual != null)
        {
            arvoreAtual.transform.localScale =
                escalaFinal;
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
=======
>>>>>>> Stashed changes
        // =========================
        // PARA O SOM
        // =========================

<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        if (crescimentoAudioSource != null)
        {
            crescimentoAudioSource.Stop();
        }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
=======
>>>>>>> Stashed changes

        crescendo = false;

        Debug.Log("🌳 Árvore cresceu!");
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // PLANTAR
=======
        // AINDA NÃO PLANTOU
>>>>>>> Stashed changes
=======
        // AINDA NÃO PLANTOU
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // REGAR
=======
        // PLANTOU, MAS NÃO REGOU
>>>>>>> Stashed changes
=======
        // PLANTOU, MAS NÃO REGOU
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // CRESCENDO
=======
        // ESTÁ CRESCENDO
>>>>>>> Stashed changes
=======
        // ESTÁ CRESCENDO
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream

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
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }
}