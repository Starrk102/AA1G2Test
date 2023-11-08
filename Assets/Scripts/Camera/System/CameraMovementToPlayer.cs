using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CameraMovementToPlayer))]
public sealed class CameraMovementToPlayer : UpdateSystem 
{
    private Filter filter;
    private Stash<CameraComponent> cameraStash;
    
    public override void OnAwake() 
    {
        filter = World.Filter.With<CameraComponent>().Build();
        cameraStash = World.GetStash<CameraComponent>();
    }

    public override void OnUpdate(float deltaTime) 
    {
        foreach (var entity in filter)
        {
            ref var camera = ref cameraStash.Get(entity);
            var pos = new Vector3(camera.player.transform.position.x, camera.camera.transform.position.y, camera.camera.transform.position.z);
            camera.camera.transform.position = pos;
        }
    }
}