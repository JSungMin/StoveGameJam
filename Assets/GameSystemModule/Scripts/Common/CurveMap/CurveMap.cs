using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Utility.CurveMap
{
    public class CurveMap : SingletonGameObject<CurveMap>
    {
        public bool isInit = false;
        public CurveMapProfile profile;

        private readonly Dictionary<string, AnimationCurve> nameProfileMap = new Dictionary<string, AnimationCurve>(); 

        public void Awake()
        {
            if(!isInit) Initialize();
        }

        public void OnDisable()
        {
            nameProfileMap.Clear();

        }
        public CurveMap Initialize()
        {
            if (profile == null)
                return this;
            foreach (var data in profile.curves)
            {
                nameProfileMap[data.name] = data.curve;
            }
            isInit = true;
            return this;
        }

        public static AnimationCurve GetCurveData(string curveName)
        {
            return Instance.nameProfileMap[curveName];
        }

        public static AnimationCurve GetCurveData(int idx)
        {
            return Instance.profile.curves[idx].curve;
        }
    }
}
