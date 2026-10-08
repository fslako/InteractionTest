using Godot;

public partial class OutlineSystem : Node3D
{
    public Camera3D Camera;

    public static HoverableObject _hoveredObject;

    public override void _Ready()
    {
        Camera = GetViewport().GetCamera3D();
    }


    public override void _Process(double delta)
    {
        if (Camera == null)
        {
            return;
        }

        Vector2 mousePosition = GetViewport().GetMousePosition();

        Vector3 rayOrigin = Camera.ProjectRayOrigin(mousePosition);
        Vector3 rayDirection = Camera.ProjectRayNormal(mousePosition);

        Vector3 rayEnd = rayOrigin + rayDirection * 1000.0f;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);

        var result = GetWorld3D().DirectSpaceState.IntersectRay(query);

        HoverableObject newObject = null;

        if (result.Count > 0)
        {
            Node collider = result["collider"].AsGodotObject() as Node;

            if (collider is HoverableObject hoverable)
            {
                newObject = hoverable;
            }
        }

        if (newObject != _hoveredObject)
        {
            // Remove outline from previous object
            if (_hoveredObject != null)
            {
                Player.interactable = false;
                _hoveredObject.SetHovered(false);   
            }

            // Add outline to new object
            if (newObject != null)
            {
                Player.interactable = true;
                newObject.SetHovered(true);
            }

            _hoveredObject = newObject;
        }
    }
}