using System.Collections;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    public abstract IEnumerator Execute(Boss boss, Transform player, BossStats stats);
}