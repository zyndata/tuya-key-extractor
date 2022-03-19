using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.AI;

public static class ExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.


    public static void SetLayerRecursively(this GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static T DeepCopy<T>(this T item)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, item);
        stream.Seek(0, SeekOrigin.Begin);
        T result = (T)formatter.Deserialize(stream);
        stream.Close();
        return result;
    }

    public static LinkedListNode<T> RemoveAt<T>(this LinkedList<T> list, int index)
    {
        LinkedListNode<T> currentNode = list.First;
        for (int i = 0; i <= index && currentNode != null; i++)
        {
            if (i != index)
            {
                currentNode = currentNode.Next;
                continue;
            }

            list.Remove(currentNode);
            return currentNode;
        }

        throw new IndexOutOfRangeException();
    }

    public static LinkedListNode<T> ElementAt<T>(this LinkedList<T> list, int index)
    {
        LinkedListNode<T> currentNode = list.First;
        for (int i = 0; i <= index && currentNode != null; i++)
        {
            if (i != index)
            {
                currentNode = currentNode.Next;
                continue;
            }

            return currentNode;
        }
        return null;
    }

    public static int ChildCountActive(this Transform t)
    {
        int activeCount = 0;
        foreach (Transform child in t)
        {
            if (child.gameObject.activeSelf)
                activeCount++;
        }
        return activeCount;
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static RaycastHit GetClosestHit(this RaycastHit[] hitResult, int currentHits, Collider ignoredCollider)
    {
        float distance = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < currentHits; i++)
        {
            if (hitResult[i].transform != null && hitResult[i].distance < distance && hitResult[i].collider != ignoredCollider)
            {
                index = i;
                distance = hitResult[i].distance;
            }
        }
        return hitResult[index];
    }

    public static RaycastHit GetClosestHit(this RaycastHit[] hitResult, int currentHits)
    {
        float distance = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < currentHits; i++)
        {
            if (hitResult[i].transform != null && hitResult[i].distance < distance)
            {
                index = i;
                distance = hitResult[i].distance;
            }
        }
        return hitResult[index];
    }

    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    public static void SetActiveOptimized(this GameObject target, bool state)
    {
        if (target != null && target.activeSelf != state)
        {
            target.SetActive(state);
        }
    }

    public static void SetActiveOptimizedAll(this GameObject[] target, bool state)
    {
        if (target != null)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] != null)
                {
                    target[i].SetActiveOptimized(state);
                }
            }
        }
    }

    public static void SetActiveOptimizedAll(this List<GameObject> target, bool state)
    {
        if (target != null)
        {
            for (int i = 0; i < target.Count; i++)
            {
                if (target[i] != null)
                {
                    target[i].SetActiveOptimized(state);
                }
            }
        }
    }

    public static void ClearChildren(this Transform transform)
    {
        if (transform != null)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public static Vector3 ClosestPointOnLine(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 originToPoint = point - lineStart;
        Vector3 lineDirection = (lineEnd - lineStart).normalized;

        float distance = Vector3.Distance(lineStart, lineEnd);
        float dot = Vector3.Dot(lineDirection, originToPoint);

        if (dot <= 0)
        {
            return lineStart;
        }
        if (dot >= distance)
        {
            return lineEnd;
        }

        Vector3 vect = lineDirection * dot;
        Vector3 closestPoint = lineStart + vect;
        return closestPoint;
    }

    public static float DistanceToClosestPointOnLine(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Distance(point, ClosestPointOnLine(point, lineStart, lineEnd));
    }
}