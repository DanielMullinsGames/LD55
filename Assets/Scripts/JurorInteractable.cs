using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pixelplacement;

public class JurorInteractable : DraggableInteractable
{
    public bool Benched { get; set; } = true; //<--

    public Disposition VoteOutcome
    {
        get
        {
            if (votes.Count == 0)
            {
                return Disposition.None;
            }
            else
            {
                return votes[0].Outcome;
            }
        }
    }

    [Title("Juror")]
    public JurorData Data => data;
    [SerializeField]
    private JurorData data = default;

    public JurorAnimController Anim => anim;
    [SerializeField]
    private JurorAnimController anim = default;

    [SerializeField]
    private GameObject voteCardPrefab = default;

    private List<Vote> votes = new();

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

    public void OnTrialEnded()
    {
        voted = false;
    }

    public IEnumerator Vote()
    {
        var vote = disposition;
        if (vote == Disposition.Uncertain)
        {
            vote = DetermineUncertainVote();
        }
        yield return anim.VoteAnimationSequence(vote, disposition == Disposition.Uncertain);
        voted = true;

        if (vote != Disposition.None)
        {
            int numVotes = data.baseVoteCount;
            for (int i = 0; i < numVotes; i++)
            {
                var voteObj = Instantiate(voteCardPrefab);
                voteObj.transform.position = transform.position + Vector3.down * 1f;
                voteObj.GetComponent<Vote>().Initialize(vote == Disposition.Guilty);
                votes.Add(voteObj.GetComponent<Vote>());
                Tween.Position(voteObj.transform, transform.position + Vector3.up * (2f + (i * 0.75f)), 0.2f, 0f, Tween.EaseOutStrong);
                Tween.Rotate(voteObj.transform, new Vector3(0f, 0f, -3f + (Random.value * 6f)), Space.World, 0.25f, 0f, Tween.EaseOutStrong);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void CleanupVotes()
    {
        foreach (var vote in votes)
        {
            vote.Cleanup();
        }
        votes.Clear();
    }

    private Disposition DetermineUncertainVote()
    {
        var leftJurorVote = GetLeftJurorVote();
        switch (data.decisionMethod)
        {
            case DecisionMethod.OppositeOfLeft:
                if (leftJurorVote == Disposition.None)
                {
                    return Disposition.None;
                }
                else
                {
                    return leftJurorVote == Disposition.Guilty ? Disposition.Innocent : Disposition.Guilty;
                }
            case DecisionMethod.SameAsLeft:
                return leftJurorVote;
            default:
                return Disposition.None;
        }
    }

    private Disposition GetLeftJurorVote()
    {
        int index = BenchArea.instance.Jurors.IndexOf(this);
        if (index > 0)
        {
            var leftJuror = BenchArea.instance.Jurors[index - 1];
            return leftJuror.VoteOutcome;
        }
        else
        {
            return Disposition.None;
        }
    }
}
