using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Holding Point")]
    public Transform holdingPoint;

    [Header("Pickup")]
    public float pickupDistance = 3f;

    [Header("Pickup Sound")]
    public AudioSource pickupAudioSource;
    public AudioClip pickupSound;

    [Header("Drop Sound")]
    public AudioClip dropSound;

    private GameObject heldObject;

    private Camera playerCamera;

    void Start()
    {
        // =========================
        // ENCONTRAR CÂMERA
        // =========================

        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError(
                "❌ PlayerPickup: Nenhuma câmera encontrada com a Tag 'MainCamera'!"
            );
        }

        // =========================
        // VERIFICAR HOLDING POINT
        // =========================

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ PlayerPickup: Holding Point não foi colocado no Inspector!"
            );
        }
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // =========================
            // PEGAR
            // =========================

            if (heldObject == null)
            {
                TryPickup();
            }

            // =========================
            // SOLTAR
            // =========================

            else
            {
                DropObject();
            }
        }
    }

    // =====================================================
    // PEGAR OBJETO
    // =====================================================

    void TryPickup()
    {
        // =========================
        // VERIFICAR CÂMERA
        // =========================

        if (playerCamera == null)
        {
            playerCamera = Camera.main;

            if (playerCamera == null)
            {
                Debug.LogError(
                    "❌ PlayerPickup: Câmera não encontrada!"
                );

                return;
            }
        }

        // =========================
        // VERIFICAR HOLDING POINT
        // =========================

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ PlayerPickup: Holding Point não está configurado!"
            );

            return;
        }

        // =========================
        // RAYCAST
        // =========================

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            pickupDistance
        ))
        {
            // =========================
            // VERIFICAR TAG
            // =========================

            if (!hit.collider.CompareTag("Pickup"))
            {
                return;
            }

            // =========================
            // PEGAR OBJETO
            // =========================

            heldObject = hit.collider.gameObject;

            // =========================
            // RIGIDBODY
            // =========================

            Rigidbody rb =
                heldObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // =========================
            // COLLIDER
            // =========================

            Collider col =
                heldObject.GetComponent<Collider>();

            if (col != null)
            {
                col.enabled = false;
            }

            // =========================
            // COLOCAR NA MÃO
            // =========================

            heldObject.transform.SetParent(
                holdingPoint
            );

            heldObject.transform.localPosition =
                Vector3.zero;

            heldObject.transform.localRotation =
                Quaternion.identity;

            // =========================
            // SOM
            // =========================

            if (pickupAudioSource != null &&
                pickupSound != null)
            {
                pickupAudioSource.PlayOneShot(
                    pickupSound
                );
            }

            Debug.Log(
                "📦 Objeto pego: " +
                heldObject.name
            );
        }
    }

    // =====================================================
    // SOLTAR OBJETO
    // =====================================================

    void DropObject()
    {
        // =========================
        // SEGURANÇA
        // =========================

        if (heldObject == null)
            return;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;

            if (playerCamera == null)
            {
                Debug.LogError(
                    "❌ PlayerPickup: Câmera não encontrada!"
                );

                return;
            }
        }

        // =========================
        // TIRAR DA MÃO
        // =========================

        heldObject.transform.SetParent(null);

        // =========================
        // POSIÇÃO DE DROP
        // =========================

        heldObject.transform.position =
            playerCamera.transform.position +
            playerCamera.transform.forward * 1.5f;

        // =========================
        // RIGIDBODY
        // =========================

        Rigidbody rb =
            heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // =========================
        // COLLIDER
        // =========================

        Collider col =
            heldObject.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        // =========================
        // SOM
        // =========================

        if (pickupAudioSource != null &&
            dropSound != null)
        {
            pickupAudioSource.PlayOneShot(
                dropSound
            );
        }

        Debug.Log(
            "📦 Objeto soltado."
        );

        // =========================
        // LIMPAR
        // =========================

        heldObject = null;
    }

    // =====================================================
    // GIZMO
    // =====================================================

    void OnDrawGizmosSelected()
    {
        if (Camera.main == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            Camera.main.transform.position,
            Camera.main.transform.forward * pickupDistance
        );
    }
}