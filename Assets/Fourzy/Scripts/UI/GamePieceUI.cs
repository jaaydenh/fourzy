using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class GamePieceUI : MonoBehaviour
    {
        public GamePieceData gamePieceData;

        [SerializeField]
        private Image gamePieceImage;

        [SerializeField]
        private GameObject notFoundImage;

        [SerializeField]
        private Material grayscaleMaterial;

        [SerializeField]
        private TextMeshProUGUI totalNumberOfPieces;

        [SerializeField]
        private CircleProgress circleProgress;

        [SerializeField]
        private GameObject gamePieceInfoFrame;

        [SerializeField]
        private Text pieceName;

        [SerializeField]
        private Image borderImage;

        [SerializeField]
        private Text totalNumberOfChampions;

        [SerializeField]
        private List<Image> glowingStars = new List<Image>();

        [SerializeField]
        private List<Image> fadedStars = new List<Image>();

        private const float WithoutInfoFrameHeight = 200.0f;
        private const float NormalHeight = 400.0f;

        public delegate void SetGamePiece(string gamePieceId);
        public static event SetGamePiece OnSetGamePiece;

        public void InitWithGamePieceData(GamePieceData gamePieceData)
        {
            this.gamePieceData = gamePieceData;

            // Test values
            gamePieceData.NumberOfStars = 3;
            gamePieceData.TotalNumberOfStars = 5;
            gamePieceData.NumberOfChampions = 0;
            gamePieceData.TotalNumberOfChampions = 100;
            gamePieceData.NumberOfPieces = 20;
            gamePieceData.TotalNumberOfPieces = 60;
            gamePieceData.state = GamePieceState.FoundAndUnlocked;

            gamePieceImage.sprite = GameContentManager.Instance.GetGamePieceSprite(gamePieceData.ID);

            LayoutElement layoutElement = this.GetComponent<LayoutElement>();

            if (gamePieceData.state == GamePieceState.NotFound)
            {
                layoutElement.preferredHeight = WithoutInfoFrameHeight;
                gamePieceImage.material = grayscaleMaterial;

                totalNumberOfPieces.gameObject.SetActive(false);
                gamePieceInfoFrame.SetActive(false);
                circleProgress.gameObject.SetActive(false);
                notFoundImage.SetActive(true);
            }
            else
            {
                layoutElement.preferredHeight = NormalHeight;
                gamePieceImage.material = gamePieceImage.defaultMaterial;

                notFoundImage.SetActive(false);
                
                pieceName.text = gamePieceData.Name;
                totalNumberOfPieces.text = gamePieceData.NumberOfPieces + "/" + gamePieceData.TotalNumberOfPieces;
                totalNumberOfChampions.text = this.FormatedNumberOfChampions(gamePieceData.NumberOfChampions, gamePieceData.TotalNumberOfChampions);
                borderImage.color = gamePieceData.BorderColor;

                this.UpdateProgressBar();
                this.UpdateStars();
            }
        }

        string FormatedNumberOfChampions(int number, int total)
        {
            string formatedText = string.Empty;
            if (number < 10)
            {
                formatedText += "0";
            }
            if (number < 100)
            {
                formatedText += "0";
            }
            formatedText += number + "/";
            if (total < 10)
            {
                formatedText += "0";
            }
            if (total < 100)
            {
                formatedText += "0";
            }
            formatedText += total;
            return formatedText;
        }

        private void UpdateProgressBar()
        {
            float collectionProgress = ((float)gamePieceData.NumberOfPieces) / gamePieceData.TotalNumberOfPieces;
            circleProgress.SetupNewValue(collectionProgress);

            if (collectionProgress < 1.0f)
            {
                if (gamePieceData.state == GamePieceState.FoundAndUnlocked)
                {
                    circleProgress.SetupNewColor(Color.green);
                }
                else
                {
                    circleProgress.SetupNewColor(Color.yellow);
                }
            }
            else
            {
                circleProgress.SetupNewColor(Color.red);
            }
        }

        private void UpdateStars()
        {
            Debug.Assert(glowingStars.Count == fadedStars.Count, "Setup the same count of stars in editor");
            for (int i = 0; i < glowingStars.Count; i++)
            {
                if (i < gamePieceData.NumberOfStars)
                {
                    fadedStars[i].enabled = false;
                    glowingStars[i].enabled = true;
                }
                else
                {
                    fadedStars[i].enabled = true;
                    glowingStars[i].enabled = false;
                }
            }
        }

        public void SetAlternateColor(bool isAlternate) 
        {
            Image rend = GetComponent<Image>();
            if (isAlternate) 
            {
                rend.material.SetVector("_HSVAAdjust", new Vector4(0.3f, 0, 0, 0));
            } else {
                rend.material.SetVector("_HSVAAdjust", new Vector4(0, 0, 0, 0));    
            }
        }
    }
}
