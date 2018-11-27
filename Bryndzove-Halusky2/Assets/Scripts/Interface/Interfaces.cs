using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// inherited anything that can pick up PickUp items in the map
public interface ICanPickup
{
    void OnPickUp(PickupType pickupType);
}
