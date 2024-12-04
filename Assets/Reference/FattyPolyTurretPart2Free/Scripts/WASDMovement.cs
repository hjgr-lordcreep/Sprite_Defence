using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDMovement : LivingEntity {

	public float speed = 20f;

    void Update () {
        Vector3 pos = transform.position;

        if (Input.GetKey ("w")) {
            pos.z += speed * Time.deltaTime;
        }

        if (Input.GetKey ("s")) {
            pos.z -= speed * Time.deltaTime;
        }

        if (Input.GetKey ("d")) {
            pos.x += speed * Time.deltaTime;
        }

        if (Input.GetKey ("a")) {
            pos.x -= speed * Time.deltaTime;
        }
         
         transform.position = pos;
    }

    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        Debug.Log("Health : "+health);
    }
}
