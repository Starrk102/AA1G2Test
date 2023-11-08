using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(JoystickRotation))]
public sealed class JoystickRotation : UpdateSystem 
{
    private Filter filter;
    private Stash<JoystickRotationComponent> joystickStash;
    private Request<JoystickRotationRequest> jRequest; 
    private Event<JoystickRotationEvent> jEvent; 
    
public override void OnAwake() 
    {
        filter = World.Filter.With<JoystickRotationComponent>().Build();
        joystickStash = World.GetStash<JoystickRotationComponent>();

        jRequest = World.GetRequest<JoystickRotationRequest>();
        jEvent = World.GetEvent<JoystickRotationEvent>();
        
        foreach (var entity in filter)
        {
            ref var joystick = ref joystickStash.Get(entity);

            joystick.axisOptions = AxisOptions.Both;
            Vector2 center = new Vector2(0.5f, 0.5f);
            joystick.background.pivot = center;
            joystick.handle.anchorMin = center;
            joystick.handle.anchorMax = center;
            joystick.handle.pivot = center;
            joystick.handle.anchoredPosition = Vector2.zero;
            joystick.input = Vector2.zero;

            joystick.sX = false;
            joystick.sY = false;

            joystick.direction = new Vector2(joystick.horizontal, joystick.vertical);

            joystick.joystickEvents.pointerDown += () => { };

            joystick.joystickEvents.pointerUp += () =>
            {
                foreach (var entity in filter)
                {
                    ref var joystick = ref joystickStash.Get(entity);
                    joystick.input = Vector2.zero;
                    joystick.handle.anchoredPosition = Vector2.zero;
                }
            };

            joystick.joystickEvents.pointerDrag += (eventData) =>
            {
                foreach (var entity in filter)
                {
                    ref var joystick = ref joystickStash.Get(entity);

                    Vector2 position =
                        RectTransformUtility.WorldToScreenPoint(joystick.camera, joystick.background.position);
                    Vector2 radius = joystick.background.sizeDelta / 2;
                    joystick.input = (eventData.position - position) / (radius * joystick.canvas.scaleFactor);
                    HandleInput(joystick.input.magnitude, joystick.input.normalized, joystick.deadZone,
                        joystick.input);
                    joystick.handle.anchoredPosition = joystick.input * radius * joystick.handleRange;
                }
            };
        }
    }

    public override void OnUpdate(float deltaTime) 
    {
        foreach (var entity in filter)
        {
            ref var joystick = ref joystickStash.Get(entity);
            
            joystick.horizontal = joystick.sX
                ? SnapFloat(joystick.input.x,
                    AxisOptions.Horizontal,
                    joystick.axisOptions,
                    joystick.input)
                : joystick.input.x;
            
            joystick.vertical = joystick.sY
                ? SnapFloat(joystick.input.y,
                    AxisOptions.Vertical,
                    joystick.axisOptions,
                    joystick.input)
                : joystick.input.y;

            jRequest.Publish(new JoystickRotationRequest()
            {
                Rotation = new Vector2(joystick.horizontal, joystick.vertical)
            });

            foreach (var request in jRequest.Consume())
            {
                jEvent.NextFrame(new JoystickRotationEvent()
                {
                    Rotation = request.Rotation
                });
            }
        }
    }
    
    private void HandleInput(float magnitude, 
        Vector2 normalised,
        float deadZone,
        Vector2 input)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
            {
                input = normalised;
                
                foreach (var entity in filter)
                {
                    ref var joystick = ref joystickStash.Get(entity);

                    joystick.input = input;
                }
            }
        }
        else
        {
            input = Vector2.zero;
            
            foreach (var entity in filter)
            {
                ref var joystick = ref joystickStash.Get(entity);

                joystick.input = input;
            }
        }
    }
    
    private float SnapFloat(float value, AxisOptions snapAxis, AxisOptions axisOptions, Vector2 input)
    {
        if (value == 0)
            return value;
        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            
        }

        return value;
    }
}