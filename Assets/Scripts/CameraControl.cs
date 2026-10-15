using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;          // Tiempo de espera para mover la cámara
    public float m_ScreenEdgeBuffer = 4f;    // Padding para que los tanques no se peguen a los bordes
    public float m_MinSize = 6.5f;           // Tamaño mínimo de zoom
    [HideInInspector] public Transform[] m_Targets; // Tanques a seguir

    private Camera m_Camera;
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        // Busco la posición intermedia entre los tanques
        FindAveragePosition();

        // Muevo la cámara de forma suave
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            // Si no está activo me lo salto
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        // Mantengo el valor de y
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }

    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            // Posición del tanque en el espacio de la cámara
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            // Diferencia entre la posición deseada y la del tanque
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Distancia vertical (arriba/abajo)
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Distancia horizontal (izquierda/derecha), dividida entre el aspect
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }

        // Aplico el padding y el zoom mínimo
        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    // La usará el GameManager para resetear posición y zoom en cada ronda
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();
        transform.position = m_DesiredPosition;
        m_Camera.orthographicSize = FindRequiredSize();
    }
}