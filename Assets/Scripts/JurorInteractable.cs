using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pixelplacement;

public class JurorInteractable : DraggableInteractable
{
    public bool Benched { get; set; } = true; //<--

    [Title("Juror")]
    public JurorData Data => data;
    [SerializeField]
    private JurorData data = default;

    public JurorAnimController Anim => anim;
    [SerializeField]
    private JurorAnimController anim = default;

    [SerializeField]
    private GameObject voteCardPrefab = default;

    private Disposition disposition = Disposition.Uncertain;
    private bool voted = false;

    protected override void ManagedInitialize()
    {
        disposition = data.disposition;
        anim.SetDispositionShown(false, immediate: true);
    }

    public override void ManagedUpdate()
    {
        base.ManagedUpdate();
        anim.SetDispositionShown(!BeingDragged && Benched && !voted);
        anim.ShowDispositionType(disposition);
    }

    public IEnumerator Vote()
    {
        var vote = disposition;
        yield return anim.VoteAnimationSequence(vote, true);
        voted = true;

        int numVotes = data.baseVoteCount;
        for (int i = 0; i < numVotes; i++)
        {
            var voteObj = Instantiate(voteCardPrefab);
            voteObj.transform.position = transform.position + Vector3.down * 1f;
            voteObj.GetComponent<Vote>().Initialize(vote == Disposition.Guilty);
            Tween.Position(voteObj.transform, transform.position + Vector3.up * (2f + (i * 0.75f)), 0.2f, 0f, Tween.EaseOutStrong);
            Tween.Rotate(voteObj.transform, new Vector3(0f, 0f, -3f + (Random.value * 6f)), Space.World, 0.25f, 0f, Tween.EaseOutStrong);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
