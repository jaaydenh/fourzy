using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCircleProgress : MonoBehaviour 
{
    IEnumerator Start () 
    {
        while (true)
        {
            float value = Random.Range(0.0f, 1.0f);
            this.GetComponent<CircleProgress>().SetupNewValue(value, true, 1.5f);
            yield return new WaitForSeconds(3);
        }
	}
}
