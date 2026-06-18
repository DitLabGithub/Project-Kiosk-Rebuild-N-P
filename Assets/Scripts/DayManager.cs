using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    public List<DaySchedule> schedules =
        new List<DaySchedule>();

    public int currentDay = 1;

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
}