using Modular_Items.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Modular_Items.Utils
{
	public class ToolModeManager
	{
		//private Dictionary<uint, SkillItem> toolModesByID;
		//private Dictionary<SkillItem, uint> IDByToolMode;

		//uint toolModeIDCounter = 0;


		//private void CreateToolModes(List<ItemComponent> components, ICoreAPI api)
		//{
		//	Dictionary<string, SkillItem> toolModesDict = new Dictionary<string, SkillItem>();
		//	toolModesByID = new Dictionary<uint, SkillItem>();
		//	SkillItem[] toolModes;
		//	ICoreClientAPI capi = api as ICoreClientAPI;

			

		//	foreach (ItemComponent component in components)
		//	{
		//		if (true)
		//		{
		//			foreach (var mode in component.toolProps.toolModeDesc)
		//			{
		//				SkillItem toolMode = new SkillItem() { Code = new AssetLocation(mode.Key), Name = Lang.Get(mode.Value) };

		//				AddToolMode(component, toolMode);
		//			}
		//		}

		//	}

		//	toolModes = ObjectCacheUtil.GetOrCreate(api, "ComponentToolModes", () =>
		//	{
		//		SkillItem[] modes = new SkillItem[toolModesByID.Count];
		//		int counter = 0;
		//		foreach (var mode in toolModesDict)
		//		{
		//			modes[counter] = mode.Value;
		//		}

		//		counter = 0;
		//		if (capi != null)
		//		{
		//			foreach (var mode in modes)
		//			{
		//				modes[counter].WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("game:textures/icons/heatmap.svg"), 48, 48, 5, ColorUtil.WhiteArgb));
		//				modes[counter].TexturePremultipliedAlpha = false;
		//			}
		//		}

		//		return modes;

		//	});

		//}

		//private void AddToolMode(ItemComponent component, SkillItem mode)
		//{
		//	if (IDByToolMode.ContainsKey(mode))
		//	{
		//		component.toolProps.toolModesByID.Add(IDByToolMode.Get(mode), mode);
		//	} else
		//	{
		//		toolModesByID.Add(toolModeIDCounter, mode);
		//		IDByToolMode.Add(mode, toolModeIDCounter);
		//		component.toolProps.toolModesByID.Add(toolModeIDCounter, mode);
		//		toolModeIDCounter++;
		//	}
		//}

	}
}
