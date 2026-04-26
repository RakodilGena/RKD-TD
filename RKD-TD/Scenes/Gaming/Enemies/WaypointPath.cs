using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace RKD_TD.Scenes.Gaming.Enemies;

internal sealed class WaypointPath
{
    public Vector2[] Waypoints { get; private set; }

    public WaypointPath(Vector2[] waypoints)
    {
        Waypoints = waypoints;
    }

    public Vector2 Start => Waypoints[0];

    public static WaypointPath FromFile(
        XDocument mapDoc,
        Vector2 tileSize)
    {
        XElement root = mapDoc.Root!;


        var waypointElements = root.Element("Spawner")!.Element("Waypoints")!.Elements("Waypoint");

        List<Vector2> waypoints = [];

        foreach (var waypointElement in waypointElements)
        {
            var split = waypointElement.Value.Split(';')
                .Select(int.Parse).ToArray();

            var waypoint = new Vector2(
                x: split[0] * tileSize.X,
                y: split[1] * tileSize.Y);

            waypoints.Add(waypoint);
        }

        return new WaypointPath(waypoints.ToArray());
    }
}