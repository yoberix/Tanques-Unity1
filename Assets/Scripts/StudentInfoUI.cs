using UnityEngine;
using UnityEngine.UI;

// Muestra nombre y matrícula en la esquina superior derecha.
public class StudentInfoUI : MonoBehaviour
{
    public string m_StudentName = "TU NOMBRE";
    public string m_Matricula = "TU MATRICULA";
    public int m_FontSize = 32;

    private void Start()
    {
        GameObject canvasGO = new GameObject("StudentInfoCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject textGO = new GameObject("StudentInfoText");
        textGO.transform.SetParent(canvasGO.transform, false);

        Text text = textGO.AddComponent<Text>();
        text.text = m_StudentName + "\n" + m_Matricula;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = m_FontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperRight;
        text.raycastTarget = false;

        Shadow shadow = textGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
        shadow.effectDistance = new Vector2(2f, -2f);

        RectTransform rt = text.rectTransform;
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-20f, -15f);
        rt.sizeDelta = new Vector2(600f, 100f);
    }
}