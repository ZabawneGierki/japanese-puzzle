using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadRandomImage : MonoBehaviour
{

    [SerializeField]
    private Sprite[] images;

    [SerializeField]
    private Image targetImage;
    // Start is called before the first frame update
    void Start()
    {
        // Select a random image from the array
        if (images.Length > 0 && targetImage != null)
        {
            int randomIndex = Random.Range(0, images.Length);
            targetImage.sprite = images[randomIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
