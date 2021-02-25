//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class DropdownHelper : MonoBehaviour
    {
        private TMP_Dropdown dropdown;

        protected void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }

        public void SetValue(string value) => dropdown.value = int.Parse(value);
    }
}