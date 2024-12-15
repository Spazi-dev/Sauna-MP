using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sauna/Cosmetic Item Catalog")]
[System.Serializable]
public class CosmeticItemCatalog : ScriptableObject
{
    [TextArea][SerializeField] string CatalogDescription;
    public CosmeticItem[] CosmeticItems;
}
