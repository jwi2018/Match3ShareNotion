using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoulettePinController : MonoBehaviour
{
    HingeJoint2D hingeJointPIN = null;
    Rigidbody2D rigidBodyPIN = null;
    GameObject _gobBeforeEnter = null;

    private void Awake()
    {
        hingeJointPIN = GetComponent<HingeJoint2D>();
        rigidBodyPIN = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.LogWarning("OnTriggerEnter2D");
        if(_gobBeforeEnter != collision.gameObject)
        {
            _gobBeforeEnter = collision.gameObject;
            rigidBodyPIN.AddForceAtPosition(new Vector2(-10f, 0f), new Vector2(0f, -40f), ForceMode2D.Impulse);
        }
    }

}
