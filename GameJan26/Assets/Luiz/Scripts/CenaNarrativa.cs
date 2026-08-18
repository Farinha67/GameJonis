using UnityEngine;

public class CenaNarrativa : MonoBehaviour
{
    [Header("Áudio da narrativa")]
    public AudioSource audioNarracao;

    [Header("Jogador")]
    public MonoBehaviour controleDoJogador;

    [Header("Objetos da narrativa")]
    public GameObject painelNarrativa;

    private bool narrativaTerminou = false;

    void Start()
    {

        if (controleDoJogador != null)
            controleDoJogador.enabled = false;


        if (painelNarrativa != null)
            painelNarrativa.SetActive(true);


        if (audioNarracao != null)
            audioNarracao.Play();
    }

    void Update()
    {

        if (!narrativaTerminou &&
            audioNarracao != null &&
            !audioNarracao.isPlaying &&
            audioNarracao.time > 0)
        {
            IniciarJogo();
        }
    }

    void IniciarJogo()
    {
        narrativaTerminou = true;


        if (painelNarrativa != null)
            painelNarrativa.SetActive(false);


        if (controleDoJogador != null)
            controleDoJogador.enabled = true;

        Debug.Log("Narrativa terminou. Jogo iniciado!");
    }
}