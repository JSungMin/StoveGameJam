using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CoreSystem.UI.Pattern
{
    public class UINavigation
    {
        private readonly Stack<UIView> viewHistory = new Stack<UIView>();
        public UIView CurrentView => viewHistory.Peek();

        public UIView Push(string name)
        {
            var views = Object.FindObjectsOfType<UIView>();
            var obj = views.FirstOrDefault(x => x.name == name);
            viewHistory.Push(obj);
            return obj;
        }

        public UIView Pop()
        {
            var obj = viewHistory.Pop();
            if (obj.state == UIView.VisibleState.Disappearing)
                obj.ImmediateHide();
            else
                obj.Hide();
            return CurrentView;
        }

        public UIView PopTo(string name)
        {
            while (viewHistory.Count != 0)
            {
                var obj = CurrentView;
                if (obj.name != name)
                {
                    Pop();
                }
                else
                {
                    return obj;
                }
            }
            Debug.LogWarning("Can't Find Pop Target : " + name);
            return null;
        }

        public UIView PopToRoot()
        {
            while (viewHistory.Count > 1)
            {
                var obj = Pop();
            }
            return Pop();
        }
    }
}
