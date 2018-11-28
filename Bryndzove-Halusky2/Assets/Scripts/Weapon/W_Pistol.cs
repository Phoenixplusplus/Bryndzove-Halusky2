using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_Pistol : W_Weapon {

    W_Pistol()
    {
        clipSize = 12;
        ammoCount = 12;
        shotDelay = 0.01f;
        reloadDelay = 0.8f;
        shotSpeed = 50f;
    }

	// Use this for initialization
	void Start ()
    {
        Character = transform.root.gameObject.GetComponent<C_Character>();
    }

    // called by base class Fire(), this sends the RPC to all players in room to spawn a paintball at the given location with other attributes
    // this is based on the Muzzle child position and we also set the paintball colour here depending on the character's team too
    public override void CreatePaintball()
    {
        base.Muzzle = transform.Find("Muzzle");

        // set paintball colour
        if (Character.Team == "Red") paintballColour = new Vector3(1, 0, 0);
        else paintballColour = new Vector3(0, 0, 1);

        photonView.RPC("CreatePaintballRPC", PhotonTargets.All, new object[]
        { Muzzle.transform.position, Muzzle.transform.rotation, paintballColour, shotSpeed, Character.Team, Owner});
    }

    [PunRPC]
    public void CreatePaintballRPC(Vector3 position, Quaternion rotation, Vector3 colour, float speed, string team, string owner)
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().SetPaintball(position, rotation, colour, speed, team, owner);
    }
}
