using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class GameCursor : ManagedBehaviour
{
    public static GameCursor instance;

    public Interactable CurrentInteractable { get; private set; }
    private Interactable cursorDownInteractable;

    public ReferenceSetToggle DisableMovement = new ReferenceSetToggle();
    public ReferenceSetToggle DisableInput = new ReferenceSetToggle();

    [SerializeField]
    private SpriteRenderer cursorRenderer = default;

    [SerializeField]
    private float defaultCursorScale = default;

    [SerializeField]
    private float pressedCursorScale = default;

    private List<string> excludedLayers = new();

    protected override void ManagedInitialize()
    {
        instance = this;
    }

    public override void ManagedUpdate()
    {
        UpdateMainInput();
        UpdateDragInput();
        UpdateVisuals();
    }

    private void UpdateMainInput()
    {
        CurrentInteractable = UpdateCurrentInteractable(CurrentInteractable, excludedLayers.ToArray());

        if (!DisableInput.True)
        {
            if (CurrentInteractable != null)
            {
                CurrentInteractable.CursorStay();
            }

            if (CurrentInteractable != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentInteractable.CursorSelectStart();
                    cursorDownInteractable = CurrentInteractable;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    CurrentInteractable.CursorSelectEnd();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    CurrentInteractable.CursorAltSelectStart();
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    CurrentInteractable.CursorAltSelectEnd();
                }
                else if (Input.mouseScrollDelta.magnitude != 0f)
                {
                    CurrentInteractable.CursorScroll(Input.mouseScrollDelta.magnitude);
                }
            }
        }
    }

    private void UpdateDragInput()
    {
        if (cursorDownInteractable != null && !DisableInput.True)
        {
            if (CurrentInteractable != cursorDownInteractable)
            {
                cursorDownInteractable.CursorDragOff();
                cursorDownInteractable = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            cursorDownInteractable = null;
        }
    }

    private Interactable UpdateCurrentInteractable(Interactable current, string[] excludeLayers)
    {
        var hitInteractable = RaycastForInteractable(~LayerMask.GetMask(excludeLayers), transform.position);

        if (hitInteractable != current)
        {
            if (current != null)
            {
                if (current.CollisionEnabled)
                {
                    current.CursorExit();
                }
            }

            if (hitInteractable != null && !DisableInput.True)
            {
                hitInteractable.CursorEnter();
            }
            else
            {
                return null;
            }
        }

        return hitInteractable;
    }

    private void UpdateVisuals()
    {
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(cursorPos.x, cursorPos.y, Camera.main.nearClipPlane);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        if (Input.GetMouseButtonUp(0))
        {
            Tween.LocalScale(cursorRenderer.transform, Vector2.one * defaultCursorScale, 0.05f, 0f, Tween.EaseInOut);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Tween.LocalScale(cursorRenderer.transform, Vector2.one * pressedCursorScale, 0.05f, 0f, Tween.EaseInOut);
        }
    }

    private Interactable RaycastForInteractable(int layerMask, Vector3 cursorPosition)
    {
        Interactable hitInteractable = null;

        var rayHits = Physics2D.RaycastAll(cursorPosition, Vector2.zero, 1000f, layerMask);
        var hitInteractables = GetInteractablesFromRayHits(rayHits);

        if (hitInteractables.Count > 0)
        {
            hitInteractables.Sort((Interactable a, Interactable b) =>
            {
                return a.CompareInteractionSortOrder(b);
            });
            hitInteractable = hitInteractables[0];
        }

        return hitInteractable;
    }

    private List<Interactable> GetInteractablesFromRayHits(RaycastHit2D[] rayHits)
    {
        var hitInteractables = new List<Interactable>();
        for (int i = 0; i < rayHits.Length; i++)
        {
            var interactable = rayHits[i].transform.GetComponent<Interactable>();
            if (interactable != null)
            {
                hitInteractables.Add(interactable);
            }
        }
        return hitInteractables;
    }
}
