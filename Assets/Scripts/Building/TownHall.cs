using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : ProductionBuilding, IResourceDepot
{
    public Player GetPlayer()
    {
        return _player;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

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
