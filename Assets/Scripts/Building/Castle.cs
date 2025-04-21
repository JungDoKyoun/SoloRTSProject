using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Castle : Building, IResourceDepot
{
    public void ReceiveResource(ResourcesType type, int amount)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        photonView.RPC("RPCReceiveResource", RpcTarget.All, _player.PlayerID, (int)type, amount);
    }

    [PunRPC]
    private void RPCReceiveResource(int playerID, int type, int amount)
    {
        var target = PlayerManager.Instance.GetPlayer(playerID);
        target?.AddResources((ResourcesType)type, amount);
    }
}
