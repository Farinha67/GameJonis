using UnityEngine;
using UnityEngine.InputSystem;

public class RegadorPickup : MonoBehaviour
{
    [Header("Holding Point")]
    public Transform holdingPoint;

    [Header("Distância para pegar")]
    public float distanciaPegar = 3f;

    [Header("Distância para soltar")]
    public float distanciaSoltar = 1.2f;

    [Header("Segurança contra paredes")]
    public float raioSeguranca = 0.25f;
    public float distanciaDaParede = 0.15f;
    public LayerMask camadaColisao = ~0;

    [Header("Água")]
    [Min(1)]
    public int capacidadeMaxima = 5;

    [SerializeField]
    private int aguaAtual = 0;

    private GameObject regadorNaMao;
    private Rigidbody rbRegador;
    private Collider[] collidersRegador;

    private bool estaNaMao = false;

    // Escala original do regador
    private Vector3 escalaOriginalRegador;

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;

        // =================================================
        // NÃO ESTÁ SEGURANDO
        // =================================================

        if (regadorNaMao == null)
        {
            TentarPegar();
            return;
        }

        // =================================================
        // ESTÁ MIRANDO NO POÇO
        // =================================================
        // IMPORTANTE:
        // Se estiver olhando para o poço, NÃO solta.
        // O PocoAgua.cs poderá usar o mesmo E para
        // encher o regador.
        // =================================================

        if (EstaMirandoNoPoco())
        {
            return;
        }

        // =================================================
        // ESTÁ MIRANDO EM ÁRVORE
        // =================================================
        // O PlantSpot vai cuidar da rega.
        // =================================================

        if (EstaMirandoEmArvore())
        {
            return;
        }

        // =================================================
        // NÃO ESTÁ MIRANDO EM POÇO NEM ÁRVORE
        // =================================================
        // Então pode soltar.
        // =================================================

