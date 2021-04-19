//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using PlayFab;
using System.Collections;
using UnityEngine;
using PlayFab.ClientModels;

namespace Fourzy.Testing
{
    public class NewModelSample : RoutinesBase
    {
        public GameboardView gameboard;

        private ClientFourzyGame game;
        public RectTransform root;
        public RectTransform target;

        protected override void Awake()
        {
            base.Awake();

            if (!gameboard) gameboard = GetComponentInChildren<GameboardView>();
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) CancelRoutine("r1");

            if (Input.GetKeyDown(KeyCode.Q))
            {
                //create turn based game
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "createTurnBased",

                }, OnResult, OnError);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                print(target.GetAnchoredPosition(root, Vector3.zero));
            }
        }

        private void OnResult(ExecuteCloudScriptResult result)
        {
            print("good: " + result.ToJson());
        }

        private void OnError(PlayFabError error)
        {
            print("bad: " + error.ToString());
        }

        protected void Start()
        {
            game = new ClientFourzyGame(
                GameContentManager.Instance.GetMiscBoard("201"), 
                UserManager.Instance.meAsPlayer, 
                new Player(2, "Player Two"));

            gameboard.Initialize(game);
            gameboard.onDraw += OnDraw;

            ////routines tests
            //StartRoutine("r1", R1(), () => StartRoutine("r2", R2(), () => { Debug.Log("r2 ended"); }));
        }

        protected void OnDestroy()
        {
            gameboard.onDraw -= OnDraw;
        }

        private void OnDraw(IClientFourzy model)
        {
            Debug.Log("On Draw");
        }

        private IEnumerator R1()
        {
            Debug.Log("r1 started");
            yield return new WaitForSeconds(5f);
            Debug.Log("r1 ended");
        }

        private IEnumerator R2()
        {
            Debug.Log("r2 started");

            yield return null;
            yield break;
        }
    }
}