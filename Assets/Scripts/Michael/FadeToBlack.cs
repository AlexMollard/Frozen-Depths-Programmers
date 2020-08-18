/*
    File name: FadeToBlack.cs
    Author: Michael Sweetman
    Summary: Toggles between fading a canvas image to be entirely transparent or opaque
    Creation Date: 12/08/2020
    Last Modified: 18/08/2020
*/

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
		// if fade is true
		if (fade)
		{
			// if the black screen is currently transparent
			if (transparent)
			{
				// increase the alpha of the black screen by (deltaTime/duration)
				blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, blackScreen.color.a + Time.deltaTime / duration);
				// if the alpha is greater than or equal to 1, the black screen is fully opaque
				if (blackScreen.color.a >= 1.0f)
				{
					// set the alpha of the blackscreen to exactly 1
					blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1.0f);
					// set transparent to false as the black screen is now opaque
					transparent = false;
					// set fade to false as the black screen has finished becoming opaque
					fade = false;
				}
			}
			// if the black screen is not completely transparent
			else
			{
				// decrease the alpha of the black screen by (deltaTime/duration)
				blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, blackScreen.color.a - Time.deltaTime / duration);
				// if the alpha is less than or equal to 0, the black screen is fully transparent
				if (blackScreen.color.a <= 0.0f)
				{
					// set the alpha of the blackscreen to exactly 0
					blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0.0f);
					// set transparent to true as the black screen is now transparent
					transparent = true;
					// set fade to false as the black screen has finished becoming transparent
					fade = false;
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		// if the collider is the player, set fade to true to start changing the alpha of the black screen
		if (other.gameObject.GetComponent<PlayerMovement>() != null)
		{
			fade = true;
		}
	}
}
