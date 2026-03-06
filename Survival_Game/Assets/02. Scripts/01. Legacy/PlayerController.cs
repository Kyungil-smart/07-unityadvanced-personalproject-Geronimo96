using JetBrains.Rider.Unity.Editor;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // 플레이어의 이동 속도, 마우스 감도, 카메라 회전 제한 각도를 인스펙터에서 조절할 수 있도록 SerializeField로 선언한다.
    
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed = 7f;
    [SerializeField]
    private float runSpeed = 30f;
    private float applySpeed;

    [SerializeField]
    private float crouchSpeed = 2f;

    [SerializeField]
    private float jumpForce = 5f;

    // 상태 변수
    private bool isRun = false;
    private bool isGround = true;
    private bool isCrouch = true;

    // 지면에 닿았는지 확인하기 위한 콜라이더 변수
    private CapsuleCollider capsuleCollider;

    // 앉았을 때 콜라이더의 높이와 중심을 조절하기 위한 변수
    [SerializeField]
    private float crouuchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // 카메라 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 회전 제한 각도
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    // 사용하는 컴포넌트
    [SerializeField]
    private Camera playerCamera;
    private Rigidbody myRigid;
    private GunController theGunController;

    // 시작 설정
    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        theGunController = FindFirstObjectByType<GunController>();

        // 초기화
        originPosY = playerCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // 매 프레임마다 이동과 카메라 회전, 캐릭터 회전을 처리하는 Update 함수를 구현한다.
    private void Update()
    {
        IsGround();
        TryRun();
        TryJump();
        TryCrouch();
        Move();
        CameraRotation();
        CharatorRotation();
    }

    // 달리기 시 스피드 조정 함수
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isNotRunning();
        }
    }

    // 달리기 실행
    private void isRunning()
    {
        // 웅크린 상태에서 달릴 시 웅크림 해제
        if (isCrouch) Crouch();

        theGunController.TryFineSightOff();

        applySpeed = runSpeed;
        isRun = true;
    }
    // 달리기 해제
    private void isNotRunning()
    {
        applySpeed = walkSpeed;
        isRun = false;
    }

    // 점프
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    // 점프 함수는 플레이어의 Rigidbody에 위쪽 방향으로 힘을 가하는 방식으로 구현한다.
    // 점프할 때는 플레이어의 현재 속도에 점프 힘을 더하는 방식으로 구현한다. 이렇게 하면 플레이어가 이동 중에도 점프할 수 있다.
    private void Jump()
    {
        // 웅크린 상태에서 점프 시 웅크림 해제
        if (isCrouch) Crouch();

        myRigid.linearVelocity = transform.up * jumpForce;
    }

    // 웅크리기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //웅크리기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch;

        if (isCrouch)
        {   
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouuchPosY;

        }
        else
        {    
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    // 웅크리기 부드러운 동작을 위한 코루틴 함수
    IEnumerator CrouchCoroutine()
    {
        float _posY = playerCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, Time.deltaTime * 5f);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                _posY,
                playerCamera.transform.localPosition.z);

            // 보간 단점 보완 ( 정확한 위치에 일치시켜라 )
            count++;
            if (count == 15) break;

            yield return null;
        }
        playerCamera.transform.localPosition = new Vector3(
            playerCamera.transform.localPosition.x,
            applyCrouchPosY,
            playerCamera.transform.localPosition.z);
    }

    // 지면 체크
    private void IsGround()
    {
        // 플레이어가 지면에 닿았는지 확인하기 위해, 플레이어의 위치에서
        // 콜라이더의 높이만큼 아래로 레이를 쏴서 충돌 여부를 확인한다.
        isGround = Physics.Raycast(transform.position, Vector3.down, 
            capsuleCollider.bounds.extents.y + 0.1f);
    }

    // 기본 이동 함수
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * _moveDirX;
        Vector3 moveVertical = transform.forward * _moveDirZ;

        Vector3 velocity = (moveHorizontal + moveVertical)
            .normalized * applySpeed;
        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    // 
    private void CameraRotation()
    {
        // 마우스의 y축 움직임을 카메라의 x축 회전에 적용한다. (마우스의 x축 움직임은 캐릭터의 y축 회전에 적용한다.)
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharatorRotation()
    {
        // 위 아래 카메라 회전과 달리 좌우 캐릭터 회전은 카메라가 아닌 캐릭터의 y축을 기준으로 회전해야 한다.
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 charatorRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(charatorRotationY));
       // Debug.Log(myRigid.rotation);
       // Debug.Log(myRigid.rotation.eulerAngles);
    }
}
