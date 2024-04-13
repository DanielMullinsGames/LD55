using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialSequencer : ManagedBehaviour
{
    [SerializeField]
    private BenchArea bench = default;

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
        // JUDGE CALLS ORDER

        foreach (var juror in bench.Jurors)
        {
            yield return juror.Vote();
            yield return new WaitForSeconds(0.1f);
        }

        // VERDICT
    }
}
