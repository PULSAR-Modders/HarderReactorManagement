using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using static PulsarModLoader.Patches.HarmonyHelpers;

namespace Harder_Reactor_Management
{
    internal class ShipInfoBase
    {
        [HarmonyPatch(typeof(PLShipInfoBase), "IsReactorTempCritical")]
        class ReactorCritical
        {
            //if the power usage >= max power allowed, cause the reactor to be critical (stability to decrease)
            static void Postfix(PLShipInfoBase __instance, ref bool __result)
            {
                //Not critical if reactor is "off"
                if (Mod.Instance.IsEnabled() && __instance.GetIsPlayerShip() && __instance.MyReactor != null && __instance.ReactorTotalPowerLimitPercent != 0)
                {
                    float reactorMaxPowerAllowed = __instance.MyStats.ReactorBoostedOutputMax * __instance.ReactorTotalPowerLimitPercent;
                    __result |= __instance.MyStats.ReactorTotalOutput >= reactorMaxPowerAllowed - 0.01f;
                }
            }
        }

        [HarmonyPatch(typeof(PLShipInfoBase), "Update")]
        class Update
        {
            //when the reactor is critical, multiply the stability change rate by StabilitySpeedMultiplier()
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //after if (this.IsReactorTempCritical())
                //PLShipInfoBase line 2420:  this.MyReactor.HeatOutput;
                List<CodeInstruction> targetSequence = new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_0), //this
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfoBase), nameof(PLShipInfoBase.MyReactor))), //MyReactor
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLReactor), nameof(PLReactor.HeatOutput))), //HeatOutput
                    new CodeInstruction(OpCodes.Mul) //*
                };

                // * StabilitySpeed(instance)
                List<CodeInstruction> patchSequence = new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_0), //instance
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShipInfoBase), nameof(ShipInfoBase.StabilitySpeedMultiplier))), //StabilitySpeed()
                    new CodeInstruction(OpCodes.Mul) //*
                };

                instructions = PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.AFTER, CheckMode.NONNULL);

                //PLShipInfoBase lines 2421-2423:  if (this.Reactor_OCActive)
                //this.CoreInstability += Time.deltaTime * 0.025f;
                targetSequence = new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_0), //this
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfoBase), "get_Reactor_OCActive")), //get_Reactor_OCActive()
                    new CodeInstruction(OpCodes.Brfalse_S), //if
                    new CodeInstruction(OpCodes.Ldarg_0), //this
                    new CodeInstruction(OpCodes.Ldarg_0), //this
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfoBase), nameof(PLShipInfoBase.CoreInstability))), //CoreInstability
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Time), "get_deltaTime")), //Time.get_deltaTime()
                    new CodeInstruction(OpCodes.Ldc_R4), //0.025
                    new CodeInstruction(OpCodes.Mul) //*
                };

                return PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.AFTER, CheckMode.NONNULL);
            }
        }

        //
        public static float StabilitySpeedMultiplier(PLShipInfoBase instance)
        {
            if (Mod.Instance.IsEnabled() && instance.GetIsPlayerShip())
            {
                //if reactor temperature is critical slider does not help
                if (instance.MyStats.ReactorTempCurrent < instance.MyStats.ReactorTempMax - 0.01f)
                {
                    //ranges from 0.2 to 1
                    return (instance.ReactorTotalPowerLimitPercent * 0.8f) + 0.2f;
                }
            }
            //else return multiplicative identity
            return 1;
        }
    }
}
