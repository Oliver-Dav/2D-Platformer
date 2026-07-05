using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/**
 * A script that implements simple platformer controls: walking and jumping 
 */

/*
* Kayson 04/02/2024: Code has been updated to reflect the COMP2160 Coding Style Guide.
* The fields which were public have been serialized so they are revealed in the 
* inspector (as they would be for public), but use  the coding principles we wish to 
* instil in students. Similarly, field names have been updated to reflect Guide.
* 
* Cody 07/02/2024: Script has been updated to use the new input system. In one of my
* iterations, I ended up implementing gravity directly, but that messed with the lessons
* because the player would stop falling if it thought it wasn't on the ground, so I 
* changed it back. I kept that version, so please contact me if we want to rewrite the
* lessons to incorporate that.
*
*/

// require that the object has a Rigidbody2D component
[RequireComponent (typeof (Rigidbody2D))]
public class PlatformerMove : MonoBehaviour 
{
	[SerializeField] private float walkSpeed = 5f;
	[SerializeField] private float jumpSpeed = 5f;
	[SerializeField] private bool walkOnAir = false;
	private Rigidbody2D rb2D;

	/*
	* Oliver 14/07/2022: The following two fields are serialized so 
	* they are revealed in the inspector (for clarity with students)
	*/
	[SerializeField] private bool facingRight = true;
	[SerializeField] private bool onGround;

	[SerializeField] private Rect groundRect = new Rect(-0.32f, -0.72f, 0.56f, 0.1f);
	[SerializeField] private LayerMask groundLayerMask = -1;

	private PlayerInputs playerInputs;

	void Awake()
	{
		// enabling and subscribing to the input system
		playerInputs = new PlayerInputs();
		playerInputs.Player.Enable();
	}

	void Start()
	{
		// record a reference to the rigidbody component to use later
		rb2D = gameObject.GetComponent<Rigidbody2D>();
	}
	
	void Update () 
	{	
		// test if the avatar is on the ground
		CheckOnGround();

		Vector2 velocity = rb2D.velocity;

		// if we're on the ground or if in-air walking is allowed, set the horizontal velocity to match the "Horizontal" input axis (scaled by walkSpeed)
		if (onGround || walkOnAir)
		{
			float inputAxis = playerInputs.Player.Movement.ReadValue<float>();
			velocity.x = walkSpeed * inputAxis;
		}

		// if we're on the ground and the Jump button is pressed, set the vertical velocity to equal the jump speed
		if(onGround && playerInputs.Player.Jump.WasPerformedThisFrame()) {
			velocity.y = jumpSpeed;
		}

		// set the velocity to the new value
		rb2D.velocity = velocity;

		// if the horizontal velocity is opposite to the direction the sprite is facing, turn around
		if (velocity.x < 0 && facingRight || velocity.x > 0 && !facingRight)
		{
			facingRight = !facingRight;
			Vector3 scale = transform.localScale;
			scale.x = -scale.x;
			transform.localScale = scale;

			// adjust the groundRect position
			groundRect.x = - groundRect.max.x;
		}

	}

	private void CheckOnGround()
	{
		// check if we are on the group by testing if there is anything inside the rectangle given by the groundRect
		// only objects in the groundLayerMask are checked

		onGround = false;

		Vector2 min = new Vector2(transform.position.x, transform.position.y) + groundRect.min;
		Vector2 max = new Vector2(transform.position.x, transform.position.y) + groundRect.max;

		Collider2D collider = Physics2D.OverlapArea(min, max, groundLayerMask);

		onGround = collider != null;

	}

	void OnDrawGizmos()
	{
		// draw a box showing the groundRect. It is red if the avatar is on the ground and white otherwise.

		Vector3 centre = transform.position;
		centre.x += groundRect.center.x;
		centre.y += groundRect.center.y;

		Vector3 size = Vector3.zero;
		size.x += groundRect.width;
		size.y += groundRect.height;

		if (onGround)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.white;
		}
		
		Gizmos.DrawWireCube(centre, size);

	}

}
