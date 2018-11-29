using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType { Random, AmmoUp, HealthUp, SpeedUp };

public class Pickup : MonoBehaviour {

    [Header("Attributes")]
    public PickupType pickUpType;
    public float bounceHeight = 0.5f;
    public float rotateRate = 90f;
    public float hideTime = 10f;

    [Header("Materials")]
    public Material baseMaterial;
    public Material secondaryMaterial1, secondaryMaterial2, secondaryMaterial3, secondaryMaterial4, secondaryMaterial5;
    Dictionary<string, Material> materialDict;
    public Material ammoUp, speedUp, healthUp;

    private Vector3 startPos;
    private bool movingUp = true;
    private float currentHideTime = 0f;

    void Awake()
    {
        // initialise material dictionary
        materialDict = new Dictionary<string, Material>();
        materialDict.Add("secondaryMaterial1", secondaryMaterial1);
        materialDict.Add("secondaryMaterial2", secondaryMaterial2);
        materialDict.Add("secondaryMaterial3", secondaryMaterial3);
        materialDict.Add("secondaryMaterial4", secondaryMaterial4);
        materialDict.Add("secondaryMaterial5", secondaryMaterial5);

        // assign random secondary colour
        int rand = Random.Range(1, 6);
        transform.Find("Present/Model").GetComponent<Renderer>().materials = new Material[] { materialDict["secondaryMaterial" + rand], baseMaterial, materialDict["secondaryMaterial" + rand], materialDict["secondaryMaterial" + rand] };

        // assign the billboard material to match pickup type
        // instead of creating subclasses, just base which pickup is which by an enum
        if (pickUpType == PickupType.Random) { pickUpType = (PickupType)Random.Range(1, 4); } 
        if (pickUpType == PickupType.AmmoUp) transform.Find("Billboard").GetComponent<Renderer>().material = ammoUp;
        if (pickUpType == PickupType.SpeedUp) transform.Find("Billboard").GetComponent<Renderer>().material = speedUp;
        if (pickUpType == PickupType.HealthUp) transform.Find("Billboard").GetComponent<Renderer>().material = healthUp;
    }

	// Use this for initialization
	void Start ()
    {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.up, Time.fixedDeltaTime * rotateRate);
        Bounce(bounceHeight);
	}

    // bounce animation
    void Bounce(float distance)
    {
        if (movingUp == true)
        {
            transform.position += new Vector3(0, Time.fixedDeltaTime / 3, 0);
            if (transform.position.y > startPos.y + distance) movingUp = !movingUp;
        }
        else
        {
            transform.position -= new Vector3(0, Time.fixedDeltaTime / 3, 0);
            if (transform.position.y < startPos.y) movingUp = !movingUp;
        }
    }

    // when any player with ICanPickup Interface collides, start coroutine to hide for a amount of time, and do an effect to player
    void OnTriggerEnter(Collider other)
    {
        ICanPickup something = other.GetComponent<ICanPickup>();
        if (something != null)
        {
            // because there is a collider that actually detects the interface collision, we must get the parent transform that has the real function we want to call
            other.transform.parent.GetComponent<ICanPickup>().OnPickUp(pickUpType);
            StartCoroutine(HidePickUp());
        }
    }

    IEnumerator HidePickUp()
    {
        Renderer r_Billboard = transform.Find("Billboard").GetComponent<Renderer>();
        Renderer r_Model = transform.Find("Present/Model").GetComponent<Renderer>();
        while (currentHideTime < hideTime)
        {
            currentHideTime += Time.deltaTime;
            GetComponent<BoxCollider>().enabled = false;
            if (r_Billboard.enabled == true) r_Billboard.enabled = false;
            if (r_Model.enabled == true) r_Model.enabled = false;
            yield return null;
        }

        currentHideTime = 0f;
        GetComponent<BoxCollider>().enabled = true;
        if (r_Billboard.enabled == false) r_Billboard.enabled = true;
        if (r_Model.enabled == false) r_Model.enabled = true;
        yield break;
    }
}
