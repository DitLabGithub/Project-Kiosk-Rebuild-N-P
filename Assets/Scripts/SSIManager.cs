using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class SSIManager : MonoBehaviour
{
    public static SSIManager Instance;
    public Image photo;
    public TextMeshProUGUI name;
    public TextMeshProUGUI age;
    public TextMeshProUGUI above18;
    public TextMeshProUGUI above21;
    public TextMeshProUGUI address;
    public TextMeshProUGUI expiryDate;
    public GameObject buttonToHideForPhoto;
    public GameObject SSICopyButton;


    private HashSet<string> askedFields = new HashSet<string>();
    private HashSet<string> requiredFields = new HashSet<string>();

    private string perfectNode;
    private string goodNode;
    private string badNode;

    private CustomSSIData currentSSI;

    private bool usingOwnProfile;

    private void Awake()
    {
        Instance = this;
    }

    public void SetSSIProfile(
    string profileName
)
    {
        usingOwnProfile =
            profileName == "own";

        currentSSI = null;

        if (usingOwnProfile)
        {
            return;
        }

        var profile =
            DialogueManager
            .Instance
            .currentNPC
            .data
            .ssiProfiles
            .Find(
                s =>
                s.key
                ==
                profileName
            );

        if (profile != null)
        {
            currentSSI =
                profile.data;
        }
        else
        {
            Debug.LogWarning(
                "SSI profile missing: "
                + profileName
            );
        }
    }
    public void SetupOrders(string orderString)
    {
        ResetSSI();

        requiredFields.Clear();

        string[] orders = orderString.Split(',');

        foreach (string order in orders)
        {
            string o = order.Trim().ToLower();

            if (o == "package")
            {
                requiredFields.Add("photo");
                requiredFields.Add("name");
                requiredFields.Add("address");
            }
            else if (o == "alcohol")
            {
                requiredFields.Add("above18");
                requiredFields.Add("photo");
                requiredFields.Add("expiryDate");
            }
            else if (o == "food")
            {
                requiredFields.Add("expiryDate");
            }
        }

        askedFields.Clear();
        if (DialogueManager.Instance.variables.Find(v => v.name == "AcceptedSSIDataOffer_D3")?.value == "true" || DialogueManager.Instance.variables.Find(v => v.name == "CommittedToSSIDataScheme_D4")?.value == "true")
        {
            UnlockCopying();
        }
    }

    public void Evaluate()
    {
        int correct = 0;

        // Count required fields asked
        foreach (var field in requiredFields)
        {
            if (askedFields.Contains(field))
                correct++;
        }

        bool missedRequired = correct < requiredFields.Count;

        bool askedExtra = false;

        foreach (var field in askedFields)
        {
            if (!requiredFields.Contains(field))
            {
                askedExtra = true;
                break;
            }
        }

        string result;

        if (missedRequired)
        {
            result = "bad";
        }
        else if (askedExtra)
        {
            result = "good";
        }
        else
        {
            result = "perfect";
        }

        Debug.Log("SSI Result: " + result);

        DialogueManager.Instance.currentNode =
    DialogueManager.Instance.currentDialogue.nodes.Find(
        n => n.id == (result == "perfect" ? perfectNode : result == "good" ? goodNode : badNode)
    );

        // Store in NPC
        DialogueManager.Instance.currentNPC.SetVariable(
     "SSI_Result",
     result);
    }

    public void FinishSSI()
    {
        Evaluate();

        UIManager.Instance.ToggleIdentification_SSI();

        DialogueManager.Instance.ShowCurrentNode();
    }


    public void RevealName()
    {
        name.text =
            usingOwnProfile
            ? DialogueManager
                .Instance
                .currentNPC
                .data
                .name
            : currentSSI.ownerName;

        askedFields.Add("name");
    }

    public void RevealAge()
    {
        age.text =
            (
                usingOwnProfile
                ? DialogueManager
                    .Instance
                    .currentNPC
                    .data
                    .age
                : currentSSI.age
            ).ToString();

        askedFields.Add("age");
    }

    public void RevealAbove18()
    {
        int a =
            usingOwnProfile
            ? DialogueManager
                .Instance
                .currentNPC
                .data
                .age
            : currentSSI.age;

        above18.text =
            a >= 18
            ? "Yes"
            : "No";

        askedFields.Add(
            "above18"
        );
    }

    public void RevealAbove21()
    {
        int a =
            usingOwnProfile
            ? DialogueManager
                .Instance
                .currentNPC
                .data
                .age
            : currentSSI.age;

        above21.text =
            a >= 21
            ? "Yes"
            : "No";

        askedFields.Add(
            "above21"
        );
    }

    public void RevealAddress()
    {
        address.text =
            usingOwnProfile
            ? DialogueManager
                .Instance
                .currentNPC
                .data
                .address
            : currentSSI.address;

        askedFields.Add(
            "address"
        );
    }

    public void RevealExpiryDate()
    {
        expiryDate.text =
            usingOwnProfile
            ? DialogueManager
                .Instance
                .currentNPC
                .data
                .expiryDate
            : currentSSI.expiryDate;

        askedFields.Add(
            "expiryDate"
        );
    }

    public void RevealPhoto()
    {
        photo.sprite =
            usingOwnProfile
            ? DialogueManager
                .Instance
                .currentNPC
                .data
                .avatar
            : currentSSI.avatar;

        buttonToHideForPhoto
            .SetActive(false);

        askedFields.Add(
            "photo"
        );
    }

    public void SetupResultNodes(
    string perfect,
    string good,
    string bad
)
    {
        perfectNode = perfect;
        goodNode = good;
        badNode = bad;
    }

    public void ResetSSI()
    {
        askedFields.Clear();

        photo.sprite = null;

        name.text = "Request";
        age.text = "Request";
        above18.text = "Request";
        above21.text = "Request";
        address.text = "Request";
        expiryDate.text = "Request";

        buttonToHideForPhoto.SetActive(true);
    }

    public void UnlockCopying()
    {
        SSICopyButton.SetActive(true);
            SSICopyButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                IdentificationCopyManager.Instance.CopySSI();
                Debug.Log("SSI copied");
            });
    }
}