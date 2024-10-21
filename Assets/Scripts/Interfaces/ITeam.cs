using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam
{
    public int TeamId { get; set; }

    public void SetTeam(int newTeamID);

}
