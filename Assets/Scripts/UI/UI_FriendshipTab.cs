using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;

public class UI_FriendshipTab : MonoBehaviour
{
    public List<string> friends;
    public string currentFriend;

    [Header("UI Elements")]
    public List<Image> highlightButtonImages;
    public Image Portrait;
    public TextMeshProUGUI Name;
    public Transform DetailsTR;
    public Transform KeyMomentsTR;
    public Image PositiveFriendship;
    public Image NegativeFriendship;
    public GameObject detailAndKeyMomentPrefab;


    public void PopulateFriend(string friend)
    {
        var npc = NPCManager.Instance.GetNPC(friend);

        Name.text = npc.data.name;
        if(npc.data.avatar != null)Portrait.sprite = npc.data.avatar;

        PopulateDetails(npc);
        PopulateKeyMoments(npc);
        PopulateFriendship(npc);
        HighlightButton(friend);

    }

    public void PopulateDetails(NPC npc)
    {
        foreach (Transform child in DetailsTR)
        {
            Destroy(child.gameObject);
        }
        var details = npc.data.details;
        foreach (var detail in details)
        {
            if(detail.unlocked)
            {
                var detailGO = Instantiate(detailAndKeyMomentPrefab, DetailsTR);
                detailGO.GetComponent<TextMeshProUGUI>().text = "- " + detail.text;
                if(detail.key == "ssi_request_guidelines")
                {
                    detailGO.transform.SetAsFirstSibling();
                }
                Canvas.ForceUpdateCanvases();

                LayoutRebuilder.ForceRebuildLayoutImmediate(DetailsTR.GetComponent<RectTransform>());

            }
        }
    }

    public void PopulateKeyMoments(NPC npc)
    {
        foreach (Transform child in KeyMomentsTR)
        {
            Destroy(child.gameObject);
        }
        var keyMoments = npc.data.keyMoments;
        foreach (var keyMoment in keyMoments)
        {  
            var keyMomentGO = Instantiate(detailAndKeyMomentPrefab, KeyMomentsTR);
            keyMomentGO.GetComponent<TextMeshProUGUI>().text = "- " +keyMoment.text;
            if (keyMoment.isPositive)
            {
                keyMomentGO.GetComponent<TextMeshProUGUI>().color = UIManager.Instance.ColorPositive;

            }
            else
            {
                keyMomentGO.GetComponent<TextMeshProUGUI>().color = UIManager.Instance.ColorNegative;
            }
            Canvas.ForceUpdateCanvases();

            LayoutRebuilder.ForceRebuildLayoutImmediate(KeyMomentsTR.GetComponent<RectTransform>());
        }
    }

    public void HighlightButton(string friend)
    {
        foreach (var buttonImage in highlightButtonImages)
        {
            var color = buttonImage.color;
            color.a = 0;
            buttonImage.color = color;
        }
        foreach (var buttonImage in highlightButtonImages)
        {
            if (buttonImage.gameObject.name == friend)
            {
                var color = buttonImage.color;
                color.a = 1;
                buttonImage.color = color;
            }
        }
    }

    public void PopulateFriendship(NPC npc)
    {
        int friendship = int.Parse(npc.GetVariable("friendship"));
        switch (friendship)
        {
            case int n when (n > 0):
                PositiveFriendship.gameObject.SetActive(true);
                NegativeFriendship.gameObject.SetActive(false);
                break;
            case int n when (n < 0):
                PositiveFriendship.gameObject.SetActive(false);
                NegativeFriendship.gameObject.SetActive(true);
                break;
            default:
                PositiveFriendship.gameObject.SetActive(false);
                NegativeFriendship.gameObject.SetActive(false);
                break;
        }
    }
}
