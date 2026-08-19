using UnityEngine;
using UnityEngine.InputSystem;

public class RegadorPickup : MonoBehaviour
{
    // =====================================================
    // HOLDING POINT
    // =====================================================

    [Header("Holding Point")]
    public Transform holdingPoint;

    // =====================================================
    // DISTÂNCIA
    // =====================================================

    [Header("Distância")]
    public float distanciaPegar = 3f;

    // =====================================================
    // ÁGUA
    // =====================================================

    [Header("Água")]
    [Tooltip("Quantidade máxima de água que o regador pode carregar.")]
    public int capacidadeMaxima = 5;

    [Tooltip("Quantidade de água inicial.")]
    public int aguaInicial = 0;

    private int quantidadeAgua;

    // =====================================================
    // REGADOR
    // =====================================================

    private GameObject regadorNaMao;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        quantidadeAgua = Mathf.Clamp(
            aguaInicial,
            0,
            capacidadeMaxima
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (regadorNaMao == null)
            {
                TentarPegar();
            }
            else
            {
                Soltar();
            }
        }
    }

    // =====================================================
    // PEGAR
    // =====================================================

    private void TentarPegar()
    {
        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ Holding Point do regador não configurado!"
            );

            return;
        }

        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError(
                "❌ Câmera principal não encontrada!"
            );

            return;
        }

        Ray ray = new Ray(
            cam.transform.position,
            cam.transform.forward
        );

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            distanciaPegar,
            ~0,
            QueryTriggerInteraction.Collide
        );

        if (hits == null || hits.Length == 0)
            return;

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Regador"))
                continue;

            regadorNaMao = hit.collider.gameObject;

            regadorNaMao.transform.SetParent(
                holdingPoint
            );

            regadorNaMao.transform.localPosition =
                Vector3.zero;

            regadorNaMao.transform.localRotation =
                Quaternion.identity;

            Rigidbody[] rigidbodies =
                regadorNaMao.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider[] colliders =
                regadorNaMao.GetComponentsInChildren<Collider>();

            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            Debug.Log(
                "💧 Regador pegado!"
            );

            Debug.Log(
                "💧 Água: " +
                quantidadeAgua +
                "/" +
                capacidadeMaxima
            );

            return;
        }
    }

    // =====================================================
    // SOLTAR
    // =====================================================

    private void Soltar()
    {
        if (regadorNaMao == null)
            return;

        Camera cam = Camera.main;

        if (cam == null)
            return;

        regadorNaMao.transform.SetParent(null);

        regadorNaMao.transform.position =
            cam.transform.position +
            cam.transform.forward * 1.5f;

        Rigidbody[] rigidbodies =
            regadorNaMao.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider[] colliders =
            regadorNaMao.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        regadorNaMao = null;

        Debug.Log(
            "💧 Regador solto!"
        );
    }

    // =====================================================
    // ESTÁ SEGURANDO
    // =====================================================

    public bool EstaSegurandoRegador()
    {
        return regadorNaMao != null;
    }

    // =====================================================
    // TEM ÁGUA
    // =====================================================

    public bool TemAgua()
    {
        return quantidadeAgua > 0;
    }

    // =====================================================
    // PEGAR QUANTIDADE
    // =====================================================

    public int GetQuantidadeAgua()
    {
        return quantidadeAgua;
    }

    // =====================================================
    // CAPACIDADE
    // =====================================================

    public int GetCapacidadeMaxima()
    {
        return capacidadeMaxima;
    }

    // =====================================================
    // GASTAR ÁGUA
    // =====================================================

    public bool UsarAgua()
    {
        if (!EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você não está segurando o regador!"
            );

            return false;
        }

        if (quantidadeAgua <= 0)
        {
            Debug.Log(
                "❌ O regador está vazio!"
            );

            return false;
        }

        quantidadeAgua--;

        Debug.Log(
            "💧 Água utilizada!"
        );

        Debug.Log(
            "💧 Água restante: " +
            quantidadeAgua +
            "/" +
            capacidadeMaxima
        );

        return true;
    }

    // =====================================================
    // ENCHER REGADOR
    // =====================================================

    public bool EncherRegador()
    {
        if (!EstaSegurandoRegador())
        {
            Debug.Log(
                "❌ Você precisa estar segurando o regador!"
            );

            return false;
        }

        if (quantidadeAgua >= capacidadeMaxima)
        {
            Debug.Log(
                "💧 O regador já está cheio!"
            );

            return false;
        }

        quantidadeAgua = capacidadeMaxima;

        Debug.Log(
            "💧 REGADOR CHEIO!"
        );

        Debug.Log(
            "💧 Água: " +
            quantidadeAgua +
            "/" +
            capacidadeMaxima
        );

        return true;
    }

    // =====================================================
    // ESVAZIAR
    // =====================================================

    public void EsvaziarRegador()
    {
        quantidadeAgua = 0;

        Debug.Log(
            "💧 Regador esvaziado."
        );
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (!EstaSegurandoRegador())
            return;

        GUIStyle estilo =
            new GUIStyle(
                GUI.skin.box
            );

        estilo.fontSize = 18;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        string texto =
            "💧 ÁGUA: " +
            quantidadeAgua +
            "/" +
            capacidadeMaxima;

        if (quantidadeAgua <= 0)
        {
            texto +=
                "\n⚠️ REGADOR VAZIO";
        }

        GUI.Box(
            new Rect(
                Screen.width - 230f,
                20f,
                210f,
                65f
            ),
            texto,
            estilo
        );
    }
}