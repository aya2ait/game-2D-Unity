using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Room currRoom;
    public float moveSpeedWhenRoomChange;
    private Transform playerTransform;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Empêcher la destruction de la caméra lors du chargement de la nouvelle scène
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
       
    }

    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (currRoom == null && playerTransform == null)
        {
            return;
        }

        Vector3 targetPos = (playerTransform != null) ? playerTransform.position : GetCameraTargetPosition();
        targetPos.z = transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeedWhenRoomChange);
    }

    Vector3 GetCameraTargetPosition()
    {
        if (currRoom == null)
        {
            return Vector3.zero;
        }

        Vector3 targetPos = currRoom.GetRoomCentre();
        targetPos.z = transform.position.z;

        return targetPos;
    }

    public bool IsSwitchingScene()
    {
        if (playerTransform == null)
        {
            return false;
        }

        return transform.position.Equals(playerTransform.position) == false;
    }

    public void UpdatePlayerTransform(Transform newPlayerTransform)
    {
        playerTransform = newPlayerTransform;
    }
}
