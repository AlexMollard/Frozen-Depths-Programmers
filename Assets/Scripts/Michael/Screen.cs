/*
    File name: Screen.cs
    Author: Michael Sweetman
    Summary: fades a canvas image to be entirely transparent or opaque
    Creation Date: 13/10/2020
    Last Modified: 13/10/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
	float duration = 0.0f;
	bool makeScreenOpaque = false;
	bool fade = false;
	Image screen;
	Color screenColor;

	private void Start()
	{
		// Get the image component
		screen = GetComponent<Image>();
		// get the current screen colour, set its starting alpha
		screenColor = new Color(screen.color.r, screen.color.g, screen.color.b, (makeScreenOpaque) ? 0.0f : 1.0f);
	}

	private void Update()
	{
		// if fade is true
		if (fade)
		{
			// if the screen should become opaque
			if (makeScreenOpaque)
			{
				// increase the alpha by (deltaTime/duration)
				screenColor.a += Time.deltaTime / duration;
				
				// if the alpha is greater than or equal to 1, the screen will be fully opaque
				if (screenColor.a >= 1.0f)
				{
					// set the alpha to exactly 1
					screenColor.a = 1.0f;
					// set fade to false as the transition is complete
					fade = false;
				}
			}
			// if the screen should become transparent
			else
			{
				// decrease the alpha by (deltaTime/duration)
				screenColor.a -= Time.deltaTime / duration;

				// if the alpha is less than or equal to 0, the screen will be fully transparent
				if (screenColor.a <= 0.0f)
				{
					// set the alpha to exactly 0
					screenColor.a = 0.0f;
					// set fade to false as the transition is complete
					fade = false;
				}
			}

			// apply the new screen colour to the screen
			screen.color = screenColor;
		}
	}

	// starts the fade and sets its duration and goal
	public void Fade(float fadeDuration, bool makeOpaque)
	{
		// set fade to true so the fade starts
		fade = true;
		// get the duration of the fade
		duration = fadeDuration;
		// get whether the screen should become opaque or transparent
		makeScreenOpaque = makeOpaque;
	}
}
