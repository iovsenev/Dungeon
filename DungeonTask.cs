using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class DungeonTask
{
    static Map _map;
    static Point _start;
    static Point _exit;
    static Point[] _chests;
    public static MoveDirection[] FindShortestPath(Map map)
    {
        _map = map;
        _start = map.InitialPosition;
        _exit = map.Exit;
        _chests = map.Chests;

        var pathToExit = GetPathToExit();
        if (pathToExit == null)
            return new MoveDirection[0];

        var pathOnStart = BfsTask.FindPaths(_map, _start, _chests).ToList();

        if (pathOnStart.Count() == 0 || _chests.Any(chest => pathToExit.Contains(chest)))
        {
            return GetMoveDirections(pathToExit);
        }

        var path = GetResultPath(pathOnStart);

        return GetMoveDirections(path);
    }

    private static IEnumerable<Point> GetResultPath(List<SinglyLinkedList<Point>> pathOnStart)
    {
        return pathOnStart
                    .Select(p => BfsTask.FindPaths(_map, p.Value, new[] { _exit }).First())
                    .Join(pathOnStart,
                        p => p.Last(),
                        g => g.Value,
                        (p, g) => p.Concat(g.Skip(1)))
                    .OrderBy(p => p.Count())
                    .First()
                    .Reverse()
                    .ToList();
    }

    private static MoveDirection[] GetMoveDirections(IEnumerable<Point> path)
    {
        return path
                    .Zip(
                        path.Skip(1),
                        (first, second) =>
                        Walker.ConvertOffsetToDirection(second - first))
                    .ToArray();
    }

    private static List<Point> GetPathToExit()
    {
        var noChestsPath = BfsTask.FindPaths(_map, _exit, new[] { _start });
        if (noChestsPath.Count() == 0)
            return null;
        return noChestsPath.First().ToList();
    }
}