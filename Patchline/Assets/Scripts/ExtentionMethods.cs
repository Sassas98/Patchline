using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public static class ExtentionMethods
    {
        public static List<GameObject> Childrens (this GameObject o)
        {
            var list = new List<GameObject>();
            foreach (Transform child in o.transform)
            {
                list.Add(child.gameObject);
            }
            return list;
        }
    }
}
