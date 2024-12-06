using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WASDMovement : LivingEntity {

    private Gun gun = null;

	public float speed = 20f;
    private Vector3 moveVec = Vector3.zero;
    private float haxis;
    private float vaxis;

    private void Awake()
    {
        gun = GetComponentInChildren<Gun>();
    }


    void FixedUpdate () {
        //Vector3 pos = transform.position;

        if (gun != null && Input.GetMouseButton(0))
        {
            gun.Fire();
        }

        haxis = Input.GetAxis("Horizontal");
        vaxis = Input.GetAxis("Vertical");

        moveVec = new Vector3(haxis, 0, vaxis).normalized;
        transform.position += moveVec * speed * Time.deltaTime;

        transform.LookAt(transform.position + moveVec);

        //if (Input.GetKey ("w")) {
        //    pos.z += speed * Time.deltaTime;
        //}

        //if (Input.GetKey ("s")) {
        //    pos.z -= speed * Time.deltaTime;
        //}

        //if (Input.GetKey ("d")) {
        //    pos.x += speed * Time.deltaTime;
        //}

        //if (Input.GetKey ("a")) {
        //    pos.x -= speed * Time.deltaTime;
        //}
         
        // transform.position = pos;
    }

    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        Debug.Log("Health : "+health);
    }
}
