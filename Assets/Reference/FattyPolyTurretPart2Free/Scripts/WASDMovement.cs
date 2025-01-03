﻿using UnityEngine;

public class WASDMovement : MonoBehaviour {

    private Gun gun = null;
    private Animator anim = null;

	public float speed = 20f;
    private Vector3 moveVec = Vector3.zero;
    private float haxis;
    private float vaxis;
    private float rotationSpeed = 5f;
    private Quaternion rotation = Quaternion.Euler(0, -45f, 0);



    private void Awake()
    {
        gun = GetComponentInChildren<Gun>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }


    private void Update () {

        // 애니메이션 처리
        PlayAnimation();

        //Vector3 pos = transform.position;

        if (gun != null && Input.GetMouseButton(0))
        {
            gun.Fire();
        }

        haxis = Input.GetAxis("Horizontal");
        vaxis = Input.GetAxis("Vertical");



        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButton(0))
        {
            // 클릭한 화면 좌표를 월드 좌표로 변환
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // 클릭한 위치로 캐릭터 회전
                RotateTowards(hitInfo.point);
                moveVec = new Vector3(haxis, 0, vaxis);

                Vector3 moveRoVec = (rotation * moveVec).normalized;
                if (CheckWall(moveRoVec)) 
                    moveRoVec = Vector3.zero;
                transform.position += moveRoVec * speed * Time.deltaTime;
            }
        }
        else
        {
            moveVec = new Vector3(haxis, 0, vaxis);

            Vector3 moveRoVec = (rotation * moveVec).normalized;
            if (CheckWall(moveRoVec))
                moveRoVec = Vector3.zero;
            transform.position += moveRoVec * speed * Time.deltaTime;
        
            transform.LookAt(transform.position + moveRoVec);
        }
    }

    private bool CheckWall(Vector3 moveVec)
    {
        if (Physics.Raycast(transform.position, moveVec, out RaycastHit hit, 1f))
        {
            if (hit.collider.CompareTag("Fortress"))
            {
                return true;
            }
        }
        return false;
    }

    private void PlayAnimation()
    {
        anim.SetFloat("Forward", vaxis);
        anim.SetFloat("Right", haxis);

        //bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        //anim.SetBool("Sprint", isSprinting);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        // 클릭한 위치와 캐릭터의 높이 맞추기
        targetPosition.y = transform.position.y;
    
        // 목표 방향 계산
        Vector3 direction = (targetPosition - transform.position).normalized;
    
        // 목표 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(direction);
    
        // 현재 회전값에서 목표 회전값으로 보간하여 회전
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    
        transform.rotation = targetRotation;
    }
}
