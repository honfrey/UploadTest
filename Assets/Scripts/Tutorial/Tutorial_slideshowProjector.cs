using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_slideshowProjector : MonoBehaviour {

    [SerializeField]
    private GameObject creditsGO;

    [SerializeField]
    private Texture[] slideDeck;

    [SerializeField]
    private float[] timeBetweenSlides;
    
    private Projector myProjector;
	// Use this for initialization
	void Start () {
        myProjector = this.GetComponentInChildren<Projector>();
        StartCoroutine(Slideshow());
	}
	
	// Update is called once per frame
	IEnumerator Slideshow () {
        
        for (int i = 0; i < slideDeck.Length; i++)
        {
            myProjector.material.SetTexture("_ShadowTex", slideDeck[i]);
            yield return new WaitForSeconds(timeBetweenSlides[i]);

            AudioManager.instance.PlaySlideShowClickClip();
        }
        //GameManager.instance.EscapeTheRoom();
    }
}
