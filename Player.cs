using Godot;

public partial class Player : CharacterBody3D
{
    [Export] public float MoveSpeed = 5.0f;
    [Export] Area3D Reach;

    private Vector3 targetPosition;
    private bool hasTarget = false;
    public static bool interactable = false;

    public override void _Ready()
    {
        targetPosition = GlobalPosition;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
        {
            if (interactable)
            {

            }
            SetMovementTarget(mouseButton.Position);
        }
    }

    private void SetMovementTarget(Vector2 mousePosition)
    {
        Camera3D camera = GetViewport().GetCamera3D();

        if (camera == null)
            return;

        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);

        Vector3 rayEnd = rayOrigin + rayDirection * 1000.0f;

        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);

        query.CollideWithBodies = true;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            targetPosition = (Vector3)result["position"];
            hasTarget = true;
        }
    }

    private void Interact(HoverableObject interactableObject)
    {
        if (interactableObject == null)
        {
            return;
        }



    }

    public override void _PhysicsProcess(double delta)
    {
        if (!hasTarget)
        {
            Velocity = Vector3.Zero;
            return;
        }

        Vector3 direction = GlobalPosition.DirectionTo(targetPosition);
        direction.Y = 0;

        if (direction.LengthSquared() < 0.05f)
        {
            Velocity = Vector3.Zero;
            hasTarget = false;
            return;
        }

        direction = direction.Normalized();

        Velocity = direction * MoveSpeed;

        MoveAndSlide();

        if (direction.LengthSquared() > 0.01f)
        {
            LookAt(GlobalPosition - direction, Vector3.Up);
        }
    }
}