using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBusSystem;

public class PlayerInteraction : MonoBehaviour
{
    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            //���������� ��� ����, ����� ��������� (����, ����, ����)
            int layerMasks = ~(1 << 5);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 1000f, layerMasks);

            if (hit.collider != null)
            {
                //���������� ������� ������� �� ����
                EventBus.RaiseEvent<IPlayerInteractionHandler>(h => h.OnClickArea(hit.point, hit.transform.gameObject.layer));
            }
        }
    }
}
