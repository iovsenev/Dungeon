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
        if (pathToExit == null )
            return new MoveDirection[0];

        IEnumerable<SinglyLinkedList<Point>> pathOnStart = BfsTask.FindPaths(_map, _start, _chests);
            
        if (pathOnStart.Count() == 0 || _chests.Any(chest => pathToExit.Contains(chest)))
        {
            return GetMoveDirections(pathToExit);
        }

        var pathOnChestToExit = BfsTask.FindPaths(_map, _exit, _chests);


        //var path = GetMinPath(sumPaths);

        var paths = from exitToChest in pathOnStart
                    join startToChest in pathOnChestToExit
                    on exitToChest.Value equals startToChest.Value
                    select new { exitToChest = exitToChest, startToChest = startToChest, Length = exitToChest.Length + startToChest.Length };

        var path = pathOnStart
            .Select(g => (  g,  pathOnChestToExit.Where(p => p.Value == g.Value).First() ));

        var path2 = pathOnStart
            .Join(pathOnChestToExit,
                p => p.Value,
                g => g.Value,
                (p, g) => new { p, g, l = p.Length + g.Length });

        //var p1 = path.Where(p => p.Count() > 0)
        //    .OrderBy(p => p.Count())
        //    .First();

        return new MoveDirection[0];
        //return GetMoveDirections(path);
    }

    static IEnumerable<Point> GetMinPath(IEnumerable<(SinglyLinkedList<Point> a, SinglyLinkedList<Point> b, int length)> paths)
    {
        var minLenth = int.MaxValue;
        var minPath = paths.First();
        foreach (var path in paths)
        {
            if ((path.Item1.Length + path.Item2.Length) < minLenth)
                minPath = path;
        }

        return  minPath.Item1.Concat(minPath.Item2.Skip(1));
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