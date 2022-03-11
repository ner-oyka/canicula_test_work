using EventBusSystem;
using UnityEngine;

public interface IPlayerInteractionHandler : IGlobalSubscriber
{
    //Event ������������ ����� ������� �� ����
    void OnClickArea(Vector2 pos, int layer);
}