using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class PieceUIGameScreen : MonoBehaviour
    {
        int counter = 0;
        int repeatMax = 5;
        Animator pieceAnimator;

        void Start() {
            pieceAnimator = this.GetComponent<Animator>();
        }

        public void PrintEvent(string s)
        {
            // Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
        }

        public void PlayAnimation() {
            
            if (counter < repeatMax) {
                // Debug.Log("PlayAnimation: counter: " + counter);
                counter++;
                pieceAnimator.Play("Jump", -1, 0);
            } else {
                counter = 0;
            }
        }

        public void StopAnimation() {
            counter = 0;
            pieceAnimator.Play("Idle");
        } 
    }
}

