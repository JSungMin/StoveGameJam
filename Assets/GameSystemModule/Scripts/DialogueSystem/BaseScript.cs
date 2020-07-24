using System;
using System.Collections.Generic;

namespace CoreSystem.Game.Dialogue
{
    public enum ScriptType
    {
        말풍선_대화,
        컷씬_대화,
        컷씬_대화_선택지
    }
    
    [Serializable]
    public class BaseScript
    {
        public ScriptType type;
        public ActorProfile speaker;
        public string content;
        public List<VMScript> actionScripts = new List<VMScript>();

#if UNITY_EDITOR
        public bool foldActionScripts = false;
#endif

        public BaseScript()
        {
            content = "";
        }

        public BaseScript(BaseScript s)
        {
            type = s.type;
            speaker = s.speaker;
            content = s.content;
        }

        public int CountOfAction => actionScripts.Count;
    }
}
