using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun currentGun;
    
    private float currentFireRate;
    
    private AudioSource audioSource;

    private bool isReload = false;
    private bool isFineSightMode = false;

    [SerializeField]
    private Vector3 originPos;


    void Start()
    {
        // 1. 마우스 커서를 화면에서 보이지 않게 숨깁니다.
         Cursor.visible = false;

        // 2. 마우스 커서를 게임 화면 중앙에 고정합니다. (슈팅 게임 화면 회전에 필수)
         Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();
        originPos = currentGun.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();
        TryFineSight();
    }

    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
    }

    private void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }

    private void Fire() // 발사 전
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
            {
                Shoot();
            }
            else
            {
                TryFineSightOff();
                StartCoroutine(ReloadCoroutine());
            }
        }
        
    }

    private void Shoot() // 발사 후
    {
        currentGun.currentBulletCount--; // 총알 수 감소
        currentFireRate = currentGun.fireRate; // 연사 속도 재계산
        PlaySE(currentGun.fireSound);
        currentGun.muzzleFlash.Play();

        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
        Debug.Log("Bang!");
    }

    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount <= currentGun.reloadBulletCount)
        {
            TryFineSightOff();
            StartCoroutine(ReloadCoroutine());
        }
    }

    // 수정된 부분: 함수명 오타(Coroutiine -> Coroutine) 수정
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount; // 현재 총알 수를 예비 총알 수에 더해줍니다.
            currentGun.currentBulletCount = 0; // 현재 총알 수를 0으로 초기화합니다.

            // 💡 디버그용 로그: 콘솔창에 이 글씨가 뜨는지 꼭 확인해 주세요!
            // Debug.Log("재장전 로직 진입 - 애니메이션 트리거 작동!");
         
            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
            Debug.Log("재장전 완료! 남은 예비 총알: " + currentGun.carryBulletCount);
        }
        else
        {
            Debug.Log("예비 총알이 없어서 장전을 할 수 없습니다!");
        }
    }

    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }

    }

    public void TryFineSightOff()
    {
        if (isFineSightMode)
        {
            FineSight();
        }
    }

    private void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);

        if (isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActiveCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());

        }
    }

    IEnumerator FineSightActiveCoroutine()
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = 
                Vector3.Lerp(currentGun.transform.localPosition, 
                currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }
    
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(originPos.x, 
            originPos.y,
            currentGun.retroActionForce);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.fineSightOriginPos.x, 
            currentGun.fineSightOriginPos.y,
            currentGun.retroActionFineSightForce);

        if (!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos;

            // 반동 시작
            while (currentGun.transform.localPosition.z <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // 총구 원위치
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // 정조준 상태 시 반동 시작
            while (currentGun.transform.localPosition.z <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 정조준 상태 시 총구 원위치
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
