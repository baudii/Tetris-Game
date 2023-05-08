using UnityEngine;

public class BackAndForthRotation : Block
{
    [SerializeField] float[] rotationStates;
    int i;
    protected override void PerformRotation()
    {
        var state = rotationStates[i % rotationStates.Length];
        transform.RotateAround(transform.TransformPoint(rotationPivot), Vector3.forward, state);
        if (!SetValidPosition())
        {
            transform.RotateAround(transform.TransformPoint(rotationPivot), Vector3.forward, rotationStates[(i+1) % rotationStates.Length]);
            i++;
        }
        i++;

    }
}
