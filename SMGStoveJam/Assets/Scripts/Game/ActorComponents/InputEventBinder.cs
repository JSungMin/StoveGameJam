using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.ActorComponents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreSystem.Game
{
    public class InputEventBinder : MonoBehaviour
    {
        public ActorBrain targetBrain;
        #region Player Input Callback Region
        //  Used In Keyboard&Mouse Schema
        public void OnMove(Vector2 value)
        {
            targetBrain.OnReceiveInput("OnMove", value);
        }
        //  Used In Other Schema
        public void OnMove(InputValue val)
        {
            targetBrain.OnReceiveInput("OnMove", val.Get<Vector2>());
        }
        public void OnLookUp()
        {
            targetBrain.OnReceiveInput("OnLookUp");
        }
        public void OnLookDown()
        {
            targetBrain.OnReceiveInput("OnLookDown");
        }
        public void OnJump()
        {
            targetBrain.OnReceiveInput("OnJump");
        }
        public void OnFire()
        {
            targetBrain.OnReceiveInput("OnFire");
        }

        public void OnParry()
        {
            targetBrain.OnReceiveInput("OnParry");
        }

        public void OnAvoid()
        {
            targetBrain.OnReceiveInput("OnAvoid");
        }

        public void OnInteract()
        {
            targetBrain.OnReceiveInput("OnInteract");
        }
        #endregion
    }
}
