using UnityEngine;

public class PlayerConrtoller : MonoBehaviour
{
    private Animator anim = null;
    private Transform tr = null;
    private GameObject go = null;

    private float mouseX = 0;

    private float mouseSensitivity = 5f;


    private void Start()
    {
        anim = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        go = GetComponent<GameObject>();
    }

    private void Update()
    {
        PlayerMove();

        // 마우스 수평축 움직임
        if (InputMouse(ref mouseX))
        {
            InputMouseProcess(mouseX);
        }

        if (Input.GetMouseButton(0))
        {
            anim.SetBool("Fire", true);
        }
        else if (Input.GetMouseButton(1))
        {
            anim.SetBool("Fire", false);
        }
    }

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

        anim.SetFloat("Forward", axisV);
        anim.SetFloat("Backward", -axisV);
        anim.SetFloat("Left", -axisH);
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
}
