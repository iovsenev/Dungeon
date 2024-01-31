using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class BfsTask
{
    static HashSet<Point> visits;
    static Queue<SinglyLinkedList<Point>> queue;
    static HashSet<Point> chest;
    static SinglyLinkedList<Point> previouslyPoint;
    static SinglyLinkedList<Point> nextPoint;

    public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
    {
        chest = chests.ToHashSet();
        queue = new Queue<SinglyLinkedList<Point>>();
        visits = new HashSet<Point>();
        nextPoint = new SinglyLinkedList<Point>(start);
        queue.Enqueue(nextPoint);
        visits.Add(start);
        while (queue.Count != 0)
        {
            previouslyPoint = queue.Dequeue();
            var walker = new Walker(previouslyPoint.Value);
            for (int i = 0; i < 4; i++)
            {
                var newWalker = walker.WalkInDirection(map, (MoveDirection)i);
                if(CheckNewPoint(newWalker))
                    continue;
                var newPoint = newWalker.Position;
                nextPoint = new SinglyLinkedList<Point>(newPoint, previouslyPoint);
                if (chest.Contains(newPoint))
                    yield return nextPoint;
                queue.Enqueue(nextPoint);
                visits.Add(newPoint);
            }
        }
    }

    static bool CheckNewPoint(Walker walker)
    {
        if (!(walker.PointOfCollision.Equals(Point.Null)))
            return true;
        return visits.Contains(walker.Position);
    }
}