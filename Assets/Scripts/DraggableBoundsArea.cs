using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBoundsArea : ManagedBehaviour
{
    public enum OutOfBoundsBehaviourType
    {
        None,
        TweenReturn,
        LockedWithin,
    }
    public Bounds Bounds => new Bounds(transform.position, extents);
    public OutOfBoundsBehaviourType OutOfBoundsBehaviour => outOfBoundsBehaviour;

    [SerializeField]
    private OutOfBoundsBehaviourType outOfBoundsBehaviour = default;

    [SerializeField]
    protected Vector2 extents = default;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, extents);
    }
}