
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    ///unused/ no time
    /// <summary>
    /// Fades in/ out sprites
    /// </summary>
    public class SpriteFader : MonoBehaviour
    {
        Renderer renderer; //?
        GameManager gameManager;
        public float fadeTime = .5f;
        private void Start()
        {
            renderer = this.GetComponent<Renderer>();
            //gameManager = Database.sGameManager;
        }
        public void StartFade(float aValue, bool returnBool)
        {
            StartCoroutine(FadeTo(aValue, fadeTime, returnBool));
        }
        //Nicht von mir \/
        //https://answers.unity.com/questions/654836/unity2d-sprite-fade-in-and-out.html
        //Antwort von fonko/*
        IEnumerator FadeTo(float aValue, float aTime, bool returnBool)
        {
            float alpha = renderer.material.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                renderer.material.color = newColor;
                yield return null;
            }
            //gameManager.FinishedFade(returnBool);
        }
        //*/............/\
    }
}
