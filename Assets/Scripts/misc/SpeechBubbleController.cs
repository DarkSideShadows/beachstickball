using UnityEngine;
using TMPro;

/* speech bubble for the peanut man */
public class SpeechBubbleController : MonoBehaviour
{
    public GameObject speechBubble;
    public TextMeshProUGUI speechText;

    void Start()
    {
        speechBubble.SetActive(false);
    }

    public void ShowSpeechBubble(string message)
    {
        speechText.text = message;
        speechBubble.SetActive(true);

    }

    public void HideSpeechBubble()
    {
        speechBubble.SetActive(false);
    }
}