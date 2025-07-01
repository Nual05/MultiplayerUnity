using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Transform[] redSpawns;
    public Transform[] blueSpawns;
    int redCount = 0;
    int blueCount = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos;
        bool isRed = redCount <= blueCount;

        if (isRed)
        {
            startPos = redSpawns[redCount % redSpawns.Length];
            redCount++;
        }
        else
        {
            startPos = blueSpawns[blueCount % blueSpawns.Length];
            blueCount++;
        }

        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);

        var teamComp = player.GetComponent<PlayerTeam>();
        if(teamComp != null)
        {
            teamComp.team = isRed ? Team.Red : Team.Blue;
        }

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
