using Godot;

public partial class InteractionArea : Area3D
{
    public override void _Ready()
    {
        Monitoring = true;
    }

    public bool CanInteractWith(HoverableObject interactable)
    {
        if (interactable == null)
            return false;

        var bodies = GetOverlappingBodies();

        GD.Print($"Bodies inside InteractionArea: {bodies.Count}");

        foreach (Node3D body in bodies)
        {
            GD.Print($"  - {body.Name}");

            if (body == interactable)
            {
                GD.Print($"FOUND INTERACTABLE: {interactable.Name}");
                return true;
            }
        }

        return false;
    }
}