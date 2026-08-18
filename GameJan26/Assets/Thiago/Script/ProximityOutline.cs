using UnityEngine;

public class ProximityOutline : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Distância")]
    public float outlineDistance = 3f;

    [Header("Outline")]
    public Outline outline;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    void Update()
    {
        if (player == null || outline == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        outline.enabled = distance <= outlineDistance;
    }
}