using UnityEngine;
using System.Collections;

/*
* Cody 07/02/2024: The SprintJoint2D field was serialized and changed 
* the brace formatting to match the other scripts.
*/

public class RopeStretch : MonoBehaviour
{

	[SerializeField] private SpringJoint2D springJoint;
	
	void Update ()
	{
		// Get the two end points in world coordinates
		Vector3 point1 = springJoint.transform.TransformPoint(springJoint.anchor);
		Vector3 point2;

		// check if we're connected to another object or the world
		if (springJoint.connectedBody == null)
		{
			// connected to world: anchor is already in world coords
			point2 = springJoint.connectedAnchor;
		}
		else
		{
			// connector to something else: transform the point from that object's coordinate space 
			// to world coordinates
			point2 = springJoint.connectedBody.transform.TransformPoint(springJoint.connectedAnchor);
		}

		// the centre of the sprite is in the middle between the two.		
		transform.position = (point1 + point2) / 2;
		// the length of the sprite is the distance between the two
		Vector3 scale = transform.localScale;
		scale.y = Vector3.Distance(point1, point2);
		transform.localScale = scale;
		// the rotatation is the angle between the vector and vectical
		Vector3 v12 = point2 - point1;
		transform.rotation = Quaternion.FromToRotation(Vector3.up, v12); 
	}
}
