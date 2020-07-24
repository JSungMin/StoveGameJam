using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.Dialogue;
using UnityEngine;

namespace CoreSystem.Game.Dialogue
{
    public interface IScriptBehaviour
    {
        void Initialize(ScriptObject obj);
        //  All Play Action Invoked in Update
        void Play(Action<bool> onComplete = null);
        void Stop();
    }
}
