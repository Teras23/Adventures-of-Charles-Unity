using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private new Rigidbody2D rigidbody;
    private Animator animator;
    private float unit = 1;
    private float speed = 2;


	// Use this for initialization
	void Start () {
	    rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
	}

    void Update() {
        if(Input.GetButtonDown("Use")) {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(1, 1), 0, Vector2.zero);

            foreach(RaycastHit2D hit in hits) {
                if(hit.transform.tag == "World Object") {
                    hit.transform.GetComponent<BaseWorldObject>().Use();
                }
            }
        }
    }

	void FixedUpdate() {

        Vector2 movement = Vector2.zero;

        if(Input.GetButton("Move Right")) {
            animator.SetInteger("MoveDirection", 2);
            movement += new Vector2(speed * unit, 0);
        }

        if(Input.GetButton("Move Left")) {
            animator.SetInteger("MoveDirection", 4);
            movement += new Vector2(-speed * unit, 0);
        }

        if(Input.GetButton("Move Up")) {
            animator.SetInteger("MoveDirection", 3);
            movement += new Vector2(0, speed * unit);
		}

        if(Input.GetButton("Move Down")) {
            animator.SetInteger("MoveDirection", 1);
            movement += new Vector2(0, -speed * unit);
        }

        rigidbody.velocity = movement;

        if(rigidbody.velocity.magnitude > 0) {
            animator.SetBool("Moving", true);
        }
        else {
            animator.SetBool("Moving", false);
        }
	}

    void LateUpdate() {
        transform.position = new Vector3(Mathf.Round(transform.position.x / 0.015625f) * 0.015625f, Mathf.Round(transform.position.y / 0.015625f) * 0.015625f, transform.position.z);
    }
}
