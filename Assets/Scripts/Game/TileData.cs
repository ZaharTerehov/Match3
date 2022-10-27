
using System.Collections.Generic;
using Game.Level;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData
{
	public Vector3Int Position { get; }
	public Tile Tile { get; }
	public TypeTile TypeTile { get; } 

	public TileData(Vector3Int position, Tile tile, TypeTile typeTile)
	{
		Position = position;
		Tile = tile;
		TypeTile = typeTile;
	}

	public bool IsTileNeighbor(TileData tile)
	{
		if (Tile != tile.Tile || Position == tile.Position)
			return false;

		if (tile.Position.x == Position.x)
		{
			if (tile.Position.y == Position.y - 1 || tile.Position.y == Position.y + 1)
				return true;
		}

		if (tile.Position.y == Position.y)
		{
			if (tile.Position.x - 1 == Position.x || tile.Position.x + 1 == Position.x)
				return true;
		}

		if (tile.Position.x != Position.x - 1 && tile.Position.x != Position.x + 1) 
			return false;
		
		if (tile.Position.y == Position.y - 1 || tile.Position.y == Position.y + 1)
			return true;

		return false;
	}
	
	public override int GetHashCode() => Position.GetHashCode();
	
	public override bool Equals([CanBeNull] object obj)
	{
		return obj is TileData data && Position.Equals(data.Position);
	}
	
	public static bool operator ==(TileData left, TileData right)
	{
		return EqualityComparer<TileData>.Default.Equals(left, right);
	}
    
	public static bool operator !=(TileData left, TileData right)
	{
		return !(left == right);
	}
}