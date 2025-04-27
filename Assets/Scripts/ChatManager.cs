using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public GameObject messagePrefab;
    public Transform contentTransform;
    public TMP_InputField inputField;
    public ScrollRect scrollRect; // Added ScrollRect reference
    
    private float contentHeight = 0; // Track content height

    public void SendMessageToChat()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            // Reset content size before adding
            ((RectTransform)contentTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            
            // Create message
            GameObject newMessage = Instantiate(messagePrefab, contentTransform);
            newMessage.GetComponent<TMP_Text>().text = inputField.text;
            
            // Position message at the bottom
            RectTransform messageRect = newMessage.GetComponent<RectTransform>();
            messageRect.anchoredPosition = new Vector2(0, -contentHeight);
            
            // Force layout rebuild to get correct sizes
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageRect);
            
            // Update total height
            contentHeight += messageRect.sizeDelta.y;
            
            // Update content size
            ((RectTransform)contentTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
            
            // Scroll to bottom
            scrollRect.verticalNormalizedPosition = 0;
            
            // Clear input
            inputField.text = "";
        }
    }
}