using UnityEngine;
using TMPro;

public class SpeechBubbleController : MonoBehaviour
{
    public GameObject speechBubble;
    public TextMeshProUGUI speechText;

    void Start()
    {
        speechBubble.SetActive(false); // initially hide the speech bubble
    }

    public void ShowSpeechBubble(string message)
    {
        speechText.text = message; // set the speech bubble text
        speechBubble.SetActive(true); // show the speech bubble
    }

    public void HideSpeechBubble()
    {
        speechBubble.SetActive(false);
    }
}