using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Animator anim = null;
    private Transform tr = null;

    private float mouseX = 0;
    private float mouseSensitivity = 5f;

    [SerializeField]
    private MuzzleController muzzleController;
    //[SerializeField]
    //private Transform target;
    //[SerializeField]
    //private Transform gunObject;


    private void Start()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
    }

    private void Update()
    {
        PlayerMove();

        // 마우스 수평축 움직임
        if (InputMouse(ref mouseX))
        {
            InputMouseProcess(mouseX);
        }

        // 마우스 왼쪽 버튼으로 총 발사
        if (Input.GetMouseButton(0))
        {
            anim.SetBool("Fire", true);
            muzzleController.SetFlicker(true);
        }
        else
        {
            anim.SetBool("Fire", false);
            muzzleController.SetFlicker(false);
        }
    }
   //private void LateUpdate()
   //{
   //    if (gunObject != null && target != null)
   //    {
   //        gunObject.LookAt(transform.position);
   //    }
   //}


        private bool InputMouse(ref float _mouseX)
    {
        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");

        if (mX == 0f && mY == 0f) return false;

        _mouseX += mX * mouseSensitivity;

        return true;
    }
    private void InputMouseProcess(float _mouseX)
    {
        tr.rotation =
            Quaternion.Euler(0f, _mouseX, 0f);
    }

    private void PlayerMove()
    {
        float axisV = Input.GetAxis("Vertical");
        float axisH = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.None))
        {
            anim.SetBool("Move", true);
        }
        else
            anim.SetBool("Move", false);

        anim.SetFloat("Forward", axisV);
        anim.SetFloat("Right", axisH);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("Sprint", true);
        }
        else
        {
            anim.SetBool("Sprint", false);
        }
    }

   //private void OnAnimatorIK(int layerIndex)
   //{
   //    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
   //    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
   //
   //    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
   //    anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
   //
   //    anim.SetIKPosition(AvatarIKGoal.RightHand, gunObject.position);
   //    anim.SetIKRotation(AvatarIKGoal.RightHand, gunObject.rotation);
   //
   //    gunObject.LookAt(target);
   //}
}
