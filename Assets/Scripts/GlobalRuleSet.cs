using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sauna/Global Rule Set")]
[System.Serializable]
public class GlobalRuleSet : ScriptableObject
{
	public int MaxNameLength;
	[TextArea] public string Description;
}
