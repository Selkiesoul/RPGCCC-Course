using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace RPGCCC.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool hasTriggered = false;
        private void OnTriggerEnter(Collider other) 
        {
            if (!hasTriggered && other.gameObject.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                hasTriggered = true;
            }
        }
    }
}
