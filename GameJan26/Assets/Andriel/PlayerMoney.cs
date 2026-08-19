using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoney : MonoBehaviour
{
    [Header("Dinheiro do jogador")]
    public int dinheiro = 100;

    [Header("UI do dinheiro")]
    public bool mostrarDinheiro = true;

    [Header("Configuração dos ganhos")]
    public float tempoMensagem = 2f;

    public float distanciaEntreMensagens = 35f;

    // Lista dos ganhos atualmente aparecendo
    private List<MensagemDinheiro> mensagens =
        new List<MensagemDinheiro>();

    // =====================================================
    // CLASSE DA MENSAGEM
    // =====================================================

    private class MensagemDinheiro
    {
        public int valor;
        public float tempoRestante;

        public MensagemDinheiro(
            int valor,
            float tempo)
        {
            this.valor = valor;
            this.tempoRestante = tempo;
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (mensagens.Count == 0)
            return;

        // Atualizar tempo das mensagens
        for (int i = mensagens.Count - 1; i >= 0; i--)
        {
            mensagens[i].tempoRestante -=
                Time.deltaTime;

            if (mensagens[i].tempoRestante <= 0f)
            {
                mensagens.RemoveAt(i);
            }
        }
    }

    // =====================================================
    // PEGAR DINHEIRO
    // =====================================================

    public int GetDinheiro()
    {
        return dinheiro;
    }

    // =====================================================
    // TEM DINHEIRO
    // =====================================================

    public bool TemDinheiro(int valor)
    {
        return dinheiro >= valor;
    }

    // =====================================================
    // ADICIONAR DINHEIRO
    // =====================================================

    public void AdicionarDinheiro(int valor)
    {
        if (valor <= 0)
            return;

        dinheiro += valor;

        // =================================================
        // CRIAR NOVA MENSAGEM
        // =================================================

        MensagemDinheiro novaMensagem =
            new MensagemDinheiro(
                valor,
                tempoMensagem
            );

        mensagens.Add(novaMensagem);

        Debug.Log(
            "+ R$" + valor
        );

        Debug.Log(
            "Saldo atual: R$" + dinheiro
        );
    }

    // =====================================================
    // REMOVER DINHEIRO
    // =====================================================

    public bool RemoverDinheiro(int valor)
    {
        if (valor <= 0)
            return true;

        if (dinheiro < valor)
        {
            Debug.Log(
                "Dinheiro insuficiente!"
            );

            return false;
        }

        dinheiro -= valor;

        Debug.Log(
            "- R$" + valor
        );

        Debug.Log(
            "Saldo atual: R$" + dinheiro
        );

        return true;
    }

    // =====================================================
    // UI
    // =====================================================

    private void OnGUI()
    {
        if (!mostrarDinheiro)
            return;

        // =================================================
        // POSIÇÃO
        // =================================================

        float largura = 300f;
        float altura = 45f;

        float x =
            (Screen.width - largura) / 2f;

        float y = 15f;

        // =================================================
        // SALDO
        // =================================================

        GUIStyle estiloSaldo =
            new GUIStyle(
                GUI.skin.label
            );

        estiloSaldo.fontSize = 28;

        estiloSaldo.fontStyle =
            FontStyle.Bold;

        estiloSaldo.alignment =
            TextAnchor.MiddleCenter;

        GUI.Label(
            new Rect(
                x,
                y,
                largura,
                altura
            ),
            "R$ " + dinheiro,
            estiloSaldo
        );

        // =================================================
        // GANHOS
        // =================================================

        GUIStyle estiloGanho =
            new GUIStyle(
                GUI.skin.label
            );

        estiloGanho.fontSize = 22;

        estiloGanho.fontStyle =
            FontStyle.Bold;

        estiloGanho.alignment =
            TextAnchor.MiddleCenter;

        // =================================================
        // MOSTRAR CADA GANHO
        // =================================================

        for (int i = 0;
             i < mensagens.Count;
             i++)
        {
            MensagemDinheiro mensagem =
                mensagens[i];

            float posicaoY =
                y +
                altura +
                5f +
                (i * distanciaEntreMensagens);

            GUI.Label(
                new Rect(
                    x,
                    posicaoY,
                    largura,
                    35f
                ),
                "+ R$ " + mensagem.valor,
                estiloGanho
            );
        }
    }
}