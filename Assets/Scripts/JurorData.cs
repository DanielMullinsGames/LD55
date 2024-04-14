using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Disposition
{
    None,
    Guilty,
    Innocent,
    Uncertain,
}

public enum DecisionMethod
{
    Normal,
    OppositeOfLeft,
    SameAsLeft,
    WithMajority,
    AgainstMajority,
}

public enum AfterVoteAction
{
    None,
    ChangeLeftToInnocent,
    ExtraVote,
    DoubleActivate,
    SwitchAll,
    GainCash,
    ChangeLeftToGuilty,
    ForceNeighbourVote,
}

public enum SpecialTrait
{
    None,
    Dies,
    CantSell,
    ExtraSell,
    SummonSkeleton,
    DiesAlways,
}

[CreateAssetMenu(fileName = "JurorData", menuName = "JurorData", order = 1)]
public class JurorData : ScriptableObject
{
    public bool rare;
    public int cost => rare ? 3 : 2;
    public string nameText;
    [TextArea]
    public string detailText;
    public Disposition disposition = Disposition.Guilty;
    public DecisionMethod decisionMethod = DecisionMethod.Normal;
    public AfterVoteAction afterVoteAction = AfterVoteAction.None;
    public SpecialTrait special = SpecialTrait.None;
}
