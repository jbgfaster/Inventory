using UnityEngine;
using UnityEngine.Events;

namespace CastingSelectSystem
{
public class Selectable : MonoBehaviour
{
    [SerializeField] private GameObject outline;
    public Material outlineMaterial;
    public bool generateOutline = false;
    private bool isHovered = false;

    private bool isDisable=false;
    public bool IsDisable
    {
        get{
            return isDisable;
        }
        set{
            isDisable=value;
        }
    }

    [SerializeField] private UnityEvent SelectableActionEvent;

    private CastingToObject castingManager;

    void Start()
    {
        castingManager=CastingToObject.Get();
        GenerateAutomaticOutline();
    }

    void Update()
    {
        if(isHovered&&!castingManager.IsBlock&&Input.GetMouseButtonDown(0))    
        {
            OnClickAction();
        }    

        if (outline != null && isHovered != outline.activeSelf)
        {
            outline.SetActive(isHovered);
        }
            
        isHovered = castingManager.IsInRaycast(this);
    }

    private void OnClickAction()
    {
        SelectableActionEvent.Invoke();
    }

    private void GenerateAutomaticOutline()
        {
            if (generateOutline && outlineMaterial != null)
            {
                MeshRenderer[] renders = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer render in renders)
                {
                    GameObject new_outline = Instantiate(render.gameObject, render.transform.position, render.transform.rotation);
                    new_outline.name = "OutlineMesh";
                    new_outline.transform.localScale = render.transform.lossyScale;

                    foreach (MonoBehaviour script in new_outline.GetComponents<MonoBehaviour>())
                        script.enabled = false;

                    MeshRenderer out_render = new_outline.GetComponent<MeshRenderer>();
                    Material[] mats = new Material[out_render.sharedMaterials.Length];
                    for (int i = 0; i < mats.Length; i++)
                        mats[i] = outlineMaterial;
                    out_render.sharedMaterials = mats;
                    out_render.allowOcclusionWhenDynamic = false;
                    out_render.receiveShadows = false;
                    out_render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    if (outline != null)
                    {
                        new_outline.transform.SetParent(outline.transform);
                    }
                    else
                    {
                        new_outline.transform.SetParent(transform);
                        outline = new_outline;
                    }
                }
            }
        }
}
}