using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;
    public GameObject TransitionPanel;
    public GameObject[] CutscenePanels;
    public GameObject Background;
    public int currentCutsceneIndex = 0;

    public float fadeTime = 1f;

    private bool changingPanel = false;


    public List<DaySchedule> schedules =
        new List<DaySchedule>();

    public int currentDay = 1;

    public bool DebugMode = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Load("DayOrder");
    }

    public void Load(string fileName)
    {
        TextAsset json =
            Resources.Load<TextAsset>(fileName);

        if (json == null)
        {
            Debug.LogError(
                "DayOrder JSON missing"
            );

            return;
        }

        DayOrderWrapper wrapper =
            JsonUtility.FromJson<DayOrderWrapper>(
                json.text
            );

        foreach (var d in wrapper.days)
        {
            DaySchedule schedule =
                new DaySchedule();

            schedule.day = d.day;

            schedule.moneyGoal = d.moneyGoal;

            schedule.npcOrder = d.npcOrder;

            schedules.Add(schedule);
        }

        Debug.Log(
            "Loaded days: "
            + schedules.Count
        );
    }

    public DaySchedule GetToday()
    {
        DaySchedule today =
            schedules.Find(
                s => s.day == currentDay
            );

        // Push today's money goal into Counter
        if (today != null)
        {
            Counter.Instance.MoneyGoal =
                today.moneyGoal;
        }

        return today;
    }
    public void StartIntroCutscene()
    {
        UIManager.Instance.HideMenus();
        currentCutsceneIndex = 0;
        Background.SetActive(true);
        ShowCurrentPanel();
    }


    void ShowCurrentPanel()
    {
        GameObject panel =
            CutscenePanels[currentCutsceneIndex];

        panel.SetActive(true);

        CanvasGroup canvas =
            panel.GetComponent<CanvasGroup>();

        canvas.alpha = 0;

        StartCoroutine(
            FadeCanvas(canvas, 0, 1)
        );
    }


    public void ChangeCutscenePanel()
    {
        if (changingPanel)
            return;

        changingPanel = true;

        GameObject panel =
            CutscenePanels[currentCutsceneIndex];

        CanvasGroup canvas =
            panel.GetComponent<CanvasGroup>();

        StartCoroutine(
            FadeOutAndSwitch(panel, canvas)
        );
    }


    IEnumerator FadeOutAndSwitch(
        GameObject panel,
        CanvasGroup canvas
    )
    {
        yield return StartCoroutine(
            FadeCanvas(canvas, 1, 0)
        );

        panel.SetActive(false);

        currentCutsceneIndex++;

        changingPanel = false;

        if (currentCutsceneIndex < CutscenePanels.Length)
        {
            ShowCurrentPanel();
        }
        else
        {
            Background.SetActive(false);
            UIManager.Instance.InitiateDay(1);
        }
    }


    IEnumerator FadeCanvas(
        CanvasGroup canvas,
        float start,
        float end
    )
    {
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            canvas.alpha =
                Mathf.Lerp(
                    start,
                    end,
                    timer / fadeTime
                );

            yield return null;
        }

        canvas.alpha = end;
    }

    public IEnumerator ShowDayTransitionPanel()
    {
        TransitionPanel.SetActive(true);
        CanvasGroup canvas = TransitionPanel.GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        var text = TransitionPanel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Day " + currentDay;
        yield return StartCoroutine(FadeCanvas(canvas, 0, 1));
        text.text = "Day " + (currentDay);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeCanvas(canvas, 1, 0));
        TransitionPanel.SetActive(false);
    }
}