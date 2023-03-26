using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{

    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();//grabs canvasgroup from fader obj       
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        IEnumerator FadeOutIn()
        {
            yield return FadeOut(3f);
            print("Faded out bro");
            yield return FadeIn(1f);
            print("Faded in bro");
        }

       public IEnumerator FadeOut(float time)
        {
            while (canvasGroup.alpha < 1)//alpha is essentially opacity
            {
                canvasGroup.alpha += Time.deltaTime / time;//increases alpha incrementaly while not >1
                yield return null; //wait 1 frame
            }
        }

       public IEnumerator FadeIn(float time)
        {
            while (canvasGroup.alpha > 0)//alpha is essentially opacity
            {
                canvasGroup.alpha -= Time.deltaTime / time;//increases alpha incrementaly while not >1
                yield return null; //wait 1 frame
            }
        }
    }
}
