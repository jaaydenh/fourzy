using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class MMTransformRandomizer : MonoBehaviour
    {
        [Header("Scale")]
        public Vector3 MinRandomPosition;
        public Vector3 MaxRandomPosition;

        [Header("Rotation")]
        public Vector3 MinRandomRotation;
        public Vector3 MaxRandomRotation;

        [Header("Scale")]
        public Vector3 MinRandomScale;
        public Vector3 MaxRandomScale;

        [Header("Settings")]
        public bool AutoRemoveAfterRandomize = false;
        public bool RemoveAllColliders = false;

        public virtual void Randomize()
        {
            RandomizePosition();
            RandomizeRotation();
            RandomizeScale();
            RemoveColliders();
            Cleanup();
        }
        
        protected virtual void RandomizePosition()
        {
            Vector3 randomPosition = MMMaths.RandomVector3(MinRandomPosition, MaxRandomPosition);
            this.transform.localPosition += randomPosition;
        }

        protected virtual void RandomizeRotation()
        {
            Vector3 randomRotation = MMMaths.RandomVector3(MinRandomRotation, MaxRandomRotation);
            this.transform.localRotation = Quaternion.Euler(randomRotation);
        }

        protected virtual void RandomizeScale()
        {
            Vector3 randomScale = MMMaths.RandomVector3(MinRandomScale, MaxRandomScale);
            this.transform.localScale = randomScale;
        }

        protected virtual void RemoveColliders()
        {
            if (RemoveAllColliders)
            {
                #if UNITY_EDITOR
                Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    DestroyImmediate(collider);
                }
                Collider2D[] colliders2D = this.gameObject.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D collider2D in colliders2D)
                {
                    DestroyImmediate(collider2D);
                }
                #endif
            }
        }

        protected virtual void Cleanup()
        {
            if (AutoRemoveAfterRandomize)
            {
                #if UNITY_EDITOR
                    DestroyImmediate(this);
                #endif
            }
        }
    }
}
