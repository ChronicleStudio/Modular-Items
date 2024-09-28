using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Modular_Items.Utils
{
	public static class RenderUtil
	{
		public static Shape AttachShapes(List<Shape> shapes)
		{
			Shape newShape = null;
			string startingComponent = null;

			foreach (Shape shape in shapes)
			{
				if (shape.GetElementByName("Haft") != null)
				{
					startingComponent = "Haft";
					newShape = shape.Clone();
					shapes.Remove(shape);
					break;
				}

			}

			if (newShape == null)
			{	

				foreach (Shape shape in shapes)
				{
					if (shape.GetElementByName("Handle") != null)
					{
						startingComponent = "Handle";
						newShape = shape.Clone();
						shapes.Remove(shape);
						break;
					}
				}
			}

			return AttachShape(newShape, shapes, startingComponent);
		}

		private static Shape AttachShape(Shape newShape, List<Shape> shapes, string startingComponent)
		{
			foreach (AttachmentPoint point in newShape.GetElementByName(startingComponent).AttachmentPoints)
			{
				foreach (Shape shape in shapes)
				{
					if (shape.GetElementByName(point.Code) != null)
					{
						Shape tempShape = new Shape()
						{
							Elements = new ShapeElement[newShape.Elements.Length + shape.Elements.Length],
							Animations = newShape.Animations,
							AnimationsByCrc32 = newShape.AnimationsByCrc32,
							AttachmentPointsByCode = newShape.AttachmentPointsByCode,							
							JointsById = newShape.JointsById,
							TextureWidth = newShape.TextureWidth,
							TextureHeight = newShape.TextureHeight,
							TextureSizes = newShape.TextureSizes,
							Textures = newShape.Textures

						};						

						for (int i = 0; i < tempShape.Elements.Length; i++)
						{
							if (i < newShape.Elements.Length)
							{
								tempShape.Elements[i] = newShape.Elements[i].Clone();
								
							}
							if (i >= newShape.Elements.Length)
							{
								if (shape.Elements[i - newShape.Elements.Length].AttachmentPoints != null)
								{
									foreach (AttachmentPoint point2 in shape.Elements[i - newShape.Elements.Length].AttachmentPoints)
									{
										//tempShape.AttachmentPointsByCode.Add(point.Code, point);
									}
								}
								tempShape.Elements[i] = shape.Elements[i - newShape.Elements.Length].Clone();
							}
						}

						newShape = tempShape.Clone();
						foreach (KeyValuePair<string, AssetLocation> kvp in shape.Textures)
						{
							if (!newShape.Textures.ContainsKey(kvp.Key))
							{
								newShape.Textures.Add(kvp.Key, kvp.Value);
							}
						}

						newShape.GetElementByName(point.Code).From[0] = newShape.GetElementByName(startingComponent).From[0] + point.PosX;
						newShape.GetElementByName(point.Code).From[1] = newShape.GetElementByName(startingComponent).From[1] + point.PosY;
						newShape.GetElementByName(point.Code).From[2] = newShape.GetElementByName(startingComponent).From[2] + point.PosZ;

						newShape.GetElementByName(point.Code).To[0] = newShape.GetElementByName(startingComponent).To[0] + point.PosX;
						newShape.GetElementByName(point.Code).To[1] = newShape.GetElementByName(startingComponent).To[1] + point.PosY;
						newShape.GetElementByName(point.Code).To[2] = newShape.GetElementByName(startingComponent).To[2] + point.PosZ;

						newShape.GetElementByName(point.Code).RotationX = newShape.GetElementByName(startingComponent).RotationX + point.RotationX;
						newShape.GetElementByName(point.Code).RotationY = newShape.GetElementByName(startingComponent).RotationY + point.RotationY;
						newShape.GetElementByName(point.Code).RotationZ = newShape.GetElementByName(startingComponent).RotationZ + point.RotationZ;




						newShape.AttachmentPointsByCode.Remove(point.Code);
						shapes.Remove(shape);

						if(newShape.GetElementByName(point.Code).AttachmentPoints != null && newShape.GetElementByName(point.Code).AttachmentPoints.Length != 0)
						{
							AttachShape(newShape, shapes, point.Code);							
						}

						break;

					}
				}
			}

			return newShape.Clone();
			
		}



		public static Shape ChooseStartingShape(List<Shape> shapes, out string startingComponent)
		{			
			Shape newShape = new Shape();
			startingComponent = null;

			foreach (Shape shape in shapes)
			{
				if (shape.GetElementByName("Handle") != null)
				{
					startingComponent = "Handle";
					newShape = shape.Clone();
					shapes.Remove(shape);
					break;
				}
			}

			if (newShape == null)
			{
				foreach (Shape shape in shapes)
				{
					if (shape.GetElementByName("Haft") != null)
					{
						startingComponent = "Haft";
						newShape = shape.Clone();
						shapes.Remove(shape);
						break;
					}
				}
			}

			return newShape;

		}

		

		private static Shape AttachShapeTo(Shape target, Shape shape, AttachmentPoint point)
		{
			Shape tempShape = target.Clone();
			tempShape.Elements = new ShapeElement[target.Elements.Length + shape.Elements.Length];

			//Shape tempShape = new Shape()
			//{
			//	Elements = new ShapeElement[target.Elements.Length + shape.Elements.Length],
			//	Animations = target.Animations,
			//	AnimationsByCrc32 = target.AnimationsByCrc32,
			//	AttachmentPointsByCode = target.AttachmentPointsByCode,
			//	JointsById = target.JointsById,
			//	TextureWidth = target.TextureWidth,
			//	TextureHeight = target.TextureHeight,
			//	TextureSizes = target.TextureSizes,
			//	Textures = target.Textures
			//};
			int counter = 0;

			foreach(var element in target.Elements)
			{
				tempShape.Elements[counter] = element.Clone();
				counter++;
			}

			foreach (var element in shape.Elements)
			{
				tempShape.Elements[counter] = element.Clone();
				counter++;
			}

			foreach (KeyValuePair<string, AssetLocation> kvp in shape.Textures)
			{
				if (!tempShape.Textures.ContainsKey(kvp.Key))
				{
					tempShape.Textures.Add(kvp.Key, kvp.Value);
				}
			}

			tempShape.GetElementByName(point.Code).From[0] += point.PosX;
			tempShape.GetElementByName(point.Code).From[1] += point.PosY;
			tempShape.GetElementByName(point.Code).From[2] += point.PosZ;
			
			tempShape.GetElementByName(point.Code).To[0] += point.PosX;
			tempShape.GetElementByName(point.Code).To[1] += point.PosY;
			tempShape.GetElementByName(point.Code).To[2] += point.PosZ;
				
			tempShape.GetElementByName(point.Code).RotationX += point.RotationX;
			tempShape.GetElementByName(point.Code).RotationY += point.RotationY;
			tempShape.GetElementByName(point.Code).RotationZ += point.RotationZ;


			return tempShape.Clone();
		}

		private static Shape CombineShapes(string name, Shape startingShape, List<Shape> shapes)
		{
			
			if(name == null || shapes == null || startingShape.Elements == null) return startingShape;
			if (startingShape.GetElementByName(name).AttachmentPoints == null || startingShape.GetElementByName(name).AttachmentPoints.Length == 0) return startingShape;

			Shape newShape = startingShape.Clone();			

			foreach(var point in newShape.GetElementByName(name).AttachmentPoints)
			{
				bool attached = false;
				Shape tempShape = new Shape();

				foreach(Shape shape in shapes)
				{
					if(shape.GetElementByName(point.Code) != null)
					{
						//shape = CombineShapes(point.Code, shape, shapes);
						newShape = AttachShapeTo(newShape, shape, point);
						tempShape = shape;
						break;
					}
				}

				if(attached)
				{
					shapes.Remove(tempShape);
				}			
				
			}

			foreach(var element in newShape.Elements)
			{
				if(element.Name == name) { continue; }
				foreach(var point2 in element.AttachmentPoints)
				{
					bool attached = false;
					Shape tempShape2 = new Shape();

					foreach (Shape shape2 in shapes)
					{
						if (shape2.GetElementByName(point2.Code) != null)
						{
							//shape = CombineShapes(point.Code, shape, shapes);
							newShape = AttachShapeTo(newShape, shape2, point2);
							tempShape2 = shape2;
							break;
						}
					}

					if (attached)
					{
						shapes.Remove(tempShape2);
					}
				}
			}


			return newShape.Clone();
		}

		//public static Shape AttachShapes(List<Shape> shapes)
		//{
		//	Shape newShape = ChooseStartingShape(shapes, out var name);


		//	CombineShapes(name, newShape, shapes);


		//	return newShape.Clone();
		//}


		/*
		 * CombineShapes - the new Shape, List of Shapes
		 * 
		 * Choose the starting Shape
		 * 
		 * AttachShape - Shape, List of Shapes		 *		
		 *		foreach attachmentpoint in Shape.ap's - 
		 *			apShape = Shapes.GetElementByName(ap.Name)
		 *			add apShape to the Element list of Shape
		 *			if(apShape.aps != null or apShape.aps.length != 0) AttachShape(apShape, List of Shapes)
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 */

		

	}

	
}
