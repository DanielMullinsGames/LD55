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
}

[CreateAssetMenu(fileName = "JurorData", menuName = "JurorData", order = 1)]
public class JurorData : ScriptableObject
{
    public int cost = 2;
    public string nameText;
    [TextArea]
    public string detailText;
    public Disposition disposition = Disposition.Guilty;
    public DecisionMethod decisionMethod = DecisionMethod.Normal;
    public AfterVoteAction afterVoteAction = AfterVoteAction.None;
}
