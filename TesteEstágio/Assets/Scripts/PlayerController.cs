using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    // Control Variables
    public GameObject inHandItem;
    public GameObject actionItem;
    public ItemScript inHandItemScript;
    public ItemScript actionItemScript;
    public GameObject throwIsland;
    public GameObject cameraRig;
    public Animator animator;
    public Vector3 throwDirection;
    public bool isSelected = false;
    public int craftingLoop = 0;
    public float throwForce = 10f;

    public string debug;
    public float remaining = 0;
    public float stopping = 0;
    public bool startedWalking = false;

    // Ambient Variables
    NavMeshAgent agent;
    bool selectable = false;
    
    // Public Class functions

    // Allows the character to be selected
    public void wakeUp() {
        selectable = true;
    }

    // Function that the animation (grabbing item) calls on its event
    public void callItem() {
        actionItemScript.pickupItem = true;
        animator.SetBool("pickupItem", false);
    }

    // Function that the animation (pickup box) calls on its event
    public void callBox() {
        animator.SetBool("carryingBox", true);
        actionItemScript.pickupBox = true;
        animator.SetBool("pickupBox", false);
    }

    // Function that the animation (throwing) calls on its event
    public void throwingBox() {
        inHandItemScript.throwBox = true;
        inHandItemScript.formerParent = inHandItem.transform.parent;
        inHandItemScript.parent = throwIsland.transform;
        animator.SetBool("throwBox", false);
        animator.SetBool("carryingBox", false);
    }

    // Function that the animation (put item down) calls on its event
    public void dropItem() {
        if (actionItem) {
            if (actionItem.tag == "OpenCrates") {
                inHandItemScript.dropItemBox = true;
            } else {
                inHandItemScript.dropItem = true;
                animator.SetBool("pickupItem", true);
            }
        } else {
            inHandItemScript.dropItem = true;
        }
        animator.SetBool("dropItem", false);
        animator.SetBool("carryingBox", false);
    }

    // Function that the animations (knife cut, wood cut, craft raft) calls on its events
    public void craftItem() {
        craftingLoop++;
        if (craftingLoop>=3)
        {
            craftingLoop = 0;
            actionItemScript.craftItem = true;
            animator.SetBool("craftRope", false);
            animator.SetBool("craftRaft", false);
            animator.SetBool("craftWood", false);
        }
    }

    // Function that the animation (BoxAction) calls on its events
    public void boxAction() {
        if (actionItem.tag == "OpenCrates") {
            actionItemScript.closeCrate = true;
            animator.SetBool("boxAction", false);
        } 
        else if (actionItem.tag == "Boxes") {
            actionItemScript.openCrate = true;
            animator.SetBool("boxAction", false);
        }
    }

    // Private Class functions

    // Function to make the character walk towards a direction
    void moveToPoint(Vector3 point) {
        animator.SetBool("isWalking", true);
        agent.SetDestination(point);
    }

    // Function that the clicks call to perform an specific event
    void performAction(int click) {
        //transform.LookAt(actionItem.transform);
        // Check right clicks
        if (click == 1) {
            if (inHandItem) {
                if (actionItem.tag == "Palms" && inHandItem.tag == "Knives") {
                    animator.SetBool("craftRope", true);
                } else if (actionItem.tag == "Trees" && inHandItem.tag == "Axes") {
                    animator.SetBool("craftWood", true);
                } else if (actionItem.tag == "UnfinishedRaft" && inHandItem.tag == "Hammers") {
                    animator.SetBool("craftRaft", true);
                } else {
                    animator.SetBool("dropItem", true);
                }
            } else if (actionItem.tag == "Boxes") {
                animator.SetBool("pickupBox", true);
            } else if (actionItem.tag == "Axes" || actionItem.tag == "Hammers" || actionItem.tag == "Knives" || actionItem.tag == "Ropes") {
                animator.SetBool("pickupItem", true);
            }
        }
        // Check middle button clicks
        else if (click == 2) {
            // Open crate interactions
            if (actionItem.tag == "OpenCrates") {
                // Put item inside open crate
                if (inHandItem) {
                    animator.SetBool("dropItem", true);
                }
                // Close the open crate
                else {
                    if (actionItemScript.items[0]) {
                        animator.SetBool("boxAction", true);
                    }
                    else {
                        actionItem = null;
                        actionItemScript = null;
                    }
                }
            }
            // Closed Crate interactions
            else if (actionItem.tag == "Boxes") {
                // Throw the closed crate
                if (inHandItem == actionItem) {
                    animator.SetBool("throwBox", true);
                } 
                // Open the closed crate
                else {
                    animator.SetBool("boxAction", true);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        // Left Button clicks
            // Character selection
        if (Input.GetMouseButtonDown(0)) {
            if (selectable) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)) {
                    // Character selection
                    if (hit.transform.gameObject == this.gameObject) {
                        if (!isSelected) {
                            isSelected = true;
                            animator.SetBool("isSelected", isSelected);
                            CameraController.instance.followTransform = transform;
                            return;
                        }
                    }
                }
            }
        }
        // Right Button clicks
            // Character movement
            // Crafting and picking up objects
        else if (Input.GetMouseButtonDown(1)) {
            if (isSelected) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)) {
                    // Character movement
                    if (hit.transform.tag == "Floor" || hit.transform.tag == "Untagged" || hit.transform.tag == "Players" || hit.transform.tag == "OpenCrates") {
                        moveToPoint(hit.point);
                    }
                    // Crafting and picking up objects
                    else {
                        if (hit.transform.tag == "Logs") {
                            actionItem = hit.transform.parent.gameObject;
                            actionItemScript = actionItem.GetComponent<ItemScript>();
                            actionItemScript.player = this;
                        } else {
                            actionItem = hit.transform.gameObject;
                            actionItemScript = actionItem.GetComponent<ItemScript>();
                            actionItemScript.player = this;
                        }
                        moveToPoint(hit.point);
                        //this.gameObject.transform.LookAt(actionItem.transform);
                        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(actionItem.transform.position), 10 * Time.deltaTime);
                        performAction(1);
                    }
                }
            }
        }
        // Middle button clicks
            // Close open crate
            // Open closed crate
            // Put itens in open crate
            // Throw closed box
        else if (Input.GetMouseButtonDown(2)) {
            if (isSelected) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)) {
                    if (inHandItem) {
                        if (inHandItem.tag == "Boxes") {
                            actionItem = inHandItem;
                            actionItemScript = actionItem.GetComponent<ItemScript>();
                            actionItemScript.player = this;
                            Vector3 hitPoint = hit.point;
                            throwIsland = hit.transform.parent.gameObject;
                            actionItemScript.throwIsland = throwIsland;
                            //actionItemScript.destiny = null;
                            Vector3 mouseDir = hitPoint - this.transform.position;
                            mouseDir = mouseDir.normalized;
                            throwDirection = mouseDir;
                            moveToPoint(hit.point);
                            //this.gameObject.transform.LookAt(actionItem.transform);
                            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(actionItem.transform.position), 10 * Time.deltaTime);
                            performAction(2);
                        } else {
                            actionItem = hit.transform.gameObject;
                            actionItemScript = actionItem.GetComponent<ItemScript>();
                            actionItemScript.player = this;
                            moveToPoint(hit.point);
                            //this.gameObject.transform.LookAt(actionItem.transform, );
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(actionItem.transform.position), 10 * Time.deltaTime);
                            performAction(2);
                        }   
                    } else {
                        actionItem = hit.transform.gameObject;
                        actionItemScript = actionItem.GetComponent<ItemScript>();
                        actionItemScript.player = this;
                        moveToPoint(hit.point);
                        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(actionItem.transform.position), 10 * Time.deltaTime);
                        performAction(2);
                    }
                }  
            }
        }
        else if (Input.GetKeyDown(KeyCode.G)) {
            if (isSelected) {
                animator.SetBool("dropItem", true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            isSelected = false;
            animator.SetBool("isSelected", isSelected);
            Vector3 camPos = cameraRig.transform.position;
            Vector3 newPos = new Vector3 (camPos.x, camPos.y + 10, camPos.z - 10);
            cameraRig.GetComponent<CameraController>().newPosition = newPos;
        }

        if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance)) {
            animator.SetBool("isWalking", false);
            if (actionItem) {
                this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation, Quaternion.LookRotation(actionItem.transform.position), 10 * Time.deltaTime);
            }
        }
    }
}
