using UnityEngine;

public class WorldPromptBillboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    //카메라 update 후에 실행되게 LateUpdate로 업데이트
    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        transform.rotation = Quaternion.LookRotation(
            transform.position - targetCamera.transform.position
        );
    }
}