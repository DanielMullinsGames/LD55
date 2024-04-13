using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingSprite : ManagedBehaviour
{
    [SerializeField]
    private SpriteRenderer tilingRenderer = default;

    [SerializeField]
    private Vector2 scrollSpeed = default;

    public override void ManagedUpdate()
    {
        // The Scrolling Sprite shader uses a color for the scrollspeed so...
        tilingRenderer.material.SetColor("_ScrollSpeed", new Color(scrollSpeed.x, scrollSpeed.y, 0f, 0f));
    }
}
