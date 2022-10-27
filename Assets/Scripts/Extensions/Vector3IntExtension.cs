
using UnityEngine;

namespace Extensions
{
	public static class Vector3IntExtension
	{
		public static Vector3Int GetRight(Vector3Int position)
		{
			return new Vector3Int(position.x + 1, position.y, 0);
		}
		
		public static Vector3Int GetLeft(Vector3Int position)
		{
			return new Vector3Int(position.x - 1, position.y, 0);
		}
		
		public static Vector3Int GetTop(Vector3Int position)
		{
			return new Vector3Int(position.x, position.y + 1, 0);
		}
		
		public static Vector3Int GetDown(Vector3Int position)
		{
			return new Vector3Int(position.x, position.y - 1, 0);
		}
	}
}