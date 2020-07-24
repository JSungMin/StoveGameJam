using System.Linq;
using System.Collections.Generic;
using CoreSystem.Game.Data;

namespace CoreSystem.Game
{
    public interface ISaveable
    {
        void OnSave();
        void OnLoad();
    }

    public class StagedObjectManager : SingletonGameObject<StagedObjectManager>
    {
        public string sceneName;
        public CoreDataSet coreDataSet;
        public List<GameActor> allGameActors = new List<GameActor>();
        public List<GameActorWithSaveable> saveables = new List<GameActorWithSaveable>();
        public SavedGameData savedData;
        // Use this for initialization
        void Awake()
        {
            Instance = this;
            coreDataSet = CoreDataSet.Instance;
            allGameActors.ForEach(x => x.OnAwake());
            //LoadSceneData();
        }

        void Start()
        {
            allGameActors.ForEach(x=>x.OnStart());
        }
	    public static void AddSaveable(GameActorWithSaveable saveable)
        {
            if (!Instance.saveables.Contains(saveable))
                Instance.saveables.Add(saveable);
        }
        public static void AddGameActor(GameActor actor)
        {
            if (!Instance.allGameActors.Contains(actor))
                Instance.allGameActors.Add(actor);
        }

        public static GameActor FindGameActor(ActorProfile profile)
        {
            return Instance.allGameActors.FirstOrDefault(x => x.Profile.actorName == profile.actorName);
        }
        public static bool RemoveSaveable(GameActorWithSaveable saveable)
        {
            return Instance.saveables.Remove(saveable);
        }

        public static bool RemoveGameActor(GameActor actor)
        {
            var result =  Instance.allGameActors.Remove(actor);
    #if UNITY_EDITOR
            DestroyImmediate(actor.gameObject);
    #else
            Destroy(actor.gameObject)
    #endif
            return result;
        }
	    public static void SaveSceneData()
        {
            Instance.savedData.Save(Instance.saveables);
            JsonHelper.PersistentObjectToJsonFile(Instance.savedData, "saveFile.json");
        }
        public static void LoadSceneData()
        {
            JsonHelper.PersistentJsonFileToObject("saveFile.json", Instance.savedData);
            Instance.savedData.Load(Instance.saveables);
        }
        public void LoadGameActorInScene()
        {
            allGameActors = FindObjectsOfType<GameActor>().ToList();
        }
        public void LoadSaveablesInScene()
        {
            for (int i = 0; i < allGameActors.Count; i++)
            {
                var saveable = allGameActors[i] as GameActorWithSaveable;
                if (null != saveable)
                {
                    AddSaveable(saveable);
                }
            }
        }
    }
}
