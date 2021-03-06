﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Paintball : MonoBehaviour {

    private GameManager gameManager;

    [Header("Attributes")]
    public Vector3 Position;
    public Quaternion Rotation;
    public string Team;
    public string Owner;
    public float Speed = 1f;
    public float Timeout = 0f;
    public float maxTimeout = 4f;
    public bool isInit = true; // to keep paintballs where they are when manager spawns the pool of them

    [Header("Decal Attributes")]
    public GameObject SplatDecal;
    public Material[] SplatMaterials;

    [Header("Ray Attributes")]
    public LayerMask layerMask;
    public float rayLength = 1000f;

    [Header("Sound Attributes")]
    public AudioSource sourceSplats;
    public AudioClip[] audioSplats;
    public AudioClip[] playerHits;

    // ray attributes
    private Vector3 hitNormal;
    private Vector3 hitPosition;
    private GameObject hitObject;

    // Use this for initialization
    void Start ()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // main paintball behaviour
        if (!isInit)
        {
            // move forward at a constant speed
            // this helps that each player just needs to send an RPC to everyone when they fire, then everything else can be handled locally as the speed never changes,
            // of course, there could be minor differences between players because everybody does not recieve this RPC at the same time, but it is not anything to worry too much about
            // (RPC is made in the weapon script)
            transform.position = transform.position + transform.forward * (Time.deltaTime * Speed);

            // timeout after a while
            Timeout += Time.deltaTime;
            if (Timeout > maxTimeout) ResetPainball();

            // splat condition
            // when the gun fires - it tells the game manager to get the pooled game object and set it's position, then cast a ray, to see where it will hit
            // with this, we can store that position and just tell the paintball to splat when we get close enough (by again getting the game manager to position a splay decal from pool)
            if (Vector3.Distance(transform.position, hitPosition) <= 1)
            {
                AudioSource.PlayClipAtPoint(audioSplats[Random.Range(0, 4)], transform.position);
                if (Team == "Red")
                {
                    // red
                    gameManager.SetSplatDecal(hitPosition, Quaternion.LookRotation(hitNormal), SplatMaterials[Random.Range(0, 10)]);
                    if (hitObject.GetComponent<ApplyPaint>().RedTeam == false)
                    {
                        hitObject.GetComponent<ApplyPaint>().RedTeam = true;
                        gameManager.redTeamPaintCount++;
                        if (hitObject.GetComponent<ApplyPaint>().BlueTeam == true)
                        {
                            hitObject.GetComponent<ApplyPaint>().BlueTeam = false;
                            gameManager.blueTeamPaintCount--;
                        }
                    }
                }
                else
                {
                    // blue
                    gameManager.SetSplatDecal(hitPosition, Quaternion.LookRotation(hitNormal), SplatMaterials[Random.Range(10, 20)]);
                    if (hitObject.GetComponent<ApplyPaint>().BlueTeam == false)
                    {
                        hitObject.GetComponent<ApplyPaint>().BlueTeam = true;
                        gameManager.blueTeamPaintCount++;
                        if (hitObject.GetComponent<ApplyPaint>().RedTeam == true)
                        {
                            hitObject.GetComponent<ApplyPaint>().RedTeam = false;
                            gameManager.redTeamPaintCount--;
                        }
                    }
                }
                ResetPainball();
            }
        }
	}

    // handle the collision with player if they are of a different team
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            C_Character character = other.transform.root.GetComponent<C_Character>();
            if (character.Team != Team)
            {
                AudioSource.PlayClipAtPoint(playerHits[Random.Range(0, 5)], other.gameObject.transform.position);
                if (character.Health == 1) character.killedBy = Owner;
                character.Health--;
                ResetPainball();
            }
        }
    }

    // ray must be cast to determine the position and rotation of the splat (only done once on initial fire), it helps that the paintball does not change direction and keeps going forward
    // this method was used because triggers cannot get normals of face that was collided with - normal collisions with rigidbody and colliders can, however the normal of the object and collision
    // is unrealiable, so raycast was used (ignoring the paintballs themselves in case the player fired in the exact same spot constantly, and ignoring some scene objects through 'layers')
    public void PaintballRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength, layerMask))
        {
            hitNormal = hit.normal;
            hitPosition = hit.point;
            hitObject = hit.transform.gameObject;

            if (hitObject.GetComponent<ApplyPaint>() == null) Debug.Log(hitObject.name + "has no ApplyPaint script.. somehow");
        }
    }

    // reseting paintball, not destorying them
    void ResetPainball()
    {
        transform.position = gameManager.paintballsStartPosition;
        Timeout = 0f;
        isInit = true;
    }
}
