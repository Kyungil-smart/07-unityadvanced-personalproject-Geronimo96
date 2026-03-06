using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName; // 총의 이름
    public float range; // 총의  사정거리
    public float accuracy; // 총의 명중률
    public float fireRate; // 총의 연사력
    public float reloadTime; // 총의 재장전 시간

    public int damage; // 총의 데미지

    public int reloadBulletCount; // 재장전 시 필요한 총알 수
    public int currentBulletCount; // 현재 총알 수
    public int maxBulletCount; // 총알의 최대 수
    public int carryBulletCount; // 소지한 총알 수

    public float retroActionForce; // 총의 반동 힘
    public float retroActionFineSightForce; // 정조준 시 반동 감소 비율

    public Vector3 fineSightOriginPos; // 정조준 시 총의 위치

    public Animator anim; // 총의 애니메이터

    public ParticleSystem muzzleFlash; // 총구 화염 효과

    public AudioClip fireSound; // 총성 
}
