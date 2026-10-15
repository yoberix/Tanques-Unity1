using UnityEngine;

// Reproduce música de fondo en bucle al iniciar el juego.
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    public AudioClip m_MusicClip;
    [Range(0f, 1f)] public float m_Volume = 0.4f;

    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = m_MusicClip;
        source.loop = true;
        source.volume = m_Volume;
        source.spatialBlend = 0f;   // sonido 2D
        source.playOnAwake = false;
        source.Play();
    }
}