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

[CreateAssetMenu(fileName = "JurorData", menuName = "JurorData", order = 1)]
public class JurorData : ScriptableObject
{
    public string nameText;
    [TextArea]
    public string detailText;
    public Disposition disposition = Disposition.Guilty;
}
