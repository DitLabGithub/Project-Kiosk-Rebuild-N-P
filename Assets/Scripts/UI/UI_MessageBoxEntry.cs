using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MessageBoxEntry : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Sender;
    public Image Icon;
    public MessageData Data;

    public void SetMessage(MessageData data)
    {
        Data = data;
        Title.text = Data.title;
        Sender.text = Data.sender;
        if (Data.read)
        {
            Icon.color = UIManager.Instance.ColorRead;
        }
        else
        {
            Icon.color = UIManager.Instance.ColorPositive;
        }
    }
}
