using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;          // Identifica qué tanque es de cada jugador
    public float m_Speed = 12f;             // Rapidez al moverse adelante/atrás
    public float m_TurnSpeed = 180f;        // Rapidez de giro (grados por segundo)
    public AudioSource m_MovementAudio;     // Audio del motor (distinto al de disparo)
    public AudioClip m_EngineIdling;        // Audio cuando el tanque está parado
    public AudioClip m_EngineDriving;       // Audio cuando se mueve
    public float m_PitchRange = 0.2f;       // Variación de afinación del motor

    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;    // Al habilitar, el tanque puede moverse
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;     // Al deshabilitar, el tanque se detiene
    }

    private void Start()
    {
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        m_MovementInputValue = TankKeys.GetMove(m_PlayerNumber);
        m_TurnInputValue = TankKeys.GetTurn(m_PlayerNumber);
        EngineAudio();
    }

    private void EngineAudio()
    {
        // Sin entrada = quieto
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}