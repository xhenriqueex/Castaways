using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

public class ItemScript : MonoBehaviour
{
    // Control Variables
    public PlayerController player;
    public bool isCarrying = false;
    public bool pickupItem = false;
    public bool pickupBox = false;
    public bool craftItem = false;
    public bool dropItem = false;
    public bool dropItemBox = false;
    public bool throwBox = false;
    public bool dropping = false;
    public bool closeCrate = false;
    public bool openCrate = false;
    public Transform destiny;
    public Transform parent;
    public Transform crateDestiny;
    public Transform formerParent;
    public GameObject[] items;
    public GameObject closedCrate;
    public GameObject ropePrefab;
    public GameObject raftPrefab;
    public float spawnDiameter = 1f;
    public GameObject newBox;
    public ItemScript newBoxScript;
    public GameObject transitionsManager;
    public GameObject throwIsland;
    public GameObject levelEnd;
    public GameObject cameraRig;
    public string[] transitions;
    public bool initPar;
    protected float anim;
    public Vector3 initPos;
    public Vector3 endPos;
    // Ambient variables
    [SerializeField] private Material highlightMaterial = null;
    [SerializeField] private Material defaultMaterial = null;
    private Transform _selection;

    // Private Class Functions

    // Creates and returns a closed box with the itens inside it
    void createBox() {
        Vector3 Spawn = this.transform.position;
        Spawn.y += 0.5f;
        newBox = (GameObject) Instantiate(closedCrate, Spawn, Quaternion.identity);
        newBox.transform.parent = this.gameObject.transform.parent;
        newBoxScript = newBox.GetComponent<ItemScript>();
        newBoxScript.items = new GameObject[10];
        newBoxScript.parent = this.gameObject.transform.parent;
        TransitionsScript transitionScript = transitionsManager.GetComponent<TransitionsScript>();
        for (int i = 0; i < 10; i++) {
            if(items[i]) {
                newBoxScript.items[i] = items[i];
                transitionScript.items[i] = items[i];
                items[i].transform.parent = newBox.transform;
                items[i].SetActive(false);
                //items[i] = null;
            }
        }
        transitionScript.printTransition = true;
        return;
    }

    // Finds an child in a Game Object
    GameObject findInChildren(GameObject obj, string name) {
        return (from x in obj.GetComponentsInChildren<Transform>()
                where x.gameObject.name == name
                select x.gameObject).First();
    }

