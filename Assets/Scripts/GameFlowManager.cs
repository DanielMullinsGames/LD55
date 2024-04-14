using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

[System.Serializable]
public class Defendant
{
    public string name;
    public Animator anim;
    public int[] guiltPhases;
    public List<GameObject> activeOnStart = new();
}

public class GameFlowManager : ManagedBehaviour
{
    public static GameFlowManager instance;
    public static int defendantIndex = 0;
    public static bool ProveInnocence => defendantIndex == 3;

    [SerializeField]
    private TrialSequencer trialSequencer = default;

    [SerializeField]
    private BuyPhaseSequencer buyPhaseSequencer = default;

    [SerializeField]
    private MetaPhaseSequencer metaPhaseSequencer = default;

    [SerializeField]
    private Transform benchParent = default;

    [SerializeField]
    private Transform defendantStand = default;

    [SerializeField]
    private Vector2 buyPhaseBenchPos = default;

    [SerializeField]
    private Vector2 trialPhaseBenchPos = default;

    [SerializeField]
    private Vector2 trialDefendantPos = default;

    [SerializeField]
    private TMPro.TextMeshPro defendantText = default;

    [SerializeField]
    private TMPro.TextMeshPro finalVerdictText = default;

    [SerializeField]
    private Transform crossHairs = default;

    [Header("DEFENDANTS")]
    [SerializeField]
    private List<Defendant> defendants = new();

    [Header("DEBUG")]
    public int debugDefendantIndex = 0;

    private readonly string[] PHASE_TRIAL_NAMES = new string[]
    {
        "mock trial #1",
        "mock trial #2",
        "pre-trial #1",
        "pre-trial #2",
        "final trial",
    };

    protected override void ManagedInitialize()
    {
        instance = this;

#if UNITY_EDITOR
        if (Time.realtimeSinceStartup < 5f)
        {
            defendantIndex = debugDefendantIndex;
        }
#endif
    }

    private void Start()
    {
        defendants.ForEach(x => x.anim.gameObject.SetActive(false));
        StartCoroutine(GameSequence());
    }

    public void ShowDefendantFear(bool fear)
    {
        defendants[defendantIndex].anim.SetBool("fear", fear);
    }

    public override void ManagedUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Space))
        {
            Time.timeScale = 5f;
        }
        else
        {
            Time.timeScale = 1f;
        }
#endif
    }

    private IEnumerator GameSequence()
    {
        yield return DefendantIntroSequence();
        yield return DefendantSequence();
    }

    private IEnumerator DefendantIntroSequence()
    {
        defendants[defendantIndex].activeOnStart.ForEach(x => x.SetActive(true));

        benchParent.transform.position = new Vector2(buyPhaseBenchPos.x, -14f);
        defendantStand.transform.position = new Vector2(0f, -14f);
        defendants[defendantIndex].anim.gameObject.SetActive(true);
        ShowDefendantFear(true);

        yield return new WaitForSeconds(1.5f);
        defendantText.gameObject.SetActive(true);
        defendantText.text = "Defendant: " + defendants[defendantIndex].name;
        AudioController.Instance.PlaySound2D("horn_1");

        yield return new WaitForSeconds(0.5f);
        AudioController.Instance.PlaySound2D("whoosh");
        Tween.Position(defendantStand, new Vector2(0f, trialDefendantPos.y), 1f, 0f, Tween.EaseOutStrong);

        yield return new WaitForSeconds(2f);
        AudioController.Instance.PlaySound2D("whoosh");
        Tween.Position(defendantStand, trialDefendantPos, 1f, 0f, Tween.EaseOutStrong);

        yield return new WaitForSeconds(0.5f);
        ShowDefendantFear(false);
        defendantText.gameObject.SetActive(false);
        AudioController.Instance.PlaySound2D("horn_1");
    }

    private IEnumerator SentencingSequence(bool succeeded)
    {
        AudioController.Instance.PlaySound2D("whoosh");
        Tween.Position(defendantStand, new Vector2(0f, trialDefendantPos.y), 1f, 0f, Tween.EaseOutStrong);
        ShowDefendantFear(true);
        yield return new WaitForSeconds(2f);

        finalVerdictText.gameObject.SetActive(true);
        AudioController.Instance.PlaySound2D("horn_1");

        if (ProveInnocence)
        {
            finalVerdictText.text = "FINAL VERDICT: " + (succeeded ? "INNOCENT!" : "GUILTY!");
            if (succeeded)
            {
                yield return new WaitForSeconds(2f);
                AudioController.Instance.PlaySound2D("whoosh");
                SceneTransition.instance.Transition(false);
                yield return new WaitForSeconds(0.6f);
                defendantIndex++;
            }
            else
            {
                yield return Failure();
            }
        }
        else
        {
            finalVerdictText.text = "FINAL VERDICT: " + (succeeded ? "GUILTY!" : "INNOCENT!");
            if (succeeded)
            {
                crossHairs.gameObject.SetActive(true);
                crossHairs.transform.position = new Vector3(10f, 10f);
                Tween.Position(crossHairs, defendantStand.position + Vector3.up * 0.5f, 2f, 0f, Tween.EaseOutStrong);
                yield return new WaitForSeconds(3f);
                SceneTransition.instance.BlackOut();
                AudioController.Instance.PlaySound2D("explode");
                yield return new WaitForSeconds(2f);
                defendantIndex++;
            }
            else
            {
                yield return Failure();
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Story_" + (defendantIndex + 1));
    }

    private IEnumerator Failure()
    {
        ShowDefendantFear(false);
        yield return new WaitForSeconds(1.5f);
        finalVerdictText.text = "GAME OVER. CLICK TO RESTART TRIAL.";
        AudioController.Instance.PlaySound2D("horn_1");
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        AudioController.Instance.PlaySound2D("whoosh");
        SceneTransition.instance.Transition(false);
        yield return new WaitForSeconds(0.6f);
    }

    private IEnumerator DefendantSequence()
    {
        var guiltReqs = defendants[defendantIndex].guiltPhases;

        int numRounds = 5;
        bool trialSucceeded = false;
        for (int i = 0; i < numRounds; i++)
        {
            yield return metaPhaseSequencer.MetaPhase(i, guiltReqs, PHASE_TRIAL_NAMES, trialSucceeded);
            yield return new WaitForSeconds(0.1f);

            Tween.Position(benchParent, buyPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            AudioController.Instance.PlaySound2D("whoosh", pitch: new AudioParams.Pitch(0.5f));
            yield return new WaitForSeconds(0.35f);
            yield return buyPhaseSequencer.BuySequence(i, guiltReqs[i], PHASE_TRIAL_NAMES[i]);
            yield return new WaitForSeconds(0.1f);
            
            Tween.Position(benchParent, trialPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            AudioController.Instance.PlaySound2D("whoosh", pitch: new AudioParams.Pitch(0.5f));
            yield return new WaitForSeconds(0.35f);
            yield return trialSequencer.TrialSequence(i == numRounds - 1, guiltReqs[i], (bool succeeded) => trialSucceeded = succeeded);

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.BackQuote))
            {
                i = 10;
            }
#endif
        }

        yield return SentencingSequence(trialSucceeded);
    }
}
