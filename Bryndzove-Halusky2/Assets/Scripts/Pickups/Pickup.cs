using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType { Random, AmmoUp, HealthUp, SpeedUp };

public class Pickup : MonoBehaviour {

    [Header("Attributes")]
    public PickupType pickUpType;
    public float bounceHeight = 0.5f;
    public float rotateRate = 90f;
    public float hideTime = 5f;

    [Header("Materials")]
    public Material baseMaterial;
    public Material secondaryMaterial1;
    public Material secondaryMaterial2;
    public Material secondaryMaterial3;
    public Material secondaryMaterial4;
    public Material secondaryMaterial5;
    Dictionary<string, Material> materialDict;
    public Material ammoUp;
    public Material speedUp;
    public Material healthUp;

    Vector3 startPos;
    bool movingUp = true;
    float currentHideTime = 0f;

    void Awake()
    {
        // assign random secondary colour
        materialDict = new Dictionary<string, Material>();
        materialDict.Add("secondaryMaterial1", secondaryMaterial1);
        materialDict.Add("secondaryMaterial2", secondaryMaterial2);
        materialDict.Add("secondaryMaterial3", secondaryMaterial3);
        materialDict.Add("secondaryMaterial4", secondaryMaterial4);
        materialDict.Add("secondaryMaterial5", secondaryMaterial5);

        int rand = Random.Range(1, 6);
        transform.Find("Present/Model").GetComponent<Renderer>().materials = new Material[] { materialDict["secondaryMaterial" + rand], baseMaterial, materialDict["secondaryMaterial" + rand], materialDict["secondaryMaterial" + rand] };

        // assign the billboard material to match pickup type
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

    public virtual void PickupEffect(GameObject other)
    {
        switch (pickUpType)
        {
            case PickupType.AmmoUp:
                {
                    Debug.Log(other.GetComponent<C_Character>().username + " got ammo box");
                    return;
                }
            case PickupType.HealthUp:
                {
                    Debug.Log(other.GetComponent<C_Character>().username + " got health box");
                    return;
                }
            case PickupType.SpeedUp:
                {
                    Debug.Log(other.GetComponent<C_Character>().username + " got speed box");
                    return;
                }
        }
    }

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

    // when any player collides, start coroutine to hide for a amount of time, and do an effect to player
    void OnTriggerEnter(Collider other)
    {
        ICanPickup something = other.GetComponent<ICanPickup>();
        if (something != null)
        {
            //something.OnPickUp();
            PickupEffect(other.gameObject.transform.parent.gameObject);
            StartCoroutine(HidePickUp());
        }
    }

    IEnumerator HidePickUp()
    {
        while (currentHideTime < hideTime)
        {
            currentHideTime += Time.deltaTime;
            GetComponent<BoxCollider>().enabled = false;
            transform.Find("Billboard").GetComponent<Renderer>().enabled = false;
            transform.Find("Present/Model").GetComponent<Renderer>().enabled = false;
            yield return null;
        }

        currentHideTime = 0f;
        GetComponent<BoxCollider>().enabled = true;
        transform.Find("Billboard").GetComponent<Renderer>().enabled = true;
        transform.Find("Present/Model").GetComponent<Renderer>().enabled = true;
        yield break;
    }
}
