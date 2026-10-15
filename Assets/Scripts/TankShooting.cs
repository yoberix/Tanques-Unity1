using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;          // Identifica a los jugadores
    public Rigidbody m_Shell;               // Prefab de la bomba
    public Transform m_FireTransform;       // Desde donde se lanzará la bomba
    public Slider m_AimSlider;              // Muestra la fuerza de lanzamiento
    public AudioSource m_ShootingAudio;     // Fuente de audio para el disparo
    public AudioClip m_ChargingClip;        // Audio mientras se carga
    public AudioClip m_FireClip;            // Audio al lanzar
    public float m_MinLaunchForce = 15f;    // Fuerza mínima
    public float m_MaxLaunchForce = 30f;    // Fuerza máxima
    public float m_MaxChargeTime = 0.75f;   // Tiempo máximo de carga

    private float m_CurrentLaunchForce;     // Fuerza al soltar el botón
    private float m_ChargeSpeed;            // Velocidad de carga
    private bool m_Fired;                   // ¿Ya se lanzó la bomba?
    private void Awake()
    {
        // Creo un Audio Source propio para el disparo, sin Loop ni Play On Awake,
        // para no mezclarlo con el del motor
        m_ShootingAudio = gameObject.AddComponent<AudioSource>();
        m_ShootingAudio.playOnAwake = false;
        m_ShootingAudio.loop = false;
        m_ShootingAudio.spatialBlend = 0f;
    }
    private void OnEnable()
    {
        // Al crear el tanque, reseteo la fuerza y la UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    private void Start()
    {
        // Velocidad de carga según el tiempo máximo y las fuerzas mínima y máxima
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    private void Update()
    {
        // Valor mínimo al slider
        m_AimSlider.value = m_MinLaunchForce;

        // Si llego al máximo y no he lanzado, disparo con fuerza máxima
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        // Si acabo de pulsar el botón de disparo
        else if (TankKeys.FireDown(m_PlayerNumber))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // Si mantengo pulsado y aún no he lanzado, cargo
        else if (TankKeys.FireHeld(m_PlayerNumber) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        // Si suelto el botón y no he lanzado, disparo
        else if (TankKeys.FireUp(m_PlayerNumber) && !m_Fired)
        {
            Fire();
        }
    }

    private void Fire()
    {
        // Solo se lanza una vez
        m_Fired = true;

        // Instancio la bomba y guardo su Rigidbody
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Velocidad de la bomba en la dirección de disparo
        shellInstance.linearVelocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // Audio de disparo
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reseteo la fuerza por precaución
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}