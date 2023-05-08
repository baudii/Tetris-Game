using UnityEngine;
using System.Collections;

public class LineClear : MonoBehaviour
{
    public void Init(int dir)
    {
        StartCoroutine(Interpolate(dir));
    }

    IEnumerator Interpolate(int dir)
    {
        yield return new WaitForSeconds(0.01f);
        float t = 0;
        var initial = transform.position.x;
        var goal = transform.position.x + 5 * dir;
        while (true)
        {
            transform.position = transform.position.WhereX(Mathf.Lerp(initial, goal, t));
            yield return null;
            if (t >= 1)
                break;
            t += Time.deltaTime * 12;
        }
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
