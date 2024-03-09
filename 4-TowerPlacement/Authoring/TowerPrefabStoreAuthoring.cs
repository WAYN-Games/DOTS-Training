using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TowerPrefabStoreAuthoring : MonoBehaviour
{
	public List<GameObject> Towers;
	
	class Baker : Baker<TowerPrefabStoreAuthoring>
	{
		public override void Bake(TowerPrefabStoreAuthoring authoring)
		{
			var bakingEntity = GetEntity(TransformUsageFlags.None);
			var buffer = AddBuffer<TowerPrefabs>(bakingEntity);
			foreach(var tower in authoring.Towers)
			{
				buffer.Add(new TowerPrefabs() { Prefab = GetEntity(tower,TransformUsageFlags.Dynamic) });
			}
		}
	}
}