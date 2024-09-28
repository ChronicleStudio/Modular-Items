using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Modular_Items.ComponentProperties
{
	public class ToolProperties
	{

		//public bool requiresToolmode = false;

		public string strikeSound = null;
		
		//public Dictionary<string, string> toolModeDesc = null;		
		//public Dictionary<uint, SkillItem> toolModesByID;

		public float attackPower = 0.0f;
		public float attackRange = 0.0f;
		public int maxDurability = 0;
		public int toolTier = 0;
		public float attackSpeed = 0.0f;
		public EnumDamageType damageType = EnumDamageType.BluntAttack;

		public Dictionary<EnumBlockMaterial, float> MiningSpeed;

		public string HeldTpHitAnimation = "";

		/// <summary>
		/// Particles that should spawn in regular intervals from this block or item when held in hands
		/// </summary>
		//public AdvancedParticleProperties[] ParticleProperties = null;

		/// <summary>
		/// The origin point from which particles are being spawned
		/// </summary>
		public Vec3f TopMiddlePos = new Vec3f(0.5f, 1, 0.5f);

		/// <summary>
		/// If set, this item will be classified as given tool
		/// </summary>
		public EnumTool? Tool;


		//public string[] attackBehaviorBehaviorNames = null;
		/// <summary>
		/// Modifiers that can alter the behavior of the item or block, mostly for held interaction
		/// </summary>
		//public AttackBehavior[] attackBehaviors = new AttackBehavior[0];
				

		public byte[] LightHsv = new byte[3];

		// Modifiers that are Multiplicative
		public float attackPowerMult = 0.0f;
		public float attackRangeMult = 0.0f;
		public int maxDurabilityMult = 0;
		public int toolTierMult = 0;
		public float miningSpeedMult = 0.0f;
		public float attackSpeedMult = 0.0f;
		
		public ToolProperties() {
			strikeSound = "game:sounds/player/strike1";
			MiningSpeed = new Dictionary<EnumBlockMaterial, float>();
			//HeldTpHitAnimation = "falx";
			
		}

	}
}
