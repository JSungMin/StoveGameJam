using System;

namespace CoreSystem.Game
{
    public class GameActorWithSaveable : GameActor, ISaveable
    {
        protected Action onLoad;
        protected Action onSave;

        public override void LoadWithAwake()
        {
            base.LoadWithAwake();
        }
        public void OnLoad()
        {
            onLoad?.Invoke();
        }
        public void OnSave()
        {
            onSave?.Invoke();
        }
    }
}
