using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                    // A qué afecta la explosión (Players)
    public ParticleSystem m_ExplosionParticles;     // Partículas de la explosión
    public AudioSource m_ExplosionAudio;            // Audio de la explosión
    public float m_MaxDamage = 100f;                // Daño si la explosión está centrada en el tanque
    public float m_ExplosionForce = 1000f;          // Fuerza añadida al tanque en el centro
    public float m_MaxLifeTime = 2f;                // Tiempo de vida de la bomba
    public float m_ExplosionRadius = 5f;            // Radio máximo de la explosión

    private void Start()
    {
        // Si no se ha destruido antes, la destruyo al acabar su tiempo de vida
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Recojo los colliders dentro del radio de la explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;

            // Añado la fuerza de la explosión
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth)
                continue;

            // Calculo y aplico el daño según la distancia
            float damage = CalculateDamage(targetRigidbody.position);
            targetHealth.TakeDamage(damage);
        }
        // Enemigos (cualquier capa): reciben daño y empuje
        Collider[] all = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
        for (int i = 0; i < all.Length; i++)
        {
            EnemyController enemy = all[i].GetComponent<EnemyController>();
            if (enemy == null) continue;

            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb) rb.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            enemy.TakeDamage(CalculateDamage(enemy.transform.position));
        }
        // Separo las partículas de la bomba y las reproduzco
        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // Destruyo el objeto de partículas cuando terminan, y la bomba
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;

        // Proporción de la distancia máxima (radio) a la que está el tanque
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Daño proporcional, nunca menor que 0
        float damage = relativeDistance * m_MaxDamage;
        return Mathf.Max(0f, damage);
    }
}