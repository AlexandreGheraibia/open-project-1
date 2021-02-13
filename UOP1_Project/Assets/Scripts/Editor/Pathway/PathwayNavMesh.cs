﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.AI;


public class PathwayNavMesh 
{
	private bool _toggled;
	private Pathway _pathway;
	
	public PathwayNavMesh(Pathway pathway)
	{
		_pathway = pathway;
		_toggled = false;
		_pathway.Hits = new List<Pathway.HitPoint>();
		_pathway.DisplayPolls = false;
		_pathway.Path = new List<Vector3>();
	}

	private bool PollsNavMesh()
	{
		NavMeshHit hit;
		bool hasHit;
		bool result = true;

		_pathway.Hits.Clear();

		for (int i = 0; i < _pathway.Waypoints.Count; i++)
		{
			hasHit = NavMesh.SamplePosition(_pathway.Waypoints[i], out hit, _pathway.MeshSize * 2, NavMesh.AllAreas);
			_pathway.Hits.Add(new Pathway.HitPoint(hasHit, hit.position));
			result &= hasHit;
		}

		return result;
	}

	private bool GenerateNavMeshPath()
	{
		bool canGeneatePath = true;
		int i = 1;
		NavMeshPath navMeshPath = new NavMeshPath();
		
		while ( i < _pathway.Waypoints.Count)
		{
			canGeneatePath &= NavMesh.CalculatePath(_pathway.Waypoints[i - 1], _pathway.Waypoints[i], NavMesh.AllAreas, navMeshPath);
			if (canGeneatePath)
			{
				for (int j = 0; j < navMeshPath.corners.Length; j++)
				{
					_pathway.Path.Add(navMeshPath.corners[j]);
				}
			}
			else
			{
				Debug.LogError("the path between " + (i-1) + " and " + i + " can't be generated by NavMeshPAth");
			}

			i++;
		}

		if (!canGeneatePath)
		{
			_pathway.Path.Clear();
		}

		return canGeneatePath;
	}

	public void OnInspectorGUI()
	{
		if (_toggled == false)
		{
			if (_toggled = GUILayout.Button("NavMesh Path"))
			{
				if (PollsNavMesh())
				{
					if (_pathway.Waypoints.Count > 1)
					{
						if (GenerateNavMeshPath())
						{
							InternalEditorUtility.RepaintAllViews();
						}
					}
					else
						Debug.LogError("Pathway need more than one point to calculate the path");
				}
				else
				{
					_pathway.DisplayPolls = true;
					InternalEditorUtility.RepaintAllViews();
				}
			}
		}
		else
		{
			if (GUILayout.Button("Handles Path"))
			{
				_toggled = false;
				_pathway.DisplayPolls = false;
				_pathway.Path.Clear();
				InternalEditorUtility.RepaintAllViews();
			}

			if (_pathway.DisplayPolls)
			{
				if (GUILayout.Button("Hide Polls"))
				{
					_pathway.DisplayPolls = false;
					InternalEditorUtility.RepaintAllViews();
				}
			
				if (GUILayout.Button("Refresh polls"))
				{
					PollsNavMesh();
					InternalEditorUtility.RepaintAllViews();
				}
				
			}
			else
			{
				if (GUILayout.Button("Show polls"))
				{
					_pathway.DisplayPolls = true;
					InternalEditorUtility.RepaintAllViews();
				}
			}
		}
	}

}
