using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "WhaleTricks/TrickRegistry")]
public class TrickRegistry : ScriptableObject
{
    public List<Trick> tricks = new List<Trick>();
}
