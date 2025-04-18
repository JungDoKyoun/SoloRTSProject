using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceDepot
{
    void ReceiveResource(ResourcesType type, int amount);
}
