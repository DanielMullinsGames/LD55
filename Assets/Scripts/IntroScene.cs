using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : ManagedBehaviour
{
    public float scrollSpeed;
    public float endSceneY;

    public override void ManagedUpdate()
    {
        transform.position += Vector3.up * Time.deltaTime * scrollSpeed;

        if (transform.position.y > endSceneY)
        {
            enabled = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}
