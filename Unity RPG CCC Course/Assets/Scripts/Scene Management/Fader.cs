using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCCC.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        float fadeInTime = 2f;
        float fadeOutTime = 2f;
        private void Awake() 
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        IEnumerator FadeOutIn()
        {
            yield return FadeOut(fadeOutTime);
            yield return FadeIn(fadeInTime);
        }

        public IEnumerator FadeOut(float time)
        {
            while(canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
                //yield return null makes the coroutine wait until the next frame to run again, so it executes the code once per frame
            }
        }

        public IEnumerator FadeIn(float time)
        {
            while(canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
