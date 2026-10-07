using Godot;

public partial class HoverableObject : RigidBody3D
{
    [Export] public Shader OutlineShader;

    private MeshInstance3D _mesh;
    private ShaderMaterial _outlineMaterial;

    public override void _Ready()
    {
        _mesh = GetNode<MeshInstance3D>("MeshInstance3D");

        if (OutlineShader == null)
        {
            GD.PrintErr($"No OutlineShader assigned to {Name}.");
            return;
        }

        _outlineMaterial = new ShaderMaterial
        {
            Shader = OutlineShader
        };
    }

    public void SetHovered(bool hovered)
    {
        if (_mesh == null)
            return;

        _mesh.MaterialOverlay =
            hovered ? _outlineMaterial : null;
    }

    public virtual void Interact(Player player)
    {
        GD.Print($"Interacted with: {Name}");
    }
}