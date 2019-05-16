//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy.Testing
{
    public class NewModelSample : MonoBehaviour
    {
        public GameboardView gameboard;

        private ClientFourzyGame game;

        protected void Awake()
        {
            if (!gameboard)
                gameboard = GetComponentInChildren<GameboardView>();
        }

        protected void Start()
        {
            game = new ClientFourzyGame(GameContentManager.Instance.GetMiscBoard("201"), UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));

            gameboard.Initialize(game);
            gameboard.onDraw += OnDraw;
        }

        protected void OnDestroy()
        {
            gameboard.onDraw -= OnDraw;
        }

        private void OnDraw(IClientFourzy model)
        {
            Debug.Log("On Draw");
        }
    }
}