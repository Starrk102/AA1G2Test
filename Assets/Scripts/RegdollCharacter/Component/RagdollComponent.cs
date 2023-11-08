using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Serialization;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct RagdollComponent : IComponent
{
    public Collider2D[] colliders;
    public Collider2D[] bodyCollider;
    public Sustainability[] sustainability;
    public OnCollisionToGround collisionEvents;
    public Animator animator;
    public AnimationScript[] leftRightLegs;
    public Characteristics[] characteristics;
    public GameObject weapon;
    public bool isWeaponEquipment;
    public bool isAim;
}