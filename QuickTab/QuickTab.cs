using System;
using Game.Mow;
using HarmonyLib;
using Model;
using Model.Ops;
using RollingStock;
using UI;
using UI.Builder;
using UI.CarInspector;
using UI.ContextMenu;

namespace QuickTab
{
    // Which car window tab a car is usually opened for, and the menu label for it.
    // Locomotives, tenders and cabooses get none.
    internal static class Tabs
    {
        public static bool TryGet(Car car, out string label, out string tabId)
        {
            label = null;
            tabId = null;
            if (car == null)
                return false;
            CarType type = car.CarType;
            if (type.IsLocomotive || type.IsTender || type.IsCaboose)
                return false;
            if (car.IsPassengerCar())
            {
                label = "Passengers";
                tabId = "pass";
            }
            else if (WorkCar.IsWorkCar(car))
            {
                label = "MOW";
                tabId = "mow";
            }
            else if (type.IsFreight)
            {
                label = "Waybill";
                tabId = "ops";
            }
            return tabId != null;
        }

        // Open the car window on a tab. CarInspector.Show calls Configure, which picks up
        // Pending (see ConfigurePatch), then rebuilds the window.
        public static void Open(Car car, string tabId)
        {
            Pending = tabId;
            try { CarInspector.Show(car); }
            finally { Pending = null; }
        }

        internal static string Pending;
    }

    [HarmonyPatch(typeof(CarInspector), "Configure")]
    internal static class ConfigurePatch
    {
        private static readonly AccessTools.FieldRef<CarInspector, UIState<string>> SelectedTab =
            AccessTools.FieldRefAccess<CarInspector, UIState<string>>("_selectedTabState");

        private static void Postfix(CarInspector __instance)
        {
            if (Tabs.Pending != null)
                SelectedTab(__instance).Value = Tabs.Pending;
        }
    }

    // The right-click menu. The game builds it in CarPickable.HandleShowContextMenu and finishes
    // with ContextMenu.Show, so the car is noted on the way in and the item added just before
    // the menu is shown.
    [HarmonyPatch(typeof(CarPickable), "HandleShowContextMenu")]
    internal static class CarMenu
    {
        private static Car _car;

        private static void Prefix(Car car) => _car = car;

        private static void Finalizer() => _car = null;

        [HarmonyPatch(typeof(ContextMenu), nameof(ContextMenu.Show))]
        private static class BeforeShow
        {
            private static void Prefix(ContextMenu __instance)
            {
                Car car = _car;
                _car = null;
                try
                {
                    if (Tabs.TryGet(car, out string label, out string tabId))
                        __instance.AddButton(ContextMenuQuadrant.General, label, SpriteName.Inspect, () => Tabs.Open(car, tabId));
                }
                catch (Exception ex)
                {
                    ModEntry.Log?.Error("QuickTab: menu for " + car?.DisplayName + ": " + ex.Message);
                }
            }
        }
    }

    // Ctrl+click on a car opens its window; with Quick Tab it opens on the same tab as the menu item.
    [HarmonyPatch(typeof(CarPickable), nameof(CarPickable.Activate))]
    internal static class CtrlClickPatch
    {
        private static void Prefix(CarPickable __instance, PickableActivateEvent evt)
        {
            if (evt.Activation == PickableActivation.Primary && evt.IsControlDown && !evt.IsShiftDown &&
                Tabs.TryGet(__instance.car, out _, out string tabId))
                Tabs.Pending = tabId;
        }

        private static void Finalizer() => Tabs.Pending = null;
    }
}
