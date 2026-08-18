using UnityEngine;
using TMPro;
public class Créditos : MonoBehaviour
{
    public float velocidade = 1.0f; // Velocidade de rolagem do texto
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, velocidade * Time.deltaTime);
    }
    public void VoltarProInicio()
    {
        rectTransform.anchoredPosition = new Vector2(0, -1100); // Reseta a posição do texto para o início
    }
}