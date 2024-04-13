using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : ManagedBehaviour
{
    public bool CollisionEnabled
    {
        get
        {
            return GetComponent<Collider2D>().enabled;
        }
    }

    public int sortOrder = 0;

    public System.Action<Interactable> CursorEntered { get; set; }
    public System.Action<Interactable> CursorExited { get; set; }
    public System.Action<Interactable> CursorSelectStarted { get; set; }
    public System.Action<Interactable> CursorSelectEnded { get; set; }
    public System.Action<Interactable> CursorAltSelectStarted { get; set; }
    public System.Action<Interactable> CursorAltSelectEnded { get; set; }

    protected bool CursorHovering { get; private set; }

    protected Collider2D Collider => GetComponent<Collider2D>();
    private Collider2D coll2D;

    protected virtual void Start() { }

    public virtual int CompareInteractionSortOrder(Interactable other)
    {
        return other.sortOrder - sortOrder;
    }

    public void SetCollisionEnabled(bool enabled)
    {
        if (enabled && !CanEnable())
        {
            return;
        }

        if (coll2D == null)
        {
            coll2D = GetComponent<Collider2D>();
        }
        if (coll2D != null)
        {
            coll2D.enabled = enabled;
        }

        if (enabled)
        {
            OnInteractionEnabled();
        }
        else
        {
            OnInteractionDisabled();
        }
    }

    public void CursorSelectStart()
    {
        CursorSelectStarted?.Invoke(this);
        OnCursorSelectStart();
    }

    public void CursorSelectEnd()
    {
        CursorSelectEnded?.Invoke(this);
        OnCursorSelectEnd();
    }

    public void CursorAltSelectStart()
    {
        CursorSelectStarted?.Invoke(this);
        OnCursorAltSelectStart();
    }

    public void CursorAltSelectEnd()
    {
        CursorSelectEnded?.Invoke(this);
        OnCursorAltSelectEnd();
    }

    public void CursorEnter()
    {
        CursorEntered?.Invoke(this);
        OnCursorEnter();
    }

    public void CursorStay()
    {
        CursorHovering = true;
        OnCursorStay();
    }

    public void CursorExit()
    {
        CursorExited?.Invoke(this);
        CursorHovering = false;
        OnCursorExit();
    }

    public void CursorDragOff()
    {

    }

    public void CursorScroll(float delta)
    {
        OnCursorScroll(delta);
    }

    public virtual void ClearDelegates() { }

    protected virtual bool CanEnable() { return true; }
    protected virtual void OnInteractionEnabled() { }
    protected virtual void OnInteractionDisabled() { }
    protected virtual void OnCursorEnter() { }
    protected virtual void OnCursorStay() { }
    protected virtual void OnCursorExit() { }
    protected virtual void OnCursorSelectStart() { }
    protected virtual void OnCursorSelectEnd() { }
    protected virtual void OnCursorAltSelectStart() { }
    protected virtual void OnCursorAltSelectEnd() { }
    protected virtual void OnCursorDrag() { }
    protected virtual void OnCursorScroll(float delta) { }
}
