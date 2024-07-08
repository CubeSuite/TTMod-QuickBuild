using BepInEx;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using FluffyUnderware.DevTools;
using FluffyUnderware.DevTools.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace QuickBuild
{
    internal static class QuickBuildGUI
    {
        // Objects & Variables
        internal static bool shouldShowGUI = false;
        internal static string currentCategory = "Root";
        internal static float sSinceClose = 0f;
        internal static float sSinceClick = 0f;

        private static bool initialisedGUI = false;
        private static bool loggedCategory = false;
        internal static Dictionary<string, List<MenuItem>> categoryOptions = new Dictionary<string, List<MenuItem>>() {
            { "Root", new List<MenuItem>() {
                new MenuItem() {
                    name = "Logistics",
                    icon = ResourceNames.ConveyorBelt
                },
                new MenuItem() {
                    name = "Production",
                    icon = ResourceNames.Assembler,
                },
                new MenuItem() {
                    name = "Structures",
                    icon = ResourceNames.CalycitePlatform5x5
                },
                new MenuItem() {
                    name = "Supports",
                    icon = ResourceNames.MetalRibBase1x2
                },
                new MenuItem() {
                    name = "Decorations",
                    icon = ResourceNames.CeilingPlant1x1
                }
            } },
            { "Root/Logistics", new List<MenuItem>(){
                new MenuItem() {
                    name = "Conveyors",
                    icon = ResourceNames.ConveyorBelt,
                },
                new MenuItem() {
                    name = "Inserters",
                    icon = ResourceNames.Inserter,
                },
                new MenuItem() {
                    name = "Monorails",
                    icon = ResourceNames.MonorailDepot
                },
                new MenuItem() {
                    name = "Power",
                    icon = ResourceNames.HighVoltageCable
                },
                new MenuItem() {
                    name = "Utility",
                    icon = ResourceNames.Container
                }
            } },
            { "Root/Logistics/Conveyors", new List<MenuItem>() {
                new MenuItem() {
                    name = "Mk 1",
                    icon = ResourceNames.ConveyorBelt,
                    buildable = ResourceNames.ConveyorBelt
                },
                new MenuItem() {
                    name = "Mk 2",
                    icon = ResourceNames.ConveyorBeltMKII,
                    buildable = ResourceNames.ConveyorBeltMKII
                },
                new MenuItem() {
                    name = "Mk 3",
                    icon = ResourceNames.ConveyorBeltMKIII,
                    buildable = ResourceNames.ConveyorBeltMKIII
                },
            } },
            { "Root/Logistics/Inserters", new List<MenuItem>() {
                new MenuItem() {
                    name = "Normal",
                    icon = ResourceNames.Inserter,
                    buildable = ResourceNames.Inserter
                },
                new MenuItem() {
                    name = "Long",
                    icon = ResourceNames.LongInserter,
                    buildable = ResourceNames.LongInserter
                },
                new MenuItem() {
                    name = "Filter",
                    icon = ResourceNames.FilterInserter,
                    buildable = ResourceNames.FilterInserter
                },
                new MenuItem() {
                    name = "Fast",
                    icon = ResourceNames.FastInserter,
                    buildable = ResourceNames.FastInserter
                },
                new MenuItem() {
                    name = "Stack",
                    icon = ResourceNames.StackInserter,
                    buildable = ResourceNames.StackInserter,
                },
                new MenuItem() {
                    name = "Stack Filter",
                    icon = ResourceNames.StackFilterInserter,
                    buildable = ResourceNames.StackFilterInserter,
                }
            } },
            { "Root/Logistics/Monorails", new List<MenuItem>() {
                new MenuItem() {
                    name = "Depot",
                    icon = ResourceNames.MonorailDepot,
                    buildable = ResourceNames.MonorailDepot
                },
                new MenuItem() {
                    name = "Pole",
                    icon = ResourceNames.MonorailPole,
                    buildable = ResourceNames.MonorailPole
                },
                new MenuItem() {
                    name = "Track",
                    icon = ResourceNames.MonorailTrack,
                    buildable = ResourceNames.MonorailTrack
                }
            } },
            { "Root/Logistics/Power", new List<MenuItem>() {
                new MenuItem() {
                    name = "Accum.",
                    icon = ResourceNames.Accumulator,
                    buildable = ResourceNames.Accumulator
                },
                new MenuItem() {
                    name = "Voltage Stepper",
                    icon = ResourceNames.VoltageStepper,
                    buildable = ResourceNames.VoltageStepper
                },
                new MenuItem() {
                    name = "HVC",
                    icon = ResourceNames.HighVoltageCable,
                    buildable = ResourceNames.HighVoltageCable,
                }
            } },
            { "Root/Logistics/Utility", new List<MenuItem>() {
                new MenuItem() {
                    name = "Chest",
                    icon = ResourceNames.Container, 
                    buildable = ResourceNames.Container
                } // ToDo: Fast travel thing
            } },
            { "Root/Production", new List<MenuItem>() {
                new MenuItem() {
                    name = "Miners",
                    icon = ResourceNames.MiningDrill
                },
                new MenuItem() {
                    name = "Smelters",
                    icon = ResourceNames.Smelter
                },
                new MenuItem() {
                    name = "Assemblers",
                    icon = ResourceNames.Assembler
                },
                new MenuItem() {
                    name = "Plants",
                    icon = ResourceNames.Thresher
                },
                new MenuItem() {
                    name = "Power",
                    icon = ResourceNames.CrankGenerator
                }
            } },
            { "Root/Production/Miners", new List<MenuItem>() {
                new MenuItem() {
                    name = "Mk 1",
                    icon = ResourceNames.MiningDrill,
                    buildable = ResourceNames.MiningDrill
                },
                new MenuItem() {
                    name = "Mk 2",
                    icon = ResourceNames.MiningDrillMKII,
                    buildable = ResourceNames.MiningDrillMKII
                },
                new MenuItem() {
                    name = "Blast",
                    icon = ResourceNames.BlastDrill,
                    buildable = ResourceNames.BlastDrill
                },
                new MenuItem() {
                    name = "Charge",
                    icon = ResourceNames.MiningCharge,
                    buildable = ResourceNames.MiningCharge
                }
            } },
            { "Root/Production/Smelters", new List<MenuItem>() {
                new MenuItem() {
                    name = "Mk 1",
                    icon = ResourceNames.Smelter,
                    buildable = ResourceNames.Smelter
                },
                new MenuItem() {
                    name = "Mk 2",
                    icon = ResourceNames.SmelterMKII,
                    buildable = ResourceNames.SmelterMKII
                },
                new MenuItem() {
                    name = "Blast",
                    icon = ResourceNames.BlastSmelter,
                    buildable = ResourceNames.BlastSmelter
                }
            } },
            { "Root/Production/Assemblers", new List<MenuItem>() {
                new MenuItem() {
                    name = "Mk 1",
                    icon = ResourceNames.Assembler,
                    buildable = ResourceNames.Assembler
                },
                new MenuItem() {
                    name = "Mk 2",
                    icon = ResourceNames.AssemblerMKII,
                    buildable = ResourceNames.AssemblerMKII
                }
            } },
            { "Root/Production/Plants", new List<MenuItem>() {
                new MenuItem() {
                    name = "Planter",
                    icon = ResourceNames.Planter,
                    buildable = ResourceNames.Planter
                },
                new MenuItem() {
                    name = "Thr. Mk 1",
                    icon = ResourceNames.Thresher,
                    buildable = ResourceNames.Thresher
                },
                new MenuItem() {
                    name = "Thr. Mk 2",
                    icon = ResourceNames.ThresherMKII,
                    buildable = ResourceNames.ThresherMKII
                }
            } },
            { "Root/Production/Power", new List<MenuItem>() {
                new MenuItem() {
                    name = "Mk 1",
                    icon = ResourceNames.CrankGenerator,
                    buildable = ResourceNames.CrankGenerator,
                },
                new MenuItem() {
                    name = "Mk 2",
                    icon = ResourceNames.CrankGenerator,
                    buildable = ResourceNames.CrankGeneratorMKII
                },
                new MenuItem() {
                    name = "Water Wheel",
                    icon = ResourceNames.WaterWheel,
                    buildable = ResourceNames.WaterWheel
                }
            } },
            { "Root/Structures", new List<MenuItem>() {
                new MenuItem() {
                    name = "Floors",
                    icon = ResourceNames.CalycitePlatform5x5,
                },
                new MenuItem() {
                    name = "Walls",
                    icon = ResourceNames.CalyciteWall3x3
                },
                new MenuItem(){
                    name = "Caps",
                    icon = ResourceNames.CalyciteWallCap3x1
                },
                new MenuItem() {
                    name = "Gates",
                    icon = ResourceNames.CalyciteGate5x5
                },
                new MenuItem() {
                    name = "Cutaways",
                    icon = ResourceNames.CalyciteWallCutaway5x5
                },
                new MenuItem() {
                    name = "Stairs",
                    icon = ResourceNames.MetalStairs3x5,
                },
                new MenuItem() {
                    name = "Ramps",
                    icon = ResourceNames.CalyciteRamp1x5
                }
            } },
            { "Root/Structures/Floors", new List<MenuItem>() {
                new MenuItem() {
                    name = "Power Floor",
                    icon = ResourceNames.PowerFloor,
                    buildable = ResourceNames.PowerFloor
                },
                new MenuItem() {
                    name = "1x1",
                    icon = ResourceNames.CalycitePlatform1x1,
                    buildable = ResourceNames.CalycitePlatform1x1
                },
                new MenuItem() {
                    name = "3x3",
                    icon = ResourceNames.CalycitePlatform3x3,
                    buildable = ResourceNames.CalycitePlatform3x3
                },
                new MenuItem() {
                    name = "5x5",
                    icon = ResourceNames.CalycitePlatform5x5,
                    buildable = ResourceNames.CalycitePlatform5x5,
                },
                new MenuItem() {
                    name = "Catwalk 3x9",
                    icon = ResourceNames.Catwalk3x9,
                    buildable = ResourceNames.Catwalk3x9
                },
                new MenuItem() {
                    name = "Catwalk 5x9",
                    icon = ResourceNames.Catwalk5x9,
                    buildable = ResourceNames.Catwalk5x9
                },
                new MenuItem() {
                    name = "Disco 1x1",
                    icon = ResourceNames.DiscoBlock1x1,
                    buildable = ResourceNames.DiscoBlock1x1,
                },
                new MenuItem() {
                    name = "Glow 1x1",
                    icon = ResourceNames.GlowBlock1x1,
                    buildable = ResourceNames.GlowBlock1x1,
                }
            } },
            { "Root/Structures/Walls", new List<MenuItem>() {
                new MenuItem() {
                    name = "3x3",
                    icon = ResourceNames.CalyciteWall3x3,
                    buildable = ResourceNames.CalyciteWall3x3
                },
                new MenuItem() {
                    name = "5x3",
                    icon = ResourceNames.CalyciteWall5x3,
                    buildable = ResourceNames.CalyciteWall5x3,
                },
                new MenuItem() {
                    name = "5x5",
                    icon = ResourceNames.CalyciteWall5x5,
                    buildable = ResourceNames.CalyciteWall5x5
                }
            } },
            { "Root/Structures/Caps", new List<MenuItem>(){
                new MenuItem() {
                    name = "Corner",
                    icon = ResourceNames.CalyciteWallCorner1x1,
                    buildable = ResourceNames.CalyciteWallCorner1x1
                },
                new MenuItem() {
                    name = "3x1",
                    icon = ResourceNames.CalyciteWallCap3x1,
                    buildable = ResourceNames.CalyciteWallCap5x1,
                },
                new MenuItem() {
                    name = "5x1",
                    icon = ResourceNames.CalyciteWallCap5x1,
                    buildable = ResourceNames.CalyciteWallCap5x1
                },
                new MenuItem() {
                    name = "3x1",
                    icon = ResourceNames.CalyciteVerticalWallCap3x1,
                    buildable = ResourceNames.CalyciteVerticalWallCap3x1
                },
                new MenuItem() {
                    name = "5x1",
                    icon = ResourceNames.CalyciteVerticalWallCap5x1,
                    buildable = ResourceNames.CalyciteVerticalWallCap5x1
                }
            } },
            { "Root/Structures/Gates", new List<MenuItem>() {
                new MenuItem() {
                    name = "5x2",
                    icon = ResourceNames.CalyciteGate5x2,
                    buildable = ResourceNames.CalyciteGate5x2
                },
                new MenuItem() {
                    name = "5x5",
                    icon = ResourceNames.CalyciteGate5x5,
                    buildable = ResourceNames.CalyciteGate5x5
                },
                new MenuItem() {
                    name = "10x5",
                    icon = ResourceNames.CalyciteGate10x5,
                    buildable = ResourceNames.CalyciteGate10x5
                }
            } },
            { "Root/Structures/Cutaways", new List<MenuItem>() {
                new MenuItem() {
                    name = "2x2",
                    icon = ResourceNames.CalyciteWallCutaway2x2,
                    buildable = ResourceNames.CalyciteWallCutaway2x2,
                },
                new MenuItem() {
                    name = "3x3",
                    icon = ResourceNames.CalyciteWallCutaway3x3,
                    buildable = ResourceNames.CalyciteWallCutaway3x3,
                },
                new MenuItem() {
                    name = "5x3",
                    icon = ResourceNames.CalyciteWallCutaway5x3,
                    buildable = ResourceNames.CalyciteWallCutaway5x3,
                },
                new MenuItem() {
                    name = "5x5",
                    icon = ResourceNames.CalyciteWallCutaway5x5,
                    buildable = ResourceNames.CalyciteWallCutaway5x5
                }
            } },
            { "Root/Structures/Stairs", new List<MenuItem>() {
                new MenuItem() {
                    name = "1x1",
                    icon = ResourceNames.MetalStairs1x1,
                    buildable = ResourceNames.MetalStairs1x1,
                },
                new MenuItem() {
                    name = "3x1",
                    icon = ResourceNames.MetalStairs3x1,
                    buildable = ResourceNames.MetalStairs3x1,
                },
                new MenuItem() {
                    name = "3x3",
                    icon = ResourceNames.MetalStairs3x3,
                    buildable = ResourceNames.MetalStairs3x3
                },
                new MenuItem() {
                    name = "3x5",
                    icon = ResourceNames.MetalStairs3x5,
                    buildable = ResourceNames.MetalStairs3x5
                } 
            } },
            { "Root/Structures/Ramps", new List<MenuItem>() {
                new MenuItem() {
                    name = "1x1",
                    icon = ResourceNames.CalyciteRamp1x1,
                    buildable = ResourceNames.CalyciteRamp1x1,
                },
                new MenuItem() {
                    name = "1x3",
                    icon = ResourceNames.CalyciteRamp1x3,
                    buildable = ResourceNames.CalyciteRamp1x3,
                },
                new MenuItem() {
                    name = "1x5",
                    icon = ResourceNames.CalyciteRamp1x5,
                    buildable = ResourceNames.CalyciteRamp1x5,
                },
            } },
            { "Root/Supports", new List<MenuItem>() {
                new MenuItem() {
                    name = "Beams",
                    icon = ResourceNames.CalyciteBeam5x1
                },
                new MenuItem() {
                    name = "Pillars",
                    icon = ResourceNames.CalycitePillar1x5
                },
                new MenuItem() {
                    name = "Arches",
                    icon = ResourceNames.MetalAngleSupport5x2
                },
                new MenuItem() {
                    name = "Ribs",
                    icon = ResourceNames.MetalRibBase1x2
                }
            } },
            { "Root/Supports/Beams", new List<MenuItem>() {
                new MenuItem(){ 
                    name = "Calycite 1x1",
                    icon = ResourceNames.CalyciteBeam1x1,
                    buildable = ResourceNames.CalyciteBeam1x1,
                },
                new MenuItem(){
                    name = "Calycite 3x1",
                    icon = ResourceNames.CalyciteBeam3x1,
                    buildable = ResourceNames.CalyciteBeam3x1,
                },
                new MenuItem(){
                    name = "Calycite 5x1",
                    icon = ResourceNames.CalyciteBeam5x1,
                    buildable = ResourceNames.CalyciteBeam5x1,
                },
                new MenuItem() {
                    name = "Metal 1x1",
                    icon = ResourceNames.MetalBeam1x1,
                    buildable = ResourceNames.MetalBeam1x1,
                },
                new MenuItem() {
                    name = "Metal 3x1",
                    icon = ResourceNames.MetalBeam1x3,
                    buildable = ResourceNames.MetalBeam1x3,
                },
                new MenuItem() {
                    name = "Metal 5x1",
                    icon = ResourceNames.MetalBeam1x5,
                    buildable = ResourceNames.MetalBeam1x5,
                }
            } },
            { "Root/Supports/Pillars", new List<MenuItem>() {
                new MenuItem(){ 
                    name = "Calycite 1x1",
                    icon = ResourceNames.CalycitePillar1x1,
                    buildable = ResourceNames.CalycitePillar1x1,
                },
                new MenuItem(){
                    name = "Calycite 1x3",
                    icon = ResourceNames.CalycitePillar1x3,
                    buildable = ResourceNames.CalycitePillar1x3,
                },
                new MenuItem(){
                    name = "Calycite 1x5",
                    icon = ResourceNames.CalycitePillar1x5,
                    buildable = ResourceNames.CalycitePillar1x5,
                },
                new MenuItem() {
                    name = "Metal 1x1",
                    icon = ResourceNames.MetalPillar1x1,
                    buildable = ResourceNames.MetalPillar1x1,
                },
                new MenuItem() {
                    name = "Metal 1x3",
                    icon = ResourceNames.MetalPillar1x3,
                    buildable = ResourceNames.MetalPillar1x3,
                },
                new MenuItem() {
                    name = "Metal 1x5",
                    icon = ResourceNames.MetalPillar1x5,
                    buildable = ResourceNames.MetalPillar1x5,
                },
            } },
            { "Root/Supports/Arches", new List<MenuItem>() {
                new MenuItem() {
                    name = "Calycite 3x3",
                    icon = ResourceNames.CalyciteAngleSupport3x3,
                    buildable = ResourceNames.CalyciteAngleSupport3x3,
                },
                new MenuItem() {
                    name = "Metal 5x2",
                    icon = ResourceNames.MetalAngleSupport5x2,
                    buildable = ResourceNames.MetalAngleSupport5x2
                }
            } },
            { "Root/Supports/Ribs", new List<MenuItem>(){
                new MenuItem() {
                    name =  "Base 1x2",
                    icon = ResourceNames.MetalRibBase1x2,
                    buildable = ResourceNames.MetalRibBase1x2
                },
                new MenuItem() {
                    name = "Middle 1x3",
                    icon = ResourceNames.MetalRibMiddle1x3,
                    buildable = ResourceNames.MetalRibMiddle1x3
                }
            } },
            { "Root/Decorations", new List<MenuItem>(){ 
                new MenuItem() {
                    name = "Lights",
                    icon = ResourceNames.WallLight1x1
                },
                new MenuItem() {
                    name = "Railings",
                    icon = ResourceNames.Railing3x1
                },
                new MenuItem() {
                    name = "Plants",
                    icon = ResourceNames.CeilingPlant1x1
                }
            } },
            { "Root/Decorations/Lights", new List<MenuItem>() {
                new MenuItem() {
                    name = "Sticks",
                    icon = ResourceNames.LightStick
                },
                new MenuItem() {
                    name = "Overhead",
                    icon = ResourceNames.OverheadLight,
                    buildable = ResourceNames.OverheadLight,
                },
                new MenuItem() {
                    name = "Lamp",
                    icon = ResourceNames.StandingLamp1x5,
                    buildable = ResourceNames.StandingLamp1x5,
                },
                new MenuItem() {
                    name = "Wall 1x1",
                    icon = ResourceNames.WallLight1x1,
                    buildable = ResourceNames.WallLight1x1
                },
                new MenuItem() {
                    name = "Wall 3x1",
                    icon = ResourceNames.WallLight3x1,
                    buildable = ResourceNames.WallLight3x1,
                },
                new MenuItem() {
                    name = "Ceiling 1x1",
                    icon = ResourceNames.HangingLamp1x1,
                    buildable = ResourceNames.HangingLamp1x1,
                },
                new MenuItem() {
                    name = "Fan 7x2",
                    icon = ResourceNames.FanLamp7x2,
                    buildable = ResourceNames.FanLamp7x2
                }
            } },
            { "Root/Decorations/Railings", new List<MenuItem>(){
                new MenuItem() {
                    name = "1x1",
                    icon = ResourceNames.Railing1x1,
                    buildable = ResourceNames.Railing1x1,
                },
                new MenuItem() {
                    name = "3x1",
                    icon = ResourceNames.Railing3x1,
                    buildable = ResourceNames.Railing3x1,
                },
                new MenuItem() {
                    name = "5x1",
                    icon = ResourceNames.Railing5x1,
                    buildable = ResourceNames.Railing5x1,
                },
                new MenuItem() {
                    name = "Corner 1x1",
                    icon = ResourceNames.RailingCorner1x1,
                    buildable = ResourceNames.RailingCorner1x1,
                },
                new MenuItem() {
                    name = "Corner 3x3",
                    icon = ResourceNames.RailingCorner3x3,
                    buildable = ResourceNames.RailingCorner3x3,
                }
            } },
            { "Root/Decorations/Plants", new List<MenuItem>() {
                new MenuItem() {
                    name = "Pot 1x1",
                    icon = ResourceNames.SmallFloorPot,
                    buildable = ResourceNames.SmallFloorPot,
                },
                new MenuItem() {
                    name = "Wall Pot 3x1",
                    icon = ResourceNames.WallPot,
                    buildable = ResourceNames.WallPot,
                },
                new MenuItem() {
                    name = "Pot 2x2",
                    icon = ResourceNames.MediumFloorPot,
                    buildable = ResourceNames.MediumFloorPot,
                },
                new MenuItem() {
                    name = "Ceiling 1x1",
                    icon = ResourceNames.CeilingPlant1x1,
                    buildable = ResourceNames.CeilingPlant1x1,
                },
                new MenuItem() {
                    name = "Ceiling 3x3",
                    icon = ResourceNames.CeilingPlant3x3,
                    buildable = ResourceNames.CeilingPlant3x3,
                },
                new MenuItem() {
                    name = "Wall 1x1",
                    icon = ResourceNames.WallPlant1x1,
                    buildable = ResourceNames.WallPlant1x1,
                },
                new MenuItem() {
                    name = "Wall 3x3",
                    icon = ResourceNames.WallPlant3x3,
                    buildable = ResourceNames.WallPlant3x3,
                }
            } }
        };

        private static float xPos => (Screen.width - 512) / 2f;
        private static float yPos => (Screen.height - 512) / 2f;

        #region Textures

        private static Texture2D oneNormal;
        private static Texture2D oneHover;
        private static Texture2D twoNormal;
        private static Texture2D twoHover;
        private static Texture2D threeNormal;
        private static Texture2D threeHover;
        private static Texture2D fourNormal;
        private static Texture2D fourHover;
        private static Texture2D fiveNormal;
        private static Texture2D fiveHover;
        private static Texture2D sixNormal;
        private static Texture2D sixHover;
        private static Texture2D sevenNormal;
        private static Texture2D sevenHover;
        private static Texture2D eightNormal;
        private static Texture2D eightHover;

        #endregion

        #region Styles

        private static GUIStyle oneNormalStyle;
        private static GUIStyle oneHoverStyle;
        private static GUIStyle twoNormalStyle;
        private static GUIStyle twoHoverStyle;
        private static GUIStyle threeNormalStyle;
        private static GUIStyle threeHoverStyle;
        private static GUIStyle fourNormalStyle;
        private static GUIStyle fourHoverStyle;
        private static GUIStyle fiveNormalStyle;
        private static GUIStyle fiveHoverStyle;
        private static GUIStyle sixNormalStyle;
        private static GUIStyle sixHoverStyle;
        private static GUIStyle sevenNormalStyle;
        private static GUIStyle sevenHoverStyle;
        private static GUIStyle eightNormalStyle;
        private static GUIStyle eightHoverStyle;

        private static GUIStyle iconStyle;
        private static GUIStyle normalTextStyle;
        private static GUIStyle hoverTextStyle;

        #endregion

        // Internal Functions

        internal static void LoadImages() {
            oneNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.1-Normal.png");
            oneHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.1-Hover.png");
            twoNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.2-Normal.png");
            twoHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.2-Hover.png");
            threeNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.3-Normal.png");
            threeHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.3-Hover.png");
            fourNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.4-Normal.png");
            fourHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.4-Hover.png");
            fiveNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.5-Normal.png");
            fiveHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.5-Hover.png");
            sixNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.6-Normal.png");
            sixHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.6-Hover.png");
            sevenNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.7-Normal.png");
            sevenHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.7-Hover.png");
            eightNormal = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.8-Normal.png");
            eightHover = ModUtils.LoadTexture2DFromFile("QuickBuild.Images.8-Hover.png");
        }

        internal static void CloseAndReset() {
            ModUtils.FreeCursor(false);
            shouldShowGUI = false;
            loggedCategory = false;
            currentCategory = "Root";
            sSinceClose = 0f;
        }

        internal static void Draw() {
            EDT.PacedLog("General", "Drawing Quick Build Menu");

            if (!initialisedGUI) InitialiseGUI();

            if(UnityInput.Current.GetMouseButtonDown(1) && sSinceClick > 0.2f) {
                sSinceClick = 0f;
                if(currentCategory != "Root") {
                    GoBackOneLevel();
                }
                else {
                    CloseAndReset();
                }
            }

            DrawSegmentBackgrounds();
            DrawSegmentForegrounds();
            EDT.SetPacedLogDelay(1f);
        }

        // Draw Functions

        private static void DrawSegmentForegrounds() {
            List<MenuItem> items = categoryOptions[currentCategory];
            foreach(MenuItem item in items) {
                int index = items.IndexOf(item);
                bool isHighlighted = ShouldHighlightItem(index, items.Count);
                Vector2 position = GetForegroundLocation(index, items.Count);

                GUI.Box(new Rect(position.x - 30, position.y - 40, 60, 60), ModUtils.GetImageForResource(item.icon), iconStyle);
                GUI.Label(new Rect(position.x - 40, position.y - 40, 80, 80), item.name, isHighlighted ? hoverTextStyle : normalTextStyle);
            }
        }

        private static void DrawSegmentBackgrounds() {
            List<MenuItem> items = categoryOptions[currentCategory];
            foreach (MenuItem item in items) {
                if (!loggedCategory) {
                    EDT.Log("General", $"Drawing Item '{item.name}' at ({xPos},{yPos})");
                    loggedCategory = true;
                }

                bool isHighlighted = ShouldHighlightItem(items.IndexOf(item), items.Count);

                GUIStyle style = GetBackgroundStyleForNumButtons(items.Count, isHighlighted);
                GUI.Box(new Rect(xPos, yPos, 512, 512), "", style);

                if (UnityInput.Current.GetMouseButtonDown(0) && isHighlighted && sSinceClick > 0.2f) {
                    OnMenuItemClicked(item);
                }

                GUIUtility.RotateAroundPivot(360f / items.Count, new Vector2(xPos + 256, yPos + 256));
            }
        }

        // Private Functions

        private static void InitialiseGUI() {
            initialisedGUI = true;

            oneNormalStyle = new GUIStyle() { normal = { background = oneNormal } };
            oneHoverStyle = new GUIStyle() { normal = { background = oneHover} };
            twoNormalStyle = new GUIStyle() { normal = { background = twoNormal } };
            twoHoverStyle = new GUIStyle() { normal = { background = twoHover } };
            threeNormalStyle = new GUIStyle() { normal = { background = threeNormal } };
            threeHoverStyle = new GUIStyle() { normal = { background = threeHover } };
            fourNormalStyle = new GUIStyle() { normal = { background = fourNormal } };
            fourHoverStyle = new GUIStyle() { normal = { background = fourHover } };
            fiveNormalStyle = new GUIStyle() { normal = { background = fiveNormal } };
            fiveHoverStyle = new GUIStyle() { normal = { background = fiveHover } };
            sixNormalStyle = new GUIStyle() { normal = { background = sixNormal } };
            sixHoverStyle = new GUIStyle() { normal = { background = sixHover } };
            sevenNormalStyle = new GUIStyle() { normal = { background = sevenNormal } };
            sevenHoverStyle = new GUIStyle() { normal = { background = sevenHover } };
            eightNormalStyle = new GUIStyle() { normal = { background = eightNormal } };
            eightHoverStyle = new GUIStyle() { normal = { background = eightHover } };

            iconStyle = new GUIStyle() { normal = { background = null } };

            normalTextStyle = new GUIStyle() {
                fontSize = 15,
                alignment = TextAnchor.LowerCenter,
                normal = { textColor = new Color(68, 68, 98) }
            };

            hoverTextStyle = new GUIStyle() {
                fontSize = 15,
                alignment = TextAnchor.LowerCenter,
                normal = { textColor = new Color(255, 165, 0) }
            };
        }

        private static GUIStyle GetBackgroundStyleForNumButtons(int numButtons, bool isHighlighted) {
            if(isHighlighted) {
                switch (numButtons) {
                    case 1: return oneHoverStyle;
                    case 2: return twoHoverStyle;
                    case 3: return threeHoverStyle;
                    case 4: return fourHoverStyle;
                    case 5: return fiveHoverStyle;
                    case 6: return sixHoverStyle;
                    case 7: return sevenHoverStyle;
                    case 8: return eightHoverStyle;
                    default:
                        QuickBuildPlugin.Log.LogError($"Could not get hover style for numButtons: {numButtons}");
                        return new GUIStyle();
                }
            }
            else {
                switch (numButtons) {
                    case 1: return oneNormalStyle;
                    case 2: return twoNormalStyle;
                    case 3: return threeNormalStyle;
                    case 4: return fourNormalStyle;
                    case 5: return fiveNormalStyle;
                    case 6: return sixNormalStyle;
                    case 7: return sevenNormalStyle;
                    case 8: return eightNormalStyle;
                    default:
                        QuickBuildPlugin.Log.LogError($"Could not get normal style for numButtons: {numButtons}");
                        return new GUIStyle();
                }
            }
        }

        private static bool ShouldHighlightItem(int index, int numButtons) {
            float mouseX = UnityInput.Current.mousePosition.x - (Screen.width / 2f);
            float mouseY = UnityInput.Current.mousePosition.y - (Screen.height / 2f);

            float radius = Mathf.Sqrt(Mathf.Pow(mouseX, 2) + Mathf.Pow(mouseY, 2));
            if (radius < 156) return false;
            
            float theta = Mathf.Rad2Deg * Mathf.Atan2(mouseX, mouseY);
            if (theta < 0) theta += 360;
            
            float degPerItem = 360f / numButtons;
            float start = index * degPerItem;
            float end = (index + 1) * degPerItem;

            return theta > start && theta <= end;
        }

        private static Vector2 GetForegroundLocation(int index, int numButtons) {
            float degPerItem = 360f / numButtons;
            float radius = 206f;
            float theta = (2 * index + 1) * degPerItem / 2f;
            theta = 90 - theta;
            if (theta < 0) theta += 360;

            EDT.PacedLog("General", $"{index}/{numButtons} has theta {theta}");

            float x = radius * Mathf.Cos(theta * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(theta * Mathf.Deg2Rad);
            
            EDT.PacedLog("General", $"(x,y) = ({x}, {y})");

            float screenX = (Screen.width / 2f) + x;
            float screenY = (Screen.height / 2f) - y;
            
            EDT.PacedLog("General", $"(screenX, screenY) = ({screenX},{screenY})");

            return new Vector2(screenX, screenY);
        }

        private static void OnMenuItemClicked(MenuItem item) {
            if (string.IsNullOrEmpty(item.buildable)) {
                currentCategory += $"/{item.name}";
            }
            else {
                StartBuilding(item);
            }

            EDT.Log("General", $"{item.name} clicked");
            sSinceClick = 0f;
        }

        private static void StartBuilding(MenuItem item) {
            ResourceInfo resource = ModUtils.GetResourceInfoByName(item.buildable);
            PlayerBuilder.EyeDropContent content = (PlayerBuilder.EyeDropContent)ModUtils.GetPrivateField("_currentEyeDropContent", Player.instance.builder);
            content.BuilderInfo = (BuilderInfo)resource;
            content.VariantIndex = 0;
            content.YawRotation = 0;
            ModUtils.SetPrivateField("_currentEyeDropContent", Player.instance.builder, content);
            Player.instance.builder.CursorUI.Set(resource, false);
            ModUtils.SetPrivateField("_currentEyeDropState", Player.instance.builder, PlayerBuilder.EEyeDropState.Building);
            CloseAndReset();
        }

        private static void GoBackOneLevel() {
            string[] parts = currentCategory.Split('/');
            parts = parts.RemoveAt(parts.Count() - 1);
            currentCategory = string.Join("/", parts);
        }
    }
}
