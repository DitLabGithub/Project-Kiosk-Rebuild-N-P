using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOption : MonoBehaviour
{
    public Button button;

    public TMP_Text optionText;

    public Image PositiveImage;
    public Image NegativeImage;

    public void ToggleImage (string type)
    {
        if (type == "positive")
        {
            PositiveImage.gameObject.SetActive(true);
            NegativeImage.gameObject.SetActive(false);
        }
        else if (type == "negative")
        {
            PositiveImage.gameObject.SetActive(false);
            NegativeImage.gameObject.SetActive(true);
        }
        else
        {
            PositiveImage.gameObject.SetActive(false);
            NegativeImage.gameObject.SetActive(false);
        }
    }
}