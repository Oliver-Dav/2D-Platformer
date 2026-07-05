using UnityEngine;
using System.Collections;

/*
* Kayson 04/02/2024: Code has been updated to reflect the COMP2160 Coding Style Guide.
* The fields which were public have been serialized so they are revealed in the 
* inspector (as they would be for public), but use  the coding principles we wish to 
* instil in students. Similarly, field names to be updated to reflect Guide.
*/

public class CoinPickup : MonoBehaviour
{

	[SerializeField] private LayerMask playerLayer;
	private AudioSource sound;
	private bool isGone = false;

	public void Start()
	{
		// store a reference to the AudioSource component (if there is one)
		sound = gameObject.GetComponent<AudioSource>();
	}

	public void OnTriggerEnter2D(Collider2D collider)
	{
		// something has collided with the coin check if it is the player

		// if the coin has already been collected, do nothing
		if (isGone)
		{
			return;
		}

		// check if the colliding object is in the playerLayer
		if (((1 << collider.gameObject.layer) & playerLayer.value) != 0)
		{
			// play a sound if there is one
			if (sound != null)
			{
				sound.Play();
			}

			// hide the sprite and disable this script
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			renderer.enabled = false;
			isGone = true;
		}
	}

}
