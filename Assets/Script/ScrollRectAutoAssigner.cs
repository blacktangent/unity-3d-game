using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoAssigner : MonoBehaviour
{
    void Start()
    {
        ScrollRect scrollRect = GetComponent<ScrollRect>();

        if (scrollRect.content == null)
        {
            // Viewport ‚Ì‰º‚É‚ ‚é Content ‚ðŽ©“®Žæ“¾
            Transform viewport = transform.Find("Viewport");
            if (viewport != null)
            {
                Transform content = viewport.Find("Content");
                if (content != null)
                {
                    scrollRect.content = content.GetComponent<RectTransform>();
                    Debug.Log("[ScrollRectAutoAssigner] Content ‚ðŽ©“®Š„‚è“–‚Ä‚µ‚Ü‚µ‚½: " + content.name);
                }
                else
                {
                    Debug.LogWarning("[ScrollRectAutoAssigner] Viewport ‰º‚É Content ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½");
                }
            }
            else
            {
                Debug.LogWarning("[ScrollRectAutoAssigner] Viewport ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½");
            }
        }
    }
}