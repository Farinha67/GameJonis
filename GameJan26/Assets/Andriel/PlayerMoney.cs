using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [Header("Dinheiro do jogador")]
    public int dinheiro = 100;

    public int GetDinheiro()
    {
        return dinheiro;
    }

    public bool TemDinheiro(int valor)
    {
        return dinheiro >= valor;
    }

    public void AdicionarDinheiro(int valor)
    {
        dinheiro += valor;

        Debug.Log("💰 +" + valor + " R$");
        Debug.Log("💰 Saldo atual: R$" + dinheiro);
    }

    public bool RemoverDinheiro(int valor)
    {
        if (dinheiro < valor)
        {
            Debug.Log("❌ Dinheiro insuficiente!");
            return false;
        }

        dinheiro -= valor;

        Debug.Log("💸 -" + valor + " R$");
        Debug.Log("💰 Saldo atual: R$" + dinheiro);

        return true;
    }
}