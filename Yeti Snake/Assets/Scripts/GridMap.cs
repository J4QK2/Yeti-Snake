using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap: MonoBehaviour
{
    [SerializeField] private int _width, _height;
 
    [SerializeField] private Tile _tilePrefab;
 
    [SerializeField] private Transform _gridCenter;
 
    private Dictionary<Vector2, Tile> _tiles;
 
    void Start() {
        GenerateGrid();
    }
 
    void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.parent = transform;
 
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        transform.position = new Vector3(_gridCenter.transform.position.x, _gridCenter.transform.position.y, -10);
    }
}
