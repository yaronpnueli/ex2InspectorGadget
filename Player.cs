using UnityEngine;
using Infra;
using Infra.Gameplay;
using Infra.Utils;

namespace Gadget {
// This is the C# syntax for adding an Attribute to something.
// Here we make sure that this script requires that its game object will also
// contain an Animator component and a RigidBody2D component. This allows us to
// safely assume these components exist, so we don't have to check the result
// of GetComponent<Animator>() or GetComponent<Rigidbody2D>().
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(OverlapChecker))]
/// <summary>
/// The player controller class.
/// All custom classes that we want to add to a game object must derive from
/// MonoBehaviour. This allows us to see it in the inspector and provides many
/// useful methods to manipulate the component and the game object it lives on.
/// </summary>
	public class Player : MonoBehaviour {
	    public int health = 1; 
	    public float jump_height = 15f; 
	    public float walk_speed = 7f; 
	    public float fall_speed = 2f;
	    public float shooting_angle = 45f;
	    public float shooting_speed = 15f; 

	    // Keys for the active abilty of the player

	    public KeyCode right_key; //Right key
	    public KeyCode left_key; //Left key
	    public KeyCode jump_key; //Jump key
	    public KeyCode shoot_key; //Shoot key

	    // These parameters' types are Unity components, so in the inspector they
	    // are shown as a field that we can drag a suitable reference to it.

	    public Transform arm_img; //the image for the arm
		public Transform hand_img; //the image for the hand (tip of the arm)
	    public Rigidbody2D shot_obj; //the shot object

	    private readonly int animatorAlive = Animator.StringToHash("Alive");
	    private readonly int animatorJumpTrigger = Animator.StringToHash("Jump");

	    private Animator animator;
	    private Rigidbody2D body;
		private OverlapChecker ground_overlap_checker;

	    /// <summary>
	    /// Checking if the players touches the ground
	    /// </summary>
	    /// <value><c>true</c> if is grounded; otherwise, <c>false</c>.</value>
	    private bool isGrounded {
	        get {
	            return ground_overlap_checker.isOverlapping;
	        }
	    }

	    /// <summary>
	    /// Awake is a Unity built-in message.
	    /// Unity calls it when the object is loaded into the scene. Think of it like
	    /// a constructor.
	    /// </summary>
	    protected void Awake() {
	        animator = GetComponent<Animator>();
	        body = GetComponent<Rigidbody2D>();
	        ground_overlap_checker = GetComponent<OverlapChecker>();

	        animator.SetBool(animatorAlive, true);
	    }

	    /// <summary>
	    /// Update is a Unity built-in message.
	    /// Unity calls it every frame - every time the screen renders the game view.
	    /// </summary>
	    protected void Update() {
			//update arm angle
	        var arm_angle = arm_img.eulerAngles;
	        arm_angle.z = shooting_angle;
	        arm_img.eulerAngles = arm_angle;

	        body.gravityScale = fall_speed;

	        var cur_speed = body.velocity;
			//if players wants to jump, and the character touches the ground, jump
			if (Input.GetKeyDown(jump_key) && isGrounded) {
	            cur_speed.y = jump_height;
	            body.velocity = cur_speed;
	            animator.SetTrigger(animatorJumpTrigger);
			//right movement
	        } else if (Input.GetKey(right_key)) {
	            cur_speed.x = walk_speed;
	            body.velocity = cur_speed;
			//left movement
	        } else if (Input.GetKey(left_key)) {
	            cur_speed.x = -walk_speed;
	            body.velocity = cur_speed;
			//Shooting
	        } else if (Input.GetKey(shoot_key)) {
	            if (!shot_obj.gameObject.activeInHierarchy) {
	                shot_obj.gameObject.SetActive(true);
	                shot_obj.position = hand_img.position;
	                shot_obj.velocity = Vector2.right.Rotate(Mathf.Deg2Rad * shooting_angle) * shooting_speed;
	            }
	        }
	    }
		
			/// <summary>
			/// Raises the collision enter2 d event.
			/// Checks for various possibilities - enemies, or end goal
			/// </summary>
			/// <param name="collision">Collision.</param>
	    protected void OnCollisionEnter2D(Collision2D collision) {
	        if (health <= 0) return;
			//end goal
	        if (collision.gameObject.CompareTag("Victory")) {
	            DebugUtils.Log("Great Job!");
	            return;
	        }
			//enemy
	        if (!collision.gameObject.CompareTag("Enemy")) return;

	        --health;
	        if (health > 0) return;
			//if health reaches zero, set lose
	        animator.SetBool(animatorAlive, false);
	        body.velocity = Vector2.zero;
	        body.gravityScale = 4f;
	        enabled = false;
	    }
	}
}
