using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Piece;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    
    public class ChessGridNavigator : IChessGridNavigator
    {
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            AStarChessMoves pathfinder = new AStarChessMoves(grid);
            var path = pathfinder.FindPath(from, to, unit);
            //обработка нулевого пути
            if (path != null) {
                return path; 
            }
            return new List<Vector2Int>() { from };
            
            
        }
    }
    public class AStarChessMoves
    {
        private ChessGrid grid;
        private Vector2Int gridSize;
        public AStarChessMoves(ChessGrid grid, Vector2Int gridSize  = default(Vector2Int))
        {
            this.grid = grid;
            this.gridSize = gridSize == default(Vector2Int) ? new Vector2Int(7, 7) : gridSize;
        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, ChessUnitType unit)
        {
            HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
            Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();

            openSet.Add(start);
            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);

            while (openSet.Count > 0)
            {
                Vector2Int current = GetLowestFScore(openSet, fScore);

                if (current == goal)
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (Vector2Int neighbor in GetValidNeighbors(current, unit))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    float tentativeGScore = gScore[current] + 1;

                    if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
            return null; 
        }

        private float Heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Манхэттенское расстояние
        }

        private Vector2Int GetLowestFScore(HashSet<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
        {
            float lowestFScore = float.MaxValue;
            Vector2Int lowestFScoreNode = Vector2Int.zero;

            foreach (Vector2Int node in openSet)
            {
                if (fScore.ContainsKey(node) && fScore[node] < lowestFScore)
                {
                    lowestFScore = fScore[node];
                    lowestFScoreNode = node;
                }
            }

            return lowestFScoreNode;
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            List<Vector2Int> path = new List<Vector2Int> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
        private void AddPawnMoves(Vector2Int cell, ref List<Vector2Int> neighbors)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dy != 0)
                {
                    Vector2Int neighbor = new Vector2Int(cell.x, cell.y + dy);
                    neighbors.Add(neighbor);
                }
            }
        }
        private void AddRookMoves(Vector2Int cell, ref List<Vector2Int> neighbors)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    if (dx != 0 && dy != 0)
                    {
                        continue;
                    }

                    Vector2Int neighbor = new Vector2Int(cell.x + dx, cell.y + dy);

                    while (IsCellValid(neighbor))
                    {
                        neighbors.Add(neighbor);
                        neighbor.x += dx;
                        neighbor.y += dy;
                    }
                }
            }
        }
        private void AddBishopMoves(Vector2Int cell, ref List<Vector2Int> neighbors)
        {
            for (int dx = -1; dx <= 1; dx += 2)
            {
                for (int dy = -1; dy <= 1; dy += 2)
                {
                    Vector2Int neighbor = new Vector2Int(cell.x + dx, cell.y + dy);
                    while (IsCellValid(neighbor))
                    {
                        neighbors.Add(neighbor);
                        neighbor.x += dx;
                        neighbor.y += dy;
                    }
                }
            }
        }
        private void AddQueenMoves(Vector2Int cell, ref List<Vector2Int> neighbors)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    { 
                        continue;
                    }

                    Vector2Int neighbor = new Vector2Int(cell.x + dx, cell.y + dy);

                    while (IsCellValid(neighbor))
                    {
                        neighbors.Add(neighbor);
                        neighbor.x += dx;
                        neighbor.y += dy;
                    }
                }
            }
        }
        private void AddKingMoves(Vector2Int cell, ref List<Vector2Int> neighbors)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx != 0 || dy != 0)
                    {
                        Vector2Int neighbor = new Vector2Int(cell.x + dx, cell.y + dy);
                        neighbors.Add(neighbor);
                    }
                }
            }
        }
        private void AddKnightMoves(Vector2Int cell, ref List<Vector2Int> neighbors)
        {
            Vector2Int[] possibleMoves = {
                new Vector2Int(-2, -1), new Vector2Int(-1, -2) ,
                new Vector2Int(2, -1), new Vector2Int(1, -2) ,
                new Vector2Int(-2, 1), new Vector2Int(-1, 2) ,
                new Vector2Int(2, 1), new Vector2Int(1, 2) ,
            };
            foreach(Vector2Int move in possibleMoves)
            {
                Vector2Int neighbor = new Vector2Int(cell.x + move.x, cell.y + move.y);
                neighbors.Add(neighbor);
            }
        }
        private List<Vector2Int> GetValidNeighbors(Vector2Int cell, ChessUnitType unit)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();


            switch (unit)
            {
                case ChessUnitType.Pon:
                    AddPawnMoves(cell, ref neighbors);
                    break;
                case ChessUnitType.King:
                    AddKingMoves(cell, ref neighbors);
                    break;
                case ChessUnitType.Queen:
                    AddQueenMoves(cell, ref neighbors);
                    break;
                case ChessUnitType.Rook:
                    AddRookMoves(cell, ref neighbors);
                    break;
                case ChessUnitType.Knight:
                    AddKnightMoves(cell, ref neighbors);
                    break;
                case ChessUnitType.Bishop:
                    AddBishopMoves(cell, ref neighbors);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }

            
            neighbors.RemoveAll(neighbor => !IsCellValid(neighbor));

            return neighbors;
        }
        private bool IsCellValid(Vector2Int cell)
        {
         
            if (cell.x < 0 || cell.x > gridSize.x || cell.y < 0 || cell.y > gridSize.y)
            {
                return false;
            }
            if (grid.Get(cell)!= null)
            {
                return false;
            }
            return true;
        }
    }
}