//using CustomNpcPortraits.SimpleDialogs;
//using DialogTest.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.DialogSystem;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Localization;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Interaction;
using Kingmaker.View.Spawners;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CustomNpcPortraits.Loggers;
using static CustomNpcPortraits.GameStrings;
using Kingmaker.ResourceLinks;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using Kingmaker.Controllers;
using Harmony12;
using Kingmaker.View;
using Kingmaker.EntitySystem;
using Kingmaker.UI.Common;
using System.IO;

namespace CustomNpcPortraits
{

    public class SuccubusThatTalks : IAreaLoadingStagesHandler
    {
        public string Name = "PoolSuccubus";
        public string AssetGuid = "7FF65275-8CFF-41CD-99A8-50D554B1C554";
        public string UnitTemplateGuid = "0d88d5c310fac90449bfd0714bb9f810";
        public string ActionName = "ActionSuccubusThatTalks";
        public string ActionGuid = "6FEB5447-E0B6-4EFD-8546-3AC213E1E2FF";
        public string ActionsTemplate = "63a4a91f33825f34baf4a99b7989feff";
        public string SpawnerPath = "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (3)";
        public string SpawnerScene = "MidnightFane_Caves_Mechanics";




        public BlueprintUnit Blueprint { get; private set; }
        public ActionsHolder ActionsHolder { get; private set; }
        public BlueprintDialogReference Dialog { get; private set; }
        public UnitSpawner Spawner { get; private set; }
        public SpawnerInteractionActions SpawnerInteractionActions { get; private set; }
        public bool IsTargetScene => SceneManager.GetSceneByName(SpawnerScene).isLoaded;

        
        

        public UnitEntityData Unit { get; private set; }

        public void Create()
        {
            // using (new ProcessLog("Unit creation time"))
            //{
            //Log($"Creating {Name}...");

            //Log($"Fetching template blueprint {UnitTemplateGuid}...");

            CreateDialog_1();
            CreateDialog_2();
            CreateDialog_3();
            CreateDialog_4();


            EventBus.Subscribe(this);
            
            
            
            //  Log($"{Name} creation finished...");
            //}
        }



        //Monique, Anita, Lilith, Juliet, Julie, Jezebel, Georgina, Ilana, Sybil, Morrigan, Verosika, Chantinelle, Shiklah, Cydaea, Andariel, Grace, Scarlet, Carrera, Mercedes, Xana,
        // Vydija, Evelynn, Alruna

