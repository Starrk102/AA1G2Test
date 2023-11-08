using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Pool;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct WeaponComponent : IComponent
{
    public bool isAim;
    public GameObject bullet;
    public Transform positionSpawnBullet;
    public Transform rotationSpawnBullet;
    public ObjectPool<GameObject> bulletPool;
}