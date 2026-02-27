using UnityEngine;

public class LegacyLook : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity = 100f; // 마우스 감도
    float xRot = 0f; // 마우스 상하 회전값

    public Transform cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 마우스가 2차원으로 움직임
        // 마우스 상하 좌우 움직임을 감지해서 캐릭터가 바라보는 방향을 바꿔주는 스크립트

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity; // 마우스 좌우 움직임 감지
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity; // 마우스 상하 움직임 감지

        transform.Rotate(Vector3.up * mouseX); // 마우스 좌우 움직임에 따라 캐릭터 회전


        // 상하 회전
        // 마우스 상하 움직임을 감지해서 캐릭터가 바라보는 방향을 바꿔주는 스크립트
        // Axis -> -1 ~ 1 사이의 값을 반환하는 함수
        xRot -= mouseY;

        // 각도제한
         xRot = Mathf.Clamp(xRot, -90f, 90f); // 상하 회전 각도 제한

        // 카메라 제한
        cam.localRotation = Quaternion.Euler(xRot, 0f, 0f); // 마우스 상하 움직임에 따라 카메라 회전
    }
}
