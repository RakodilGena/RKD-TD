using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Collisions;

public readonly struct Circle
{
    /// <summary>
    /// The x-coordinate of the center of this circle.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// The y-coordinate of the center of this circle.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// The length, in pixels, from the center of this circle to the edge.
    /// </summary>
    public int Radius { get; }

    /// <summary>
    /// Gets the y-coordinate of the highest point on this circle.
    /// </summary>
    public int Top => Y - Radius;

    /// <summary>
    /// Gets the y-coordinate of the lowest point on this circle.
    /// </summary>
    public int Bottom => Y + Radius;

    /// <summary>
    /// Gets the x-coordinate of the leftmost point on this circle.
    /// </summary>
    public int Left => X - Radius;

    /// <summary>
    /// Gets the x-coordinate of the rightmost point on this circle.
    /// </summary>
    public int Right => X + Radius;


    public Vector2 Location => new(X, Y);


    /// <summary>
    /// Creates a new circle with the specified position and radius.
    /// </summary>
    /// <param name="x">The x-coordinate of the center of the circle.</param>
    /// <param name="y">The y-coordinate of the center of the circle..</param>
    /// <param name="radius">The length from the center of the circle to an edge.</param>
    public Circle(int x, int y, int radius)
    {
        X = x;
        Y = y;
        Radius = radius;
    }

    public Circle(Vector2 location, int radius) : this((int)location.X, (int)location.Y, radius)
    {
    }

    /// <summary>
    /// Returns a value that indicates whether the specified circle intersects with this circle.
    /// </summary>
    /// <param name="other">The other circle to check.</param>
    /// <returns>true if the other circle intersects with this circle; otherwise, false.</returns>
    public bool Intersects(Circle other)
    {
        var radii = Radius + other.Radius;
        int radiiSquared = radii * radii;
        float distanceSquared = Vector2.DistanceSquared(Location, other.Location);
        return distanceSquared < radiiSquared;
    }
}