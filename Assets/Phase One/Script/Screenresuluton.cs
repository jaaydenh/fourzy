using UnityEngine;
using System.Collections;

public class Screenresuluton : MonoBehaviour {

	public GameObject cube;
	void Start()
	{

		#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_EDITOR_WIN
			float s = (((float)Screen.height / (float)Screen.width) - (854f / 480f));
			if (Mathf.Abs(s) > 0.4f)
			s = s * 1.08f;
			else if (Mathf.Abs(s) > 0.1f)
			s = s * 0.95f;
			cube.transform.localScale -= new Vector3(s, 0, 0);
		#endif

	}
}
