using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/*
* Cody 07/02/2024: Script has been modified to match the non-animated one in both style
* and to use new input system.
*/

// require that the object has a Rigidbody2D and an Animator
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Animator))]

public class PlatformerMoveAnimated : MonoBehaviour
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

	private Animator animator;

	void Awake()
	{
		// enabling and subscribing to the input system
		playerInputs = new PlayerInputs();
		playerInputs.Player.Enable();

		// record a reference to the animator component to use later
		// and set its Moving and OnGround flags to false
		animator = GetComponent<Animator>();
		animator.SetBool("Moving", false);
		animator.SetBool("OnGround", false);
	}

	void Start()
	{
		// record a reference to the rigidbody component to use later
		rb2D = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
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
			// activate the animation if we are moving
			animator.SetBool("Moving", velocity.x != 0);
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
			Vector3 s = transform.localScale;
			s.x = -s.x;
			transform.localScale = s;

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

		// tell the animator whether we are on the ground
		animator.SetBool("OnGround", onGround);

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
