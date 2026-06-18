using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Item : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public string itemName;
    public int price;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!UIManager.Instance.Ref_ItemHover.gameObject.activeSelf)
        {
            UIManager.Instance.ShowItemHover(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideItemHover();
    }

    public void AddToCart()
    {
        Counter.Instance.AddItem(gameObject);
    }

    public void RemoveFromCart()
    {
        Counter.Instance.RemoveItem(gameObject);

    }

}