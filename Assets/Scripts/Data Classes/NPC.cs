using System.Collections.Generic;

[System.Serializable]
public class NPC
{
    public CharacterData data;

    public NPC(CharacterData data)
    {
        this.data = data;

        InjectRuntimeDefaults();
    }

    // GET
    public string GetVariable(string key)
    {
        if (data.variables == null)
            return "";

        VariableData v = data.variables.Find(x => x.name == key);

        return v != null ? v.value : "";
    }

    // SET
    public void SetVariable(string key, string value)
    {
        if (data.variables == null)
            data.variables = new List<VariableData>();

        VariableData v = data.variables.Find(x => x.name == key);

        if (v != null)
        {
            v.value = value;
        }
        else
        {
            data.variables.Add(new VariableData
            {
                name = key,
                value = value
            });
        }
    }

    // HAS
    public bool HasVariable(string key)
    {
        if (data.variables == null)
            return false;

        return data.variables.Exists(x => x.name == key);
    }

    // DEFAULTS
    private void InjectRuntimeDefaults()
    {
        if (data.variables == null)
            data.variables = new List<VariableData>();

        if (!HasVariable("ID_Result"))
            SetVariable("ID_Result", "none");

        if (!HasVariable("ID_Checked"))
            SetVariable("ID_Checked", "false");

        if (!HasVariable("UpsoldNumber"))
            SetVariable("UpsoldNumber", "0");
    }
}