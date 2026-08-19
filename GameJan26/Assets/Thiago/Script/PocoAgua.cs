using UnityEngine;
using UnityEngine.InputSystem;

public class PocoAgua : MonoBehaviour
{
    // =====================================================
    // CONFIGURAÇÃO
    // =====================================================

    [Header("Configuração")]
    public float distanciaInteracao = 3f;

    [Header("Tecla")]
    public Key teclaInteracao = Key.E;

    // =====================================================
    // PLAYER
    // =====================================================

    private Transform player;
    private RegadorPickup regadorPickup;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError(
                "❌ Player não encontrado! Confira se a Tag está como Player."
            );

            return;
        }

        player = playerObject.transform;

        regadorPickup =
            playerObject.GetComponent<RegadorPickup>();

        if (regadorPickup == null)
        {
            Debug.LogError(
                "❌ O Player não possui o componente RegadorPickup!"
            );
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (player == null)
            return;

        if (regadorPickup == null)
        {
            regadorPickup =
                player.GetComponent<RegadorPickup>();

            if (regadorPickup == null)
                return;
        }

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distancia > distanciaInteracao)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            EncherRegador();
        }
    }

    // =====================================================
    // ENCHER REGADOR
    // =====================================================

    private void EncherRegador()
    {
        // Verifica se está segurando
        if (!regadorPickup.EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você precisa estar segurando o regador!"
            );

            return;
        }

        // Já está cheio
        if (regadorPickup.EstaCheio())
        {
            Debug.Log(
                "💧 O regador já está cheio!"
            );

            return;
        }

        // Enche
        regadorPickup.EncherRegador();

        Debug.Log(
            "💧 Regador cheio!"
        );
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (player == null)
            return;

        if (regadorPickup == null)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distancia > distanciaInteracao)
            return;

        GUIStyle estilo =
            new GUIStyle(GUI.skin.box);

        estilo.fontSize = 18;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        string texto;

        // =================================================
        // SEM REGADOR
        // =================================================

        if (!regadorPickup.EstaSegurandoRegador())
        {
            texto =
                "💧 PEGUE O REGADOR\n" +
                "E DEPOIS VOLTE AO POÇO";
        }

        // =================================================
        // REGADOR CHEIO
        // =================================================

        else if (regadorPickup.EstaCheio())
        {
            texto =
                "💧 REGADOR CHEIO\n" +
                regadorPickup.GetAguaAtual() +
                " / " +
                regadorPickup.GetCapacidadeMaxima();
        }

        // =================================================
        // REGADOR VAZIO/PARCIAL
        // =================================================

        else
        {
            texto =
                "💧 PRESSIONE E PARA\n" +
                "ENCHER O REGADOR\n" +
                regadorPickup.GetAguaAtual() +
                " / " +
                regadorPickup.GetCapacidadeMaxima();
        }

        GUI.Box(
            new Rect(
                (Screen.width - 420f) / 2f,
                Screen.height - 150f,
                420f,
                80f
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
        Gizmos.color =
            Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaInteracao
        );
    }
}