using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Reglas")]
    public int m_MaxLosses = 5;             // Derrotas (intentos) para que termine el juego
    public float m_MaxGameTime = 240f;      // Tiempo máximo: 4 minutos
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public float m_GameOverDelay = 10f;     // Tiempo que se ve la pantalla final

    [Header("Referencias")]
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public Text m_TimerText;
    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;
    [Header("Enemigos")]
    public int m_EnemyCount = 3;
    public float m_EnemySpawnRadius = 12f;
    public float m_EnemyMinDistanceToTank = 7f;
    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;

    private float m_TimeLeft;               // Tiempo restante del temporizador
    private float m_TotalPlayTime;          // Tiempo total jugado
    private bool m_TimedOut;
    private bool m_GameOver;
    private System.Collections.Generic.List<EnemyController> m_Enemies = new System.Collections.Generic.List<EnemyController>();
    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        m_TimeLeft = m_MaxGameTime;
        UpdateTimerUI();

        SpawnAllTanks();
        SetCameraTargets();
        StartCoroutine(GameLoop());
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];
        for (int i = 0; i < targets.Length; i++)
            targets[i] = m_Tanks[i].m_Instance.transform;

        m_CameraControl.m_Targets = targets;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameOver)
        {
            // Dejo ver la pantalla final y reinicio el juego
            yield return new WaitForSeconds(m_GameOverDelay);
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

        private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        SpawnEnemies();
        SetEnemiesActive(false);
        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        SetEnemiesActive(true);
        m_MessageText.text = string.Empty;

        // La ronda sigue mientras haya más de un tanque y no se acabe el tiempo
        while (!OneTankLeft() && !m_TimedOut)
        {
            m_TimeLeft -= Time.deltaTime;
            m_TotalPlayTime += Time.deltaTime;

            if (m_TimeLeft <= 0f)
            {
                m_TimeLeft = 0f;
                m_TimedOut = true;
            }

            UpdateTimerUI();
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        SetEnemiesActive(false);

        m_RoundWinner = null;
        m_GameWinner = null;

        if (m_TimedOut)
        {
            // Se acabó el tiempo: pierden ambos jugadores
            m_GameOver = true;
            m_MessageText.text = "TIEMPO AGOTADO!\n\nAMBOS JUGADORES PIERDEN\n\n"
                                 + "Tiempo jugado: " + FormatTime(m_TotalPlayTime);
        }
        else
        {
            m_RoundWinner = GetRoundWinner();

            if (m_RoundWinner != null)
            {
                m_RoundWinner.m_Wins++;

                // Los demás pierden un intento
                for (int i = 0; i < m_Tanks.Length; i++)
                    if (m_Tanks[i] != m_RoundWinner)
                        m_Tanks[i].m_Losses++;
            }

            m_GameWinner = GetGameWinner();
            m_GameOver = m_GameWinner != null;
            m_MessageText.text = EndMessage();
        }

        yield return m_EndWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;
        for (int i = 0; i < m_Tanks.Length; i++)
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];

        return null;    // Empate
    }

    // Si un jugador llega al máximo de derrotas, gana el otro
    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Losses >= m_MaxLosses)
            {
                for (int j = 0; j < m_Tanks.Length; j++)
                    if (j != i)
                        return m_Tanks[j];
            }
        }
        return null;
    }

    private string EndMessage()
    {
        // Pantalla final: ganador, puntaje y tiempo
        if (m_GameWinner != null)
        {
            int score = m_GameWinner.m_Wins * 100;
            return m_GameWinner.m_ColoredPlayerText + " GANA EL JUEGO!\n\n"
                   + "Puntaje: " + score + "\n"
                   + "Rondas ganadas: " + m_GameWinner.m_Wins + "\n"
                   + "Tiempo para ganar: " + FormatTime(m_TotalPlayTime);
        }

        string message = "EMPATE!";
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " GANA LA RONDA!";

        message += "\n\n\n";
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " VICTORIAS | "
                       + m_Tanks[i].m_Losses + "/" + m_MaxLosses + " DERROTAS\n";
        }
        return message;
    }

    private void UpdateTimerUI()
    {
        if (m_TimerText != null)
            m_TimerText.text = "TIEMPO " + FormatTime(m_TimeLeft);
    }

    private string FormatTime(float seconds)
    {
        int total = Mathf.CeilToInt(seconds);
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].Reset();
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].EnableControl();
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].DisableControl();
    }
        private void SpawnEnemies()
    {
        ClearEnemies();

        // Centro del mapa: punto medio entre los puntos de aparición
        Vector3 center = Vector3.zero;
        for (int i = 0; i < m_Tanks.Length; i++)
            center += m_Tanks[i].m_SpawnPoint.position;
        center /= Mathf.Max(1, m_Tanks.Length);

        Transform[] targets = new Transform[m_Tanks.Length];
        for (int i = 0; i < targets.Length; i++)
            targets[i] = m_Tanks[i].m_Instance.transform;

        for (int n = 0; n < m_EnemyCount; n++)
        {
            // Busco una posición lejos de los tanques
            Vector3 pos = center;
            for (int tries = 0; tries < 20; tries++)
            {
                Vector2 r = Random.insideUnitCircle * m_EnemySpawnRadius;
                pos = new Vector3(center.x + r.x, 0f, center.z + r.y);

                bool farEnough = true;
                for (int i = 0; i < m_Tanks.Length; i++)
                    if (Vector3.Distance(pos, m_Tanks[i].m_SpawnPoint.position) < m_EnemyMinDistanceToTank)
                        farEnough = false;
                if (farEnough) break;
            }

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Enemy";
            go.transform.localScale = new Vector3(1.6f, 1.2f, 1.6f);
            go.transform.position = new Vector3(pos.x, 1.2f, pos.z);
            go.GetComponent<Renderer>().material.color = new Color(0.55f, 0.1f, 0.65f);
            go.AddComponent<Rigidbody>();

            EnemyController enemy = go.AddComponent<EnemyController>();
            enemy.m_Targets = targets;
            m_Enemies.Add(enemy);
        }
    }

    private void SetEnemiesActive(bool active)
    {
        for (int i = 0; i < m_Enemies.Count; i++)
            if (m_Enemies[i] != null)
                m_Enemies[i].m_Active = active;
    }

    private void ClearEnemies()
    {
        for (int i = 0; i < m_Enemies.Count; i++)
            if (m_Enemies[i] != null)
                Destroy(m_Enemies[i].gameObject);
        m_Enemies.Clear();
    }
}