using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootButton : MonoBehaviour
{
    public void Shoot()
    {
      GetComponent<Image>().color = new Color(255, 138, 0, 233);
    }

    public void Release()
    {
      GetComponent<Image>().color = new Color(0, 255, 0, 255);
    }
}
