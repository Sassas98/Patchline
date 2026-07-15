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
                CMD.Let => 2,
                CMD.Set => 1,
                CMD.Wait => 1,
                CMD.If => 2,
                CMD.Elif => 1,
                CMD.Else => 1,
                CMD.Loop => 3,
                CMD.Stop => 1,
                CMD.Skip => 1,
                CMD.List => 3,
                CMD.Push => 1,
                CMD.Inject => 1,
                _ => 0,
            };
        }
    }
}
