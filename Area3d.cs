using Godot;
using System;
using System.Collections.Generic;

public partial class Area3d : Area3D
{
	public static List<Node> interactableObjects = new List<Node>();

	public void OnBodyEntered(Node InteractableObject)
    {
        if (InteractableObject.IsInGroup("Interactable"))
        {
            GD.Print("pallo");
			interactableObjects.Add(InteractableObject);
			
			// foreach (var item in interactableObjects)
			// {
			// 	GD.Print(InteractableObject);
			// }
			
        }
    }
	public void OnBodyExited(Node InteractableObject)
    {
        if (InteractableObject.IsInGroup("Interactable"))
        {
            GD.Print("ei pallo");
			interactableObjects.Remove(InteractableObject);
			
			// foreach (var item in interactableObjects)
			// {
			// 	GD.Print(InteractableObject);
			// }
        }
    }
}
