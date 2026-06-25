using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;


    public List<VariableData> variables =
    new List<VariableData>();

    public NPC currentNPC;

    public DialogueDayData currentDialogue;

    public NodeData currentNode;

    private Coroutine typingCoroutine;

    [SerializeField]
    private float typingSpeed = 0.03f;

    private bool isTyping;

    private string fullText;

    private int currentNPCIndex = 0;

    private DaySchedule currentSchedule;

    
    private void Awake()
    {
        Instance = this;
    }

    public void StartDay(int day)
    {
        DayManager.Instance.currentDay = day;

        currentSchedule = DayManager.Instance.GetToday();

        currentNPCIndex = 0;

        UIManager.Instance.SetMoneyGoal(Counter.Instance.MoneyGoal);
        UIManager.Instance.SetDay(day);

        StartNextNPCDialogue();
    }

    public void StartNextNPCDialogue()
    {
        if (currentNPCIndex >= currentSchedule.npcOrder.Count)
        {
            if (DayManager.Instance.DebugMode)
            {
                Debug.Log("Day complete");
                UIManager.Instance.ShowLevelSelect();
                Counter.Instance.Money = 0;

                return;
            }
            else
            {
                if (DayManager.Instance.currentDay + 1 > DayManager.Instance.schedules.Count)
                {
                    Debug.Log("Day complete");
                    UIManager.Instance.ShowLevelSelect();
                    Counter.Instance.Money = 0;

                    return;
                }
                else
                {
                    StartCoroutine(DayManager.Instance.ShowDayTransitionPanel());
                    UIManager.Instance.InitiateDay(DayManager.Instance.currentDay + 1);
                    Counter.Instance.Money = 0;
                    return;
                }
            }
        }

        string npcName =
            currentSchedule.npcOrder[currentNPCIndex];

        currentNPC =
            NPCManager.Instance.GetNPC(npcName);

        currentDialogue =
            currentNPC.data.dialogues.Find(
                d => d.day == DayManager.Instance.currentDay
            );

        if (currentDialogue == null)
        {
            Debug.LogError("Dialogue missing");
            return;
        }

        currentNode = currentDialogue.nodes[0];

        ShowCurrentNode();
    }

    public void ShowCurrentNode()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        UIManager.Instance.ShowNode(currentNode);

        TextMeshProUGUI dialogueText =
            UIManager.Instance.npcText;

        typingCoroutine =
            StartCoroutine(
                TypeText(
                    currentNode.text,
                    dialogueText
                )
            );
    }
    IEnumerator TypeText(string text,TextMeshProUGUI dialogueText
)
    {
        isTyping = true;

        fullText = text;

        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;

            yield return new WaitForSeconds(
                typingSpeed
            );
        }

        isTyping = false;
    }
    public void SelectOption(OptionData option)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);

            UIManager.Instance.npcText.text =
                fullText;

            isTyping = false;

            return;
        }

        ApplyEffects(option.effects);

        string nextNodeId = option.nextNodes[0];

        if (nextNodeId == "End_Dialogue")
        {
            if (Counter.Instance.hasActiveRequest)
            {
                Debug.Log("Cannot end dialogue: active request not fulfilled yet.");
                return;
            }

            currentNPCIndex++;
            StartNextNPCDialogue();
            return;
        }

        currentNode =
            currentDialogue.nodes.Find(
                n => n.id == nextNodeId
            );

        if (currentNode == null)
        {
            Debug.LogError("Node not found: " + nextNodeId);
            return;
        }

        ShowCurrentNode();
    }
    void ApplyEffects(List<EffectData> effects)
    {
        foreach (var effect in effects)
        {
            // =========================
            // KEY MOMENTS
            // =========================

            if (!string.IsNullOrEmpty(effect.keyMomentText))
            {
                KeyMomentData moment =
                    new KeyMomentData();

                moment.text =
                    effect.keyMomentText;

                moment.isPositive =
                    effect.keyMomentPositive;

                currentNPC.data.keyMoments.Add(moment);

                UIManager.Instance.StartBlink("FriendshipButton");

                Debug.Log(
                    "Added key moment: "
                    + moment.text
                );
            }

            // =========================
            // MESSAGES
            // =========================

            if (!string.IsNullOrEmpty(effect.messageTitle))
            {
                MessageData message =
                    new MessageData();

                message.title =
                    effect.messageTitle;

                message.sender =
                    effect.messageSender;

                message.content =
                    effect.messageContent;

                message.read = false;

                MessagesManager.Instance.messages
                    .Add(message);

                Debug.Log(
                    "Added message: "
                    + message.title
                );
            }

            // =========================
            // SPECIAL EFFECTS
            // =========================

            switch (effect.variable)
            {
                case "open_id_tab":

                    UIManager.Instance
                        .ToggleIDentification_ID();

                    break;

                case "open_ssi_tab":

                    UIManager.Instance
                        .ToggleIdentification_SSI();

                    string[] parts =
                        effect.value.Split('|');

                    string profile =
                        parts[0];

                    string orders =
                        parts.Length > 1
                        ? parts[1]
                        : "";

                    SSIManager.Instance
                        .SetSSIProfile(profile);

                    SSIManager.Instance
                        .SetupOrders(orders);

                    SSIManager.Instance
                        .SetupResultNodes(
                            effect.perfectNode,
                            effect.goodNode,
                            effect.badNode
                        );

                    break;

                case "unlock_detail":

                    Detail detail =
                        currentNPC.data.details.Find(
                            d => d.key == effect.value
                        );

                    if (detail != null)
                    {
                        detail.unlocked = true;

                        Debug.Log(
                            "Unlocked detail: "
                            + detail.key
                        );
                    }
                    UIManager.Instance.StartBlink("FriendshipButton");

                    break;

                case "open_wares_tab":

                    Counter.Instance.SetupRequestedItems(effect.value);
                    UIManager.Instance.StartBlink("WaresButton");

                    break;

                case "ReceivedCopyDevice":

                    UIManager.Instance.Ref_ID.UnlockCopying();
                    UIManager.Instance.Ref_SSI.UnlockCopying();
                    break;

                case "AddMoney":
                    Counter.Instance.Money += int.Parse(effect.value);
                    UIManager.Instance.UpdateMoney(Counter.Instance.Money);
                    break;

                case "start_blink":
                    UIManager.Instance
                        .StartBlink(effect.value);
                    break;

                case "stop_blink":
                    UIManager.Instance
                        .StopBlink(effect.value);
                    break;
                case "UnlockFavouriteFoods":
                    foreach (NPC npc in NPCManager.Instance.npcs)
                    {
                        if(npc.data.details.Exists(d => d.key == "favorite_items"))
                        {
                            Detail foodDetail = npc.data.details.Find(d => d.key == "favorite_items");
                            foodDetail.unlocked = true;
                            Debug.Log($"Unlocked favourite food for {npc.data.name}");
                        }
                    }
                    break;



                default:

                    // Normal runtime variable
                    if (!string.IsNullOrEmpty(effect.variable))
                    {
                        SetVariable(
    effect.variable,
    effect.value
);
                    }

                    break;
            }
        }
    }
    public bool MeetsRequirements(
    List<RequirementData> requirements)
    {
        if (requirements == null
            || requirements.Count == 0)
        {
            return true;
        }

        foreach (var requirement in requirements)
        {
            string value =
    GetVariable(
        requirement.variable
    );

            if (value != requirement.value)
            {
                return false;
            }
        }

        return true;
    }

    public void SetVariable(
    string variableName,
    string value
)
    {
        // SPECIAL CASE
        if (variableName == "friendship")
        {
            currentNPC.SetVariable(
                variableName,
                value
            );

            return;
        }

        VariableData variable =
            variables.Find(
                v => v.name == variableName
            );

        if (variable != null)
        {
            variable.value = value;
        }
        else
        {
            variables.Add(
                new VariableData
                {
                    name = variableName,
                    value = value
                }
            );
        }
    }

    public string GetVariable(
    string variableName
)
    {
        // SPECIAL CASE
        if (variableName == "friendship")
        {
            return currentNPC.GetVariable(
                variableName
            );
        }

        VariableData variable =
            variables.Find(
                v => v.name == variableName
            );

        return variable != null
            ? variable.value
            : null;
    }

    public IEnumerator InitiateDayWithDelay(int day, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartDay(day);
    }
}