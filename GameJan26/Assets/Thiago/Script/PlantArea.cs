using UnityEngine;

public class PlantArea : MonoBehaviour
{
    [Header("Configuração")]
    public MeshCollider areaPlantio;

    [Header("Visual")]
    public bool mostrarAreaNoEditor = true;

    [Header("Área de Plantio")]
    public float alturaPermitida = 2f;

    private void Reset()
    {
        areaPlantio = GetComponent<MeshCollider>();
    }

    private void Awake()
    {
        if (areaPlantio == null)
            areaPlantio = GetComponent<MeshCollider>();
    }

    private void OnValidate()
    {
        if (areaPlantio == null)
            areaPlantio = GetComponent<MeshCollider>();
    }

    // =====================================================
    // PODE PLANTAR
    // =====================================================

    public bool PodePlantar(Vector3 ponto)
    {
        if (areaPlantio == null)
            return false;

        if (areaPlantio.sharedMesh == null)
            return false;

        Bounds bounds = areaPlantio.bounds;

        bounds.Expand(
            new Vector3(
                0f,
                alturaPermitida,
                0f
            )
        );

        if (!bounds.Contains(ponto))
            return false;

        Ray ray =
            new Ray(
                ponto + Vector3.up * 0.5f,
                Vector3.down
            );

        RaycastHit hit;

        if (areaPlantio.Raycast(
                ray,
                out hit,
                alturaPermitida + 1f))
        {
            return true;
        }

        Ray ray2 =
            new Ray(
                ponto + Vector3.down * 0.5f,
                Vector3.up
            );

        if (areaPlantio.Raycast(
                ray2,
                out hit,
                alturaPermitida + 1f))
        {
            return true;
        }

        return false;
    }

    // =====================================================
    // VISUAL
    // =====================================================

    private void OnDrawGizmos()
    {
        if (!mostrarAreaNoEditor)
            return;

        if (areaPlantio == null)
            areaPlantio =
                GetComponent<MeshCollider>();

        if (areaPlantio == null)
            return;

        if (areaPlantio.sharedMesh == null)
            return;

        Gizmos.color =
            new Color(
                0f,
                1f,
                0f,
                0.25f
            );

        Gizmos.DrawWireMesh(
            areaPlantio.sharedMesh,
            areaPlantio.transform.position,
            areaPlantio.transform.rotation,
            areaPlantio.transform.lossyScale
        );
    }
}