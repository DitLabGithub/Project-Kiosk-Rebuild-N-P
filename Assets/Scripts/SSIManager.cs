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
    public TextMeshProUGUI valid;
    public TextMeshProUGUI address;
    public TextMeshProUGUI personalID;
    public TextMeshProUGUI authorized;
    public GameObject buttonToHideForPhoto;
    public GameObject SSICopyButton;


    private HashSet<string> askedFields = new HashSet<string>();
    private HashSet<string> requiredFields = new HashSet<string>();

    private string perfectNode;
    private string moreNode;
    private string lessNode;

    public CustomSSIData currentSSI;


    private void Awake()
    {
        Instance = this;
    }

    public void SetSSIProfile(string profileName)
    {
        currentSSI = null;

        var profile =
            DialogueManager
            .Instance
            .currentNPC
            .data
            .ssiProfiles
            .Find(
                s => s.key == profileName
            );

        if (profile != null)
        {
            currentSSI = profile.data;
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
                requiredFields.Add("valid");
            }
            else if (o == "alcohol")
            {
                requiredFields.Add("above18");
                requiredFields.Add("photo");
                requiredFields.Add("valid");

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
            result = "less";
        }
        else if (askedExtra)
        {
            result = "more";
        }
        else
        {
            result = "perfect";
        }

        Debug.Log("SSI Result: " + result);

        DialogueManager.Instance.currentNode =
    DialogueManager.Instance.currentDialogue.nodes.Find(
        n => n.id == (
            result == "perfect"
                ? perfectNode
                : result == "more"
                    ? moreNode
                    : lessNode
        )
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
        name.text = currentSSI.ownerName;

        askedFields.Add("name");
    }

    public void RevealAge()
    {
        age.text =
            currentSSI.age.ToString();

        askedFields.Add("age");
    }

    public void RevealAbove18()
    {
        above18.text =
            currentSSI.age >= 18
            ? "Yes"
            : "No";

        askedFields.Add("above18");
    }

    public void RevealValid()
    {
        valid.text =
            currentSSI.valid ? "Yes" : "No";

        askedFields.Add("valid");
    }

    public void RevealAddress()
    {
        address.text =
            currentSSI.address;

        askedFields.Add("address");
    }

    public void RevealPersonalID()
    {
        personalID.text =
            currentSSI.personalID;

        askedFields.Add("personalID");
    }

    public void RevealPhoto()
    {
        photo.sprite =
            currentSSI.avatar;

        buttonToHideForPhoto
            .SetActive(false);

        askedFields.Add("photo");
    }

    public void RevealAuthorized()
    {
        authorized.text =
            string.Join(", ", currentSSI.authorizedRepresentatives);
        if(authorized.text == "")
        {
            authorized.text = "None";
        }   
    }

    public void SetupResultNodes(
    string perfect,
    string more,
    string less
)
    {
        perfectNode = perfect;
        moreNode = more;
        lessNode = less;
    }

    public void ResetSSI()
    {
        askedFields.Clear();

        photo.sprite = null;

        name.text = "Request";
        age.text = "Request";
        above18.text = "Request";
        valid.text = "Request";
        address.text = "Request";
        personalID.text = "Request";

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