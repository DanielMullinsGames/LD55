using Pixelplacement;
using Sirenix.OdinInspector;
using UnityEngine;

public class DraggableInteractable : Interactable
{
    public static DraggableInteractable CurrentDraggable { get; private set; }
    public static System.Action<DraggableInteractable> CurrentDraggableStarted;
    public static System.Action<DraggableInteractable> CurrentDraggableEnded;

    public System.Action DragStarted;
    public System.Action DragEnded;

    public bool BeingDragged => CurrentDraggable == this;
    public bool DraggingDisabled { get; set; }

    protected enum State
    {
        Idle,
        Dragging,
    }

    [Title("Draggable")]
    [SerializeField]
    private bool snapToCursorCenter = false;

    [SerializeField]
    private DraggableBoundsArea boundsArea = default;

    protected State DragState { get; private set; }

    private Vector2 dragStartPos;
    private Vector2 dragStartCursorPos;

    public static void ResetStatics()
    {
        CurrentDraggable = null;
        CurrentDraggableStarted = CurrentDraggableEnded = null;
    }

    protected override void OnCursorSelectStart()
    {
        if (!DraggingDisabled)
        {
            StartDrag();
        }
    }

    public override void ManagedUpdate()
    {
        switch (DragState)
        {
            case State.Dragging:
                if (snapToCursorCenter)
                {
                    transform.position = PixelSnapElement.RoundToPixel(GameCursor.instance.transform.position);
                }
                else
                {
                    transform.position = PixelSnapElement.RoundToPixel(dragStartPos + ((Vector2)GameCursor.instance.transform.position - dragStartCursorPos));
                }

                if (InteractionEnded())
                {
                    EndDrag();
                }
                break;
        }

        UpdateOutOfBoundsBehaviour();
    }

    public void SetBoundsArea(DraggableBoundsArea boundsArea)
    {
        this.boundsArea = boundsArea;
    }

    protected virtual void OnDragStart() { }
    protected virtual void OnDragEnd() { }
    protected virtual void OnResisting(float distFromOrigin, Vector2 resistStartCursorPos) { }
    protected virtual void OnResistEnd(bool reachedFullExtent, Vector2 resistStartCursorPos) { }

    private bool InteractionEnded()
    {
        return Input.GetMouseButtonUp(0) || GameCursor.instance.DisableInput.True || !CollisionEnabled;
    }

    private void StartDrag()
    {
        CurrentDraggable = this;
        CurrentDraggableStarted?.Invoke(this);

        DragState = State.Dragging;
        dragStartPos = transform.position;
        dragStartCursorPos = GameCursor.instance.transform.position;
        Tween.Stop(transform.GetInstanceID());

        OnDragStart();
        DragStarted?.Invoke();
    }

    protected void EndDrag()
    {
        CurrentDraggableEnded?.Invoke(CurrentDraggable);
        CurrentDraggable = null;

        DragState = State.Idle;
        var pos = GetPositionForBounds();
        if (boundsArea != null && !boundsArea.Bounds.Contains(pos) && boundsArea.OutOfBoundsBehaviour == DraggableBoundsArea.OutOfBoundsBehaviourType.TweenReturn)
        {
            Tween.Position(transform,
                PixelSnapElement.RoundToPixel((Vector2)boundsArea.Bounds.ClosestPoint(pos) + GetColliderOffset()),
                0.5f, 0f, Tween.EaseOutStrong);
        }

        OnDragEnd();
        DragEnded?.Invoke();
    }

    private void UpdateOutOfBoundsBehaviour()
    {
        if (boundsArea != null)
        {
            var bounds = boundsArea.Bounds;
            if (!bounds.Contains(transform.position))
            {
                switch (boundsArea.OutOfBoundsBehaviour)
                {
                    // NOTE: This seems to not work properly due to Collider.bounds.center not updating at the same rate as the Transform's position
                    // HACK: If Collider.bounds.offset is 0, just use transform.position. Will become an issue when we want to use this behaviour with
                    // an offset collider...
                    case DraggableBoundsArea.OutOfBoundsBehaviourType.LockedWithin:
                        var pos = GetPositionForBounds();
                        float clampedX = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
                        float clampedY = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);
                        transform.position = PixelSnapElement.RoundToPixel(new Vector2(clampedX, clampedY) + GetColliderOffset());
                        break;
                }
            }
        }
    }

    private Vector2 GetPositionForBounds()
    {
        if (Collider.offset.magnitude == 0f)
        {
            return transform.position;
        }
        else
        {
            return Collider.bounds.center;
        }
    }

    private Vector2 GetColliderOffset()
    {
        if (Collider.offset.magnitude == 0f)
        {
            return Vector2.zero;
        }
        else
        {
            return transform.position - Collider.bounds.center;
        }
    }
}
