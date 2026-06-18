using System.Collections.Generic;

[System.Serializable]
public class OptionData
{
    public string text;

    public List<RequirementData> requirements;
    public List<EffectData> effects;

    public List<string> nextNodes;
}