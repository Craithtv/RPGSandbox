using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.2f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());//grabs scene after Start, making sure stats are correct
        }

        private IEnumerator LoadLastScene()
        {
           
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);//loads up last scene on start
            Fader fader = FindObjectOfType<Fader>(); //grabs fader ref
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);//hides camera jarring when loading and warping player
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {//call to saving system load
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
