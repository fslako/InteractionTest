using Godot;

public partial class OutlineSystem : Node3D
{
    [Export] public Camera3D Camera;

    private HoverableObject _hoveredObject;

    public HoverableObject HoveredObject => _hoveredObject;

    public override void _Process(double delta)
    {
        if (Camera == null)
            return;

        HoverableObject newObject = GetObjectUnderMouse();

        if (newObject == _hoveredObject)
            return;

        // Remove outline from old object
        if (_hoveredObject != null)
        {
            _hoveredObject.SetHovered(false);
        }

        // Add outline to new object
        if (newObject != null)
        {
            newObject.SetHovered(true);
        }

        _hoveredObject = newObject;
    }

    private HoverableObject GetObjectUnderMouse()
    {
        Vector2 mousePosition = GetViewport().GetMousePosition();

        Vector3 rayOrigin =
            Camera.ProjectRayOrigin(mousePosition);

        Vector3 rayDirection =
            Camera.ProjectRayNormal(mousePosition);

        Vector3 rayEnd =
            rayOrigin + rayDirection * 1000.0f;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(
                rayOrigin,
                rayEnd
            );

        query.CollideWithBodies = true;

        var result =
            GetWorld3D().DirectSpaceState.IntersectRay(query);

        if (result.Count == 0)
            return null;

        Node collider =
            result["collider"].AsGodotObject() as Node;

        return FindHoverableParent(collider);
    }

    private HoverableObject FindHoverableParent(Node node)
    {
        while (node != null)
        {
            if (node is HoverableObject hoverable)
                return hoverable;

            node = node.GetParent();
        }

        return null;
    }
}