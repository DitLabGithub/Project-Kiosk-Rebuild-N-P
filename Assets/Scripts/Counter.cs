using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public static Counter Instance { get; private set; }
    public int Money;
    public int MoneyGoal;
    public RectTransform itemContainer;
    public TextMeshProUGUI requestedItemsText;
    public List<GameObject> ItemPrefabs;
    public List<Item> requestedItems = new List<Item>();
    public List<Item> itemsInCart = new List<Item>();

    public bool hasActiveRequest = false;

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

    public void AddItem(GameObject GO)
    {
        Item item = GO.GetComponent<Item>();

        foreach (var itemPrefab in ItemPrefabs)
        {
            Item prefabItem =
                itemPrefab.GetComponent<Item>();

            if (prefabItem.itemName == item.itemName)
            {
                GameObject newItem =
                    Instantiate(itemPrefab, itemContainer);

                Item newItemComponent =
                    newItem.GetComponent<Item>();

                itemsInCart.Add(newItemComponent);

                newItem.GetComponent<Button>()
                    .onClick.AddListener(() =>
                    RemoveItem(newItem));

                Debug.Log(
                    "Added item to cart: "
                    + newItemComponent.itemName
                );

                break;
            }
        }
        UIManager.Instance.UpdateCartList();
        CheckForSaleButton();
    }
    public void RemoveItem(GameObject item)
    {
        itemsInCart.Remove(item.GetComponent<Item>());
        Destroy(item);
        UIManager.Instance.HideItemHover();
        UIManager.Instance.UpdateCartList();
        CheckForSaleButton();
    }

    public void Clear()
    {
        itemsInCart.Clear();
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
        CheckForSaleButton();
    }

    public void Sell()
    {
        if (itemsInCart.Count == 0)
        {
            Debug.Log("No items in cart to sell.");
            return;
        }
            string requestedItemNames = " " + string.Join(", ", requestedItems.Select(i => i.itemName));
        int upsoldThisTransaction = 0;

        // VALIDATION PASS

        Dictionary<string, int> requestedCounts =
            new Dictionary<string, int>();

        Dictionary<string, int> cartCounts =
            new Dictionary<string, int>();

        // Count requested items
        foreach (var requestedItem in requestedItems)
        {
            if (!requestedCounts.ContainsKey(requestedItem.itemName))
            {
                requestedCounts[requestedItem.itemName] = 0;
            }

            requestedCounts[requestedItem.itemName]++;
        }

        // Count cart items
        foreach (var item in itemsInCart)
        {
            if (!cartCounts.ContainsKey(item.itemName))
            {
                cartCounts[item.itemName] = 0;
            }

            cartCounts[item.itemName]++;
        }

        // 1. Validate exact requested item quantities
        foreach (var pair in requestedCounts)
        {
            string itemName = pair.Key;
            int requiredAmount = pair.Value;

            int cartAmount =
                cartCounts.ContainsKey(itemName)
                ? cartCounts[itemName]
                : 0;

            if (cartAmount != requiredAmount)
            {
                UIManager.Instance.npcText.text =
                    "I didn't ask for this, I asked for"
                    + requestedItemNames + ".";
                return;
            }
        }

        // 2. Validate all cart items
        foreach (var pair in cartCounts)
        {
            string itemName = pair.Key;
            int amount = pair.Value;

            bool requested =
                requestedCounts.ContainsKey(itemName);

            bool favourite =
                DialogueManager.Instance.currentNPC
                .data.favouriteItems
                .Contains(itemName);

            // Item is neither requested nor favourite
            if (!requested && !favourite)
            {
                UIManager.Instance.npcText.text =
                    "I didn't ask for this, I asked for"
                    + requestedItemNames + ".";

                return;
            }

            // Requested items must EXACTLY match quantity
            if (requested)
            {
                int requiredAmount =
                    requestedCounts[itemName];

                if (amount != requiredAmount)
                {
                    UIManager.Instance.npcText.text =
                    "I didn't ask for this, I asked for"
                    + requestedItemNames + ".";

                    return;
                }
            }

            // Favourite items can only appear once
            // BUT ONLY if they are NOT also requested
            if (favourite && !requested)
            {
                if (amount > 1)
                {
                    UIManager.Instance.npcText.text =
                    "I didn't ask for this, I asked for"
                    + requestedItemNames + ".";
                    return;
                }
            }
        }

        // SELL PASS
        foreach (var item in itemsInCart)
        {
            bool requested =
                requestedItems.Exists(r => r.itemName == item.itemName);

            bool favourite =
                DialogueManager.Instance.currentNPC
                .data.favouriteItems
                .Contains(item.itemName);

            Money += item.price;

            string baseName = item.itemName.Replace(" ", "");

            // NORMAL SALE
            if (requested)
            {
                Debug.Log("Sold requested item: " + item.itemName);

                StartCoroutine(RegisterSale("Sold", baseName));
            }
            // UPSELL SALE
            else if (favourite)
            {
                upsoldThisTransaction++;

                Debug.Log("Upsold item: " + item.itemName);

                StartCoroutine(RegisterSale("Upsold", baseName));
            }
        }

        // SAVE GLOBAL UPSOLD COUNT
        if (upsoldThisTransaction > 0)
        {
            int currentUpsold =
                int.Parse(
                    DialogueManager.Instance.currentNPC
                    .GetVariable("UpsoldNumber")
                );

            currentUpsold += upsoldThisTransaction;

            DialogueManager.Instance.currentNPC
                .SetVariable("UpsoldNumber", currentUpsold.ToString());
        }

        Clear();
        DialogueManager.Instance.ShowCurrentNode();
        UIManager.Instance.UpdateMoney(Money);
        requestedItems.Clear();
        hasActiveRequest = false;
        CheckForSaleButton();
        Debug.Log("Transaction successful");
    }
    public void SetupRequestedItems(string items)
    {
        requestedItems.Clear();

        string[] splitItems = items.Split(',');

        foreach (string itemName in splitItems)
        {
            string trimmedName = itemName.Trim();

            foreach (GameObject prefab in ItemPrefabs)
            {
                Item item = prefab.GetComponent<Item>();

                if (item.itemName == trimmedName)
                {
                    requestedItems.Add(item);

                    Debug.Log(
                        "Requested item added: "
                        + item.itemName
                    );

                    break;
                }
            }
        }
        if (requestedItems.Count > 0)
        {
            hasActiveRequest = true;
            requestedItemsText.text = "";
            foreach (var item in requestedItems)
            {
                requestedItemsText.text += item.itemName + "\n";
            }
        }
        CheckForSaleButton();
    }
    public void CheckForSaleButton()
    {
        bool shouldShow =
            hasActiveRequest &&
            requestedItems.Count > 0;

        UIManager.Instance.Ref_SellButton.SetActive(shouldShow);

        // No active request → hard reset UI blinking
        if (!shouldShow)
        {
            requestedItemsText.text = "";

            UIManager.Instance.StopBlink("SellButton");
            UIManager.Instance.StopBlink("WaresButton");

            return;
        }

        // Active request exists → show requested items UI
        if (requestedItemsText != null)
        {
            requestedItemsText.text = "";

            foreach (var item in requestedItems)
            {
                requestedItemsText.text += item.itemName + "\n";
            }
        }

        // CASE 1: Player has items in cart → focus Sell button
        if (itemsInCart.Count > 0)
        {
            UIManager.Instance.StartBlink("SellButton");
            UIManager.Instance.StopBlink("WaresButton");
        }
        // CASE 2: Cart empty but request exists → focus Wares button
        else
        {
            UIManager.Instance.StopBlink("SellButton");
            UIManager.Instance.StartBlink("WaresButton");
        }
    }
    private IEnumerator RegisterSale(string prefix, string baseName)
    {
        int index = 1;

        string variableName = prefix + baseName + index;

        var npc = DialogueManager.Instance.currentNPC;

        while (npc.HasVariable(variableName))
        {
            index++;
            variableName = prefix + baseName + index;
        }

        npc.SetVariable(variableName, "true");

        UIManager.Instance.StopBlink("SellButton");
        UIManager.Instance.StopBlink("WaresButton");
        UIManager.Instance.StartBlink("Money");
        yield return new WaitForSeconds(2f);
        UIManager.Instance.StopBlink("Money");
    }


}