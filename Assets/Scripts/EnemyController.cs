using UnityEngine;

// Enemigo que persigue al tanque más cercano, lo empuja y le hace daño por contacto.
// Las bombas pueden destruirlo.
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [HideInInspector] public Transform[] m_Targets;   // Lo asigna el GameManager
    [HideInInspector] public bool m_Active;           // Solo se mueve mientras se juega

    public float m_Speed = 4f;
    public float m_TurnSpeed = 5f;
    public float m_ContactDamage = 8f;                // Daño por golpe
    public float m_DamageInterval = 1f;               // Segundos entre golpes
    public float m_Health = 60f;

    private Rigidbody m_Rigidbody;
    private float m_NextDamageTime;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.mass = 5f;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY
                                | RigidbodyConstraints.FreezeRotationX
                                | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (!m_Active)
        {
            m_Rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        Transform target = FindClosestTarget();
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;
        dir.Normalize();

        Quaternion look = Quaternion.LookRotation(dir);
        m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, look, m_TurnSpeed * Time.fixedDeltaTime));
        m_Rigidbody.MovePosition(m_Rigidbody.position + dir * m_Speed * Time.fixedDeltaTime);
    }

    private Transform FindClosestTarget()
    {
        if (m_Targets == null) return null;

        Transform best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (m_Targets[i] == null || !m_Targets[i].gameObject.activeSelf) continue;
            float d = (m_Targets[i].position - transform.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = m_Targets[i]; }
        }
        return best;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!m_Active || Time.time < m_NextDamageTime) return;

        TankHealth health = collision.gameObject.GetComponent<TankHealth>();
        if (health != null)
        {
            health.TakeDamage(m_ContactDamage);
            m_NextDamageTime = Time.time + m_DamageInterval;
        }
    }

    // Lo llaman las bombas
    public void TakeDamage(float amount)
    {
        m_Health -= amount;
        if (m_Health <= 0f)
            Destroy(gameObject);
    }
}