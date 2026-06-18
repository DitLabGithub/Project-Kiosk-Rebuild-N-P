using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    public List<NPC> npcs = new List<NPC>();

    private DataLoader dataLoader;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dataLoader = GetComponent<DataLoader>();

        NPC amon = dataLoader.LoadNPC("Amon");

        if (amon != null)
        {
            npcs.Add(amon);

           // Debug.Log("Loaded NPC in the manager: " + amon.data.name);
        }
        NPC fridgy = dataLoader.LoadNPC("Fridgy");

        if (fridgy != null)
        {
            npcs.Add(fridgy);
            // Debug.Log("Loaded NPC in the manager: " + amon.data.name);
        }
        NPC Lily = dataLoader.LoadNPC("Lily");

        if (Lily != null)
        {
            npcs.Add(Lily);
            // Debug.Log("Loaded NPC in the manager: " + amon.data.name);
        }
        NPC Robin = dataLoader.LoadNPC("Robin");

        if (Robin != null)
        {
            npcs.Add(Robin);
            // Debug.Log("Loaded NPC in the manager: " + amon.data.name);
        }
        NPC Shaun = dataLoader.LoadNPC("Shaun");

        if (Shaun != null)
        {
            npcs.Add(Shaun);
            // Debug.Log("Loaded NPC in the manager: " + amon.data.name);
        }
        NPC Jop = dataLoader.LoadNPC("Jop");

        if (Jop != null)
        {
            npcs.Add(Jop);
            // Debug.Log("Loaded NPC in the manager: " + amon.data.name);
        }

    }
    public NPC GetNPC(string npcName)
    {
        return npcs.Find(npc => npc.data.name == npcName);
    }
}