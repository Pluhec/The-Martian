using UnityEngine;
using UnityEngine.UI;

public class FireAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 0.1f;
    private Image image;
    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
        }
    }
}
