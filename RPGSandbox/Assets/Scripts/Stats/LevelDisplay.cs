using UnityEngine;
using TMPro;
using System;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats level;

        private void Awake()
        {
            level = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = String.Format("{0:0}", level.CalculateLevel());//limits how many decimals are displayed
        }
    }
}
