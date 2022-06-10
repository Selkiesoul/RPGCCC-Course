using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPGCCC.Control;
using UnityEngine.AI;

namespace RPGCCC.SceneManagement
{

    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z
        }
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.2f;
        [SerializeField] float fadeOutTime = 2f;
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;

        private void OnTriggerEnter(Collider other) 
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            Fader fader = FindObjectOfType<Fader>();

            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load has not been set");
                yield break;
                // ^ this works like return in an enumerator, since if you try to use return it is expecting a return value
            }

            DontDestroyOnLoad(gameObject);
            DisableMovement();
            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            savingWrapper.Load();

            Portal spawnPortal = GetSpawnPortal();
            UpdatePlayer(spawnPortal);
            DisableMovement();
            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
            EnableMovement();

            Destroy(gameObject);
        }

        void EnableMovement()
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerController>().enabled = true;
        }

        void DisableMovement()
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerController>().enabled = false;
        }


        private void UpdatePlayer(Portal spawnPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = spawnPortal.spawnPoint.position;
            player.transform.rotation = spawnPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
            //.transform.position = gameObject.GetComponentInChildren(transform.position);
        }

        private Portal GetSpawnPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                // ^ links up a specific portal in the original scene to the portal with the same enum in the next scene
                return portal;
            }
            return null;
        }


    }
}
