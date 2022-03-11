using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaEntities", menuName = "Game/Area", order = 1)]
public class AreaEntitiesScriptableObject : ScriptableObject
{
    public string layerName;
    public List<Entity> entities;
    public int maxCount = 5;
    public bool useDynamicSortingOrder;
}