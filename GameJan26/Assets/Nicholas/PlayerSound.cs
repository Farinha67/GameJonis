using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip movementSound;
    public AudioClip fSound;
    public AudioClip shiftSound;

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // WASD
        bool wasd =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed;

        // Setas
        bool arrows =
            Keyboard.current.upArrowKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed;

        bool moving = wasd || arrows;

        // SHIFT
        bool shift =
            Keyboard.current.leftShiftKey.isPressed ||
            Keyboard.current.rightShiftKey.isPressed;

        // SHIFT + movimento
        if (shift && moving)
        {
            PlayLoop(shiftSound);
        }
        // Movimento normal
        else if (moving)
        {
            PlayLoop(movementSound);
        }
        // Parado
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.clip = null;
        }

        // Som do F
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (fSound != null)
            {
                audioSource.PlayOneShot(fSound);
            }
        }
    }

    void PlayLoop(AudioClip sound)
    {
        if (sound == null)
            return;

        if (audioSource.clip != sound || !audioSource.isPlaying)
        {
            audioSource.Stop();

            audioSource.clip = sound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}