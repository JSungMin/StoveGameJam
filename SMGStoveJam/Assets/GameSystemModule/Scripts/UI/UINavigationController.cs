using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.UI.Pattern
{
    public class UINavigationController : MonoBehaviour
    {
        private static UINavigation _currentNav;

        public static UINavigation CurrentNav
        {
            get => _currentNav;
            set
            {
                if (value == null)
                    return;
                _currentNav.CurrentView.Hide();
                _currentNav = value;
                _currentNav.CurrentView.Show();
            }
        }

    }
}
