using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// all UI cause I hate canvas
public class GameManager : MonoBehaviour
{
    public int targetScore = 3;
    public int startingAttempts = 3;
    public float timeLimit = 60f;   // seconds; the game is lost when this hits 0

    private int currentScore = 0;
    private int currentAttempts;

    public bool isPlaying = true;

    private Canvas canvas;
    private TextMeshProUGUI deliveriesText;
    private TextMeshProUGUI attemptsText;
    private TextMeshProUGUI timerText;
    private float timeLeft;

    void Start()
    {
        currentAttempts = startingAttempts;
        timeLeft = timeLimit;

        BuildCanvas();
        BuildHUD();
        UpdateUI();
    }

    void Update()
    {
        if (!isPlaying) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            UpdateTimerText();
            EndGame("TIME'S UP!", new Color(0.95f, 0.3f, 0.3f));
            return;
        }

        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = "Time: " + seconds;

        // Turn red for the last 10 seconds
        timerText.color = seconds <= 10 ? new Color(1f, 0.3f, 0.3f) : Color.white;
    }

    public void AddScore()
    {
        if (!isPlaying)
            return;

        currentScore++;

        UpdateUI();

        if (currentScore >= targetScore)
        {
            WinGame();
        }
    }

    public void LoseAttempt()
    {
        if (!isPlaying)
            return;

        currentAttempts--;

        UpdateUI();

        if (currentAttempts <= 0)
        {
            LoseGame();
        }
    }

    void UpdateUI()
    {
        deliveriesText.text = "Deliveries: " + currentScore + " / " + targetScore;
        attemptsText.text = "Attempts: " + currentAttempts;
    }

    void WinGame()
    {
        EndGame("YOU WIN!", new Color(0.35f, 0.9f, 0.4f));
    }

    void LoseGame()
    {
        EndGame("GAME OVER", new Color(0.95f, 0.3f, 0.3f));
    }

    void EndGame(string title, Color titleColor)
    {
        isPlaying = false;

        // Free the mouse so the button can be clicked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        BuildEndScreen(title, titleColor);
    }

    // Reloads the scene, which resets everything
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---------- UI built in code ----------

    void BuildCanvas()
    {
        EnsureEventSystem();

        GameObject canvasObj = new GameObject("GameUI");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    void BuildHUD()
    {
        Vector2 topLeft = new Vector2(0f, 1f);

        deliveriesText = MakeText(canvas.transform, "DeliveriesText", "", 52, Color.white,
            topLeft, topLeft, new Vector2(40, -30), new Vector2(900, 70),
            FontStyles.Bold, TextAlignmentOptions.TopLeft, true);

        attemptsText = MakeText(canvas.transform, "AttemptsText", "", 38, Color.white,
            topLeft, topLeft, new Vector2(40, -100), new Vector2(900, 55),
            FontStyles.Normal, TextAlignmentOptions.TopLeft, true);

        // Timer, top center
        Vector2 topCenter = new Vector2(0.5f, 1f);
        timerText = MakeText(canvas.transform, "TimerText", "", 64, Color.white,
            topCenter, topCenter, new Vector2(0, -30), new Vector2(500, 80),
            FontStyles.Bold, TextAlignmentOptions.Top, true);

        MakeText(canvas.transform, "ObjectiveText",
            "Deliver " + targetScore + " objects and avoid the hazards!", 34,
            new Color(1f, 0.92f, 0.5f),
            topLeft, topLeft, new Vector2(40, -155), new Vector2(1200, 50),
            FontStyles.Normal, TextAlignmentOptions.TopLeft, true);
    }

    void BuildEndScreen(string title, Color titleColor)
    {
        Vector2 center = new Vector2(0.5f, 0.5f);

        // Dark overlay over the whole screen
        GameObject dim = new GameObject("Dim", typeof(RectTransform), typeof(Image));
        dim.transform.SetParent(canvas.transform, false);
        Stretch(dim.GetComponent<RectTransform>());
        dim.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        MakeText(canvas.transform, "Title", title, 130, titleColor,
            center, center, new Vector2(0, 150), new Vector2(1400, 180),
            FontStyles.Bold, TextAlignmentOptions.Center);

        MakeText(canvas.transform, "Summary",
            "Deliveries: " + currentScore + " / " + targetScore, 50, Color.white,
            center, center, new Vector2(0, 30), new Vector2(1000, 80),
            FontStyles.Normal, TextAlignmentOptions.Center);

        // Try Again button
        GameObject btnObj = new GameObject("TryAgainButton", typeof(RectTransform), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(canvas.transform, false);

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = btnRect.anchorMax = center;
        btnRect.pivot = center;
        btnRect.anchoredPosition = new Vector2(0, -130);
        btnRect.sizeDelta = new Vector2(440, 120);

        Image btnImage = btnObj.GetComponent<Image>();
        btnImage.color = Color.white;

        Button button = btnObj.GetComponent<Button>();
        button.targetGraphic = btnImage;

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.55f, 0.9f);
        colors.highlightedColor = new Color(0.35f, 0.7f, 1f);
        colors.pressedColor = new Color(0.12f, 0.35f, 0.65f);
        colors.selectedColor = colors.normalColor;
        button.colors = colors;

        button.onClick.AddListener(RestartGame);

        TextMeshProUGUI label = MakeText(btnObj.transform, "Label", "Try Again", 56, Color.white,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            FontStyles.Bold, TextAlignmentOptions.Center);
        Stretch(label.rectTransform);
    }

    TextMeshProUGUI MakeText(Transform parent, string objName, string text, float size, Color color,
                             Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 boxSize,
                             FontStyles style, TextAlignmentOptions align, bool outline = false)
    {
        GameObject obj = new GameObject(objName, typeof(RectTransform));
        obj.transform.SetParent(parent, false);

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = boxSize;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = align;
        tmp.raycastTarget = false;   // text never blocks the button click

        if (outline)
        {
            tmp.outlineWidth = 0.25f;
            tmp.outlineColor = Color.black;
        }

        return tmp;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Buttons need an EventSystem using the new Input System module
    void EnsureEventSystem()
    {
        EventSystem es = FindFirstObjectByType<EventSystem>();

        if (es == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            es = esObj.AddComponent<EventSystem>();
        }

        StandaloneInputModule oldModule = es.GetComponent<StandaloneInputModule>();
        if (oldModule != null)
            Destroy(oldModule);

        if (es.GetComponent<InputSystemUIInputModule>() == null)
            es.gameObject.AddComponent<InputSystemUIInputModule>();
    }
}
