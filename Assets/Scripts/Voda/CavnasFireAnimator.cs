using UnityEngine;
using UnityEngine.UI;

public class CanvasFireAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 0.1f;

    private Image image;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool isPlaying = true;

    void Start()
    {
        image = GetComponent<Image>();
        // image.enabled = false;
    }

    void Update()
    {
        if (isPlaying)
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

    public void StartAnimation()
    {
        isPlaying = true;
        image.enabled = true;
    }

    public void StopAnimation()
    {
        isPlaying = false;
        image.enabled = false;
    }

    public void ToggleAnimation()
    {
        if (isPlaying)
            StopAnimation();
        else
            StartAnimation();
    }
}