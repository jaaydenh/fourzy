//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GeneratorTesterTokenView : MonoBehaviour
    {
        public Transform tokenParent;

        /// <summary>
        /// 0 - NONE, 1 - ALLOWED, 2 - FORBIDDEN
        /// </summary>
        private int state;
        private TokenView tokenView;
        private Image _image;

        public bool IsAllowed => state == 1;

        public bool IsForbidden => state == 2;
        public TokenType TokenType { get; private set; }

        protected void Start()
        {
            _image = GetComponent<Image>();

            SetState(0);
        }

        public GeneratorTesterTokenView SetData(TokenType tokenType)
        {
            TokenType = tokenType;

            tokenView = Instantiate(GameContentManager.Instance.GetTokenPrefab(tokenType), tokenParent)
                .SetData(TokenFactory.Create(tokenType.TokenTypeToString()));
            tokenView.transform.localPosition = Vector3.zero;

            return this;
        }

        public void OnTap()
        {
            SetState((state + 1) % 3);
        }

        private void SetState(int state)
        {
            this.state = state;

            switch (state)
            {
                case 0:
                    _image.color = Color.clear;

                    break;

                case 1:
                    _image.color = Color.green;

                    break;

                case 2:
                    _image.color = Color.red;

                    break;
            }
        }
    }
}