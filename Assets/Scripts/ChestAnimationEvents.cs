using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimationEvents : MonoBehaviour {

    public void PlayTicketsAnim()
    {
        EndingManager.instance.PlayTicketAnim();
    }
}
