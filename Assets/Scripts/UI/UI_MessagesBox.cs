using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MessagesBox : MonoBehaviour
{
    public Transform Container;
    public GameObject MessageEntryPrefab;

    public GameObject MessageFull;
    public TextMeshProUGUI MessageFullTitle;
    public TextMeshProUGUI MessageFullContent;
    public TextMeshProUGUI MessageFullSender;

    public void PopulateMessages()
    {
        CleanContainer();
        foreach (MessageData message in MessagesManager.Instance.messages)
        {
            GameObject entry = Instantiate(MessageEntryPrefab, Container);
            UI_MessageBoxEntry entryScript = entry.GetComponent<UI_MessageBoxEntry>();
            entryScript.SetMessage(message);
            entry.GetComponent<Button>().onClick.AddListener(() => ShowMessageFull(message));
        }
    }

    public void ShowMessageFull(MessageData message)
    {
        CleanContainer();
        MessageFull.SetActive(true);
        MessageFullTitle.text = message.title;
        MessageFullContent.text = message.content;
        MessageFullSender.text = message.sender;
        message.read = true;
    }

    public void CleanContainer()
    {
        MessageFull.SetActive(false);
        foreach (Transform child in Container)
        {
            Destroy(child.gameObject);
        }
    }
}
