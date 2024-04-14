using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : ManagedBehaviour
{
    public static TutorialManager instance;
    public List<GameObject> tutorialObjects = new();

    protected override void ManagedInitialize()
    {
        instance = this;
    }

    public void ShowTutorial(int index)
    {
        tutorialObjects[index].SetActive(true);
        AudioController.Instance.PlaySound2D("negate_" + Random.Range(1, 4));
    }

    public void HideCurrentTutorial()
    {
        if (tutorialObjects.Exists(x => x.activeInHierarchy))
        {
            tutorialObjects.ForEach(x => x.SetActive(false));
            AudioController.Instance.PlaySound2D("negate_" + Random.Range(1, 4));
        }
    }
}
