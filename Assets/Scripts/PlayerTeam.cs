using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum Team
{
    Red,
    Blue
}

public class PlayerTeam : NetworkBehaviour
{
    [SyncVar] public Team team;
    public Material redMat;
    public Material blueMat;

    public override void OnStartClient()
    {
        base.OnStartClient();

    
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material = team == Team.Red ? redMat : blueMat;
        }
    }
}
