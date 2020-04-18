using System.Collections;
using UnityEngine;

public class TempText : MonoBehaviour
{
    private TextMesh textMesh;
    public float upSpeed = 0.1f;
    public float destroyTime = 5f;
    private float startTime;

    public void Init(string text)
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = text;
    }

    void Start()
    {
        startTime = Time.time;
        textMesh = GetComponent<TextMesh>();
        StartCoroutine(Destroy());
    }

    void Update()
    {
        textMesh.color = new Color(0, 0, 0,
             Mathf.LerpUnclamped(1, 0, (Time.time - startTime) / destroyTime));
        transform.position = transform.position + Vector3.up * upSpeed;
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
