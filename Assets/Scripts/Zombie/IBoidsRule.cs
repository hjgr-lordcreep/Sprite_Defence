using System.Collections.Generic;
using UnityEngine;

public interface IBoidsRule
{
    // ���̵�(agent)�� �ֺ� �̿�(neighbors)�� ������� ������ ���
    Vector3 GetDirection(Transform agent, List<Transform> neighbors);
}
