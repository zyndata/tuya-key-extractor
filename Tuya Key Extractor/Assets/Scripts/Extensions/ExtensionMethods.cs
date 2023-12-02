using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	public static int ChildCountActive (this Transform t)
	{
		int activeCount = 0;

		foreach (Transform child in t)
		{
			if (child.gameObject.activeSelf)
			{
				activeCount++;
			}
		}

		return activeCount;
	}

	public static void SetActiveOptimized (this GameObject target, bool state)
	{
		if (target.activeSelf != state)
		{
			target.SetActive(state);
		}
	}

	public static void SetActiveOptimizedAll (this GameObject[] target, bool state)
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

	public static void SetActiveOptimizedAll (this List<GameObject> target, bool state)
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

	public static void ClearChildren (this Transform transform)
	{
		if (transform != null)
		{
			foreach (Transform child in transform)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}
}
