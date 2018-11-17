using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_Shotgun : W_Weapon {

    W_Shotgun()
    {
        clipSize = 8;
        ammoCount = 8;
        shotDelay = 0.2f;
        reloadDelay = 2f;
        shotSpeed = 50f;
    }

    // Use this for initialization
    void Start()
    {
        Character = transform.root.gameObject.GetComponent<C_Character>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Fire()
    {
        return base.Fire();
        // do other shotgun only related stuff below if needed
    }

    public override bool Reload()
    {
        return base.Reload();
        // do other shotgun only related stuff below if needed
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
        for (int i = 0; i < 6; i++)
        {
            Quaternion originalRot = rotation;
            rotation.eulerAngles += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            GameObject.Find("GameManager").GetComponent<GameManager>().SetPaintball(position, rotation, colour, speed, team);
            rotation = originalRot;
        }
    }
}
