using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : ProductionBuilding, IResourceDepot
{
    public void ReceiveResource(ResourcesType type, int amount, Player player)
    {
        player.AddResources(type, amount);
    }

    [PunRPC]
    private void RPCReceiveResource(int playerID, ResourcesType type, int amount)
    {
        var target = PlayerManager.Instance.GetPlayer(playerID);
        target.SetResource(type, amount);
    }
}
