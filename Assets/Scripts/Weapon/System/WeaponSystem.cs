using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Pool;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(WeaponSystem))]
public sealed class WeaponSystem : UpdateSystem 
{
    private Filter filter;
    private Stash<WeaponComponent> weaponStash;
    private Event<IsAimEvent> isAimEvent;

    private float time = 0;
    
    public override void OnAwake() 
    {
        filter = World.Filter.With<WeaponComponent>().Build();
        weaponStash = World.GetStash<WeaponComponent>();

        isAimEvent = World.GetEvent<IsAimEvent>();
        
        foreach (var entity in filter)
        {
            ref var weapon = ref weaponStash.Get(entity);
            weapon.bulletPool = 
                new ObjectPool<GameObject>(CreateBullet,
                    ActionOnGet, 
                    ActionOnRelease, 
                    ActionOnDestroy,
                    true,
                    100,
                    100);

        }
    }

    public override void OnUpdate(float deltaTime) 
    {
        foreach (var entity in filter)
        {
            ref var weapon = ref weaponStash.Get(entity);

            foreach (var aimEvent in isAimEvent.publishedChanges)
            {
                weapon.isAim = aimEvent.isAim;
            }
            
            if (weapon.isAim)
            {
                time += deltaTime;
                if(time > 0.5f)
                {
                    weapon.bulletPool.Get();
                    time = 0;
                }
            }
            else
            {
                time = 0;
            }
        }
    }

    private GameObject CreateBullet()
    {
        GameObject bullet = null;
        
        foreach (var entity in filter)
        {
            ref var weapon = ref weaponStash.Get(entity);
            var bulletPrefab = weapon.bullet;
            var position = weapon.positionSpawnBullet.position;
            var rotation = weapon.rotationSpawnBullet.rotation;
            bullet = Instantiate(bulletPrefab, position, rotation);
            bullet.GetComponent<Rigidbody2D>().AddForce(position * 100, ForceMode2D.Force);
            bullet.GetComponent<BulletCollision>().pool = weapon.bulletPool;
        }
        
        return bullet;
    }
    
    private void ActionOnGet(GameObject bullet)
    {
        foreach (var entity in filter)
        {
            ref var weapon = ref weaponStash.Get(entity);
            var position = weapon.positionSpawnBullet.position;
            bullet.transform.position = position;
            //bullet.transform.right = weapon.positionSpawnBullet.right;
            bullet.GetComponent<Rigidbody2D>().AddForce(position * 100, ForceMode2D.Force);
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody2D>().AddForce(position * 100, ForceMode2D.Force);
            Debug.Log(position.ToString());
        }
    }
    
    private void ActionOnRelease(GameObject bullet)
    {
        bullet.SetActive(false);
    }
    
    private void ActionOnDestroy(GameObject bullet)
    {
        Destroy(bullet);
    }
}