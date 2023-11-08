using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(IgnoreCollision))]
public sealed class IgnoreCollision : UpdateSystem
{
    private Filter filter;
    private Stash<RagdollComponent> ragdollStash;
    
    public override void OnAwake()
    {
        filter = World.Filter.With<RagdollComponent>().Build();
        ragdollStash = World.GetStash<RagdollComponent>();

        foreach (var entity in filter)
        {
            ref var ragdoll = ref ragdollStash.Get(entity);

            foreach (var colliderBody in ragdoll.bodyCollider)
            {
                for (int j = 0; j < 6; j++)
                {
                    Physics2D.IgnoreCollision(ragdoll.colliders[j], colliderBody);
                }
            }
            
            for (var i = 0; i < ragdoll.colliders.Length; i++)
            {
                for (var k = i+1; k < ragdoll.colliders.Length; k++)
                {
                    Physics2D.IgnoreCollision(ragdoll.colliders[i], ragdoll.colliders[k]);
                }
            }
        }
    }

    public override void OnUpdate(float deltaTime) 
    {
        
    }
}