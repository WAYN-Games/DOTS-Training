using Unity.Entities;
using UnityEngine;

public class LimitedLifeTimeEntity : MonoBehaviour
{
	public float Duration;
	class Baker : Baker<LimitedLifeTimeEntity>
	{
		public override void Bake(LimitedLifeTimeEntity authoring)
		{
			var bakingEntity = GetEntity(TransformUsageFlags.None);
			AddComponent(bakingEntity,new LimitedLifeTime() { TimeRemaining = authoring.Duration });
		}
	}
}