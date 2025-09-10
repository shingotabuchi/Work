using UnityEngine;
using System;
using System.Collections.Generic;

namespace Fwk.Utility
{
    public static class SkinnedMeshUtility
    {
        public static void TransferRenderer(SkinnedMeshRenderer target, SkinnedMeshRenderer destination)
        {
            Transform rootBone = destination.rootBone;
            Dictionary<string, Transform> boneDictionary = new Dictionary<string, Transform>();
            Transform[] rootBoneChildren = rootBone.GetComponentsInChildren<Transform>();
            foreach (Transform child in rootBoneChildren)
            {
                boneDictionary[child.name] = child;
            }

            Transform[] newBones = new Transform[target.bones.Length];
            for (int i = 0; i < target.bones.Length; i++)
            {
                if (boneDictionary.TryGetValue(target.bones[i].name, out Transform newBone))
                {
                    newBones[i] = newBone;
                }
            }
            target.bones = newBones;
            target.rootBone = destination.rootBone;
            target.probeAnchor = destination.probeAnchor;
        }
    }
}