        public void CreateDialog_1()
        {
            Name = "PoolSuccubus_1";
            AssetGuid = "5c2f4bd5-2259-4a05-9080-1aba05bfb13e";

            UnitTemplateGuid = "98ee09f7378a93542ab8ec41223f2cb2";
            SpawnerPath = "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (3)";
            SpawnerScene = "MidnightFane_Caves_Mechanics";

            var template = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(UnitTemplateGuid);

            Blueprint = Helpers.CopyAndAdd(template, Name, AssetGuid);

            Blueprint.SetLocalisedName("succubus-galatea", "Galatea");

            var bref = Blueprint.ToReference<BlueprintUnitReference>();

            //{mf|Master|Mistress}
            BlueprintDialogReference dialog;



   
            if (ResourcesLibrary.TryGetBlueprint<BlueprintDialog>("3180a5aeae0c484a8bfe95acf459878c") != null)
            {
                dialog = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>("3180a5aeae0c484a8bfe95acf459878c").ToReference<BlueprintDialogReference>();
            }
            else
                dialog = SimpleDialogBuilder.CreateDialog(
                     name: "simpledialog.galatea.base",
                     guid: "3180a5ae-ae0c-484a-8bfe-95acf459878c",
                     firstCue: new List<BlueprintCueBaseReference>()
                     {
                     SimpleDialogBuilder.CreateCue(
                         name: "simpledialog.galatea.greet",
                         guid: "0a5b9ac7-217c-4db6-bfb7-766cb7fa8e37", // this guid is referenced below
                         speaker: bref,
                         text: "{n}Succubi live for thousands of years, and they keenly study human nature in order to master the ways of temptation. This one, like most of her kind, knows more about humans, than humans know about themselves.{/n} \"Do you want to know what we think about your kind? Ask anything, I won\'t hold anything back... It's not in my nature!\"",
                         answerList: new List<BlueprintAnswerBaseReference>()
                         {
                             SimpleDialogBuilder.CreateAnswerList(
                                 name: "simpledialog.galatea.greet.answerlist",
                                 guid: "19AF2283-3563-460D-BE36-D4E0D582B7FC",
                                 answers: new List<BlueprintAnswerBaseReference>()
                                 {
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.galatea.greet.answer.1",
                                         guid: "1CAA5F9D-B12B-4A3F-B708-BFD0036A1421",
                                         text: "\"Why do people cheat?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.galatea.loop.1",
                                                 guid: "C2F218D7-AB52-4313-A582-C39986CC4450",
                                                 speaker: bref,
                                                 text: "\"Mortals feel like they live additional lives whenever they see themselves through someone else's eyes, and sexual connection with someone is the most intimate, therefore the most potent way to feel like a new life. (Except for falling in deep love, but that happens only a couple of times during their lives and as a rule of thumb with beings way out of their league, so this path invariably remains unfulfilled. And in the rare cases when it is fullfilled, in time it will fade. There are ways to keep Love alive, but that's another question.) Anyhow, the smart ones feel having multiple sexual partners is the only way to actually live more than one life. You know, their lives being extremely short and all. Sex with a new conquer is like drinking the elixir of life. You don't need a new lover all the time though for this effect to work. It is enough to have two and use them as palate cleansers after one another, alternating between the two. And this should also explain why these open minded individuals normally think everyone else is just a tourist in their own lives, because either they do not have the ability to see the simple truth that is how short their life actually is, or they don’t care to waste their time so badly and they only realize all this when it is too late or maybe never.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "0A5B9AC7-217C-4DB6-BFB7-766CB7FA8E37".ToGUID() // loop back to the beginning.
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.galatea.greet.answer.2",
                                         guid: "fa5d2f0b-24c0-45d9-b1a9-eeffe41f42bc",
                                         text: "\"How does it feel to sleep with a really beautiful woman?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.galatea.loop.2",
                                                 guid: "6708bedb-201d-40b2-ae90-b4599bc65658",
                                                 speaker: bref,
                                                 text: "\"It feels exactly how a woman feels who wanted a baby for years and finally she had one. It allows you to feel absolution, but just like in the case of the baby, it won’t last, the baby grows up, and leaves the nest, and similarly but on a much smaller timescale the novelty of the beautiful woman wears off and you start to think about cheating. However. No matter how parents try to keep feeling the absolution by making new babies, such endeavor is unsustainable and also it has a biological limit, the reproductive years. The feeling of big Love however can be tricked to remain strong, if you live together in a threesome relationship, then you can forever feel the absolution, not least because being one of the same sex parts in a threesome relationship comes with the perk of both of the same sex ones feeling everlasting love towards the third one. Which is a pretty good predictor of relationship sustainability. As long as we cannot own someone 100%, our love is not going to cease. From the third one's side if he is a male, the feeling of being in the middle of 2 bombshels’s admiration is going to provide him with forever insane level of energy and courage to take on the world and tackle insurmountable odds in anything he decides to accomplish. In fact if you so inclined this works the other way around too. A guy and his cool bro in a threesome relationship with a super hot woman, will prevent the guys to ever lose the feeling of novelty with her granting everlasting love to both of them.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "0A5B9AC7-217C-4DB6-BFB7-766CB7FA8E37".ToGUID() // loop back to the beginning.
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.galatea.greet.answer.3",
                                         guid: "2f023fc0-36b1-44c5-bc9d-9d06ece1e5cf",
                                         text: "\"Why is sex such a taboo topic in so many societies?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.galatea.loop.3",
                                                 guid: "b02b38aa-65f8-40dd-a3f7-ca07568923da",
                                                 speaker: bref,
                                                 text: "\"Human society is essentially about pretending that sex doesn't exist in polite circumstances. Sexuality reminds them of their creaturliness and therefore their mortality, in a convoluted way. Often they must feel they are part of something bigger than themselves in a sense where their legacy can live on, so their minuscule lives won't feel as much in vain. Getting reminded of their creatureliness questions their elevated status as part of something bigger, hence they are terrified of bringing sexuality into the forefront of polite society's perception. They are also frustrated about any openly discussed ideas around sex, because they think anything that brings sexuality to the chance attention of children will make them learn about sex and then all those children are going to automatically and irreversibly become somehow cheap and disturbed in the best case and corrupted and rotten the worst case. For these reasons the topic of sex is like black magic for most humans.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "0A5B9AC7-217C-4DB6-BFB7-766CB7FA8E37".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.galatea.greet.answer.4",
                                         guid: "e677b044-5c15-4fcb-8c41-57a200facdd5",
                                         text: "\"Is there a link between sex and violence?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.galatea.loop.4",
                                                 guid: "d6210f81-345d-4685-af76-198c3f72d9b0",
                                                 speaker: bref,
                                                 text: "\"Sex and violence are two sides of the same coin. Sex is creating, violence is destroying. You might also have heard before that there is no creation without destruction. That is why we all have both tendencies to create and to destroy too. Also the only way to act out and channel aggression to peace on society's level, if enough people would role play power play during sex, ideally alternating with emotional love making to emphasize both style of sex via the contrast effect. This is what make love not war means. As you undoubtedly see most demons end up reveling in, instead of channeling violence. You know, at the end of the day humans and demons..., we are not that different from each other.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "0A5B9AC7-217C-4DB6-BFB7-766CB7FA8E37".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.galatea.greet.answer.5",
                                         guid: "8f2e2819-8a6d-4ad3-9473-93fc01e94f03",
                                         text: "\"Why do some mortals pursue immortality?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.galatea.loop.5",
                                                 guid: "aee50ea5-7ad1-4db2-bacf-49c71f79665e",
                                                 speaker: bref,
                                                 text: "\"It is not logical to not want to maximize one’s options. Death is the single biggest and most extreme limiting factor imaginable, especially if we compare their minuscule existence to the rest of the universe, and they are organic parts of that infinite universe, or at least they supposed to see themselves as such, but their limited thinking in the box makes most of them unable to see that.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "0A5B9AC7-217C-4DB6-BFB7-766CB7FA8E37".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.galatea.greet.answer.6",
                                         guid: "6951eafd-be2e-4502-b973-7a7fcecf0b91",
                                         text: "\"Do the inherent dualities of all creation mean that people think more in logically absolutist ways, taking as something either existing or something as not existing, and nothing between them?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.galatea.loop.6",
                                                 guid: "aafd286b-5697-4db7-a2b7-784366eecfe2",
                                                 speaker: bref,
                                                 text: "\"This comes down to the fact that humans' perception of their world on a fundamental (reptilian) level is hard rigged to see everything in terms of survival. Danger, no danger? Run or stay? Fight or freeze? Then your mammalian brain does the same on the societal level. Us or them? Friend or Foe? Only your prefrontal cortex can abstract away these paradigms and see most opposites for what they are, that is complementing parts of each other, for example like land and ocean."+Environment.NewLine+"Humans need conscious effort to train their reptilian and mammalian brain to see phenomena in the world on a continuum, instead of as binary pairs. Note, that the section described as continuum between the land and the ocean (the beach) is remarkably tiny, most of the constituting parts are definitively pure ocean and definitively pure land. Note also, that existing and nonexisting is not the essence of logic but one of its tools to describe dichotomous approaches like the default, untrained reptilian and mammalian perspectives. Logic is perfectly capable to describe spectrums or analogue systems to any degree that can arise in terms of practical usability."+Environment.NewLine+"That being said, opposites do emphasize each other especially when they meet. That's why when they are the closest to each other, things can get interesting. For example, in the case of people, during the act of sex or in the case of land and ocean, where the beach is. Without differentiation everything would be just grey, like the color of mixed paint. The trick is to learn to stay in the habitable zone, just like your planet stays there around your sun. Too close to polarize in either direction that is differentiating (waterless, bone dry land and ocean are separate floating balls) or homogenizing (thick mud), and advanced life won't be sustainable anymore.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "0A5B9AC7-217C-4DB6-BFB7-766CB7FA8E37".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.galatea.exit",
                                         guid: "52e84e46-8144-4a82-b850-c7b562a5ce21",
                                         text: "\"Thank you for you time.\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.galatea.exit.response",
                                                 guid: "defe9f7c-684e-4c71-9e67-c00fd2b72115",
                                                 speaker: bref,
                                                 text: "\"You know.., I like you!\""
                                             )
                                         })


                                 })
                         })
                     });



  
        }



        public void CreateDialog_2()
        {
            Name = "PoolSuccubus_2";
            AssetGuid = "c3edcaa2-ad86-47c1-a07f-6ab47ef7cdaf";

            UnitTemplateGuid = "98ee09f7378a93542ab8ec41223f2cb2";
            SpawnerPath = "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (2)";
            SpawnerScene = "MidnightFane_Caves_Mechanics";

            var template = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(UnitTemplateGuid);

            Blueprint = Helpers.CopyAndAdd(template, Name, AssetGuid);

            Blueprint.SetLocalisedName("succubus-clit", "Clitinae");

            var bref = Blueprint.ToReference<BlueprintUnitReference>();

            BlueprintDialogReference dialog;





            if (ResourcesLibrary.TryGetBlueprint<BlueprintDialog>("d18e8e83e305404ebfc0fb0352d9aa80") != null)
            {
                dialog = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>("d18e8e83e305404ebfc0fb0352d9aa80").ToReference<BlueprintDialogReference>();
                // Main.DebugLog("1");

            }
            else
                dialog = SimpleDialogBuilder.CreateDialog(
                     name: "simpledialog.clit.base",
                     guid: "d18e8e83-e305-404e-bfc0-fb0352d9aa80",
                            
                     firstCue: new List<BlueprintCueBaseReference>()
                     {
                     SimpleDialogBuilder.CreateCue(
                         name: "simpledialog.clit.greet",
                         guid: "641f8c35-1804-4eb4-a704-70e8857f63d9", // this guid is referenced below
                         speaker: bref,
                         text: "\"I love to be the best mating choice for mortals before I kill them, therefore I accumulated vast knowldge of human nature and also philosophy in order to astound them with my body and my intellect. Do you want to try both?\"",
                         answerList: new List<BlueprintAnswerBaseReference>()
                         {
                             SimpleDialogBuilder.CreateAnswerList(
                                 name: "simpledialog.clit.greet.answerlist",
                                 guid: "01343d7f-dfd6-488e-b586-0b4edd1ab599",
                                 answers: new List<BlueprintAnswerBaseReference>()
                                 {
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.clit.greet.answer.1",
                                         guid: "8dde3755-6904-4830-8a96-a99322995a66",
                                         text: "\"Is there any tell of women who are good in bed?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.clit.loop.1",
                                                 guid: "d8ff9a73-6ea1-42bc-89a4-6d17bfbdbcb5",
                                                 speaker: bref,
                                                 text: "\"Women who tend to forget about themselves in small things behaving like as if they have no idea that they are in a public place (while sober). For example they would observe details of some meaningless thing from up close, like children do, at least longer than it is seemingly warranted. These women are closer to their discoverer self that is unfettered by society’s mundane expectations, and more often than not their exploratory and enthusiastic nature is reflected in the bedroom too.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "641f8c35-1804-4eb4-a704-70e8857f63d9".ToGUID() // loop back to the beginning.
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.clit.greet.answer.2",
                                         guid: "4f9f43f9-8bfe-452e-8d8d-77d2464521ca",
                                         text: "\"How does an enlightened being deal with the instinct of lust? Why is spiritual logic like \"these desires are transient\" or that they \"can never fulfill you\" insufficient to quell such powerful feelings that return daily with the promise of fulfillment?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.clit.loop.2",
                                                 guid: "fe22f386-1658-4514-9e31-5b530296f1bd",
                                                 speaker: bref,
                                                 text: "\"An enlightened being integrates lust instead of suppressing it. Same with ego. You have to ride the wave, instead of trying to walk on water. Look up tantric sex.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "641f8c35-1804-4eb4-a704-70e8857f63d9".ToGUID() // loop back to the beginning.
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.clit.greet.answer.3",
                                         guid: "e7023b0a-881f-4f3d-bf33-955d888dc107",
                                         text: "\"Is beauty truly subjective and every ugly person is thought of as beautiful by someone?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.clit.loop.3",
                                                 guid: "8faf04f3-abcb-4792-ae50-5dff5c5894c1",
                                                 speaker: bref,
                                                 text: "\"This is a nice sentiment, except looks is actually physically real. Everything else is superficial because it is only in the imagination, and those things don't even exist. So the answer is no. Also as long as ugly people know they are ugly, they wouldn't try to con pretty people into doomed relationships with their personality or money. Ugly people should not think they are pretty, but they should not think their ugliness doesn't matter either. The entitlement of ugly people is causing every bad thing on earth. By not knowing their place they viciously fight for wealth, oppress others and fuel sex trafficking and corruption on every level imaginable. Ugly people should be extremely grateful if pretty people used them for sex for example especially if regular sex. If it was up to me the pretty and ugly people should not even be allowed by law to live in the same district. It is extremely demoralizing. That being said there are very few people who sculpt a toned body for themselves, remove all body hair, get a tan (perhaps a labiaplasty) and they won't become hot.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "641f8c35-1804-4eb4-a704-70e8857f63d9".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.clit.greet.answer.4",
                                         guid: "f2df29eb-f446-4e84-af2e-1c147d043910",
                                         text: "\"How would life be different if we were all bisexual?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.clit.loop.4",
                                                 guid: "f266ad7f-f699-416d-92d6-b751a7f4947a",
                                                 speaker: bref,
                                                 text: "\"Everyone kinda is bisexual. Sexuality is a spectrum and there are no people who are 100% straight or gay oriented all the time.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "641f8c35-1804-4eb4-a704-70e8857f63d9".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.clit.greet.answer.5",
                                         guid: "482dfaea-b79b-4259-aec1-e96966f13c26",
                                         text: "\"Why do males get upset if women have multiple affairs but expect women to not be upset when males have affairs?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.clit.loop.5",
                                                 guid: "58e5d9f0-b4d3-4c87-a212-7e7912e5ba77",
                                                 speaker: bref,
                                                 text: "\"Because men are not being ever subjected to absolutely anything resembling to expectation management. In fact from the men's perspective he has always been made to believe two distinct myth complexes. The first is the \"True love\" myth complex which tenets are: True love is only true love if it makes the woman never fancy another man again. Women only settle with guys they are in love with. And lastly, she is with him, therefore she is in love with him, therefore she won't ever fancy anyone else. The other one is the \"Waiting before sex\" myth complex: Women who feel they face too strong competition, think they can get ahead of the curve if they pretend they would never cheat or not promiscuous, and this only stands out as anything of value if they simultaneously promote the idea and convince men that any other behaviour means being a whore. Women made men believe if they make men wait before sex this means they are not whores. (Like this was something so hard to pretend, especially if she has side gigs, nevertheless tons of men idiotically let themselves made to believe this utter nonsense). Men now consequently believe women in general can be firmly divided into 2 distinct groups, such as wife materials and whores. The conclusion is men subjected to zero expectation management about natural female patterns of desire makes men extremely susceptible to be strongly primed by women to assume that a woman's value as a long term companion strictly depends on her ability to convince him about herself being the opposite of whores.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "641f8c35-1804-4eb4-a704-70e8857f63d9".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.clit.greet.answer.6",
                                         guid: "e3c58bb6-1903-4a9c-9f03-7453cbb1d9e6",
                                         text: "\"What things are forever beyond our ability to understand?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.clit.loop.6",
                                                 guid: "85450633-3b66-4dff-9f5f-043b5d513e73",
                                                 speaker: bref,
                                                 text: "\"Just how extremely short your life is. How you are burying yourselves in the pragmatism of the weekdays to engage your mental systems to protect your psyche from constantly being aware of your own mortality. Such hard wired template of smokes and mirrors fashions your mind to thrive in self deception. And where self deception is natural, progress largely stagnates compared to the leaps and bounds otherwise. This in turn hinders your ability to develop sustainable eternal youth for yourselves. Talk about self fulfilling prophecy. I laugh at this a lot whenever I think about it.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "641f8c35-1804-4eb4-a704-70e8857f63d9".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.clit.exit",
                                         guid: "7a7ae3e9-8b3d-46a1-82ff-f86d4466dd76",
                                         text: "\"This was very enlightening.\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.clit.exit.response",
                                                 guid: "26a98824-a1e4-49b8-80b9-8f2063fa4cd7",
                                                 speaker: bref,
                                                 text: "\"You should come and see what I can do with this jug of coconut oil... it really is the best!\""
                                             )
                                         })

                                 })
                         })
                     });


   
        }

        public void CreateDialog_3()
        {
            Name = "PoolSuccubus_3";
            AssetGuid = "62d11150-8247-4df7-9e6a-0fcdd90f6062";

            UnitTemplateGuid = "98ee09f7378a93542ab8ec41223f2cb2";
            SpawnerPath = "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (1)";
            SpawnerScene = "MidnightFane_Caves_Mechanics";

            var template = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(UnitTemplateGuid);

            Blueprint = Helpers.CopyAndAdd(template, Name, AssetGuid);

            Blueprint.SetLocalisedName("succubus-moni", "Monique");

            var bref = Blueprint.ToReference<BlueprintUnitReference>();


            BlueprintDialogReference dialog;



            if (ResourcesLibrary.TryGetBlueprint<BlueprintDialog>("00533f67b1c54292a2f4dc3c6d2cf12d") != null)
            {
                dialog = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>("00533f67b1c54292a2f4dc3c6d2cf12d").ToReference<BlueprintDialogReference>(); 
                // Main.DebugLog("1");

            }
            else
                dialog = SimpleDialogBuilder.CreateDialog(
                     name: "simpledialog.moni.base",
                     guid: "00533f67-b1c5-4292-a2f4-dc3c6d2cf12d",

                     firstCue: new List<BlueprintCueBaseReference>()
                     {
                     SimpleDialogBuilder.CreateCue(
                         name: "simpledialog.moni.greet",
                         guid: "b5bbf0af-db23-4719-ab95-a57461242e3e", // this guid is referenced below
                         speaker: bref,
                         text: "\"Do you think I have a pretty little head but it is empty? You'd be surprised how much I know about your kind...\"",
                         answerList: new List<BlueprintAnswerBaseReference>()
                         {
                             SimpleDialogBuilder.CreateAnswerList(
                                 name: "simpledialog.moni.greet.answerlist",
                                 guid: "9f499fb9-7a97-421b-9021-9321f04018d9",
                                 answers: new List<BlueprintAnswerBaseReference>()
                                 {
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.moni.greet.answer.1",
                                         guid: "de2c12e7-fda1-44d7-b8d9-b234273a9570",
                                         text: "\"If you are in a relationship, yet fantasize about sexual activity with others without ever doing anything physical, is that also cheating?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.moni.loop.1",
                                                 guid: "8ba171b5-d6ca-42be-a249-c6cd9d57179c",
                                                 speaker: bref,
                                                 text: "\"The reason why it is often not considered cheating is because people don't have their definitions right in their heads. I can demonstrate this using sexual orientation. Ask yourself are you straight if your intentions in general are to sleep with women or men? If you are a man, then you would think that if your answer is women, then you are straight, and the you would be right. However, that not requires to actually sleep with women right? So you are straight even if you don't sleep with anybody. It is the same with cheating. You are a cheater if you fantasize about others even if you are not doing it. Just like with being an alcoholic, or a meat eater, or s gambler, or a music fan or anything literally that is your orientation, preference or addiction, except you are not doing it, that is still your preference, orientation or addiction. The marked difference here is you can't live out in real life most of your preferences and orientations without actually doing them. You can fantasize about drinking, but you cannot be drunk without drinking alcohol. But with masturbation you can have perfectly real and real life orgasms while you fantasize about others, so it is an entirely different ballgame. People who fantasize about others in a relationship should own up to being cheaters by nature. Even though they may not be currently doing it, at least as long as their willpower holds, in actual fact their primary preference is cheating, which means if it was accepted by their partner and society, then they would lead a cheating lifestyle and therefore they are cheaters by nature and with masturbation they even have real life orgasms with imagined others and given the depths of their justifications, they cannot even imagine living in any other way.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "b5bbf0af-db23-4719-ab95-a57461242e3e".ToGUID() // loop back to the beginning.
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.moni.greet.answer.2",
                                         guid: "77a28f0f-f027-4f60-8aa8-50e4bdb1a556",
                                         text: "\"What is the strangest custom of humans?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.moni.loop.2",
                                                 guid: "6bca0274-0b1f-4f48-b5b3-9bad672bad4a",
                                                 speaker: bref,
                                                 text: "\"For me the strangest one is that in the so called \"polite\" and public reality and conversations millions of people pretend as if sexuality didn't exist, and they also spend most of their time in such environments, until they actually believe it doesn't exist. Then they need drinks in order to be able to let loose and have sex, where the alcoholic haze prevents them from chosing quality partners and minding usual precautions. Then the following days and weeks they spend the time with worrying about carnal diseases or getting pregnant and feeling guilty because of their lowered standards while drunk. Finally they conclude that sex is dangerous (as if you could call that sex, and it wasn't them who disregarded any cautions), and one night stands are not worth it (as if they did anything right). All the while people have the mentally challenged idea to call this awkward, repressed, suppressed, sexually uneducated, alcohol ridden idiotic and dangerous mess as \"too much freedom\", as if any of these people did what they are doing because they are so free sexually, while they are in fact the exact opposite.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "b5bbf0af-db23-4719-ab95-a57461242e3e".ToGUID() // loop back to the beginning.
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.moni.greet.answer.3",
                                         guid: "85c6ce19-612f-4b46-99e0-994b0a4522e4",
                                         text: "\"How does a woman get out of the sex zone (booty call zone)?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.moni.loop.3",
                                                 guid: "f87beeec-923a-4ce4-972c-c98f64855de2",
                                                 speaker: bref,
                                                 text: "\"If she is serious then there is a way. Wait for years and years. If she waits until he gets disappointed with all of his other long term relationships and he sees that she's been the only one he could rely on for steady sex forever, then he will realize she is the one. Remember history is created by those who show up. Just be always present, whenever he wants sex or only a bj. Ideally both of you should et yourself checked out by the priests for carnal diseases every year. Remember every single person who got something trusted their partners. Without exception, every single one of them. Let that sink in for a moment. Also, figure out what his ultimate desire is that he won't get from other girls and reliably provide it. He might need a lot of experience in the first place to know himself enough to know what that is and to realize that it is not easy to get. More often than not for a guy it is 3somes with 2 girls though. Sometimes it is 2 guys and one girl. She should figure it out and make herself unexpendable. If he has no such sexual kink then he needs to get his hormones checked or perhaps he is not even worth the trouble.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "b5bbf0af-db23-4719-ab95-a57461242e3e".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.moni.greet.answer.4",
                                         guid: "5755a96f-83bf-4efa-9049-df39e0ce279c",
                                         text: "\"I want to know how can a girlfriend find girls for threesomes for her boyfriend. Any suggestions on how can one make this happen?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.moni.loop.4",
                                                 guid: "8d30003c-ed49-4c30-9c80-52c3193d3b56",
                                                 speaker: bref,
                                                 text: "\"The easiest for would be to go to a place with music and dance, find a girl who is dancing alone, or rather sitting lonely on the side and pull her in for dancing. It would help a lot if you are actually pretty. Then dance with her sexy dance. If you touch a lot, maybe even kiss, then ask her about the threesome. Remember, girls prefer to wait for someone to make things happen. You might have to try it dozens of times though, because like everything in life this is a numbers game.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "b5bbf0af-db23-4719-ab95-a57461242e3e".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.moni.greet.answer.5",
                                         guid: "23171786-616e-4269-8143-b26f98968dec",
                                         text: "\"You demons don't have natural selection because you live so long, us mortal races survive better because of our individual variety within our species, don't you think?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.moni.loop.5",
                                                 guid: "36de1cf7-4c5b-499a-98f7-c60bf4b9512a",
                                                 speaker: bref,
                                                 text: "\"That works with creatures who don't know about their own mortality either because lack of sentience or abundance of self hypnotic logic killer trance, ie \"death is ok if it happens in the indeterminate 40 or 50 years\". Otherwise everyone would work on to solve mortality first, while realizing the only logical way to solve each and every other problem if there is enough time to solve them, but then that is actually the one and only way to solve them. Every solution needs time. Without time nothing can be solved. First you need time. Then you can do anything else. Everything else is pure, unadulterated insanity.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "b5bbf0af-db23-4719-ab95-a57461242e3e".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.moni.greet.answer.6",
                                         guid: "d16f0d44-000d-40bf-8248-7d4417fbda01",
                                         text: "\"Why do the Gods let so much bad things to happen to good people?\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.moni.loop.6",
                                                 guid: "51285fc8-ea6b-44e5-96cd-58f742dbdae8",
                                                 speaker: bref,
                                                 text: "\"You could say the Gods gave mortals everything they need to create heaven on all their planes, they just don't do it. Mortals especially like blaming the Gods for something and not even understand it's their own doing (more precisely not doing) the whole time.\"",
                                                 cueSelection: new Kingmaker.DialogSystem.CueSelection()
                                                 {
                                                     Cues = new List<BlueprintCueBaseReference>()
                                                     {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "b5bbf0af-db23-4719-ab95-a57461242e3e".ToGUID() // loop back to the beginning
                                                        }
                                                     },
                                                     Strategy = Strategy.First
                                                 })
                                         }),
                                     SimpleDialogBuilder.CreateAnswer(
                                         name: "simpledialog.moni.exit",
                                         guid: "e82b02b1-9258-4b15-b0e4-3b3c599f2452",
                                         text: "\"See you later.\"",
                                         nextCues: new List<BlueprintCueBaseReference>()
                                         {
                                             SimpleDialogBuilder.CreateCue(
                                                 name: "simpledialog.moni.exit.response",
                                                 guid: "b13fc846-fac1-4f8a-9b97-855b8dbfa345",
                                                 speaker: bref,
                                                 text: "\"I wish...!!\""
                                             )
                                         })

                                 })
                         })
                     });


            /*
            Dialog = SimpleDialogBuilder.CreateDialog("simpledialog.moni.base", Guid.NewGuid().ToString().ToUpper(), new List<BlueprintCueBaseReference>()
            {
                SimpleDialogBuilder.CreateCue("simpledialog.moni.greet", Guid.NewGuid().ToString().ToUpper(), bref, "{mf|Master|Mistress}, I am Moni who had no dialog but I can speak now!", new List<BlueprintAnswerBaseReference>()
                {
                    SimpleDialogBuilder.CreateAnswerList("simpledialog.citizen.greet.answerlist", "785607FF-2B69-44AC-B959-CBAD6BED959C", new List<BlueprintAnswerBaseReference>()
                    {
                        /*SimpleDialogBuilder.CreateAnswer("simpledialog.citizen.greet.answer.good", "9D4E451B-F6DF-4425-8345-94324D6DA455", "You sound great!", new List<BlueprintCueBaseReference>()
                        {
                            SimpleDialogBuilder.CreateCue("simpledialog.citizen.exit.good", "E82E6E36-3EC4-4EF5-A9C5-16E2F94529E1", bref, "Thanks, I am off to tell the world!", new List<BlueprintAnswerBaseReference>())
                        }),
                        SimpleDialogBuilder.CreateAnswer("simpledialog.citizen.greet.answer.bad", "C343C70D-C063-4B43-A131-F0065AC8466A", "I liked you better when you couldn't talk.", new List<BlueprintCueBaseReference>()
                        {
                            SimpleDialogBuilder.CreateCue("simpledialog.citizen.exit.bad", "253E4E65-CFCB-48B3-B7C3-44D738343DA4", bref, "Maybe I should drown you in this pool of water.", new List<BlueprintAnswerBaseReference>())
                        })
                    })
                })
            });
*/

        }

        
        public void CreateDialog_4()
        {
            Name = "MfaneSuccubusGuard";
            AssetGuid = "d4c326fa-919f-494d-8909-a9f72a26f333";
            //cleopatra dark-red hair smallblack wings
            UnitTemplateGuid = "2db556136eac2544fa9744314c2a5713";

            //UnitTemplateGuid = "3feeacfe8c18470cbd2df6e58cfd1db4";
            //UnitTemplateGuid = "958a7e03d5e04cb1aebae6374b8554fc";
            //UnitTemplateGuid = "5aae12755ba78b644b4d4d6745716ede";


            SpawnerPath = "[cr21] NocticulaPriestessPack/[OnlyPC] [CR11_SuccubusRanger] (2)";
            SpawnerScene = "MidnightFane_Caves_Mechanics";


            var template = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(UnitTemplateGuid);

            Blueprint = Helpers.CopyAndAdd(template, Name, AssetGuid);

            Blueprint.SetLocalisedName("succubus-cyda", "Cydaea");

            var bref = Blueprint.ToReference<BlueprintUnitReference>();

            var dialog = SimpleDialogBuilder.CreateDialog("simpledialog.cydaea.base", "b9518b8e-32d4-4755-b179-dbbab95ec2cb", new List<BlueprintCueBaseReference>()
            {
                SimpleDialogBuilder.CreateCue("simpledialog.cydaea.greet", "489d28ba-5dfc-4e9c-9ac7-67f6a58a1404", bref, "{mf|Master|Mistress}, I am Cydaea who had no dialog but I can speak now!", new List<BlueprintAnswerBaseReference>()
                {
                    /*SimpleDialogBuilder.CreateAnswerList("simpledialog.citizen.greet.answerlist", "785607FF-2B69-44AC-B959-CBAD6BED959C", new List<BlueprintAnswerBaseReference>()
                    {
                        /*SimpleDialogBuilder.CreateAnswer("simpledialog.citizen.greet.answer.good", "9D4E451B-F6DF-4425-8345-94324D6DA455", "You sound great!", new List<BlueprintCueBaseReference>()
                        {
                            SimpleDialogBuilder.CreateCue("simpledialog.citizen.exit.good", "E82E6E36-3EC4-4EF5-A9C5-16E2F94529E1", bref, "Thanks, I am off to tell the world!", new List<BlueprintAnswerBaseReference>())
                        }),
                        SimpleDialogBuilder.CreateAnswer("simpledialog.citizen.greet.answer.bad", "C343C70D-C063-4B43-A131-F0065AC8466A", "I liked you better when you couldn't talk.", new List<BlueprintCueBaseReference>()
                        {
                            SimpleDialogBuilder.CreateCue("simpledialog.citizen.exit.bad", "253E4E65-CFCB-48B3-B7C3-44D738343DA4", bref, "Maybe I should drown you in this pool of water.", new List<BlueprintAnswerBaseReference>())
                        })
                    })*/
                })
            });

            
        }
        private void CreateActionHolder(string aguid, string dguid)
        {
           // Log($"Creating ActionHolder for {Name}...");

            
            ActionName = "ActionSuccubusThatTalks" + Name;
            ActionGuid = aguid;

            //Main.DebugLog(ActionName);

            if (ResourcesLibrary.TryGetScriptable<ActionsHolder>(aguid) == null)
                ActionsHolder = Helpers.CreateAndAddESO<ActionsHolder>(ActionName, ActionGuid);
            else
                ActionsHolder = ResourcesLibrary.TryGetScriptable<ActionsHolder>(aguid);

            var startDialog = Helpers.CreateElement<StartDialog>(ActionsHolder);


            startDialog.m_Dialogue = ResourcesLibrary.TryGetBlueprint<BlueprintDialog>(dguid).ToReference<BlueprintDialogReference>();

            ActionsHolder.Actions = new ActionList()
            {
                Actions = new GameAction[]
                {
                    startDialog
                }
            };

           // Log($"Added dialog..." + Dialog.NameSafe());

        }

        public void ReplaceWithCustom(string spawnerScene, string guid, string spawnerPath)
        {

            BlueprintUnit unit = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(guid.ToGUID());
            //Log($"Target scene {SpawnerScene} detected!");
            //Log($"Fetching spawner: {SpawnerPath}");


            var spawnerholder = SceneManager.GetSceneByName(spawnerScene).GetRootGameObjects().Where(obj => obj.name == "Spawners").FirstOrDefault().transform.Find(spawnerPath);

            if (spawnerholder == null)
            {
                Log($"SpawnerPath: {SpawnerPath} not found...");
                return;
            }

            Spawner = spawnerholder.GetComponent<UnitSpawner>();

            if (Spawner == null)
            {
                Log($"Spawner: SpawnerComponent not found...");
                return;
            }

            //Log($"Replacing target unit with {Name}...");

            Spawner.Blueprint = unit;




                //(ResourcesLibrary.TryGetBlueprint(new BlueprintGuid(Guid.Parse("1fa8d5c724bf55e45808664bc2dc7b7e")));

            //Log($"Fetching spawner interactions...");

            SpawnerInteractionActions = Spawner.gameObject.GetComponent<SpawnerInteractionActions>();

            if (SpawnerInteractionActions == null)
            {
                Log("Fetching spawner interactions has failed...");
                return;
            }

            SpawnerInteractionActions.Actions = ActionsHolder.ToReference<ActionsReference>();

            //Log($"Spawning {Name}...");

            Unit = Spawner.ForceReSpawn();

            if (Unit == null)
                Log("Spawning failed...");
            else
            if (Unit.Blueprint.name.ToLower().Contains("poolsuccubus"))
            {
              
              //  var anim = new AnimationClipWrapperLink() { AssetId = "32e47d1043d7b9e47ae461da79120c65" };

               // Unit.View.AnimationManager.Execute(Unit.View.AnimationManager.CreateHandle(UnitAnimationActionClip.Create(anim.Load(), "")));
            }
        }

        public void OnAreaScenesLoaded() { }

        public void OnAreaLoadingComplete()
        {
            if (SceneManager.GetSceneByName("MidnightFane_Caves_Mechanics").isLoaded)

            {
                CreateActionHolder("1bc6fe52-73a0-4f63-bbc4-4a4ed3953ec4", "3180a5ae-ae0c-484a-8bfe-95acf459878c");
                ReplaceWithCustom("MidnightFane_Caves_Mechanics", "5c2f4bd5-2259-4a05-9080-1aba05bfb13e", "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (3)");

                CreateActionHolder("1a50a8e9-eace-4997-b4d0-8a11cc48cb35", "d18e8e83-e305-404e-bfc0-fb0352d9aa80");
                ReplaceWithCustom("MidnightFane_Caves_Mechanics", "c3edcaa2-ad86-47c1-a07f-6ab47ef7cdaf", "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (2)");

                CreateActionHolder("1de57573-1dd8-4235-88fe-bec67ab3e450", "00533f67-b1c5-4292-a2f4-dc3c6d2cf12d");
                ReplaceWithCustom("MidnightFane_Caves_Mechanics", "62d11150-8247-4df7-9e6a-0fcdd90f6062", "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (1)");

                CreateActionHolder("7692e0da-cf2a-4b3a-899d-c80f530cee60", "b9518b8e-32d4-4755-b179-dbbab95ec2cb");
                ReplaceWithCustom("MidnightFane_Caves_Mechanics", "d4c326fa-919f-494d-8909-a9f72a26f333", "[cr21] NocticulaPriestessPack/[OnlyPC] [CR11_SuccubusRanger] (2)");

            }


            // EventBus.Subscribe(this);

        }
        public void Dispose()
        {
            EventBus.Unsubscribe(this);
        }


        [HarmonyPatch(typeof(EntityCreationController), "SpawnUnit", new Type[] { typeof(BlueprintUnit), typeof(UnitEntityView), typeof(Vector3), typeof(Quaternion), typeof(SceneEntitiesState), typeof(string) })]
        public static class EntityCreationController_SpawnUnit_Patch
        {
            // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
            private static bool Prefix(EntityCreationController __instance, BlueprintUnit unit, ref UnitEntityView prefab, Vector3 position, Quaternion rotation, SceneEntitiesState state, ref UnitEntityData __result)
            {

                if (unit.CharacterName.Equals("Galatea"))
                {
                    //UIUtility.SendWarning("spawn!!!");
                    UnitViewLink Prefab = new UnitViewLink();

                    //white short hair
                    //Prefab.AssetId = "1fa8d5c724bf55e45808664bc2dc7b7e";

                    //long blond hair yellow bra
                    Prefab.AssetId = "bf95a250743df3b438ba19062975df38";

                    //long white hair ponytail blue bra
                    //Prefab.AssetId = "98e0aabf75be8bc4f87d91b5cb20f7cc";

                    //white hair cleopatra style grey bra
                    //Prefab.AssetId = "0aea0a130731ecd4bb4d19f61355e355";

                    // UnitEntityView unitEntityView = UnityEngine.Object.Instantiate<UnitEntityView>(Prefab.Load(), position, rotation);
                    //unitEntityView.Blueprint = unit;
                    //__result = (UnitEntityData)__instance.SpawnEntityWithView(unitEntityView, state);
                    prefab = Prefab.Load();


                    //SaveMap(prefab, "_BumpMap");

                    foreach (SkinnedMeshRenderer smr in prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        if (smr.material != null)
                        {

                            try
                            {

                                Texture2D bumpMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_004_SuccubusIncubus_Female_Atlas_n.png"),
                                    512, 512);

                                Texture2D baseMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_004_SuccubusIncubus_Female_Atlas_d.png"),
                                    smr.material.mainTexture.width, smr.material.mainTexture.height);

                                Texture2D masksMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_004_SuccubusIncubus_Female_Atlas_m.png"),
                                    smr.material.mainTexture.width, smr.material.mainTexture.height);

                                if (baseMap != null && baseMap.GetRawTextureData().Length > 1)
                                {
                                    smr.material.SetTexture("_BaseMap", baseMap);
                                    //smr.material.mainTexture = baseMap;
                                    smr.material.SetTexture("_MasksMap", masksMap);
                                    smr.material.SetTexture("_BumpMap", bumpMap);
                                }
                                //  smr.material.SetColor("_RimColor", new Color(0, 0, 0, 1));
                                //  smr.material.SetFloat("_RimLighting", 1);
                            }
                            catch (Exception x) { Main.DebugError(x); }
                        }
                    }

                    return true;
                }
                else if (unit.CharacterName.Equals("Clitinae"))
                {
                    UnitViewLink Prefab = new UnitViewLink();
                    //white hair cleopatra style grey bra
                    Prefab.AssetId = "bf95a250743df3b438ba19062975df38";
                    prefab = Prefab.Load();

                    foreach (SkinnedMeshRenderer smr in prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        if (smr.material != null)
                        {

                            try
                            {

                                Texture2D bumpMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_SuccubusNaked_Atlas_n.png"),
                                    512, 512);

                                Texture2D baseMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_SuccubusNaked_Atlas_d.png"),
                                    smr.material.mainTexture.width, smr.material.mainTexture.height);

                            //    Texture2D masksMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_004_SuccubusIncubus_Female_Atlas_m.png"),
                              //      smr.material.mainTexture.width, smr.material.mainTexture.height);

                                if (baseMap != null && baseMap.GetRawTextureData().Length > 1)
                                {
                                    smr.material.SetTexture("_BaseMap", baseMap);
                                    //smr.material.mainTexture = baseMap;
                                   // smr.material.SetTexture("_MasksMap", masksMap);
                                    smr.material.SetTexture("_BumpMap", bumpMap);
                                }
                                //  smr.material.SetColor("_RimColor", new Color(0, 0, 0, 1));
                                //  smr.material.SetFloat("_RimLighting", 1);
                            }
                            catch (Exception x) { Main.DebugError(x); }
                        }
                    }

                    return true;
                }
                else if (unit.CharacterName.Equals("Monique"))
                {
                    UnitViewLink Prefab = new UnitViewLink();
                    //white hair cleopatra style grey bra
                    Prefab.AssetId = "bf95a250743df3b438ba19062975df38";
                    prefab = Prefab.Load();

                    foreach (SkinnedMeshRenderer smr in prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        if (smr.material != null)
                        {

                            try
                            {

                                Texture2D bumpMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_004_SuccubusIncubus_Female_Atlas_n.png"),
                                    512, 512);

                                Texture2D baseMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "white_skin_black_long_hair_naked_d.png"),
                                    smr.material.mainTexture.width, smr.material.mainTexture.height);

                                Texture2D masksMap = ReadTexture(Path.Combine(Main.GetCompanionPortraitsDirectory(), "BCT_004_SuccubusIncubus_Female_Atlas_m.png"),
                                    smr.material.mainTexture.width, smr.material.mainTexture.height);

                                if (baseMap != null && baseMap.GetRawTextureData().Length > 1)
                                {
                                    smr.material.SetTexture("_BaseMap", baseMap);
                                    //smr.material.mainTexture = baseMap;
                                    smr.material.SetTexture("_MasksMap", masksMap);
                                    smr.material.SetTexture("_BumpMap", bumpMap);
                                }
                                //  smr.material.SetColor("_RimColor", new Color(0, 0, 0, 1));
                                //  smr.material.SetFloat("_RimLighting", 1);
                            }
                            catch (Exception x) { Main.DebugError(x); }
                        }
                    }

                    return true;
                }
                else if (unit.CharacterName.Equals("Cydaea"))
                {
                    UnitViewLink Prefab = new UnitViewLink();

                    //brown dreadlocks in ponytail, big red wings, full plate-ish with chain skirt thingy
                    //Prefab.AssetId = "7354470dfac63304ba0bc2ed96080f1a";
                    //short red hair, big red wings, full plate-ish with chain skirt thingy
                    //Prefab.AssetId = "f37c835bb9ef6d34a8a4b4ae514b8497";
                    //black mohawk, small black wings, serious full plate
                    //Prefab.AssetId = "c1353df810238f14b86596a179dd1208";
                    //black messy ponytail/bun, small black wings, leather armor
                    //Prefab.AssetId = "c5e3257f44a135e4489e88c1e73d611f";

                    //hoodie, red wings, studded armor 
                    //Prefab.AssetId = "5efb75acb6e18274ba3b0c34f937888f";
                    //hoodie, red wings, chain armor
                    //Prefab.AssetId = "992d087780118954cb71fc9bb2c07e70";

                    //Prefab.AssetId = "e099752964d2f5942bf6838fe058c09a";

                      Prefab.AssetId = "f37c835bb9ef6d34a8a4b4ae514b8497";
                    
                    prefab = Prefab.Load();
                    return true;
                }
                else 
                    return true;
                
            }

        }

        public static void SaveMap(UnitEntityView prefab, string mapType)
        {
            foreach (SkinnedMeshRenderer smr in prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
            {

                if (smr.material != null)
                {
                    //Main.DebugLog("c");

                    Texture texture = smr.material.GetTexture(mapType);


                    if (texture != null)
                    {
                        Main.DebugLog("SaveMap smr: " + texture.name);


                        //Texture texture = smr.material.mainTexture;
                        try
                        {
                            Texture2D t2 = new Texture2D(texture.width, texture.height);

                            //t2 = (smr.material.mainTexture as Texture2D);

                            RenderTexture currentRT = RenderTexture.active;

                            RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 32);
                            Graphics.Blit(texture, renderTexture);

                            RenderTexture.active = renderTexture;
                            t2.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                            t2.Apply();

                            Color[] pixels = t2.GetPixels();

                            RenderTexture.active = currentRT;

                            if (t2 != null)
                                if (t2.GetRawTextureData().Length > 1)
                                    File.WriteAllBytes(Path.Combine(Main.GetCompanionPortraitsDirectory(), texture.name + ".png"), t2.EncodeToPNG());
                                else
                                    Main.DebugLog("texture size is 0");
                            else
                                Main.DebugLog("texture is null");

                        }
                        catch (Exception e)
                        {
                            Main.DebugError(e);
                        }
                    }
                }
            }


        }


        public static Texture2D ReadTexture(string path, int x, int y)
        {
            byte[] array = File.ReadAllBytes(path);


            Texture2D texture2D = new Texture2D(x, y, TextureFormat.ARGB32, true);

            texture2D.filterMode = FilterMode.Point;


            texture2D.anisoLevel = 9;
            ImageConversion.LoadImage(texture2D, array);


            RenderTexture renderTex = RenderTexture.GetTemporary(
                                 texture2D.width,
                                 texture2D.height,
                                 32,
                                 RenderTextureFormat.ARGB32,
                                 RenderTextureReadWrite.sRGB);

            renderTex.antiAliasing = 8;
            renderTex.anisoLevel = 9;
            renderTex.filterMode = FilterMode.Trilinear;
            Graphics.Blit(texture2D, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;

            Texture2D readableText = new Texture2D(texture2D.width, texture2D.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            return readableText;

        }

    }
}



