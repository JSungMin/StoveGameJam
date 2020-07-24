using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreSystem.Additional.CommandAction
{
    [CreateAssetMenu(menuName = "GameData/CommandAction/SkillCommand")]
    public class SkillCommand : ScriptableObject
    {
        [System.Serializable]
        public class InputDesc
        {
            public int frameThreshold;
            public string actionName;

            public bool Equal(InputDesc comp)
            {
                var frameCond = frameThreshold == comp.frameThreshold;
                var actionCond = actionName == comp.actionName;
                return frameCond && actionCond;
            }

            public bool Equal(int threshold, string action)
            {
                var frameCond = frameThreshold == threshold;
                var actionCond = actionName == action;
                return frameCond && actionCond;
            }
        }

        public InputDesc[] commandDescArray;
        public SkillProfile profile;

        public int CountOfCommand => commandDescArray.Length;
         
        public InputDesc GetDesc(int idx) => commandDescArray[idx];
        public int GetThreshold(int idx) => commandDescArray[idx].frameThreshold;
        public string GetActionName(int idx) => commandDescArray[idx].actionName;

    }
}
