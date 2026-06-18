using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MessagesManager : MonoBehaviour
{
    public static MessagesManager Instance { get; private set; }

    public List<MessageData> messages = new List<MessageData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