        Soltar();
    }

    // =====================================================
    // VERIFICAR POÇO
    // =====================================================

    private bool EstaMirandoNoPoco()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return false;

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
            return false;

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            PocoAgua poco =
                hit.collider.GetComponent<PocoAgua>();

            if (poco == null)
            {
                poco =
                    hit.collider.GetComponentInParent<PocoAgua>();
            }

            if (poco == null)
            {
                poco =
                    hit.collider.GetComponentInChildren<PocoAgua>();
            }

            if (poco != null)
            {
                return true;
            }
        }

        return false;
    }

    // =====================================================
    // VERIFICAR ÁRVORE
    // =====================================================

    private bool EstaMirandoEmArvore()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return false;

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
            return false;

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            PlantSpot spot =
                hit.collider.GetComponent<PlantSpot>();

            if (spot == null)
            {
                spot =
                    hit.collider.GetComponentInParent<PlantSpot>();
            }

            if (spot == null)
            {
                spot =
                    hit.collider.GetComponentInChildren<PlantSpot>();
            }

            if (spot == null)
                continue;

            if (!spot.controladorDePlantio &&
                spot.TemArvore())
            {
                return true;
            }
        }

        return false;
    }

    // =====================================================
    // PEGAR
    // =====================================================

    private void TentarPegar()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return;

        if (holdingPoint == null)
        {
            Debug.LogError(
                "❌ Holding Point não configurado!"
            );

            return;
        }

        Ray ray = new Ray(
            cam.transform.position,
            cam.transform.forward
        );

        // Procura todos os objetos no caminho.
        // Isso evita a lixeira bloquear o regador.

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

        GameObject objetoRegador = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            GameObject encontrado =
                EncontrarRegador(hit.collider);

            if (encontrado != null)
            {
                objetoRegador = encontrado;
                break;
            }
        }

        if (objetoRegador == null)
        {
            Debug.Log(
                "❌ Nenhum regador encontrado."
            );

            return;
        }

        // =================================================
        // REGADOR
        // =================================================

        regadorNaMao =
            objetoRegador;

        // =================================================
        // SALVAR ESCALA ORIGINAL
        // =================================================

        escalaOriginalRegador =
            regadorNaMao.transform.lossyScale;

        // =================================================
        // RIGIDBODY
        // =================================================

        rbRegador =
            regadorNaMao.GetComponent<Rigidbody>();

        if (rbRegador == null)
        {
            rbRegador =
                regadorNaMao.GetComponentInChildren<Rigidbody>();
        }

        // =================================================
        // COLLIDERS
        // =================================================

        collidersRegador =
            regadorNaMao.GetComponentsInChildren<Collider>(
                true
            );

        // =================================================
        // DESATIVAR FÍSICA
        // =================================================

        if (rbRegador != null)
        {
            rbRegador.isKinematic = true;
            rbRegador.useGravity = false;

            rbRegador.linearVelocity =
                Vector3.zero;

            rbRegador.angularVelocity =
                Vector3.zero;
        }

        // =================================================
        // DESATIVAR COLLIDERS
        // =================================================

        if (collidersRegador != null)
        {
            foreach (Collider col in collidersRegador)
            {
                if (col != null)
                    col.enabled = false;
            }
        }

        // =================================================
        // COLOCAR NA MÃO
        // =================================================

        regadorNaMao.transform.SetParent(
            holdingPoint,
            true
        );

        regadorNaMao.transform.position =
            holdingPoint.position;

        regadorNaMao.transform.rotation =
            holdingPoint.rotation;

        // =================================================
        // MANTER ESCALA ORIGINAL
        // =================================================

        Vector3 escalaPai =
            holdingPoint.lossyScale;

        if (
            Mathf.Abs(escalaPai.x) > 0.0001f &&
            Mathf.Abs(escalaPai.y) > 0.0001f &&
            Mathf.Abs(escalaPai.z) > 0.0001f
        )
        {
            regadorNaMao.transform.localScale =
                new Vector3(
                    escalaOriginalRegador.x / escalaPai.x,
                    escalaOriginalRegador.y / escalaPai.y,
                    escalaOriginalRegador.z / escalaPai.z
                );
        }

        estaNaMao = true;

        Debug.Log(
            "💧 Regador pegado!"
        );

        Debug.Log(
            "💧 Água: " +
            aguaAtual +
            "/" +
            capacidadeMaxima
        );
    }

    // =====================================================
    // ENCONTRAR REGADOR
    // =====================================================

    private GameObject EncontrarRegador(
        Collider collider
    )
    {
        if (collider == null)
            return null;

        // =================================================
        // SUBIR HIERARQUIA
        // =================================================

        Transform atual =
            collider.transform;

        while (atual != null)
        {
            if (atual.CompareTag("Regador"))
            {
                return atual.gameObject;
            }

            atual =
                atual.parent;
        }

        // =================================================
        // PROCURAR NOS FILHOS
        // =================================================

        Transform[] filhos =
            collider.GetComponentsInChildren<Transform>(
                true
            );

        foreach (Transform filho in filhos)
        {
            if (filho.CompareTag("Regador"))
            {
                return filho.gameObject;
            }
        }

        return null;
    }

    // =====================================================
    // SOLTAR
    // =====================================================

    public void Soltar()
    {
        if (regadorNaMao == null)
            return;

        Vector3 direcao =
            transform.forward;

        Camera cam = Camera.main;

        if (cam != null)
        {
            direcao =
                cam.transform.forward;
        }

        direcao.y = 0f;

        if (direcao.sqrMagnitude < 0.01f)
        {
            direcao =
                transform.forward;

            direcao.y = 0f;
        }

        direcao.Normalize();

        // =================================================
        // POSIÇÃO BASE
        // =================================================

        Vector3 origem =
            transform.position +
            Vector3.up * 0.5f;

        Vector3 posicaoSoltar =
            origem +
            direcao * distanciaSoltar;

        posicaoSoltar.y =
            transform.position.y + 0.5f;

        // =================================================
        // PROCURAR PAREDE
        // =================================================

        if (Physics.SphereCast(
            origem,
            raioSeguranca,
            direcao,
            out RaycastHit hitParede,
            distanciaSoltar,
            camadaColisao,
            QueryTriggerInteraction.Ignore
        ))
        {
            posicaoSoltar =
                hitParede.point -
                direcao *
                (raioSeguranca + distanciaDaParede);

            posicaoSoltar.y =
                transform.position.y + 0.5f;
        }

        // =================================================
        // EVITAR NASCER DENTRO DE OBJETO
        // =================================================

        Collider[] bloqueios =
            Physics.OverlapSphere(
                posicaoSoltar,
                raioSeguranca,
                camadaColisao,
                QueryTriggerInteraction.Ignore
            );

        if (bloqueios.Length > 0)
        {
            posicaoSoltar =
                transform.position +
                direcao * 0.4f;

            posicaoSoltar.y =
                transform.position.y + 0.5f;

            Collider[] segundaVerificacao =
                Physics.OverlapSphere(
                    posicaoSoltar,
                    raioSeguranca,
                    camadaColisao,
                    QueryTriggerInteraction.Ignore
                );

            if (segundaVerificacao.Length > 0)
            {
                posicaoSoltar =
                    transform.position +
                    Vector3.up * 0.5f;
            }
        }

        // =================================================
        // GARANTIR CHÃO
        // =================================================

        Ray rayChao =
            new Ray(
                posicaoSoltar + Vector3.up * 2f,
                Vector3.down
            );

        if (Physics.Raycast(
            rayChao,
            out RaycastHit hitChao,
            5f,
            camadaColisao,
            QueryTriggerInteraction.Ignore
        ))
        {
            float alturaMinima =
                hitChao.point.y + 0.15f;

            if (posicaoSoltar.y < alturaMinima)
            {
                posicaoSoltar.y =
                    alturaMinima;
            }
        }

        // =================================================
        // TIRAR DA MÃO
        // =================================================

        regadorNaMao.transform.SetParent(
            null,
            true
        );

        regadorNaMao.transform.position =
            posicaoSoltar;

        // =================================================
        // ESCALA ORIGINAL
        // =================================================

        regadorNaMao.transform.localScale =
            escalaOriginalRegador;

        // =================================================
        // FÍSICA
        // =================================================

        if (rbRegador != null)
        {
            rbRegador.isKinematic = false;
            rbRegador.useGravity = true;

            rbRegador.linearVelocity =
                Vector3.zero;

            rbRegador.angularVelocity =
                Vector3.zero;
        }

        // =================================================
        // COLLIDERS
        // =================================================

        if (collidersRegador != null)
        {
            foreach (Collider col in collidersRegador)
            {
                if (col != null)
                    col.enabled = true;
            }
        }

        estaNaMao = false;

        regadorNaMao = null;
        rbRegador = null;
        collidersRegador = null;

        Debug.Log(
            "💧 Regador solto!"
        );
    }

    // =====================================================
    // ENCHER REGADOR
    // =====================================================

    public void EncherRegador()
    {
        aguaAtual =
            capacidadeMaxima;

        Debug.Log(
            "💧 Regador cheio: " +
            aguaAtual +
            "/" +
            capacidadeMaxima
        );
    }

    // =====================================================
    // USAR ÁGUA
    // =====================================================

    public bool UsarAgua()
    {
        if (aguaAtual <= 0)
        {
            Debug.Log(
                "❌ Regador vazio!"
            );

            return false;
        }

        aguaAtual--;

        Debug.Log(
            "💧 Água usada! Restam: " +
            aguaAtual +
            "/" +
            capacidadeMaxima
        );

        return true;
    }

    // =====================================================
    // TEM ÁGUA
    // =====================================================

    public bool TemAgua()
    {
        return aguaAtual > 0;
    }

    // =====================================================
    // ÁGUA ATUAL
    // =====================================================

    public int GetAguaAtual()
    {
        return aguaAtual;
    }

    // =====================================================
    // COMPATIBILIDADE COM POÇO
    // =====================================================

    public int GetQuantidadeAgua()
    {
        return aguaAtual;
    }

    // =====================================================
    // CAPACIDADE
    // =====================================================

    public int GetCapacidadeMaxima()
    {
        return capacidadeMaxima;
    }

    // =====================================================
    // CHEIO
    // =====================================================

    public bool EstaCheio()
    {
        return aguaAtual >= capacidadeMaxima;
    }

    // =====================================================
    // VAZIO
    // =====================================================

    public bool EstaVazio()
    {
        return aguaAtual <= 0;
    }

    // =====================================================
    // ESTÁ SEGURANDO
    // =====================================================

    public bool EstaSegurando()
    {
        return
            regadorNaMao != null &&
            estaNaMao;
    }

    // =====================================================
    // COMPATIBILIDADE
    // =====================================================

    public bool EstaSegurandoRegador()
    {
        return EstaSegurando();
    }

    // =====================================================
    // COMPATIBILIDADE ANTIGA
    // =====================================================

    public bool UsarRegador()
    {
        return UsarAgua();
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (!estaNaMao)
            return;

        GUIStyle estilo =
            new GUIStyle(
                GUI.skin.box
            );

        estilo.fontSize = 16;

        estilo.fontStyle =
            FontStyle.Bold;

        estilo.alignment =
            TextAnchor.MiddleCenter;

        GUI.Box(
            new Rect(
                Screen.width - 220f,
                Screen.height - 100f,
                200f,
                60f
            ),
            "💧 ÁGUA\n" +
            aguaAtual +
            " / " +
            capacidadeMaxima,
            estilo
        );
    }

    // =====================================================
    // GIZMO
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color =
            Color.cyan;

        Vector3 origem =
            transform.position +
            Vector3.up * 0.5f;

        Vector3 direcao =
            transform.forward;

        direcao.y = 0f;

        if (direcao.sqrMagnitude > 0.01f)
            direcao.Normalize();

        Vector3 ponto =
            origem +
            direcao * distanciaSoltar;

        Gizmos.DrawWireSphere(
            ponto,
            raioSeguranca
        );

        Gizmos.DrawLine(
            origem,
            ponto
        );
    }
}