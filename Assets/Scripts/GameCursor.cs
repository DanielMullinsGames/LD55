using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class GameCursor : ManagedBehaviour
{
    [SerializeField]
    private SpriteRenderer cursorRenderer = default;

    [SerializeField]
    private float defaultCursorScale = default;

    [SerializeField]
    private float pressedCursorScale = default;

    public override void ManagedUpdate()
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
}
