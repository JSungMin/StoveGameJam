
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.Data
{
    [System.Serializable]
    public class SavedGameData
    {
        public string saveFileName;
        public string sceneName;
        //  Saved Actor called element
        public List<SavedActorData> elements = new List<SavedActorData>();
        
        [System.Serializable]
        public class SavedActorData
        {
            public string elementName;
            public string profileJson;
            public string timerJson;
            public Vector3 position;
            public Quaternion rotation;

            public SavedActorData(GameActorWithSaveable actor)
            {
                elementName = actor.name;
                profileJson = JsonUtility.ToJson(actor.Profile);
                position = actor.transform.position;
                rotation = actor.transform.rotation;
            }
            public void CopyTo(GameActorWithSaveable actor)
            {
                JsonUtility.FromJsonOverwrite(profileJson, actor.copiedProfile);
                actor.transform.position = position;
                actor.transform.rotation = rotation;
            }
        }

        public void AddData(GameActorWithSaveable mObject)
        {
            elements.Add(new SavedActorData(mObject));
        }
        public void Save(List<GameActorWithSaveable> mObjects)
        {
            elements = new List<SavedActorData>();
            mObjects.ForEach(x =>
            {
                AddData(x);
                x.OnSave();
                //  TODO : Make SaveSceneData To File
            });
        }
        public void Load(List<GameActorWithSaveable> mObjects)
        {
            if (mObjects.Count != Count)
            {
                Debug.LogWarning("저장되지 않은 씬 오브젝트가 존재합니다.");
            }
            for (int i = 0; i < Count; i++)
            {
                var x = elements[i];
                x.CopyTo(mObjects[i]);
                mObjects[i].OnLoad();
            }
        }
        public int Count => elements.Count;
    }
}
