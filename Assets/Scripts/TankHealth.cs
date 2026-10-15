using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;                 // Salud inicial del tanque
    public Slider m_Slider;                               // Slider que representa la salud
    public Image m_FillImage;                             // Imagen de relleno del slider
    public Color m_FullHealthColor = Color.green;         // Color con salud completa
    public Color m_ZeroHealthColor = Color.red;           // Color con salud vacía
    public GameObject m_ExplosionPrefab;                  // Prefab de la explosión

    private AudioSource m_ExplosionAudio;                 // Audio de la explosión
    private ParticleSystem m_ExplosionParticles;          // Partículas de la explosión
    private float m_CurrentHealth;                        // Salud actual
    private bool m_Dead;                                  // ¿Está muerto?

    private void Awake()
    {
        // Instancio el prefab de la explosión y guardo sus referencias
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Lo desactivo hasta que explote el tanque
        m_ExplosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // Al habilitar el tanque, reseteo la salud
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;
        SetHealthUI();

        // Si la salud llega a 0 y aún no ha explotado, muere
        if (m_CurrentHealth <= 0f && !m_Dead)
            OnDeath();
    }

    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;

        // Color entre rojo y verde según el porcentaje de salud
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    private void OnDeath()
    {
        m_Dead = true;

        // Coloco la explosión en la posición del tanque y la reproduzco
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // Desactivo el tanque
        gameObject.SetActive(false);
    }
}