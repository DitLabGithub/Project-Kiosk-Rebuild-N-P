using UnityEngine;

public class UI_CartList : MonoBehaviour
{
    public static UI_CartList instance;
    public GameObject EntryPrefab;
    public Transform Container;

    private void Awake()
    {
        instance = this;
    }
    public void PopulateList()
    {
        CleanList();
        if (EntryPrefab != null)
        {
            foreach (Item item in Counter.Instance.itemsInCart)
            {
                var entry = Instantiate(EntryPrefab, Container);
                UI_ItemHover script = entry.GetComponent<UI_ItemHover>();
                script.SetCartEntry(item);
            }
        }
    }
    public void CleanList()
    {
        foreach (Transform child in Container.transform) 
        {
            Destroy(child.gameObject);  
        }
    }
}
