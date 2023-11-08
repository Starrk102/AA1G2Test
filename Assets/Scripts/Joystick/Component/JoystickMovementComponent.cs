using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct JoystickMovementComponent : IComponent
{
    public float horizontal;
    public float vertical;
    public Vector2 direction;
    
    public RectTransform background;
    public RectTransform handle;
    public AxisOptions axisOptions;
    public float handleRange;
    public float deadZone;
    public bool sX;
    public bool sY;

    public Vector2 input;

    public RectTransform baseRect;
    
    public JoystickEvents joystickEvents;
    public Camera camera;
    public Canvas canvas;
}