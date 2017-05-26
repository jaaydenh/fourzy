using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
	public class Utility {

			public static void SetSpriteAlpha(GameObject go, float alpha) {
				SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
				Color c = sprite.color;
				c.a = 0.0f;
				sprite.color = c;
			}
	}
}

