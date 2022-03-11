using EventBusSystem;
using UnityEngine;

public interface IPlayerInteractionHandler : IGlobalSubscriber
{
    //Event отправляется после нажатия на зону
    void OnClickArea(Vector2 pos, int layer);
}