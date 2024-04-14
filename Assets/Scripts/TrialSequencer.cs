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
    private const float STAND_OFFSCREEN_Y = -5f;
    private float standOnScreenY;

    private void Start()
    {
        textOnScreenY = requirementTextParent.position.y;
        requirementTextParent.position = new Vector2(requirementTextParent.position.x, REQ_TEXT_OFFSCREEN_Y);

        standOnScreenY = judgeStand.position.y;
        judgeStand.transform.position = new Vector2(judgeStand.position.x, STAND_OFFSCREEN_Y);
    }

    public IEnumerator TrialSequence(int requiredVotes, System.Action<bool> completedCallback)
    {
        // Setup
        int numVotes = 0;
        UpdateReqText(requiredVotes, numVotes);

        Tween.Position(judgeStand, new Vector2(judgeStand.position.x, standOnScreenY), 3f, 0f, Tween.EaseOutStrong);
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.05f, 3f, 0f);
        yield return new WaitForSeconds(2.5f);

        Tween.Position(requirementTextParent, new Vector2(requirementTextParent.position.x, textOnScreenY), 1f, 0f, Tween.EaseOutStrong);
        yield return new WaitForSeconds(1f);

        judgeAnim.SetTrigger("order");
        bench.Jurors.ForEach(x => x.SetCollisionEnabled(false));
        yield return new WaitForSeconds(2f);

        // Activate Jurors
        foreach (var juror in bench.Jurors)
        {
            yield return juror.VoteSequence();
            yield return new WaitForSeconds(0.4f);
        }

        // Count Votes
        foreach (var juror in bench.Jurors)
        {
            if (juror.VoteOutcome == Disposition.Innocent)
            {
                juror.CleanupVotes();
                yield return new WaitForSeconds(0.01f);
            }
        }
        yield return new WaitForSeconds(1f);
        foreach (var juror in bench.Jurors)
        {
            if (juror.VoteOutcome == Disposition.Guilty)
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
        bool guilty = numVotes >= requiredVotes;
        PunchReqText(() => requirementText.text = guilty ? "GUILTY!" : "INNOCENT!");
        yield return new WaitForSeconds(1f);

        // TODO: show defendant... killed?

        // Cleanup
        Tween.Position(judgeStand, new Vector2(judgeStand.position.x, STAND_OFFSCREEN_Y), 2f, 0f, Tween.EaseIn);
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.03f, 2f, 0f);
        Tween.Position(requirementTextParent, new Vector2(requirementTextParent.position.x, REQ_TEXT_OFFSCREEN_Y), 1f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(2f);

        bench.Jurors.ForEach(x => x.OnTrialEnded());
    }

    public void OnCameraShakeKeyframe()
    {
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
