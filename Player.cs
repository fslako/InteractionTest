using System;
using System.Linq;
using Godot;

public partial class Player : CharacterBody3D
{
    [Export] public float MoveSpeed = 5.0f;
    [Export] Area3D Reach;
    [Export] Camera3D Camera;
    [Export] Marker3D CameraMarker;

    private Vector3 targetPosition;
    private bool hasTarget = false;
    public static bool interactable = false;

    public override void _Ready()
    {
        targetPosition = GlobalPosition;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (interactable && Area3d.interactableObjects.Contains(OutlineSystem._hoveredObject))
        {
            if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
            {
                // Camera.Position = OutlineSystem._hoveredObject.Position + new Vector3(0,1,2);
                // Camera.RotationDegrees = new Vector3(-30,0,0);
                var tween = CreateTween();
                tween.TweenProperty(Camera, "position", OutlineSystem._hoveredObject.Position + new Vector3(0,1,2), 0.5);
                tween.SetParallel();
                tween.TweenProperty(Camera, "rotation_degrees", new Vector3(-30,0,0), 0.5);
                Visible = false;
            }
        }
        else
        {
            

            if (@event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left &&
            mouseButton.Pressed)
            {
                SetMovementTarget(mouseButton.Position);
                // Camera.Position = CameraMarker.Position;
                // Camera.RotationDegrees = CameraMarker.RotationDegrees;
                var tween = CreateTween();
                tween.TweenProperty(Camera, "position", CameraMarker.Position, 0.5);
                tween.SetParallel();
                tween.TweenProperty(Camera, "rotation_degrees", CameraMarker.RotationDegrees, 0.5);
                Visible = true;
            }
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