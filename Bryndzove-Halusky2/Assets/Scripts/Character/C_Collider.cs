using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Collider : MonoBehaviour, ICanPickup {

    // this script is used on the character as a separate entity to detect pickups
    public void OnPickUp(PickupType pickupType) { }

}
