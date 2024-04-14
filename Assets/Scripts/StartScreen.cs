using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : ManagedBehaviour
{
    public override void ManagedUpdate()
    {
        if (Time.timeSinceLevelLoad > 1f && Input.anyKeyDown || Input.GetMouseButton(0))
        {
            AudioController.Instance.PlaySound2D("horn_1");
            enabled = false;
            SceneTransition.instance.Transition(false);
            CustomCoroutine.WaitThenExecute(0.6f, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
            });
        }
    }
}
