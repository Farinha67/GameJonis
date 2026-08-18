using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public PlayerMoney playerMoney;
    public TMP_Text dinheiroText;

    void Update()
    {
        if (playerMoney == null || dinheiroText == null)
            return;

        dinheiroText.text =
            "💰 R$ " + playerMoney.GetDinheiro();
    }
}