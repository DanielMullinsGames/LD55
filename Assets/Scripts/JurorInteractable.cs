using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pixelplacement;

public class JurorInteractable : DraggableInteractable
{
    public JurorCard Card { get; set; }
    public GameObject Prefab { get; set; }

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

    [SerializeField]
    private Interactable buyButton = default;

    [SerializeField]
    private TMPro.TextMeshPro buyButtonText = default;

    [SerializeField]
    private GameObject rareSparkles = default;

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
        anim.SetDispositionShown(!BeingDragged && !voted);
        anim.ShowDispositionType(disposition);
    }

    public void ConfigureForBuyPhase(System.Action<JurorInteractable> buyCallback)
    {
        enabled = false;
        anim.enabled = false;
        anim.SetDispositionShown(false);
        anim.SetHandsShown(false);
        anim.SortingGroup.sortingLayerName = "Foreground";
        anim.SortingGroup.sortingOrder = 5;

        buyButtonText.text = "summon $" + Data.cost;
        buyButton.gameObject.SetActive(true);
        buyButton.CursorSelectStarted += (Interactable i) => buyCallback?.Invoke(this);

        SetCollisionEnabled(false);

        if (Data.rare)
        {
            rareSparkles.SetActive(true);
        }
    }

    public void OnTrialEnded()
    {
        if (data.special == SpecialTrait.Dies && Random.value > 0.5f)
        {
            Destroy(gameObject);
            AudioController.Instance.PlaySound2D("juror_die", 0.5f);
            AudioController.Instance.PlaySound2D("negate_3");
        }
        else
        {
            voted = false;
            SetCollisionEnabled(true);
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

    protected override void OnDragEnd()
    {
        if (SellArea.instance.gameObject.activeInHierarchy && GetComponent<Collider2D>().bounds.Intersects(SellArea.instance.Bounds))
        {
            if (data.special != SpecialTrait.CantSell)
            {
                SellArea.instance.SellJuror(this);
            }
        }
    }

    private IEnumerator CastVote(Disposition vote)
    {
        AudioController.Instance.PlaySound2D("paper_short", 1f, pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.VerySmall), repetition: new AudioParams.Repetition(0.05f));

        var voteObj = Instantiate(voteCardPrefab);
        voteObj.transform.position = transform.position + Vector3.down * 1f;
        voteObj.GetComponent<Vote>().Initialize(vote == Disposition.Guilty);
        Tween.Position(voteObj.transform, transform.position + Vector3.up * (2f + (votes.Count * 0.75f)), 0.2f, 0f, Tween.EaseOutStrong);
        Tween.Rotate(voteObj.transform, new Vector3(0f, 0f, -3f + (Random.value * 6f)), Space.World, 0.25f, 0f, Tween.EaseOutStrong);
        votes.Add(voteObj.GetComponent<Vote>());
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator PostVoteActionSequence(AfterVoteAction action, Disposition vote)
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
            case AfterVoteAction.ChangeLeftToGuilty:
                anim.SetTrigger("fist");
                yield return new WaitForSeconds(0.5f);
                var left = GetLeftJuror();
                if (left != null)
                {
                    left.ChangeVotes(Disposition.Guilty);
                }
                yield return new WaitForSeconds(1f);
                break;
            case AfterVoteAction.SwitchAll:
                anim.SetTrigger("gun");
                yield return new WaitForSeconds(0.5f);
                int index = BenchArea.instance.Jurors.IndexOf(this);
                for (int i = 0; i < index; i++)
                {
                    bool toGuilty = BenchArea.instance.Jurors[i].VoteOutcome == Disposition.Innocent;
                    BenchArea.instance.Jurors[i].ChangeVotes(toGuilty ? Disposition.Guilty : Disposition.Innocent);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.1f);
                break;
            case AfterVoteAction.DoubleActivate:
                anim.SetTrigger("fist");
                yield return new WaitForSeconds(0.5f);
                int index2 = BenchArea.instance.Jurors.IndexOf(this);
                for (int i = 0; i < index2; i++)
                {
                    var j = BenchArea.instance.Jurors[i];
                    if (j.data.afterVoteAction != AfterVoteAction.None)
                    {
                        yield return j.PostVoteActionSequence(j.data.afterVoteAction, j.VoteOutcome);
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.1f);
                break;
            case AfterVoteAction.GainCash:
                yield return new WaitForSeconds(0.1f);
                anim.SetTrigger("voteguilty");
                yield return new WaitForSeconds(0.25f);
                CashManager.instance.AdjustCash(1);
                yield return new WaitForSeconds(0.25f);
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
            case DecisionMethod.WithMajority:
                int index = BenchArea.instance.Jurors.IndexOf(this);
                int guilty = 0;
                int innocent = 0;
                for (int i = 0; i < index; i++)
                {
                    if (BenchArea.instance.Jurors[i].VoteOutcome == Disposition.Guilty)
                    {
                        guilty += BenchArea.instance.Jurors[i].NumVotes;
                    }
                    else if (BenchArea.instance.Jurors[i].VoteOutcome == Disposition.Innocent)
                    {
                        innocent += BenchArea.instance.Jurors[i].NumVotes;
                    }
                }
                return innocent > guilty ? Disposition.Innocent : Disposition.Guilty;
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
