using System;
using System.Collections;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(RagdollController))]
public sealed class RagdollController : UpdateSystem 
{
    private Filter filter;
    private Stash<RagdollComponent> ragdollStash;
    private Event<JoystickMovementEvent> joystickMovementEvent;
    private Event<JoystickRotationEvent> joystickRotationEvent;
    private Event<EquipWeaponEvent> equipWeaponEvent;

    private Request<IsAimRequest> isAimRequest;
    private Event<IsAimEvent> isAimEvent;

    private float horizontalMovement;
    private float verticalMovement;
    private float horizontalRotation;
    private float verticalRotation;

    private GameObject foundWeapon;
    
    private float speed = 500;
    //private float jump = 15;
    
    public override void OnAwake() 
    {
        filter = World.Filter.With<RagdollComponent>().Build();
        ragdollStash = World.GetStash<RagdollComponent>();
        
        joystickMovementEvent = World.GetEvent<JoystickMovementEvent>();
        joystickRotationEvent = World.GetEvent<JoystickRotationEvent>();
        equipWeaponEvent = World.GetEvent<EquipWeaponEvent>();
        isAimRequest = World.GetRequest<IsAimRequest>();
        isAimEvent = World.GetEvent<IsAimEvent>();

        foreach (var entity in filter)
        {
            ref var ragdoll = ref ragdollStash.Get(entity);
            ragdoll.collisionEvents.weaponGameObject = objectCollision =>
            {
                foundWeapon = objectCollision;
            };
        }
    }

    public override void OnUpdate(float deltaTime) 
    {
        foreach (var entity in filter)
        {
            ref var ragdoll = ref ragdollStash.Get(entity);
            
            foreach (Sustainability sustainability in ragdoll.sustainability)
            {
                sustainability.EnableSustainability(deltaTime);
            }

            foreach (var jM in joystickMovementEvent.publishedChanges)
            {
                horizontalMovement = jM.Movement.x;
                verticalMovement = jM.Movement.y;
            }

            foreach (var jR in joystickRotationEvent.publishedChanges)
            {
                horizontalRotation = jR.Rotation.x;
                verticalRotation = jR.Rotation.y;
            }

            foreach (var qEvent in equipWeaponEvent.publishedChanges)
            {
                if (!ragdoll.isWeaponEquipment)
                {
                    if (ragdoll.collisionEvents.isWeapon)
                    {
                        ragdoll.weapon = foundWeapon;
                        
                        foreach (var collider2D in ragdoll.sustainability)
                        {
                            Physics2D.IgnoreCollision(ragdoll.weapon.GetComponent<Collider2D>(), collider2D.bone.gameObject.GetComponent<Collider2D>());
                        }
                        
                        ragdoll.weapon.transform.parent = ragdoll.sustainability[11].bone.transform;
                        ragdoll.weapon.transform.position = ragdoll.sustainability[11].bone.transform.position;
                        ragdoll.weapon.transform.rotation = new Quaternion(0f,0f,-0.707f,0.707f);
                        ragdoll.weapon.AddComponent<HingeJoint2D>();
                        ragdoll.weapon.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
                        ragdoll.weapon.GetComponent<HingeJoint2D>().connectedBody = ragdoll.sustainability[11].bone;
                        ragdoll.weapon.GetComponent<HingeJoint2D>().anchor = new Vector2(-0.5f, 0 );
                        ragdoll.weapon.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
                        ragdoll.weapon.GetComponent<HingeJoint2D>().useLimits = true;
                        ragdoll.weapon.GetComponent<HingeJoint2D>().limits = new JointAngleLimits2D { min = 90, max = 90 };
                        ragdoll.isWeaponEquipment = true;
                    }
                }
                else
                {
                    foreach (var collider2D in ragdoll.sustainability)
                    {
                        Physics2D.IgnoreCollision(ragdoll.weapon.GetComponent<Collider2D>(), collider2D.bone.gameObject.GetComponent<Collider2D>(), false);
                    }
                    
                    ragdoll.weapon.transform.parent = null;
                    Destroy(ragdoll.weapon.GetComponent<HingeJoint2D>());
                    ragdoll.isWeaponEquipment = false;
                    ragdoll.weapon = foundWeapon;
                }
            }

            if (horizontalRotation != 0 || verticalRotation != 0)
            {
                var z = Mathf.Atan2(horizontalRotation, -verticalRotation) * Mathf.Rad2Deg;

                for (int i = 9; i < 15; i++)
                {
                    ragdoll
                        .sustainability[i]
                        .bone
                        .MoveRotation(Mathf.LerpAngle(ragdoll.sustainability[11].bone.rotation, 
                            z, 
                            300 * deltaTime));

                    ragdoll.isAim = true;
                }
            }
            else
            {
                for (int i = 9; i < 15; i++)
                {
                    ragdoll.sustainability[i].rotation = 0;
                }
                
                ragdoll.isAim = false;
            }
            
            isAimRequest.Publish(new IsAimRequest()
            {
                isAim = ragdoll.isAim
            });

            foreach (var request in isAimRequest.Consume())
            {
                isAimEvent.NextFrame(new IsAimEvent()
                {
                    isAim = request.isAim
                });
            }
            
            if (horizontalMovement != 0)
            {
                if (horizontalMovement > 0 && verticalMovement < 0.35f)
                {
                    ragdoll.animator.Play("RightWalk");
                    ragdoll.sustainability[0].rotation = ragdoll.leftRightLegs[0].rotation;
                    ragdoll.sustainability[3].rotation = ragdoll.leftRightLegs[1].rotation;
                    MoveRight(ragdoll.sustainability[7].bone, deltaTime);
                }
                else if(verticalMovement < 0.35f)
                {
                    ragdoll.animator.Play("LeftWalk");
                    ragdoll.sustainability[0].rotation = ragdoll.leftRightLegs[0].rotation;
                    ragdoll.sustainability[3].rotation = ragdoll.leftRightLegs[1].rotation;
                    MoveLeft(ragdoll.sustainability[7].bone, deltaTime);
                }
            }
            else
            {
                ragdoll.animator.Play("Idle");
                ragdoll.sustainability[0].rotation = 0;
                ragdoll.sustainability[3].rotation = 0;
            }

            if (verticalMovement != 0)
            {
                if (verticalMovement > 0 && horizontalMovement < 0.35f)
                {
                    if (ragdoll.collisionEvents.isGround)
                    {
                        ragdoll.sustainability[0].rotation = ragdoll.leftRightLegs[0].rotation;
                        ragdoll.sustainability[3].rotation = ragdoll.leftRightLegs[1].rotation;
                        Jump(ragdoll.sustainability[0].bone,  ragdoll.sustainability[3].bone);
                    }
                }
            }
        }
    }

    void Jump(Rigidbody2D leftLeg, Rigidbody2D rightLeg)
    {
        leftLeg.AddForce(Vector2.up * speed * 2);
        rightLeg.AddForce(Vector2.up * speed * 2);
    }
    
    void MoveRight(Rigidbody2D body, float time)
    {
        body.AddForce(Vector2.right * speed * 40 * time);
    }
    
    void MoveLeft(Rigidbody2D body, float time)
    {
        body.AddForce(Vector2.left * speed * 30 * time);
    }
}