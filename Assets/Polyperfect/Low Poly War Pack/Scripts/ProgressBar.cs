using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PolyPerfect
{
    namespace War
    {
        [ExecuteInEditMode]
        public class ProgressBar : MonoBehaviour
        {
            public float max;
            public float current;
            public Image image;
            public Image backgroundImage;
            // Update is called once per frame
            void Update()
            {
                if (current < max)
                {
                    backgroundImage.color = Color.black;
                    image.fillAmount = current / max;
                }
                else
                {
                    backgroundImage.color = Color.white;
                }
            }

        }
    }
}