namespace Mapbox.Unity.MeshGeneration.Modifiers
{
	using UnityEngine;
	using Mapbox.Unity.MeshGeneration.Data;
	using Mapbox.Unity.MeshGeneration.Components;
	using Mapbox.Unity.MeshGeneration.Interfaces;
	using System.Collections.Generic;
	using Mapbox.Unity.Map;
	using System;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using Mapbox.VectorTile.Geometry;

	using Mapbox.Utils;
	using Mapbox.VectorTile.ExtensionMethods;
	using Mapbox.CheapRulerCs;

	[CreateAssetMenu(menuName = "Mapbox/Modifiers/Prefab Modifier")]
	public class PrefabModifier : GameObjectModifier
	{
		private Dictionary<GameObject, GameObject> _objects;
		[SerializeField]
		private SpawnPrefabOptions _options;
		private List<GameObject> _prefabList = new List<GameObject>();
		private static Dictionary<string, double[]> pos;

		public override void Initialize()
		{
			if (_objects == null)
			{
				_objects = new Dictionary<GameObject, GameObject>();
			}
		}

		public override void SetProperties(ModifierProperties properties)
		{
			_options = (SpawnPrefabOptions)properties;
			_options.PropertyHasChanged += UpdateModifier;
		}

		public override void Run(VectorEntity ve, UnityTile tile)
		{
			pos = VectorTileFactory.positionList;
			if (_options.prefab == null)
			{
				return;
			}

			GameObject go = null;

			if (_objects.ContainsKey(ve.GameObject))
			{
				go = _objects[ve.GameObject];
			}
			else
			{
				go = Instantiate(_options.prefab);
				_prefabList.Add(go);
				_objects.Add(ve.GameObject, go);
				go.transform.SetParent(ve.GameObject.transform, false);
			}
			PositionScaleRectTransform(ve, tile, go);
			// Calculate Distance between POIs and delete if too close
			var rangeBool = true;
			var geom = ve.Feature.Data.Geometry<float>();
			var location = ve.Feature.Data.GeometryAsWgs84((ulong)tile.CanonicalTileId.Z, (ulong)tile.CanonicalTileId.X, (ulong)tile.CanonicalTileId.Y)[0][0];
			var locationDouble = new double[] { location.Lat, location.Lng };
			var maxDistance = 150.0f;
			CheapRuler cr = new CheapRuler(locationDouble[1], CheapRulerUnits.Meters);
			foreach (KeyValuePair<string, double[]> position in pos)
			{
				if (cr.Distance(locationDouble, position.Value) < maxDistance)
				{
					rangeBool = false;
					break;
				}
			}
			if (rangeBool && ve.Feature.Data.Id.ToString() != null)
			{
				pos.Add(ve.Feature.Data.Id.ToString(), locationDouble);
			}
			else
			{
				go.Destroy();
			}
			if (_options.AllPrefabsInstatiated != null)
			{
				_options.AllPrefabsInstatiated(_prefabList);
			}
		}

		public void PositionScaleRectTransform(VectorEntity ve, UnityTile tile, GameObject go)
		{
			RectTransform goRectTransform;
			IFeaturePropertySettable settable = null;
			var centroidVector = new Vector3();
			foreach (var point in ve.Feature.Points[0])
			{
				centroidVector += point;
			}
			centroidVector = centroidVector / ve.Feature.Points[0].Count;
			

			go.name = ve.Feature.Data.Id.ToString();

			goRectTransform = go.GetComponent<RectTransform>();
			if (goRectTransform == null)
			{
				go.transform.localPosition = centroidVector;
				if (_options.scaleDownWithWorld)
				{
					go.transform.localScale = _options.prefab.transform.localScale * (tile.TileScale);
				}
			}
			else
			{
				goRectTransform.anchoredPosition3D = centroidVector;
				if (_options.scaleDownWithWorld)
				{
					goRectTransform.localScale = _options.prefab.transform.localScale * (tile.TileScale);
				}
			}

			settable = go.GetComponent<IFeaturePropertySettable>();
			if (settable != null)
			{
				settable.Set(ve.Feature.Properties);
			}
		}

		public override void Clear()
		{
			base.Clear();
			foreach (var gameObject in _objects.Values)
			{
				gameObject.Destroy();
			}

			foreach (var gameObject in _prefabList)
			{
				gameObject.Destroy();
			}
		}
	}
}
