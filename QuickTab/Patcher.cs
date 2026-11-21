using System;
using HarmonyLib;

namespace QuickTab
{
    internal static class Patcher
    {
        private static Harmony _harmony;

        public static void Patch(string id)
        {
            _harmony = new Harmony(id);
            try
            {
                _harmony.PatchAll(typeof(Patcher).Assembly);
                ModEntry.Log?.Info("QuickTab: added Waybill, Passengers and MOW to the cars' right-click menu.");
            }
            catch (Exception ex)
            {
                // A game update renamed something: leave the game untouched.
                _harmony.UnpatchAll(id);
                _harmony = null;
                ModEntry.Log?.Error("QuickTab: patching failed, no Quick Tab menu items: " + ex.Message);
            }
        }

        public static void Unpatch()
        {
            _harmony?.UnpatchAll(_harmony.Id);
            _harmony = null;
        }
    }
}
