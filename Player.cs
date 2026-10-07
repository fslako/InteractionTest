using Godot;

public partial class Player : CharacterBody3D
{
    [Export] public float MoveSpeed = 5.0f;

    [Export]
    public Area3D Reach;

    [Export]
    public InteractionArea InteractionArea;

    [Export]
    public OutlineSystem OutlineSystem;

    private Vector3 targetPosition;
    private bool hasTarget = false;

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
            GD.Print("LEFT CLICK");

            HoverableObject clickedObject = GetObjectUnderMouse(mouseButton.Position);

            if (clickedObject != null)
            {
                GD.Print($"CLICKED OBJECT: {clickedObject.Name}");

                TryInteract(clickedObject);
                return;
            }

            GD.Print("No interactable found - moving player.");

            SetMovementTarget(mouseButton.Position);
        }
    }

    private void TryInteract(HoverableObject interactable)
    {
        GD.Print($"TRYING TO INTERACT WITH: {interactable.Name}");

        if (InteractionArea == null)
        {
            GD.PrintErr("InteractionArea is NULL!");
            return;
        }

        if (!InteractionArea.CanInteractWith(interactable))
        {
            GD.Print($"TOO FAR AWAY FROM: {interactable.Name}");
            return;
        }

        GD.Print($"WITHIN RANGE: {interactable.Name}");

        interactable.Interact(this);
    }

    private void SetMovementTarget(Vector2 mousePosition)
    {
        Camera3D camera = GetViewport().GetCamera3D();

        if (camera == null)
            return;

        Vector3 rayOrigin =
            camera.ProjectRayOrigin(mousePosition);

        Vector3 rayDirection =
            camera.ProjectRayNormal(mousePosition);

        Vector3 rayEnd =
            rayOrigin + rayDirection * 1000.0f;

        PhysicsDirectSpaceState3D spaceState =
            GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(
                rayOrigin,
                rayEnd
            );

        query.CollideWithBodies = true;

        // Don't click the player itself.
        query.Exclude = new Godot.Collections.Array<Rid>
        {
            GetRid()
        };

        var result =
            spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            targetPosition = (Vector3)result["position"];
            hasTarget = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!hasTarget)
        {
            Velocity = Vector3.Zero;
            return;
        }

        Vector3 direction =
            GlobalPosition.DirectionTo(targetPosition);

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
            LookAt(
                GlobalPosition - direction,
                Vector3.Up
            );
        }
    }
    private HoverableObject GetHoveredObject(Vector2 mousePosition)
    {
        Camera3D camera = GetViewport().GetCamera3D();

        if (camera == null)
            return null;

        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);
        Vector3 rayEnd = rayOrigin + rayDirection * 1000.0f;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);

        query.CollideWithBodies = true;

        // Don't hit the player itself.
        query.Exclude = new Godot.Collections.Array<Rid>
        {
            GetRid()
        };

        var result =
            GetWorld3D().DirectSpaceState.IntersectRay(query);

        if (result.Count == 0)
            return null;

        Node collider =
            result["collider"].AsGodotObject() as Node;

        while (collider != null)
        {
            if (collider is HoverableObject hoverable)
                return hoverable;

            collider = collider.GetParent();
        }

        return null;
    }
    private HoverableObject GetObjectUnderMouse(Vector2 mousePosition)
    {
        Camera3D camera = GetViewport().GetCamera3D();

        if (camera == null)
        {
            GD.PrintErr("No Camera3D found!");
            return null;
        }

        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);

        Vector3 rayEnd = rayOrigin + rayDirection * 1000.0f;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(
                rayOrigin,
                rayEnd
            );

        query.CollideWithBodies = true;

        query.Exclude = new Godot.Collections.Array<Rid>
        {
            GetRid()
        };

        var result =
            GetWorld3D().DirectSpaceState.IntersectRay(query);

        if (result.Count == 0)
        {
            GD.Print("Raycast hit nothing.");
            return null;
        }

        Node collider =
            result["collider"].AsGodotObject() as Node;

        GD.Print($"Raycast hit: {collider.Name}");

        while (collider != null)
        {
            if (collider is HoverableObject hoverable)
            {
                return hoverable;
            }

            collider = collider.GetParent();
        }

        GD.Print("Hit something, but it isn't a HoverableObject.");

        return null;
    }
}