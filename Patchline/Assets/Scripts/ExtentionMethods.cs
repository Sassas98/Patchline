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

        public static int DaiCosto(this CMD cmd)
        {
            return cmd switch
            {
                CMD.Let => 3,
                CMD.Set => 2,
                CMD.Wait => 0,
                CMD.If => 3,
                CMD.Elif => 2,
                CMD.Else => 2,
                CMD.Loop => 4,
                CMD.Stop => 2,
                CMD.Skip => 2,
                CMD.List => 4,
                CMD.Push => 2,
                CMD.Inject => 2,
                _ => 0,
            };
        }
    }
}
