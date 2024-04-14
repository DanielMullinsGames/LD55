using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : ManagedBehaviour
{
    public static SceneTransition instance;

    [SerializeField]
    private SpriteMask mask = default;

    protected override void ManagedInitialize()
    {
        instance = this;
        Transition(true);
    }

    public void Transition(bool transitionIn)
    {
        StopAllCoroutines();
        if (transitionIn)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        while (mask.alphaCutoff < 1f)
        {
            mask.alphaCutoff += Time.deltaTime * 2f;
            yield return new WaitForEndOfFrame();
        }
        mask.alphaCutoff = 1f;
    }

    private IEnumerator FadeIn()
    {
        mask.alphaCutoff = 1f;
        while (mask.alphaCutoff > 0f)
        {
            mask.alphaCutoff -= Time.deltaTime * 2f;
            yield return new WaitForEndOfFrame();
        }
        mask.alphaCutoff = 0f;
    }
}
