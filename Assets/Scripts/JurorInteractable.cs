using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pixelplacement;

public class JurorInteractable : DraggableInteractable
{
    public bool Benched { get; set; } = true; //<--
    public JurorCard Card { get; set; }

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
    public int NumVotes => votes.Count;

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
        SetCollisionEnabled(true);
    }

    public void CleanupVotes()
    {
        foreach (var vote in votes)
        {
            vote.Cleanup();
        }
        votes.Clear();
    }

    public IEnumerator VoteSequence()
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
            yield return CastVote(vote);
        }

        if (data.afterVoteAction != AfterVoteAction.None)
        {
            yield return PostVoteActionSequence(data.afterVoteAction, vote);
        }
    }

    public void ChangeVotes(Disposition outcome)
    {
        foreach (var vote in votes)
        {
            vote.ChangeVote(outcome == Disposition.Guilty);
        }
    }

    private IEnumerator CastVote(Disposition vote)
    {
        var voteObj = Instantiate(voteCardPrefab);
        voteObj.transform.position = transform.position + Vector3.down * 1f;
        voteObj.GetComponent<Vote>().Initialize(vote == Disposition.Guilty);
        Tween.Position(voteObj.transform, transform.position + Vector3.up * (2f + (votes.Count * 0.75f)), 0.2f, 0f, Tween.EaseOutStrong);
        Tween.Rotate(voteObj.transform, new Vector3(0f, 0f, -3f + (Random.value * 6f)), Space.World, 0.25f, 0f, Tween.EaseOutStrong);
        votes.Add(voteObj.GetComponent<Vote>());
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator PostVoteActionSequence(AfterVoteAction action, Disposition vote)
    {
        Card.SetActionBadgePulsing(true);
        yield return new WaitForSeconds(0.5f);
        switch (action)
        {
            case AfterVoteAction.ChangeLeftToInnocent:
                anim.SetTrigger("priest");
                yield return new WaitForSeconds(0.5f);
                var leftJuror = GetLeftJuror();
                if (leftJuror != null)
                {
                    leftJuror.ChangeVotes(Disposition.Innocent);
                }
                yield return new WaitForSeconds(1f);
                break;
            case AfterVoteAction.ExtraVote:
                yield return CastVote(vote);
                break;
        }
        yield return new WaitForSeconds(0.1f);
        Card.SetActionBadgePulsing(false);
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

    private JurorInteractable GetLeftJuror()
    {
        int index = BenchArea.instance.Jurors.IndexOf(this);
        if (index > 0)
        {
            return BenchArea.instance.Jurors[index - 1];
        }
        else
        {
            return null;
        }
    }

    private Disposition GetLeftJurorVote()
    {
        var leftJuror = GetLeftJuror();
        if (leftJuror != null)
        {
            return leftJuror.VoteOutcome;
        }
        else
        {
            return Disposition.None;
        }
    }
}
