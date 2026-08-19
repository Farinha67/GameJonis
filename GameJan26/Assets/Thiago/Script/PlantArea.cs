using UnityEngine;

public class PlantArea : MonoBehaviour
{
    [Header("Configuração")]
    public MeshCollider areaPlantio;

    [Header("Visual")]
    public bool mostrarAreaNoEditor = true;

    [Header("Área de plantio")]
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
    // VERIFICAR SE PODE PLANTAR
    // =====================================================

    public bool PodePlantar(Vector3 ponto)
    {
        if (areaPlantio == null)
            return false;

        if (areaPlantio.sharedMesh == null)
            return false;

        // Primeiro verifica se o ponto está próximo
        // da área da Mesh.
        Bounds bounds = areaPlantio.bounds;

        // Permite uma pequena tolerância vertical.
        bounds.Expand(
            new Vector3(
                0f,
                alturaPermitida,
                0f
            )
        );

        if (!bounds.Contains(ponto))
            return false;

        // Cria um raio para baixo.
        Ray raio =
            new Ray(
                ponto + Vector3.up * 0.5f,
                Vector3.down
            );

        RaycastHit hit;

        // Verifica especificamente o Mesh Collider
        if (areaPlantio.Raycast(
                raio,
                out hit,
                alturaPermitida + 1f))
        {
            return true;
        }

        // Segunda tentativa:
        // raio vindo de baixo para cima.
        Ray raioBaixo =
            new Ray(
                ponto + Vector3.down * 0.5f,
                Vector3.up
            );

        if (areaPlantio.Raycast(
                raioBaixo,
                out hit,
                alturaPermitida + 1f))
        {
            return true;
        }

        return false;
    }

    // =====================================================
    // VISUAL NO EDITOR
    // =====================================================

    private void OnDrawGizmos()
    {
        if (!mostrarAreaNoEditor)
            return;

        if (areaPlantio == null)
            areaPlantio = GetComponent<MeshCollider>();

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