using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPGCCC.Core;
using RPGCCC.Control;

namespace RPGCCC.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        PlayerController playerController;

        private void OnEnable() 
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable() 
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        private void Awake() 
        {
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
        }
        void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            playerController.enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            playerController.enabled = true;
        }
    }
}
