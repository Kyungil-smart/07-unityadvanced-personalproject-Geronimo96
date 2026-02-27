using JetBrains.Rider.Unity.Editor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private float walkSpeed = 7f;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private Camera playerCamera;


    // [SerializeField]
    private Rigidbody myRigid;

    private void Start()
    {
        myRigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        CameraRotation();
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * _moveDirX;
        Vector3 moveVertical = transform.forward * _moveDirZ;

        Vector3 velocity = (moveHorizontal + moveVertical)
            .normalized * walkSpeed;
        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 playerRotationY = new Vector3(0f, _mouseX, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(playerRotationY));
        currentCameraRotationX -= _mouseY * lookSensitivity;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
