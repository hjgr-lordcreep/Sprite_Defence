using System.Collections.Generic;
using UnityEngine;

public interface IBoidsRule
{
    // 보이드(agent)와 주변 이웃(neighbors)을 기반으로 방향을 계산
    Vector3 GetDirection(Transform agent, List<Transform> neighbors);
}
