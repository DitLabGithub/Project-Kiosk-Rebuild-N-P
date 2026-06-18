using System.Collections.Generic;

[System.Serializable]
public class NodeData
{
    public string id;

    public string text;

    public List<RequirementData> requirements;

    public List<OptionData> options;
}
