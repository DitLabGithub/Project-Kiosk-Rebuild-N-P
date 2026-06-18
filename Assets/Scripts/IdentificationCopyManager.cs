using UnityEngine;

public class IdentificationCopyManager : MonoBehaviour
{
    public static IdentificationCopyManager Instance { get; private set; }
    public int CoppiedID = 0;
    public int CoppiedSSI = 0;

    public int RecordedConversations = 0;   

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void CopyID()
    {
        CoppiedID++;
    }

    public void CopySSI()
    {
        CoppiedSSI++;
    }

    public void RecordConversation()
        {
            RecordedConversations++;
    }
}
