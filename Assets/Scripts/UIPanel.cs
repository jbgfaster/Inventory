using UnityEngine;

public class UIPanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isVisible = false;
    public bool IsVisible
    {
        get{
            return isVisible;
        }
        private set
        {
            isVisible=value;
        }
    }

    void Awake()
    {
        canvasGroup=GetComponent<CanvasGroup>();
        if(canvasGroup==null)
        {
            canvasGroup=new CanvasGroup();
        }
    }

    public virtual void Show()
    {
        canvasGroup.alpha=1f;
        canvasGroup.blocksRaycasts=true;
        IsVisible=true;
    }

    public virtual void Hide()
    {
        canvasGroup.alpha=0f;
        canvasGroup.blocksRaycasts=false;
        IsVisible=false;
    }
}
