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
            GD.PrintErr("OutlineShader has not been assigned.");
            return;
        }

        _outlineMaterial = new ShaderMaterial();
        _outlineMaterial.Shader = OutlineShader;
    }

    public void SetHovered(bool hovered)
    {
        if (_mesh == null)
            return;

        if (hovered)
        {
            _mesh.MaterialOverlay = _outlineMaterial;
        }
        else
        {
            _mesh.MaterialOverlay = null;
        }
    }
}