// 
// 
// B920EBA3-4D97-4B66-BDB2-997CD43FA355
// D9A1A4D8-064E-4899-952C-349E3D97CAFC
// 4146EF7F-73AB-47FB-88AD-068C6EAA4CFA
// B1359644-8C83-4480-853A-886642D86116
// 88B3E1D6-E31F-45A6-8624-B0560F2BFA8E
// 71733297-E232-44C9-802C-AA41A96FE366
// 5D3FC429-0538-478E-911E-2E9F0B0CA264
// D3CB0CEE-E3A7-431A-B0B9-47C5510E3C0F
// 67628A89-B548-4708-A3D5-2B503AEE7C90
// 72C79958-7C10-4F3B-AEB1-F912BC9754E8
// E982742B-DAB3-4DA4-8D67-A14B69932505
// 46C8F098-A1AF-49A8-A469-496CFCA34EA2
// BEB38053-1408-43ED-81B9-696BA5C34A91
// BFD1954C-A489-4444-8B95-EE90B71B1464
// 379568C9-2791-41C1-8B65-6CE532DEFD51
// 3827292E-E021-48CA-8014-CB3839AAC583
// D49FFEB5-8056-4970-9C6D-6273390A6C0A
// 972E5309-E81E-4488-9128-EC7E463BEEB6
// 6679C299-4607-4F76-8158-5DA4167ED611
// 240FC969-5E60-430D-B96A-3CD437659539
// 692EBDBE-A282-4758-9AB9-1EFC1F313CCA
// 35AF9B8F-3170-43A3-B9E2-FC972D2B9852
// 9C6D5E3D-DEF5-4016-850A-25BE4E2BEE3B
// DC83BE83-F523-4D5C-AF53-F37748549F8C
// 12E9E6FB-AAFD-42CB-86C5-43FE265DC7F3
// B3AD8CF8-27C5-47BC-94E1-899C673EF258
// 59618887-14C2-4876-A55C-709D72931919
// D511B237-DD95-4143-B565-48B24380289D
// ADADF5B0-6056-4B85-B5AC-BDB5819B8D58
// 2191234E-5DA6-4C2D-A826-3E5F73ECFC7F
// 29B76051-1B0E-4B24-8959-EBDAD2878644
// EB79BCBD-1BBE-423C-8374-225C7F277E5D
// 680C1205-8C3B-414D-992A-EFFF5D28592F
// 8FFB7CC9-D0C9-4BDA-9899-E2C98D7E19EF
// FD655B48-FE24-40A8-9A90-349DB351141F
// B008E267-9CEC-4A13-80D3-263D6BBF5799
// E71FBBCF-77B4-4637-AB23-59B1BC3534E8
// 255CE94D-3BBF-4C28-996E-B76AD3F9432E
// EB36D112-050E-4A7F-B822-6E8DA83D9EED
// 41A438A0-C6A0-4467-B2AB-1D29B1023DCC
// 6FC11DDB-F01F-4736-B6BF-8B30F4BA3DC8
// A9CF0AA5-0904-4971-902A-01CB76223DB3
// 31CE9849-0348-43C9-804E-A6093AD10494
// 2ADDB9F1-97D1-48E5-B0F9-E3E434F2BC3C
// 932B2BAE-33A3-422E-B415-8A0B5D29EE3C
// 3FF2472A-1FA8-4BF3-BC52-3DE09B7A3F2F
// 2EE4FF09-025C-4E12-A8A1-DF71E1E84D78
// 8DDECDF8-0980-46D4-BC59-F73357F1F974
// 8D400237-63E4-4802-9853-F11EA699A3F6
// DB0F561D-D107-4C45-8A84-D5F58E198E34
// 93B553FD-BA7F-44C4-AAC6-27B4DDC0D678
// 085579A1-5706-477C-AF6D-A65F67148782
// EDDB8A74-2F6C-4B1F-887B-6362EDCA53F5
// 022BD21B-935E-4745-BF28-4679DC8C289D
// E92DEF39-472D-4779-BEA4-27118A54C114
// A1A1043D-6ABA-4A04-B51F-F04C9505B618
// C96EF75C-F74B-4AAC-99E5-849CEDD55826
// 79E5C9DB-8AF0-45B9-9302-0E25A6E8F7CC
// 9E1A31B0-CBF3-4499-9061-6288D9654D40
// 294FBB13-484A-4B79-98BA-6D92912C247F
// 9330A0F4-33B9-4467-A50E-93B30A13FE22
// 04C880FB-222E-41E2-9B31-B63F98A50C86
// 14FB04ED-D7A3-4DEB-85B0-D5CF263B5C2C
// 9D3836E4-D3A1-4510-8F52-37F7999295A3
// 12314D37-CB79-4E10-BE1A-BDD28EC69319
// 4CD1B788-235B-4D85-B07B-4BC56719D7C1
// C62C92F5-3029-4842-9D30-4D3886584288
// 1DE08239-1CB2-48BF-88EE-3C6AD9A7921D
// 68ECBBA3-ECFA-42B8-87E0-291466D53E69
// 4AA5F8D1-DBD2-4F3E-984F-46DED48CFB40
// 20F9CA0D-E166-4261-8E16-54DD3AA90F21
// 433F1932-167F-41D1-8E54-485567C02389
// 8A3880FF-85F3-442D-A075-1E5816ED195D
// 61A980ED-12D6-4EFF-B0B2-732A0DFDB858
// 399D2FE4-E5B2-45A7-B331-D41233EC936F
// 28B1646F-3220-4E98-ADAF-4C3DB01164C9
// F7F78A01-F6A7-4638-8E0C-B368A72F0630
// 2DB2BBBB-48AE-4316-9B37-E5A9C6FA6F70
// CA5241CD-205D-48C0-A878-FAEC8DC4EDEE
// C1176B5F-DEC6-4220-B56F-3CCA7E6FD7AD
// 89DCAB13-2124-459F-ABED-1C9682C67493
// BCC0BD29-CCC3-496A-8C9C-C46C2E53058E
// 88688F94-EB94-471F-96D8-C5403E00009D
// B3CDA895-6F97-4424-8CF9-7CFC867653AF
// 91DB927F-D5A6-4126-8F9A-D4300AFACECB
// 1A6EB1C2-F5FB-4ABA-96AC-CD607E539108
// 3426517F-8198-4959-888B-EC657E201C1F
// B98EC2AD-05F5-4C2D-8287-91CFCD5FC4F7
// 950C940A-01D2-4A95-A604-D129AE6A2FBC
// 8F75E9C8-5556-4A88-A5E1-4880394BB6A1
// 94D3F7DC-F0C2-4A6E-98F6-D1505B272B75
// 80C8A30F-322E-4FCD-AF9D-6472C895F657
// 889E25F7-2403-487B-A6C7-C8BB07C4A3C8
// D2799754-9682-4466-AAAC-40584BCAC7ED
// F1E51056-6B80-4F6F-8C4C-45C8252A8603
// 363AD281-1C49-4D48-85BF-33B790B69031
// A5C598CD-7ED4-4E02-A372-7E7EBBB49D22
// EFEC245C-BA89-4F0C-802B-128C6D648ADF