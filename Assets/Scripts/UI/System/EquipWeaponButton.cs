using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(EquipWeaponButton))]
public sealed class EquipWeaponButton : UpdateSystem 
{
    private Filter filter;
    private Stash<EquipWeaponButtonComponent> equipStash;
    private Request<EquipWeaponRequest> bRequest; 
    private Event<EquipWeaponEvent> bEvent; 
    
    public override void OnAwake() 
    {
        filter = World.Filter.With<EquipWeaponButtonComponent>().Build();
        equipStash = World.GetStash<EquipWeaponButtonComponent>();
        
        bRequest = World.GetRequest<EquipWeaponRequest>();
        bEvent = World.GetEvent<EquipWeaponEvent>();

        foreach (var entity in filter)
        {
            ref var button = ref equipStash.Get(entity);
            
            button.button.onClick.AddListener(() =>
            {
                bRequest.Publish(new EquipWeaponRequest());

                foreach (var request in bRequest.Consume())
                {
                    bEvent.NextFrame(new EquipWeaponEvent());
                }
            });
        }
    }

    public override void OnUpdate(float deltaTime) 
    {
        
    }
}