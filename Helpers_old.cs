// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)
/*
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using System;
using System.Collections.Generic;
//using BubbleGauntlet.Config;
//using BubbleGauntlet.Extensions;
using Kingmaker.DialogSystem.Blueprints;
using System.Text.RegularExpressions;

namespace CustomNpcPortraits
{
    public class EntryComparer : IComparer<(int begin, int end)>
    {
        public int Compare((int begin, int end) x, (int begin, int end) y)
        {
            if (y.end >= x.end)
                return -1;
            if (y.begin < x.begin)
                return 1;
            return 0;
        }

        public static EntryComparer Instance = new EntryComparer();
    }



   
    public static class Helpers
    {
        public static T Create<T>(Action<T> init = null) where T : new()
        {
            var result = new T();
            init?.Invoke(result);
            return result;
        }
 
        private static readonly Regex stripper = new Regex(@"[^A-Za-z_]");



        public static T CreateBlueprint<T>([NotNull] string name, Action<T> init = null, Action<T> earlyInit = null) where T : SimpleBlueprint, new()
        {
            var result = new T
            {
                name = name,
                AssetGuid = new BlueprintGuid( Guid.NewGuid() )
        };
            earlyInit?.Invoke(result);
            Dialog.AddBlueprint(result);
            init?.Invoke(result);
            return result;
        }
        

        // All localized strings created in this mod, mapped to their localized key. Populated by CreateString.

        static readonly Dictionary<String, LocalizedString> textToLocalizedString = new Dictionary<string, LocalizedString>();

        public static LocalizedString CreateString(string key, string value)
        {
            // See if we used the text previously.
            // (It's common for many features to use the same localized text.
            // In that case, we reuse the old entry instead of making a new one.)
            if (textToLocalizedString.TryGetValue(value, out var localized))
            {
                return localized;
            }

#if DEBUG
            var current = LocalizationManager.CurrentPack.GetText(key, false);
            if (current != "" && current != value)
            {
                Main.DebugLog($"Info: duplicate localized string `{key}`, different text.");
            }
#endif
            LocalizationManager.CurrentPack.PutString(key, value);
            localized = new LocalizedString { m_ShouldProcess = false, m_Key = key };
            textToLocalizedString[value] = localized;
            return localized;
        }





 


        public static BlueprintDialog CreateDialog(string name, Action<BlueprintDialog> init)
        {
            return Helpers.CreateBlueprint<BlueprintDialog>(name, dialog => {
                dialog.Conditions = new ConditionsChecker();
                dialog.StartActions = new ActionList();
                dialog.FinishActions = new ActionList();
                dialog.ReplaceActions = new ActionList();

                dialog.FirstCue = new Kingmaker.DialogSystem.CueSelection();
                dialog.FirstCue.Cues = new List<BlueprintCueBaseReference>();

                init(dialog);
            });
        }
    }
    
public delegate ref S FastRef<T, S>(T source = default);

    public delegate void FastSetter<T, S>(T source, S value);

    public delegate S FastGetter<T, S>(T source);

    public delegate object FastInvoke(object target, params object[] paramters);
}
*/