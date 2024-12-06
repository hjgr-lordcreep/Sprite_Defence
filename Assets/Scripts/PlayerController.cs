using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : LivingEntity
{
    private Gun gun = null;

    private Animator anim = null;
    private Transform tr = null;

    public float speed = 20f;
    private Vector3 moveVec = Vector3.zero;
    private float haxis;
    private float vaxis;

    private float mouseX = 0;
    public float mouseSensitivity = 5f;

    //[SerializeField]
    //private MuzzleController muzzleController;
    //[SerializeField]
    //private Transform target;
    //[SerializeField]
    //private Transform gunObject;

    private void Awake()
    {
        gun = GetComponentInChildren<Gun>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        PlayerAnim();

        //Vector3 pos = transform.position;

        if (gun != null && Input.GetMouseButton(0))
        {
            anim.SetBool("Fire", true);
            //muzzleController.SetFlicker(true);
            gun.Fire();
        }
        else
        {
            anim.SetBool("Fire", false);
            //muzzleController.SetFlicker(false);
        }

        haxis = Input.GetAxis("Horizontal");
        vaxis = Input.GetAxis("Vertical");

        //moveVec = new Vector3(haxis, 0, vaxis).normalized;
        //transform.position += moveVec * speed * Time.deltaTime;

        transform.LookAt(transform.position + moveVec);

        // 마우스 수평축 움직임
        if (InputMouse(ref mouseX))
        {
            InputMouseProcess(mouseX);
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

    private void PlayerAnim()
    {
        anim.SetFloat("Forward", vaxis);
        anim.SetFloat("Right", haxis);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("Sprint", true);
        }
        else
        {
            anim.SetBool("Sprint", false);
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint,
    Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        Debug.Log("Health : " + health);
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
