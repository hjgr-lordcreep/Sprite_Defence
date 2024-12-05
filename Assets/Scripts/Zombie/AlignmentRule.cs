using System.Collections.Generic;
using UnityEngine;

public class AlignmentRule : IBoidsRule
{
    public Vector3 GetDirection(Transform agent, List<Transform> neighbor)
    {
        if (agent == null)
            return Vector3.zero;

        // �̿��� ���ٸ�, �������� ����
        if (neighbor == null || neighbor.Count == 0)
            return agent.forward;

        Vector3 neighborDir = Vector3.zero;

        foreach (var ne in neighbor)
        {
            neighborDir += ne.transform.forward;
        }

        return (neighborDir /= neighbor.Count).normalized;
    }
}