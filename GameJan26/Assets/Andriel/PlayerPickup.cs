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

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                DropObject();
            }
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldObject = hit.collider.gameObject;

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                Collider col = heldObject.GetComponent<Collider>();

                if (col != null)
                {
                    col.enabled = false;
                }

                heldObject.transform.SetParent(holdingPoint);

                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;

                // Som de pegar
                if (pickupAudioSource != null && pickupSound != null)
                {
                    pickupAudioSource.PlayOneShot(pickupSound);
                }
            }
        }
    }

    void DropObject()
    {
        heldObject.transform.SetParent(null);

        heldObject.transform.position =
            Camera.main.transform.position +
            Camera.main.transform.forward * 1.5f;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col = heldObject.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        // Som de soltar
        if (pickupAudioSource != null && dropSound != null)
        {
            pickupAudioSource.PlayOneShot(dropSound);
        }

        heldObject = null;
    }
}