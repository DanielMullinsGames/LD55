using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : ManagedBehaviour
{
    public float scrollSpeed;
    public float endSceneY;

    AudioSource musicSource;

    private void Start()
    {
        musicSource = AudioController.Instance.PlaySound2D("intromusic", 0.75f);
    }

    public override void ManagedUpdate()
    {
        transform.position += Vector3.up * Time.deltaTime * scrollSpeed;

        if (transform.position.y > endSceneY)
        {
            enabled = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Story_1");
            AudioController.Instance.FadeSourceVolume(musicSource, 0f, 1f);
        }
    }
}
