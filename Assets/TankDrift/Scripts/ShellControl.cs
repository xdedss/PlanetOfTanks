using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ShellControl : MonoBehaviour {

    public float speed;
    public float damnageAtSpeed;
    Rigidbody rigid;

	void Start () {
        rigid = GetComponent<Rigidbody>();
        if (rigid)
        {
            rigid.velocity += speed * transform.forward;
        }
	}
	
	void Update () {
		
	}

    private void FixedUpdate()
    {
        transform.LookAt(transform.position + rigid.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var layer = collision.collider.gameObject.layer;
        switch (layer)
        {
            case 10:
                HitGround(collision);
                break;
            case 12:
                Debug.Log("how could it collide with another shell?");
                break;
            default:
                break;
        }

        var damnageControl = collision.collider.GetComponent<DamnageControl>();
        if (damnageControl)
        {
            Hit(damnageControl, collision);
        }
    }

    private void Hit(DamnageControl dc, Collision collision)
    {
        var contact = collision.contacts[0];
        var cosvel = Mathf.Clamp01(Vector3.Dot(collision.relativeVelocity.normalized, contact.normal));
        var damnage = damnageAtSpeed * rigid.velocity.magnitude / speed * cosvel;
        dc.TakeDamnage(damnage, contact.normal, contact.point);
        Singleton<ParticleManager>.instance.Emit("SparkHit", contact.point, Quaternion.LookRotation(contact.normal + (collision.relativeVelocity + rigid.velocity)));
        Singleton<ParticleManager>.instance.Emit("DustHit", contact.point, Quaternion.LookRotation(contact.normal + (collision.relativeVelocity + rigid.velocity)));
        Destroy(gameObject);
    }

    private void HitGround(Collision collision)
    {
        var contact = collision.contacts[0];
        //Singleton<ParticleManager>.instance.Emit("SparkHit", contact.point, Quaternion.LookRotation(contact.normal));

        Singleton<ParticleManager>.instance.Emit("DustHit", contact.point, Quaternion.LookRotation(contact.normal + (collision.relativeVelocity + rigid.velocity)));
        
        //UnityEditor.EditorApplication.isPaused = true;
        Destroy(gameObject);
    }
}
