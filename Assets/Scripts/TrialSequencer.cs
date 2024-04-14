using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class TrialSequencer : ManagedBehaviour
{
    [SerializeField]
    private BenchArea bench = default;

    [SerializeField]
    private Transform judgeStand = default;

    [SerializeField]
    private Animator judgeAnim = default;

    [SerializeField]
    private Transform requirementTextParent = default;

    [SerializeField]
    private TMPro.TextMeshPro requirementText = default;

    [SerializeField]
    private Camera cam = default;

    private const float REQ_TEXT_OFFSCREEN_Y = 6f;
    private float textOnScreenY;
    private const float STAND_OFFSCREEN_Y = -7f;
    private float standOnScreenY;

    private void Start()
    {
        textOnScreenY = requirementTextParent.position.y;
        requirementTextParent.position = new Vector2(requirementTextParent.position.x, REQ_TEXT_OFFSCREEN_Y);

        standOnScreenY = judgeStand.position.y;
        judgeStand.transform.position = new Vector2(judgeStand.position.x, STAND_OFFSCREEN_Y);
    }

    public IEnumerator TrialSequence(bool lastTrial, int requiredVotes, System.Action<bool> completedCallback)
    {
        // Setup
        int numVotes = 0;
        UpdateReqText(requiredVotes, numVotes);

        Tween.Position(judgeStand, new Vector2(judgeStand.position.x, standOnScreenY), 3f, 0f, Tween.EaseOutStrong);
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.05f, 3f, 0f);
        AudioController.Instance.PlaySound2D("weird_power", pitch: new AudioParams.Pitch(0.7f));
        yield return new WaitForSeconds(2.5f);

        Tween.Position(requirementTextParent, new Vector2(requirementTextParent.position.x, textOnScreenY), 1f, 0f, Tween.EaseOutStrong);
        yield return new WaitForSeconds(1f);

        AudioController.Instance.PlaySound2D("judge_order");
        judgeAnim.SetTrigger("order");
        bench.Jurors.ForEach(x => x.SetCollisionEnabled(false));
        yield return new WaitForSeconds(2f);

        // Activate Jurors
        foreach (var juror in bench.Jurors)
        {
            yield return juror.VoteSequence();
            yield return new WaitForSeconds(0.4f);
        }

        var successDisp = GameFlowManager.ProveInnocence ? Disposition.Innocent : Disposition.Guilty;
        var failureDisp = GameFlowManager.ProveInnocence ? Disposition.Guilty : Disposition.Innocent;

        // Count Votes
        foreach (var juror in bench.Jurors)
        {
            if (juror.VoteOutcome == failureDisp)
            {
                juror.CleanupVotes();
                yield return new WaitForSeconds(0.01f);
            }
        }
        yield return new WaitForSeconds(1f);
        foreach (var juror in bench.Jurors)
        {
            if (juror.VoteOutcome == successDisp)
            {
                numVotes += juror.NumVotes;
                PunchReqText(() => UpdateReqText(requiredVotes, numVotes));
                juror.CleanupVotes();
                yield return new WaitForSeconds(0.5f);
            }
        }
        yield return new WaitForSeconds(0.25f);

        // Verdict
        judgeAnim.SetTrigger("verdict");
        yield return new WaitForSeconds(0.9f);
        bool success = numVotes >= requiredVotes;
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.BackQuote))
        {
            success = true;
        }
#endif
        bool guilty = success;
        if (GameFlowManager.ProveInnocence)
        {
            guilty = !guilty;
        }
        PunchReqText(() => requirementText.text = guilty ? "GUILTY!" : "INNOCENT!");
        AudioController.Instance.PlaySound2D(guilty ? "judge_guilty" : "judge_innocent");

        yield return new WaitForSeconds(1f);

        if (success)
        {
            GameFlowManager.instance.ShowDefendantFear(true);
        }

        // Cleanup
        Tween.Position(judgeStand, new Vector2(judgeStand.position.x, STAND_OFFSCREEN_Y), 2f, 0f, Tween.EaseIn);
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.03f, 2f, 0f);
        Tween.Position(requirementTextParent, new Vector2(requirementTextParent.position.x, REQ_TEXT_OFFSCREEN_Y), 1f, 0f, Tween.EaseIn);
        AudioController.Instance.PlaySound2D("weird_power", pitch: new AudioParams.Pitch(0.8f));
        yield return new WaitForSeconds(2f);

        if (!lastTrial)
        {
            bench.Jurors.ForEach(x => x.OnTrialEnded());
        }
        GameFlowManager.instance.ShowDefendantFear(false);
        completedCallback?.Invoke(success);
    }

    public void OnCameraShakeKeyframe()
    {
        AudioController.Instance.PlaySound2D("gavel_hit", pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.VerySmall));
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
    }

    private void UpdateReqText(int required, int numVotes)
    {
        requirementText.text = string.Format("{0} of {1} guilty votes needed", numVotes, required);
    }

    private void PunchReqText(System.Action impactCallback)
    {
        Tween.LocalScale(requirementTextParent, Vector2.one * 0.9f, 0.1f, 0f, Tween.EaseInOut, completeCallback: () =>
        {
            impactCallback?.Invoke();
            Tween.LocalScale(requirementTextParent, Vector2.one, 0.1f, 0f, Tween.EaseInOut);
        });
    }
}
