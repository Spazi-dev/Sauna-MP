using UnityEngine;

public class CosmeticItem : MonoBehaviour
{
	[SerializeField] MeshRenderer colorableMesh;
	public string ItemName;
	public string CharacterBone;

	public void SetItemColor(Color col)
	{
		colorableMesh.material.SetColor("_BaseColor", col);
	}
}
