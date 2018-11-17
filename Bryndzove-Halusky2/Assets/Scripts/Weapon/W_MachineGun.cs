using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_MachineGun : W_Weapon {

    W_MachineGun()
    {
        clipSize = 30;
        ammoCount = 30;
        shotDelay = 0.05f;
        reloadDelay = 1.5f;
        shotSpeed = 50f;
    }

    // Use this for initialization
    void Start ()
    {
        Character = transform.root.gameObject.GetComponent<C_Character>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public override bool Fire()
    {
        return base.Fire();
        // do other machine gun only related stuff below if needed
    }

    public override bool Reload()
    {
        return base.Reload();
        // do other machine gun only related stuff below if needed
    }

    public override void CreatePaintball()
    {
        base.Muzzle = transform.Find("Muzzle");

        // set paintball colour
        if (Character.Team == "Red") paintballColour = new Vector3(1, 0, 0);
        else paintballColour = new Vector3(0, 0, 1);

        photonView.RPC("CreatePaintballRPC", PhotonTargets.All, new object[] 
        { Muzzle.transform.position, Muzzle.transform.rotation, paintballColour, shotSpeed, Character.Team});
    }

    [PunRPC]
    public void CreatePaintballRPC(Vector3 position, Quaternion rotation, Vector3 colour, float speed, string team)
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().SetPaintball(position, rotation, colour, speed, team);
    }
}
