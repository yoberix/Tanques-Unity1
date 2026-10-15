using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;                              // Color del tanque
    public Transform m_SpawnPoint;                           // Posición/dirección de aparición
    [HideInInspector] public int m_PlayerNumber;             // Número de jugador
    [HideInInspector] public string m_ColoredPlayerText;     // Texto "PLAYER 1" con color
    [HideInInspector] public GameObject m_Instance;          // Instancia del tanque
    [HideInInspector] public int m_Wins;                     // Victorias
    [HideInInspector] public int m_Losses;   // Rondas perdidas (intentos)
    private TankMovement m_Movement;
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;

    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        // Mismo número de jugador en todos los scripts
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        // Texto "PLAYER 1" con el color del tanque
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        // Coloreo los renderers del tanque
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = m_PlayerColor;
    }

    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;
        m_CanvasGameObject.SetActive(false);
    }

    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;
        m_CanvasGameObject.SetActive(true);
    }

    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;
        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}