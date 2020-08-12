using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public Image blackScreen;
    public float duration = 0.2f;
	public bool transparent = true;
	bool fade = false;

	private void Update()
	{
		if (fade)
		{
			if (transparent)
			{
				blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, blackScreen.color.a + Time.deltaTime / duration);
				if (blackScreen.color.a >= 1.0f)
				{
					blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1.0f);
					transparent = false;
					fade = false;
				}
			}
			else
			{
				blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, blackScreen.color.a - Time.deltaTime / duration);
				if (blackScreen.color.a <= 0.0f)
				{
					blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0.0f);
					transparent = true;
					fade = false;
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<PlayerMovement>() != null)
		{
			fade = true;
		}
	}
}
