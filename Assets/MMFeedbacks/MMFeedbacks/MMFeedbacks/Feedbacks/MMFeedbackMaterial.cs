using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will let you change the material of the target renderer everytime it's played.")]
    [FeedbackPath("Renderer/Material")]
    public class MMFeedbackMaterial : MMFeedback
    {
        /// sets the inspector color for this feedback
        #if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.RendererColor; } }
        #endif

        /// the possible methods to switch materials
        public enum Methods { Sequential, Random }

        [Header("Material")]
        /// the renderer to change material on
        public Renderer TargetRenderer;
        /// the selected method
        public Methods Method;
        /// whether or not the sequential order should loop
        [MMFEnumCondition("Method", (int)Methods.Sequential)]
        public bool Loop = true;
        /// whether or not to always pick a new material in random mode
        [MMFEnumCondition("Method", (int)Methods.Random)]        
        public bool AlwaysNewMaterial = true;
        /// the initial index to start with
        public int InitialIndex = 0;
        /// the list of materials to pick from
        public List<Material> Materials;

        protected int _currentIndex;

        /// <summary>
        /// On init, grabs the current index
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            _currentIndex = InitialIndex;
        }

        /// <summary>
        /// On play feedback, we change the material if possible
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Materials.Count == 0)
            {
                Debug.LogError("[MMFeedbackMaterial on " + this.name + "] The Materials array is empty.");
                return;
            }

            int newIndex = DetermineNextIndex();

            if (Materials[newIndex] == null)
            {
                Debug.LogError("[MMFeedbackMaterial on " + this.name + "] Attempting to switch to a null material.");
                return;
            }

            TargetRenderer.material = Materials[newIndex];
        }

        /// <summary>
        /// Determines the new material to pick
        /// </summary>
        /// <returns></returns>
        protected virtual int DetermineNextIndex()
        {
            switch(Method)
            {
                case Methods.Random:
                    int random = Random.Range(0, Materials.Count);
                    if (AlwaysNewMaterial)
                    {
                        while (_currentIndex == random)
                        {
                            random = Random.Range(0, Materials.Count);
                        }
                    }
                    _currentIndex = random;
                    return _currentIndex;                    

                case Methods.Sequential:
                    _currentIndex++;
                    if (_currentIndex >= Materials.Count)
                    {
                        _currentIndex = Loop ? 0 : _currentIndex;
                    }
                    return _currentIndex;
            }
            return 0;
        }
    }
}