    Vector3 calculatePath (Vector3 start, Vector3 end, float height, float t) {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    // Start is called before de first frame update
    void Start() {
        if (items.Length == 0) {
            items = new GameObject[10];
        }
        if (transitions.Length == 0) {
            transitions = new string[10];
            for (int i=0; i<10; i++) {
                transitions[i] = "";
            }
        }
        transitionsManager = GameObject.Find("TransitionsManager");
    }

    // Update is called once per frame
    void Update() {
        // Picking up the item
        if ((!isCarrying && pickupItem) || (!isCarrying && pickupBox)) {
            formerParent = this.gameObject.transform.parent;
            parent = findInChildren(player.gameObject, "hand").transform;
            if (player.actionItem.tag == "Axes") {
                destiny = findInChildren(parent.gameObject, "destinationAxe").transform;
            } else if (player.actionItem.tag == "Knives") {
                destiny = findInChildren(parent.gameObject, "destinationKnife").transform;
            } else if (player.actionItem.tag == "Hammers") {
                destiny = findInChildren(parent.gameObject, "destinationHammer").transform;
            } else if (player.actionItem.tag == "Ropes") {
                destiny = findInChildren(parent.gameObject, "destinationRope").transform;
            } else if (this.tag == "Boxes") {
                destiny = findInChildren(parent.gameObject, "destinationBox").transform;
            }
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshCollider>().enabled = false;
            GetComponent<NavMeshObstacle>().enabled = false;
            this.transform.parent = parent;
            this.transform.rotation = destiny.rotation;
            this.transform.position = destiny.position;
            isCarrying = true;
            pickupItem = false;
            pickupBox = false;
            player.inHandItem = gameObject;
            player.inHandItemScript = this;
            player.actionItem = null;
            player.actionItemScript = null;
            player.animator.SetBool("pickupItem", false);
            player.animator.SetBool("pickupBox", false);
            return;
        } 
        
        if (isCarrying && dropItem) {
            this.transform.parent = formerParent;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<MeshCollider>().enabled =true;
            GetComponent<NavMeshObstacle>().enabled = true;
            isCarrying = false;
            dropItem = false;
            player.inHandItem = null;
            player.inHandItemScript = null;
            player = null;
            return;
        }

        if (isCarrying && dropItemBox) {
            this.transform.parent = player.actionItem.transform;
            if (this.gameObject.tag == "Ropes") {
                this.transform.position = player.actionItem.transform.Find("crateDestinyRope").transform.position;
            } else {
                this.transform.position = player.actionItem.transform.Find("crateDestiny").transform.position;
            }
            //GetComponent<Rigidbody>().useGravity = true;
            //GetComponent<BoxCollider>().enabled = true;
            //GetComponent<MeshCollider>().enabled =true;
            //GetComponent<NavMeshObstacle>().enabled = true;
            int i = 0;
            while (player.actionItemScript.items[i]){
                i++;
            }
            player.actionItemScript.items[i] = player.inHandItem;
            isCarrying = false;
            dropItemBox = false;
            player.inHandItem = null;
            player.inHandItemScript = null;
            player.actionItem = null;
            player.actionItemScript = null;
            player = null;
            return;
        }

        if (isCarrying && throwBox) {
            GameObject crateIsland = findInChildren (throwIsland, "landingCrate");
            destiny = null;
            //endPos = crateIsland.transform.position;
            //initPos = this.gameObject.transform.position;
            //initPar = true;
            this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
            this.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f,0f,0f);
            this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,0f));
            this.gameObject.transform.position = crateIsland.transform.position;
            this.gameObject.transform.parent = parent;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<MeshCollider>().enabled = true;
            GetComponent<NavMeshObstacle>().enabled = true;
            GetComponent<Rigidbody>().AddForce(player.throwDirection * player.throwForce);
            isCarrying = false;
            throwBox = false;
            player.inHandItem = null;
            player.inHandItemScript = null;
            player.actionItem = null;
            player.actionItemScript = null;
            player = null;
            return;
        }
        //Debug.Log(this.gameObject.transform.position);

        if (closeCrate) {
            this.gameObject.SetActive(false);
            createBox();
            closeCrate = false;
            return;
        }

        if (openCrate) {
            Vector3 randomPos;
            Vector3 posit = transform.position;
            for (int i = 0; i < 10; i++) {
                Debug.Log("Loop open item: ", items[i]);
                if (items[i]) {
                    randomPos.x = UnityEngine.Random.Range(posit.x - spawnDiameter, posit.x + spawnDiameter);
                    randomPos.y = posit.y + 1;
                    randomPos.z = UnityEngine.Random.Range(posit.z - spawnDiameter, posit.z + spawnDiameter);
                    items[i].transform.position = randomPos;
                    items[i].SetActive(true);
                    items[i].transform.parent = throwIsland.transform;
                    items[i].GetComponent<Rigidbody>().useGravity = true;
                    items[i].GetComponent<BoxCollider>().enabled = true;
                    items[i].GetComponent<MeshCollider>().enabled = true;
                    items[i].GetComponent<NavMeshObstacle>().enabled = true;
                    // encontrar em qual ilha ele está
                    //items[i].transform.parent = 
                }
            }
            this.gameObject.SetActive(false);
            //foreach (GameObject item in items) {
            //    randomPos.x = Random.Range(posit.x - spawnDiameter, posit.x + spawnDiameter);
            //    randomPos.y = posit.y + 1;
            //    randomPos.z = Random.Range(posit.z - spawnDiameter, posit.z + spawnDiameter);
            //    item.transform.position = randomPos;
            //    item.SetActive(true);
                // Trocar o parent para o prop da outra ilha
            //}
            //this.gameObject.SetActive(false);
            openCrate = false;
            //int i = 0;
            //while (items[i]) {
            //    items[i] = null;
            //    i++;
            //}
            //foreach (GameObject item in items) {
            //    items[i] = null;
            //    i++;
            //}
            return;
        }

        if (craftItem) {
            if (this.tag == "Palms") {
                Vector3 randomPos;
                Vector3 posit = transform.position;
                randomPos.x = UnityEngine.Random.Range(posit.x - spawnDiameter, posit.x + spawnDiameter);
                randomPos.y = posit.y + 1;
                randomPos.z = UnityEngine.Random.Range(posit.z - spawnDiameter, posit.z + spawnDiameter);
                Instantiate(ropePrefab, randomPos, Quaternion.identity);
                craftItem = false;
            } else if (this.tag == "UnfinishedRaft") {
                Transform location = this.gameObject.transform.parent.Find("raftLocation");
                Vector3 pos = location.position;
                GameObject raft = Instantiate(raftPrefab, pos, Quaternion.identity);
                raft.transform.rotation = location.rotation;
                this.gameObject.SetActive(false);
                craftItem = false;
                player.isSelected = false;
                player.animator.SetBool("isSelected", player.isSelected);
                cameraRig.GetComponent<CameraController>().followTransform = null;
                cameraRig.GetComponent<CameraController>().newPosition = new Vector3 (97f, 10f, -23.7f);
                transitionsManager.SetActive(false);
                levelEnd.SetActive(true);
            } // else if (actionItem.tag == "Trees") {}
            player.actionItem = null;
            player.actionItemScript = null;
            return;
        }

        //if (initPar) {
            //Debug.Log("ENTREI NO PAR");
            //anim += Time.deltaTime;
            //anim = anim % 5f;
            //this.gameObject.transform.position = calculatePath(initPos, endPos, 5f, anim / 5f);
            //Debug.Log(anim);
            //if(anim >= 4.8f) {
                //initPar = false;
                //this.gameObject.transform.position = endPos;
                //GetComponent<Rigidbody>().useGravity = true;
                //GetComponent<BoxCollider>().enabled = true;
                //GetComponent<NavMeshObstacle>().enabled = true;
            //}
        //}

        if (_selection != null) {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }
        var rayHighlight = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitHighlight;
        if (Physics.Raycast(rayHighlight, out hitHighlight)) {
            var selection = hitHighlight.transform;
            if (this.tag == "UnfinishedRaft") {
                if (selection.CompareTag("Logs")) {
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer != null) {
                        selectionRenderer.material = highlightMaterial;
                    }
                    _selection = selection;
                }
            } else if (selection.CompareTag(this.tag)) {
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null) {
                    selectionRenderer.material = highlightMaterial;
                }
                _selection = selection;
            }
        }

        //if ()
    }
}