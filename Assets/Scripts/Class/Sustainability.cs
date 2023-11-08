using System;
using UnityEngine;

[Serializable]
public class Sustainability
{
    public Rigidbody2D bone;
    public float rotation;
    public float force;

    public void EnableSustainability(float time)
    {
        var lerpAngle = Mathf.LerpAngle(bone.rotation, rotation, force * time);
        bone.MoveRotation(lerpAngle);
    }
}