using UnityEngine;

public class TestMover : MonoBehaviour
{
    public float moveHeight = 1f;
    public float moveDuration = 2f;

    Vector3 startPos;
    Vector3 endPos;
    float timer = 0f;
    bool goingUp = true;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + Vector3.up * moveHeight;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / moveDuration;
        if (goingUp)
            transform.position = Vector3.Lerp(startPos, endPos, t);
        else
            transform.position = Vector3.Lerp(endPos, startPos, t);

        if (t >= 1f)
        {
            timer = 0f;
            goingUp = !goingUp;
        }
    }
}
