using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBusSystem;

public class EntitySpawner : MonoBehaviour, IPlayerInteractionHandler
{
    //Содержит список Scribtable objects зон в которых содержатся данные по объектам спавна
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
        //Находим зону на которую кликнули
        AreaEntitiesScriptableObject area = areaEntities.Find(h => h.layerName == LayerMask.LayerToName(layer));
        if (area != null)
        {
            //Рандомно находим объект для спавна
            int entitySpawnIndex = Random.Range(0, area.entities.Count);
            Entity ent = area.entities[entitySpawnIndex];
            //Находим gameobject родитель для объекта
            Transform areaTransform = GameObject.Find("Art").transform.Find(area.layerName);

            //если доступен спавн
            if (IsAvaliable(areaTransform, ent.typeId, ent.maxCount))
            {
                SpawnEntity(area, ent, pos, areaTransform);
            }
            else
            {
                //если спавн случайно выбранного объекта не доступен, то ищем доступный и спавним
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
    /// Спавн нового объекта на выбронную зону
    /// </summary>
    /// <param name="area">Зона по которой кликнули</param>
    /// <param name="entity">Выбранный объект спавна</param>
    /// <param name="pos">Место, куда кликнули</param>
    /// <param name="parent">GameObject-родитель для объекта</param>
    private void SpawnEntity(AreaEntitiesScriptableObject area, Entity entity, Vector3 pos, Transform parent)
    {
        GameObject go = Instantiate(entity.prefab);
        go.name = entity.typeId.ToString();
        go.transform.position = pos;
        go.transform.parent = parent;
        go.GetComponent<SpawnedEntity>().typeId = entity.typeId;

        //Если включена динамичкая сортировка спрайтов
        if (area.useDynamicSortingOrder)
        {
            //порядок сортировки равен y координате места, куда кликнули
            go.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(pos.y * -10);
        }
    }

    /// <summary>
    /// Проверяет, доступен ли для спавна объект. Если maxCount указан -1, то объект всегда дотупен для спавна (в примере это корабль - единственный объект в зоне моря)
    /// </summary>
    /// <param name="area">GameObject-родитель, в котором спавнятся объекты</param>
    /// <param name="typeId">Тип объекта</param>
    /// <param name="maxCount">Максимальное кол-во одновременно находящихся в зоне объектов</param>
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
    /// Проверяем gameobject потомков на количество. Если больше 5, то удаляем самый первый
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
