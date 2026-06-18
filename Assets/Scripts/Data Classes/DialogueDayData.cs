using System.Collections.Generic;

[System.Serializable]
public class DialogueDayData
{
    public int day;

    public List<NodeData> nodes;
}

[System.Serializable]
public class DayOrderWrapper
{
    public List<DayOrderData> days;
}

[System.Serializable]
public class DayOrderData
{
    public int day;

    public int moneyGoal;

    public List<string> npcOrder;
}

[System.Serializable]
public class DaySchedule
{
    public int day;

    public int moneyGoal;

    public List<string> npcOrder;
}