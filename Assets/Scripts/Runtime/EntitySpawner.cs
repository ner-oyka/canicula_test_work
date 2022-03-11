using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBusSystem;

public class EntitySpawner : MonoBehaviour, IPlayerInteractionHandler
{
    //�������� ������ Scribtable objects ��� � ������� ���������� ������ �� �������� ������
    [SerializeField]
    private List<AreaEntitiesScriptableObject> areaEntities = new List<AreaEntitiesScriptableObject> ();

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void OnClickArea(Vector2 pos, int layer)
    {
        //������� ���� �� ������� ��������
        AreaEntitiesScriptableObject area = areaEntities.Find(h => h.layerName == LayerMask.LayerToName(layer));
        if (area != null)
        {
            //�������� ������� ������ ��� ������
            int entitySpawnIndex = Random.Range(0, area.entities.Count);
            Entity ent = area.entities[entitySpawnIndex];
            //������� gameobject �������� ��� �������
            Transform areaTransform = GameObject.Find("Art").transform.Find(area.layerName);

            //���� �������� �����
            if (IsAvaliable(areaTransform, ent.typeId, ent.maxCount))
            {
                SpawnEntity(area, ent, pos, areaTransform);
            }
            else
            {
                //���� ����� �������� ���������� ������� �� ��������, �� ���� ��������� � �������
                foreach (var e in area.entities)
                {
                    if (IsAvaliable(areaTransform, e.typeId, e.maxCount))
                    {
                        SpawnEntity(area, e, pos, areaTransform);
                        break;
                    }
                }
            }

            RefreshAreaEntities(areaTransform);
        }

    }

    /// <summary>
    /// ����� ������ ������� �� ��������� ����
    /// </summary>
    /// <param name="area">���� �� ������� ��������</param>
    /// <param name="entity">��������� ������ ������</param>
    /// <param name="pos">�����, ���� ��������</param>
    /// <param name="parent">GameObject-�������� ��� �������</param>
    private void SpawnEntity(AreaEntitiesScriptableObject area, Entity entity, Vector3 pos, Transform parent)
    {
        GameObject go = Instantiate(entity.prefab);
        go.name = entity.typeId.ToString();
        go.transform.position = pos;
        go.transform.parent = parent;
        go.GetComponent<SpawnedEntity>().typeId = entity.typeId;

        //���� �������� ���������� ���������� ��������
        if (area.useDynamicSortingOrder)
        {
            //������� ���������� ����� y ���������� �����, ���� ��������
            go.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(pos.y * -10);
        }
    }

    /// <summary>
    /// ���������, �������� �� ��� ������ ������. ���� maxCount ������ -1, �� ������ ������ ������� ��� ������ (� ������� ��� ������� - ������������ ������ � ���� ����)
    /// </summary>
    /// <param name="area">GameObject-��������, � ������� ��������� �������</param>
    /// <param name="typeId">��� �������</param>
    /// <param name="maxCount">������������ ���-�� ������������ ����������� � ���� ��������</param>
    /// <returns></returns>
    private bool IsAvaliable(Transform area, int typeId, int maxCount)
    {
        if (maxCount == -1)
        {
            return true;
        }

        List<SpawnedEntity> spawnedEntity = new List<SpawnedEntity>();
        foreach (Transform t in area)
        {
            spawnedEntity.Add(t.GetComponent<SpawnedEntity>());
        }

        int count = spawnedEntity.FindAll(f => f.typeId == typeId).Count;
        
        if (count < maxCount)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// ��������� gameobject �������� �� ����������. ���� ������ 5, �� ������� ����� ������
    /// </summary>
    /// <param name="area"></param>
    private void RefreshAreaEntities(Transform area)
    {
        if (area.childCount > 5)
        {
            Destroy(area.GetChild(0).gameObject);
        }
    }
}
