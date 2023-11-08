using System;
using UnityEngine;
using UnityEngine.Pool;

public class BulletCollision : MonoBehaviour
{
    public ObjectPool<GameObject> pool;

    private float time = 0;
    
    private void Update()
    {
        time += Time.deltaTime;
        
        if (time > 3)
        {
            pool.Release(this.gameObject);
            time = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Weapon") && !other.gameObject.CompareTag("Bullet"))
        {
            if (gameObject != null)
            {
                pool.Release(this.gameObject);
            }
        }
    }
}
