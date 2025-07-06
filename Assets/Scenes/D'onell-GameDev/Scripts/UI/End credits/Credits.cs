using UnityEngine;

public class Credits : MonoBehaviour
{
    public RectTransform rectTransform;
    
    private float scrollSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        scrollSpeed = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition += new Vector2 (0, scrollSpeed * Time.deltaTime);
    }
}
