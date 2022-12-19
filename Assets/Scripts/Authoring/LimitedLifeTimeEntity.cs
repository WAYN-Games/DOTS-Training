using Unity.Entities;
using UnityEngine;

public class LimitedLifeTimeEntity : MonoBehaviour
{
	public float Duration;
	class Baker : Baker<LimitedLifeTimeEntity>
	{
		public override void Bake(LimitedLifeTimeEntity authoring)
		{
			AddComponent(new LimitedLifeTime() { TimeRemaining = authoring.Duration });
		}
	}
}

public struct LimitedLifeTime : IComponentData
{
	public float TimeRemaining;
}