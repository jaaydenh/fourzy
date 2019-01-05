using UnityEngine;
using Fourzy;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

public class MenuItems : MonoBehaviour {


#if UNITY_EDITOR
    [UnityEditor.MenuItem("Fourzy/Take screenshot")]
    static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot("../test.png");
    }

    [UnityEditor.MenuItem("Fourzy/DeleteAllPlayerPrefs")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [UnityEditor.MenuItem("Fourzy/ResetPuzzlePacks")]
    static void ResetPuzzlePacks()
    {
        for (int x = 1; x < 10; x++)
        {
            for (int i = 0; i < 12; i++)
            {
                if (i < 10) {
                    PlayerPrefs.DeleteKey("PuzzleChallengeID:" + x + "00"+i);
                } else {
                    PlayerPrefs.DeleteKey("PuzzleChallengeID:" + x + "0"+i);
                }
            }
        }
    }

    [UnityEditor.MenuItem("Fourzy/ResetOnboarding")]
    static void ResetOnboarding()
    {
        PlayerPrefs.DeleteKey("onboardingStage");
        PlayerPrefs.DeleteKey("onboardingStep");
    }

    [UnityEditor.MenuItem("Fourzy/ResetTokenInstructionPopups")]
    static void ResetTokenInstructionPopups()
    {
        int count = System.Enum.GetNames(typeof(Token)).Length;
        for (int i = 0; i < count; i++)
        {
            PlayerPrefsWrapper.SetInstructionPopupDisplayed(i, false);
        }
    }

    [UnityEditor.MenuItem("Fourzy/CompleteOnboarding")]
    static void CompleteOnboarding()
    {
        PlayerPrefs.SetInt("onboardingStage", 5);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel0")]
    static void SetPuzzleToLevel1()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 0);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel5")]
    static void SetPuzzleToLevel5()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 5);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel10")]
    static void SetPuzzleToLevel10()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 10);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel15")]
    static void SetPuzzleToLevel15()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 15);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel20")]
    static void SetPuzzleToLevel20()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 20);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel25")]
    static void SetPuzzleToLevel25()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 25);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel30")]
    static void SetPuzzleToLevel30()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 30);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel35")]
    static void SetPuzzleToLevel35()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 35);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel40")]
    static void SetPuzzleToLevel40()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 40);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel45")]
    static void SetPuzzleToLevel45()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 45);
    }

    [UnityEditor.MenuItem("Fourzy/TestGameId")]
    static void TestGameId()
    {
        GameState GS = GameManager.Instance.activeGame.gameState;
        Console.Log("UniqueId=" + GS.UniqueId);
        File.WriteAllText(@"e:\temp\fourzy\testing.uniqueid", "UniqueId=" + GS.UniqueId);

    }

    [UnityEditor.MenuItem("Fourzy/EvaluateBoard")]
    static void EvaluateCurrentBoard()
    {
        Dictionary<string,GameState> OpenStates = new Dictionary<string,GameState>();
        Dictionary<string,GameState> WinStates = new Dictionary<string,GameState>();
        Dictionary<string,GameState> ClosedStates = new Dictionary<string,GameState>();
        Dictionary<string,GameState> AIStates = new Dictionary<string,GameState>();

        GameState GS = GameManager.Instance.activeGame.gameState;
        OpenStates.Add(GS.UniqueId,GS);

        int initial_move = GS.MoveList.Count;
        int count = 10000;
        int count_over = 0;
        int count_win_1 = 0;
        int count_win_2 = 0;

        int eval_limit = 10;

        PlayerEnum EvalPlayer = PlayerEnum.ONE;

        while (OpenStates.Count > 0)
        {
            //Get a New State
            GameState GS0 = OpenStates[OpenStates.Keys.First()];
            OpenStates.Remove(OpenStates.Keys.First());
            File.AppendAllText(@"e:\temp\fourzy\current_analysis.txt", string.Format("{0},{1},{2}\r\n", count,GS0.MoveList.Count,OpenStates.Count));

            Console.Log("eval=" + count + ":" + GS0.MoveList.Count + ":" + OpenStates.Count);

            if (GS0.Winner == EvalPlayer)
            {
                WinStates.Add(GS0.UniqueId,GS0);
                continue;
            }

            //If the depth is too deep, close it.
            if (GS0.MoveList.Count >= eval_limit)
            {
                ClosedStates.Add(GS0.UniqueId,GS0);
            }

            //Check if it's the eval player or opponent
            //If it is the eval player, add each move to the eval list
            //If it is the opponent, review each 

            List<GameState> NewStates = new List<GameState>();
            if ((GS0.IsPlayerOneTurn && EvalPlayer == PlayerEnum.ONE) || (!GS0.IsPlayerOneTurn && EvalPlayer == PlayerEnum.TWO))
            {
                bool found_win_player = false;
                foreach (Move m1 in GS0.GetPossibleMoves())
                {
                    GameState GSEval = GS0.Clone();
                    GSEval.MovePiece(m1, false);
                    NewStates.Add(GSEval);

                    if (GSEval.IsGameOver)
                    {
                        count_over++;
                        if (GSEval.Winner == EvalPlayer)
                        {
                            found_win_player = true;
                            WinStates.Add(GSEval.UniqueId,GSEval);
                            break;
                        }

                    }
                    else
                    {
                        NewStates.Add(GSEval);
                    }


                }
                if (!found_win_player)
                {
                    foreach (GameState g in NewStates)
                    {
                        if (!OpenStates.ContainsKey(g.UniqueId))
                        {
                            OpenStates.Add(g.UniqueId, g);
                        }
                    }
                }

            } else
            {
                //If it's the opponent's turn. Make a Winning Move, 
                bool found_ai_win = false;
                foreach (Move m1 in GS0.GetPossibleMoves())
                {
                    GameState GSEval = GS0.Clone();
                    GSEval.MovePiece(m1, false);
                    if (GSEval.IsGameOver)
                    {
                        if (GSEval.Winner != EvalPlayer)
                        {
                            found_ai_win = true;
                            AIStates.Add(GSEval.UniqueId,GSEval);
                            break;
                        }

                    }
                    else
                    {
                        NewStates.Add(GSEval);
                    }
                }

                if (!found_ai_win)
                {
                    foreach (GameState g in NewStates)
                    {
                        if (!OpenStates.ContainsKey(GS.UniqueId))
                        {
                            OpenStates.Add(GS.UniqueId, g);
                        }
                    }
                }

            }

            //Console.Log(count);
            count--;
            if (count <= 0) break;
        }

        StringBuilder info = new StringBuilder();
        info.Append("over=" + count_over);
        info.Append("win_1=" + count_win_1);
        info.Append("win_2=" + count_win_2);
        info.Append("eval=" + OpenStates.Count);
        info.Append("closed=" + ClosedStates.Count);
        info.Append("ai=" + AIStates.Count);
        info.Append("win=" + WinStates.Count);

        File.WriteAllText(@"e:\temp\fourzy\summary_analysis.txt", info.ToString());

    }


#endif

}
