using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryScene : ManagedBehaviour
{
    [SerializeField]
    private TMPro.TextMeshPro text = default;

    public List<string> lines = new();

    private void Start()
    {
        StartCoroutine(StorySequence());
        text.gameObject.SetActive(false);
    }

    private IEnumerator StorySequence()
    {
        var music = AudioController.Instance.PlaySound2D("waitingmusic");
        yield return new WaitForSeconds(2f);
        bool stoppedMusic = false;
        foreach (var line in lines)
        {
            if (line == "[stop]")
            {
                stoppedMusic = true;
                AudioController.Instance.FadeSourceVolume(music, 0f, 0.1f);
                continue;
            }
            text.gameObject.SetActive(true);
            text.text = line;
            AudioController.Instance.PlaySound2D("negate_" + Random.Range(1, 4));
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => Input.anyKey || Input.GetMouseButton(0));
            text.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        if (!stoppedMusic)
        {
            AudioController.Instance.FadeSourceVolume(music, 0f, 1f);
        }
        yield return new WaitForSeconds(0.25f);
        SceneTransition.instance.Transition(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
