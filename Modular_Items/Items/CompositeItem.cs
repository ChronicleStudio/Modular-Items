using Modular_Items.Utils;
using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.Client.NoObf;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace Modular_Items.Items
{
	public class CompositeItem : Item, ITexPositionSource, IContainedMeshSource
	{
		Dictionary<string, ITexPositionSource> textureSources = new Dictionary<string, ITexPositionSource>();
		ITexPositionSource tempTextureSource;
		ICoreClientAPI clientAPI;
		
		
		public TextureAtlasPosition this[string textureCode]
		{
			get
			{
				if (textureSources.TryGetValue(textureCode, out tempTextureSource))
				{
					return textureSources[textureCode][textureCode];
				}
				return null;
			}
		}

		public Size2i AtlasSize { get; set; }

		public override void OnLoaded(ICoreAPI api)
		{
			if(api.Side == EnumAppSide.Client)
			{
				clientAPI = api as ICoreClientAPI;
			}
			base.OnLoaded(api);
		}

		/// <summary>
		/// Dispose of the resources
		/// </summary>
		/// <param name="api"></param>
		public override void OnUnloaded(ICoreAPI api)
		{
			base.OnUnloaded(api);
			ICoreClientAPI capi = api as ICoreClientAPI;
			if (capi == null) return;

			object obj;

			if (capi.ObjectCache.TryGetValue("itemMeshRefs", out obj))
			{
				Dictionary<string, MultiTextureMeshRef> meshrefs = obj as Dictionary<string, MultiTextureMeshRef>;

				foreach (var val in meshrefs)
				{
					val.Value.Dispose();
				}

				capi.ObjectCache.Remove("itemMeshRefs");
			}

			if (capi.ObjectCache.TryGetValue("itemShapeRefs", out obj))
			{
				capi.ObjectCache.Remove("itemShapeRefs");
			}
		}

		/// <summary>
		/// Called before rendering to generate the items shape and textures from its components
		/// </summary>
		/// <param name="capi"></param>
		/// <param name="itemstack"></param>
		/// <param name="target"></param>
		/// <param name="renderinfo"></param>
		public override void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
		{		
			List<ItemComponent> components = GetComponents(capi, itemstack);
			Shape shapeRef = new Shape();						

			string shapeKey = GetShapeKey(components);			

			shapeRef = GetShapeRef(capi, shapeKey, itemstack, components);

			renderinfo.ModelRef = GetMeshRef(capi, shapeKey, itemstack, shapeRef, components, renderinfo);						

			Size2i textureSize = new Size2i(16, 16);

			renderinfo.NormalShaded = true;
			renderinfo.TextureSize = textureSize;
			renderinfo.CullFaces = true;
			
		}

		/// <summary>
		/// GenMesh for BlockEntity rendering
		/// </summary>
		/// <param name="itemstack"></param>
		/// <param name="targetAtlas"></param>
		/// <param name="atBlockPos"></param>
		/// <returns></returns>
		public MeshData GenMesh(ItemStack itemstack, ITextureAtlasAPI targetAtlas, BlockPos atBlockPos)
		{
			var cnts = new ContainedTextureSource(api as ICoreClientAPI, targetAtlas, new Dictionary<string, AssetLocation>(), string.Format("For render in CompositeItems {0}", Code));
			cnts.Textures.Clear();

			var components = GetComponents(clientAPI, itemstack);
			
			MeshData mesh;
						
			var shapeKey = GetShapeKey(components);
			var shapeRef = GetShapeRef(clientAPI, shapeKey, itemstack, components);						

			foreach (var comp in components)
			{
				var shape = clientAPI.TesselatorManager.GetCachedShape(comp.Shape.Base).Clone();

				if (comp.construction == "metal") {
					cnts.Textures[comp.FirstCodePart() + "material"] = new AssetLocation("game:block/metal/ingot/" + comp.LastCodePart());
				}
				if (comp.construction == "wood")
				{
					cnts.Textures[comp.FirstCodePart() + "material"] = new AssetLocation("game:block/wood/debarked/" + comp.LastCodePart());
				}
				
			}

			mesh = GenMesh(clientAPI, itemstack, shapeRef, components);

			clientAPI.Tesselator.TesselateShape(shapeKey, shapeRef, out mesh, cnts);						

			return mesh.Clone();
		}

		/// <summary>
		/// GenMesh for the CompositeItem
		/// </summary>
		/// <param name="capi"></param>
		/// <param name="itemstack"></param>
		/// <param name="shaperef"></param>
		/// <param name="components"></param>
		/// <param name="tesselator"></param>
		/// <returns></returns>
		public MeshData GenMesh(ICoreClientAPI capi, ItemStack itemstack, Shape shaperef, List<ItemComponent> components, ITesselatorAPI tesselator = null)
		{
			if (tesselator == null) tesselator = capi.Tesselator;

			if (shaperef == null) return null;

			var shapeKey = GetShapeKey(components);

			this.AtlasSize = capi.ItemTextureAtlas.Size;

			textureSources.Clear();

			int counter = 0;

			foreach (var elem in components)
			{
				Item tempItem = elem;
				textureSources.Add(tempItem.Code.FirstCodePart() + "material", tesselator.GetTextureSource(tempItem));
				counter++;
			}

			MeshData mesh;
						
			tesselator.TesselateShape(shapeKey, shaperef, out mesh, this);			

			return mesh.Clone();
		}

		/// <summary>
		/// When this item is created, it takes into itself each of the ItemComponents
		/// </summary>
		/// <param name="allInputslots"></param>
		/// <param name="outputSlot"></param>
		/// <param name="byRecipe"></param>
		public override void OnCreatedByCrafting(ItemSlot[] allInputslots, ItemSlot outputSlot, GridRecipe byRecipe)
		{
			base.OnCreatedByCrafting(allInputslots, outputSlot, byRecipe);

			ITreeAttribute componentTree = outputSlot.Itemstack.Attributes.GetOrAddTreeAttribute("components");

			foreach (ItemSlot slot in allInputslots)
			{
				if (!slot.Empty && slot.Itemstack.Item is ItemComponent)
				{
					componentTree.SetItemstack(slot.Itemstack.Item.Id.ToString(), slot.Itemstack.Clone());
				}
			}
		}

		/// <summary>
		/// For IContainedMeshSource
		/// </summary>
		/// <param name="itemstack"></param>
		/// <returns></returns>		
		public string GetMeshCacheKey(ItemStack itemstack)
		{
			var components = GetComponents(clientAPI, itemstack);
			return GetShapeKey(components);
		}

		/// <summary>
		/// Get shapeRef from the cache or create one
		/// </summary>
		/// <param name="capi"></param>
		/// <param name="shapeKey"></param>
		/// <param name="itemstack"></param>
		/// <param name="components"></param>
		/// <returns></returns>
		private Shape GetShapeRef(ICoreClientAPI capi, string shapeKey, ItemStack itemstack, List<ItemComponent> components)
		{
			Shape shapeRef = new Shape();

			Dictionary<string, Shape> shapeRefs = ObjectCacheUtil.GetOrCreate(capi, "itemShapeRefs", () =>
			{
				return new Dictionary<string, Shape>();
			});

			if (!shapeRefs.TryGetValue(shapeKey, out shapeRef))
			{
				List<Shape> shapes = new List<Shape>();

				foreach (var component in components)
				{
					Item tempItem = component;
					AssetLocation tempItemLoc = tempItem.Shape.Base.Clone().WithPathPrefix("shapes/").WithPathAppendix(".json");
					Shape tempShape = capi.Assets.TryGet(tempItemLoc).ToObject<Shape>();

					shapes.Add(tempShape.Clone());
				}

				shapeRefs[shapeKey] = shapeRef = RenderUtil.AttachShapes(shapes);

				itemstack.Attributes.SetString("shapeKey", shapeKey);
			}

			return shapeRef.Clone();
		}

		/// <summary>
		/// Get MultiTextureMeshRef from the cache or create one
		/// </summary>
		/// <param name="capi"></param>
		/// <param name="shapeKey"></param>
		/// <param name="itemstack"></param>
		/// <param name="shapeRef"></param>
		/// <param name="components"></param>
		/// <param name="renderinfo"></param>
		/// <returns></returns>
		private MultiTextureMeshRef GetMeshRef(ICoreClientAPI capi, string shapeKey, ItemStack itemstack, Shape shapeRef, List<ItemComponent> components, ItemRenderInfo renderinfo)
		{
			MeshData mesh;

			Dictionary<string, MultiTextureMeshRef> meshRefs = ObjectCacheUtil.GetOrCreate(capi, "itemMeshRefs", () =>
			{
				return new Dictionary<string, MultiTextureMeshRef>();
			});

			if (!meshRefs.TryGetValue(shapeKey, out renderinfo.ModelRef))
			{
				mesh = GenMesh(capi, itemstack, shapeRef, components);
				renderinfo.ModelRef = meshRefs[shapeKey] = mesh == null ? renderinfo.ModelRef : capi.Render.UploadMultiTextureMesh(mesh);
			}

			return renderinfo.ModelRef;
		}

		/// <summary>
		/// Get Components from the itemstack attributes
		/// </summary>
		/// <param name="capi"></param>
		/// <param name="itemstack"></param>
		/// <returns></returns>
		private List<ItemComponent> GetComponents(ICoreClientAPI capi, ItemStack itemstack)
		{
			var componentsTree = itemstack.Attributes.GetOrAddTreeAttribute("components");

			List<ItemComponent> components = new List<ItemComponent>();

			foreach (var component in componentsTree)
			{
				var stack = componentsTree.GetItemstack(component.Key);
				stack.ResolveBlockOrItem(capi.World);
				components.Add(stack.Item as ItemComponent);
			}

			return components;
		}

		/// <summary>
		/// Create a shapeKey based on the IDs of the components used to make the item
		/// </summary>
		/// <param name="components"></param>
		/// <returns></returns>
		private string GetShapeKey(List<ItemComponent> components)
		{
			string shapeKey = "CompositeItemModelRef-";

			foreach (var component in components)
			{
				shapeKey += component.Id.ToString() + "-";
			}

			return shapeKey;
		}
	}
	
}
