using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerSound : MonoBehaviour

{

    [Header("Audio Source")]

    public AudioSource audioSource;

    [Header("Sons")]

    public AudioClip movementSound;

    public AudioClip fSound;

    public AudioClip shiftSound;

    private void Awake()

    {

        // =====================================================

        // GARANTIR AUDIO SOURCE

        // =====================================================

        if (audioSource == null)

        {

            audioSource = GetComponent<AudioSource>();

            // Se não existir, cria automaticamente

            if (audioSource == null)

            {

                audioSource = gameObject.AddComponent<AudioSource>();

                Debug.Log(

                    "🔊 AudioSource não estava configurado. " +

                    "Um novo AudioSource foi criado automaticamente no Player."

                );

            }

        }

        // Configuração do AudioSource

        audioSource.playOnAwake = false;

        audioSource.loop = false;

    }

    private void Update()

    {

        // =====================================================

        // VERIFICAR TECLADO

        // =====================================================

        if (Keyboard.current == null)

            return;

        // =====================================================

        // MOVIMENTO WASD

        // =====================================================

        bool wasd =

            Keyboard.current.wKey.isPressed ||

            Keyboard.current.aKey.isPressed ||

            Keyboard.current.sKey.isPressed ||

            Keyboard.current.dKey.isPressed;

        // =====================================================

        // MOVIMENTO DAS SETAS

        // =====================================================

        bool arrows =

            Keyboard.current.upArrowKey.isPressed ||

            Keyboard.current.downArrowKey.isPressed ||

            Keyboard.current.leftArrowKey.isPressed ||

            Keyboard.current.rightArrowKey.isPressed;

        bool moving = wasd || arrows;

        // =====================================================

        // SHIFT

        // =====================================================

        bool shift =

            Keyboard.current.leftShiftKey.isPressed ||

            Keyboard.current.rightShiftKey.isPressed;

        // =====================================================

        // SOM DE MOVIMENTO

        // =====================================================

        if (shift && moving)

        {

            PlayLoop(shiftSound);

        }

        else if (moving)

        {

            PlayLoop(movementSound);

        }

        else

        {

            PararSomMovimento();

        }

        // =====================================================

        // SOM DO F

        // =====================================================

        if (Keyboard.current.fKey.wasPressedThisFrame)

        {

            TocarSomF();

        }

    }

    // =====================================================

    // TOCAR SOM EM LOOP

    // =====================================================

    private void PlayLoop(AudioClip sound)

    {

        // Se não existe som configurado,

        // simplesmente não faz nada.

        if (sound == null)

            return;

        // Segurança

        if (audioSource == null)

            return;

        // Se já está tocando exatamente esse som,

        // não reinicia o áudio.

        if (audioSource.clip == sound &&

            audioSource.isPlaying)

        {

            return;

        }

        // Para o som anterior

        audioSource.Stop();

        // Coloca o novo som

        audioSource.clip = sound;

        audioSource.loop = true;

        // Toca

        audioSource.Play();

    }

    // =====================================================

    // PARAR SOM DE MOVIMENTO

    // =====================================================

    private void PararSomMovimento()

    {

        if (audioSource == null)

            return;

        if (audioSource.isPlaying)

        {

            audioSource.Stop();

        }

        audioSource.clip = null;

        audioSource.loop = false;

    }

    // =====================================================

    // SOM DO F

    // =====================================================

    private void TocarSomF()

    {

        if (audioSource == null)

            return;

        if (fSound == null)

            return;

        audioSource.PlayOneShot(fSound);

    }

}
