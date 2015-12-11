using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
  public class SpeedLimitCollider : MonoBehaviour
  {
    void OnTriggerEnter (Collider other)
    {
      Debug.Log("Enter");
      Debug.Log(other.tag);
      Debug.Log(other.name);
    }

    void OnTriggerStay (Collider other)
    {

    }

    void OnTriggerExit (Collider other)
    {
      Debug.Log("Exit");
    }
  }
}
