using UnityEngine;

public class LegacyMove : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f; // 이동 속도

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //w, s 키로 앞뒤로 이동
        //a, d 키로 좌우로 이동
        // 축 형태 이동

        float h = Input.GetAxisRaw("Horizontal"); // 수평 - 키 세팅(a,d) : 좌우
        float v = Input.GetAxisRaw("Vertical"); // 수직 - 키 세팅(w,s) : 앞뒤

        Vector3 move = transform.forward * v + transform.right * h; // 축 형태 이동

        this.transform.position += move * moveSpeed * Time.deltaTime; // 이동 속도 조절
    }
}