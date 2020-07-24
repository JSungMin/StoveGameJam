using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game
{
    public class GameSaver : SingletonGameObject<GameSaver> {
        public string savePath;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                StagedObjectManager.SaveSceneData();
            else if (Input.GetKeyDown(KeyCode.O))
                StagedObjectManager.LoadSceneData();
        }
    }
}
