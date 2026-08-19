using UnityEngine;

public class TrashValue : MonoBehaviour
{
    [Header("Valor do Lixo")]
    public int dinheiro = 25;

    [HideInInspector]
    public bool foiDescartado = false;

    public int ReceberValor()
    {
        if (foiDescartado)
            return 0;

        foiDescartado = true;

        return dinheiro;
    }
}