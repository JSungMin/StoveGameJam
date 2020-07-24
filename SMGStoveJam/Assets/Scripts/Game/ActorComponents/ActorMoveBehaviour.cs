using System;
using System.Collections;
using CoreSystem.Utility;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public class ActorMoveBehaviour : ActorBehaviour
    {
        public RayCollisionDetector2D collisionDetector;
        //  TODO : 아래 둘 중 하나만 있어야함
        public CharacterController controller;
        public Rigidbody rigid;
        public BoxCollider rigidCollider;
        public ForceMode forceMode;

        public ActorBehaviourEntry.MoveEntry MoveEntry => Brain.MoveEntry;
        private int caseId = 0;

        private Action<ActorBrain> OnGround;
        private Action<ActorBrain> OnFall;
        private Action<ActorBrain> OnLanding;

        private BehaviourJob moveJob;

        public override void LoadWithStart(GameActor a)
        {
            base.LoadWithStart(a);
            if (controller == null && rigid != null)
                caseId = 1;
            else if (controller != null && rigid == null)
                caseId = 0;
            else
                caseId = -1;
        }

        public float CalcSpeed()
        {
            //  TODO : 상태이상 적용 및 프로필 스탯 적용한 속도값 반환
            var basicSpeed = actor.Profile.basicStat.elements[1];
            var addSpeed = actor.Profile.additionalStat.elements[1];

            return basicSpeed + addSpeed;
        }

        public ActorMoveBehaviour SetCallback(Action<ActorBrain> onGround, Action<ActorBrain> onFall, Action<ActorBrain> onLanding)
        {
            OnGround = onGround;
            OnFall = onFall;
            OnLanding = onLanding;
            return this;
        }
        public BehaviourJob GetJob(bool shouldStart = true)
        {
            moveJob?.Kill();
            moveJob = BehaviourJob.Make(ILoopDo(), shouldStart);
            return moveJob;
        }

        public void StopJob() => moveJob?.Kill();
        private IEnumerator ILoopDo()
        {
            while (true)
                yield return BehaveAsCoroutine();
        }
        public ActorMoveBehaviour AddForce(Vector3 force)
        {
            MoveEntry.AddVelocity(force);
            return this;
        }

        public ActorMoveBehaviour SetVelocity(Vector3 velocity)
        {
            MoveEntry.addVelocity = velocity;
            if (caseId == 0)
                controller.Move(velocity);
            else if (caseId == 1)
                rigid.velocity = velocity;
            return this;
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Rigidbody body = hit.collider.attachedRigidbody;
            //Collider col = hit.collider;
            //var prevVelocity = controller.velocity;
            //var targetVelocity = prevVelocity;
            //bool isNoRigid = (body == null || body.isKinematic);
            //// no rigidbody
            //if (isNoRigid)
            //{
            //    //Debug.Log("HIT OBJ : " + hit.gameObject.name);
            //    //Debug.Log("--Surface : " + hit.normal);
            //    //Debug.Log("--Move Direction : " + hit.moveDirection);
            //    var dot = Vector3.Dot(hit.normal, hit.moveDirection);
            //    //Debug.Log("--Dot : " + dot);
            //    if (dot < 0f)
            //    {
            //        targetVelocity.y = 0f;
            //    }
            //}
            //else
            //{
            //    // Calculate push direction from move direction,
            //    // we only push objects to the sides never up and down
            //    Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            //    // If you know how fast your character is trying to move,
            //    // then you can also multiply the push velocity by that.
            //    var pushPower = controller.velocity.magnitude * 1.25f;
            //    // Apply the push
            //    body.velocity = pushDir * pushPower;
            //}

            //BehaviourEntry.moveEntry.direction = targetVelocity.normalized;
            //BehaviourEntry.moveEntry.addVelocity = targetVelocity;
        }

        private void OnRigidbodySweep(RaycastHit hit)
        {
            var groundTest = rigidCollider.bounds.min.y > hit.point.y;
            MoveEntry.FillEntry(groundTest, rigid.velocity);
            if (groundTest && !MoveEntry.isGrounded)
                OnLanding?.Invoke(Brain);
            else if(groundTest)
                OnGround?.Invoke(Brain);
        }
        private void ApplyVelocityWithController()
        {
            var finalVelocity = MoveEntry.prevVelocity;

            var dir = MoveEntry.direction;
            var speed = Mathf.Abs(MoveEntry.addVelocity.x);
            var prevSpeed = MoveEntry.prevVelocity.magnitude;
            var jump = MoveEntry.addVelocity.y;

            finalVelocity.x += dir.x * speed;
            if(MoveEntry.isGrounded && Math.Abs(prevSpeed) > 0.01f)
                finalVelocity.x *= Mathf.Max(prevSpeed - 0.5f * prevSpeed, 0f)/ prevSpeed;
            
            finalVelocity.y += jump + Physics.gravity.y * Time.deltaTime;
            var moveVel = finalVelocity * Time.deltaTime;

            var flag = controller.Move(moveVel);
            var isGrounded = collisionDetector.TestBottomTop(moveVel).Count != 0;
            //var isGrounded = flag == CollisionFlags.Below||flag == CollisionFlags.CollidedBelow;
            var isLanding = isGrounded && !MoveEntry.isGrounded;
            MoveEntry.FillEntry(isGrounded, finalVelocity);
            if(isLanding)
                OnLanding?.Invoke(Brain);
            else if (isGrounded)
            {
                OnGround?.Invoke(Brain);
                MoveEntry.prevVelocity.y = 0f;
            }
            else
            {
                OnFall?.Invoke(Brain);
            }
        }

        private void ApplyVelocityWithRigidbody()
        {
            var curVelocity = rigid.velocity;
            var force = Vector3.zero;

            var dir = MoveEntry.direction;
            var speed = MoveEntry.addVelocity.x;
            var jump = MoveEntry.addVelocity.y;

            force.x += dir.x * speed;
            force.y += jump;

            var sweepOut = new RaycastHit();
            var sweepBool = rigid.SweepTest(dir, out sweepOut, force.magnitude * 1.1f);
            rigid.AddForce(force, forceMode);
            if (sweepBool)
            {
                OnRigidbodySweep(sweepOut);
            }
            else
            {
                MoveEntry.FillEntry(false, rigid.velocity);
                OnFall?.Invoke(Brain);
            }
        }
        protected override void Do(ActorBehaviourEntry entry)
        {
            if (caseId == 0)
            {
                ApplyVelocityWithController();
            }
            else
            {
                ApplyVelocityWithRigidbody();
            }
        }
    }
}
