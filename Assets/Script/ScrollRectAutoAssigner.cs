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
            // Viewport �̉��ɂ��� Content �������擾
            Transform viewport = transform.Find("Viewport");
            if (viewport != null)
            {
                Transform content = viewport.Find("Content");
                if (content != null)
                {
                    scrollRect.content = content.GetComponent<RectTransform>();
                    Debug.Log("[ScrollRectAutoAssigner] Content ���������蓖�Ă��܂���: " + content.name);
                }
                else
                {
                    Debug.LogWarning("[ScrollRectAutoAssigner] Viewport ���� Content ��������܂���ł���");
                }
            }
            else
            {
                Debug.LogWarning("[ScrollRectAutoAssigner] Viewport ��������܂���ł���");
            }
        }
    }
}