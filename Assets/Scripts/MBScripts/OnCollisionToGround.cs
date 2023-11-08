using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionToGround : MonoBehaviour
{
    public bool isGround;
    public bool isWeapon;

    public Action<GameObject> weaponGameObject;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
            isGround = true;
        
        if(other.gameObject.CompareTag("Weapon"))
        {
            isWeapon = true;
            weaponGameObject.Invoke(other.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
            isGround = false;
        
        if(other.gameObject.CompareTag("Weapon"))
        {
            isWeapon = false;
            weaponGameObject.Invoke(null);
        }
    }
}
