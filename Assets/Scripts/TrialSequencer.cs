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
    private TMPro.TextMeshPro requirementText = default;

    [SerializeField]
    private Camera cam = default;

    private const float STAND_OFFSCREEN_Y = -5f;
    private float standOnScreenY;

    private void Start()
    {
        standOnScreenY = judgeStand.position.y;
        judgeStand.transform.position = new Vector2(judgeStand.position.x, STAND_OFFSCREEN_Y);
    }

#if UNITY_EDITOR
    public override void ManagedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TrialSequence(null));
        }
    }
#endif

    public IEnumerator TrialSequence(System.Action<bool> completedCallback)
    {
        Tween.Position(judgeStand, new Vector2(judgeStand.position.x, standOnScreenY), 3f, 0f, Tween.EaseOutStrong);
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.05f, 3f, 0f);
        yield return new WaitForSeconds(3.5f);

        judgeAnim.SetTrigger("order");
        yield return new WaitForSeconds(2f);

        foreach (var juror in bench.Jurors)
        {
            yield return juror.VoteSequence();
            yield return new WaitForSeconds(0.5f);
        }

        // COUNT

        foreach (var juror in bench.Jurors)
        {
            juror.CleanupVotes();
            yield return new WaitForSeconds(0.01f);
        }

        // VERDICT

        yield return new WaitForSeconds(1f);
        bench.Jurors.ForEach(x => x.OnTrialEnded());
    }

    public void OnCameraShakeKeyframe()
    {
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
    }
}
