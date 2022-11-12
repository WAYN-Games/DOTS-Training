using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TowerRegister : MonoBehaviour
{
	public List<GameObject> Towers;

	class Baker : Baker<TowerRegister>
	{
		public override void Bake(TowerRegister authoring)
		{
			var Buffer = AddBuffer<Towers>();
			foreach(var tower in authoring.Towers)
			{
				Buffer.Add(new Towers() { Prefab = GetEntity(tower) });
			}
		}
	}
}

public struct Towers : IBufferElementData
{
	public Entity Prefab;
}