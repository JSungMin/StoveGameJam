using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.UI.Pattern
{
    public interface IViewOperation
    {
        UIView Hide();
        UIView Show();

        UIView ImmediateShow();
        UIView ImmediateHide();
    }
    //  UIView Will be derived detail class => Command Pattern
    public class UIView : MonoBehaviour, IViewOperation
    {
        public enum VisibleState
        {
            Appearing,
            Appeared,
            Disappearing,
            Disappeared
        }

        public VisibleState state;
        //  TODO : Add some Properties
        //  Property : StartOption
        //  Property : ShowOption
        //  Property : HideOption

        public virtual UIView Hide()
        {
            state = VisibleState.Disappearing;
            return this;
        }

        public virtual UIView Show()
        {
            state = VisibleState.Appearing;
            return this;
        }

        public UIView ImmediateShow()
        {
            state = VisibleState.Appeared;
            return this;
        }

        public UIView ImmediateHide()
        {
            state = VisibleState.Disappeared;
            return this;
        }
    }
}
