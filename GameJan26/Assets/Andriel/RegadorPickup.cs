using UnityEngine;
using UnityEngine.InputSystem;

public class RegadorPickup : MonoBehaviour
{
    [Header("Holding Point")]
    public Transform holdingPoint;

    [Header("Distância")]
    public float distanciaPegar = 3f;

    private GameObject regadorNaMao;

    void Update()
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

    void TentarPegar()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return;

        Ray ray = new Ray(
            cam.transform.position,
            cam.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaPegar))
        {
            if (hit.collider.CompareTag("Regador"))
            {
                regadorNaMao = hit.collider.gameObject;

                regadorNaMao.transform.SetParent(holdingPoint);

                regadorNaMao.transform.localPosition = Vector3.zero;
                regadorNaMao.transform.localRotation = Quaternion.identity;

                Rigidbody rb = regadorNaMao.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                Collider col = regadorNaMao.GetComponent<Collider>();

                if (col != null)
                {
                    col.enabled = false;
                }

                Debug.Log("💧 Regador pegado!");
            }
        }
    }

    void Soltar()
    {
        regadorNaMao.transform.SetParent(null);

        regadorNaMao.transform.position =
            Camera.main.transform.position +
            Camera.main.transform.forward * 1.5f;

        Rigidbody rb = regadorNaMao.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col = regadorNaMao.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        regadorNaMao = null;

        Debug.Log("💧 Regador solto!");
    }
}