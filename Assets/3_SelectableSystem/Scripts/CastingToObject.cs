using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CastingSelectSystem
{
public class CastingToObject : MonoBehaviour
{
    [SerializeField] private LayerMask selectable_layer = ~0;

    private HashSet<Selectable> raycast_list = new HashSet<Selectable>();
    
    private bool isBlock;

    public bool IsBlock
    {
        get{
            return isBlock;
        }
        set{
            isBlock = value;
        }
    }

    private static CastingToObject instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        RaycastSelectables();
    }

    public void RaycastSelectables()
    {
        raycast_list.Clear();

        if(IsMouseOverUI())
        {
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 99f, selectable_layer.value);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                Selectable select = hit.collider.GetComponentInParent<Selectable>();
                if (select != null&&!select.IsDisable)
                    raycast_list.Add(select);
            }
        }
    }

    public bool IsInRaycast(Selectable select)
    {
        return raycast_list.Contains(select);
    }

    public bool IsMouseOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = GetMousePosition();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public Vector2 GetMousePosition()
    {
        Pointer input = GetInput();
        if (input != null)
            return input.position.ReadValue();
        return Vector2.zero;
    }

    public Pointer GetInput()
    {
        return Mouse.current;
    }

    public static CastingToObject Get()
    {
        return instance;
    }
}
}