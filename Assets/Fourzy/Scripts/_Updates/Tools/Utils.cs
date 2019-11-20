//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Menu;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tools
{
    /// <summary>
    /// Utilieis class
    /// </summary>
    public static class Utils
    {
        private static System.Random rng = new System.Random();

        public static int ElementIndex<T>(this T[] array, T element)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Equals(element))
                    return i;

            return -1;
        }

        /// <summary>
        /// String to enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        //https://stackoverflow.com/questions/16100/how-should-i-convert-a-string-to-an-enum-in-c
        public static T ToEnum<T>(this string value) => (T)Enum.Parse(typeof(T), value, true);

        /// <summary>
        /// Angle to vector
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static float AngleTo(this Vector2 vec1, Vector2 vec2) => Mathf.DeltaAngle(Mathf.Atan2(vec1.y, vec1.x) * Mathf.Rad2Deg, Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg);

        public static Vector2 VectorFromAngle(this float angle) => new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));

        public static string RemoveLastChar(this string value) => value.Substring(0, value.Length - 2);

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) return default(T);

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T Next<T>(this IEnumerable<T> enumerable, T current)
        {
            if (enumerable == null) return default(T);

            var list = enumerable as IList<T> ?? enumerable.ToList();

            if (list.Count == 0)
                return default(T);
            else
            {
                if (list.IndexOf(current) == list.Count - 1)
                    return list[0];
                else
                    return list[list.IndexOf(current) + 1];
            }
        }

        public static IList<T> InsertList<T>(this IEnumerable<T> enumerable, int index, IEnumerable<T> toAdd)
        {
            if (enumerable == null) return null;

            var list = enumerable as IList<T> ?? enumerable.ToList();
            var toAddList = toAdd as IList<T> ?? toAdd.ToList();
            
            for (int _index = index; _index < index + toAddList.Count; _index++)
                list.Insert(_index, toAddList[_index - index]);

            return list;
        }

        /// <summary>
        /// Add element to array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] AddElementToEnd<T>(this T[] values, T value)
        {
            T[] result = new T[values.Length + 1];

            Array.Copy(values, result, values.Length);
            result[values.Length] = value;

            return result;
        }

        /// <summary>
        /// Add element to array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] AddElementToStart<T>(this T[] values, T value)
        {
            T[] result = new T[values.Length + 1];

            Array.Copy(values, 0, result, 1, values.Length);
            result[0] = value;

            return result;
        }

        public static GameActionMove InDirection(this GameActionMove actionMove, Direction direction, int distance = 1)
            => new GameActionMove(actionMove.Piece, actionMove.Start.Neighbor(direction, distance), actionMove.Start);

        public static GameActionMove AsMoveAction(this GameAction action) => action as GameActionMove;

        public static GameActionTokenMovement AsActionTokenMovement(this GameAction action) => action as GameActionTokenMovement;

        public static BoardLocation[] AsBoardLocations(this GameAction[] actions)
        {
            List<BoardLocation> result = new List<BoardLocation>();

            for (int index = 0; index < actions.Length; index++)
            {
                if (actions[index].GetType() == typeof(GameActionMove) && LastBoardLocationCheck(result, actions[index].AsMoveAction().Start)) result.Add(actions[index].AsMoveAction().Start);
                else if (actions[index].GetType() == typeof(GameActionTokenMovement) && LastBoardLocationCheck(result, actions[index].AsActionTokenMovement().Start)) result.Add(actions[index].AsActionTokenMovement().Start);

                if (index == actions.Length - 1)
                {
                    if (actions[index].GetType() == typeof(GameActionMove) && LastBoardLocationCheck(result, actions[index].AsMoveAction().End)) result.Add(actions[index].AsMoveAction().End);
                    else if (actions[index].GetType() == typeof(GameActionTokenMovement) && LastBoardLocationCheck(result, actions[index].AsActionTokenMovement().End)) result.Add(actions[index].AsActionTokenMovement().End);
                }
            }

            return result.ToArray();
        }

        public static GameBoardData ToGameBoardData(this GameBoardDefinition definition)
        {
            GameBoardData _data = new GameBoardData();

            _data.Rows = definition.Rows;
            _data.Columns = definition.Columns;
            _data.Area = definition.Area;
            _data.BoardSpaceData = definition.BoardSpaceData;

            return _data;
        }

        public static GameContentManager.PrefabType AsPrefabType(this RewardType currencyType)
        {
            switch (currencyType)
            {
                case RewardType.COINS:
                    return GameContentManager.PrefabType.REWARDS_COIN;

                case RewardType.TICKETS:
                    return GameContentManager.PrefabType.REWARDS_TICKET;

                case RewardType.GEMS:
                    return GameContentManager.PrefabType.REWARDS_GEM;

                case RewardType.GAME_PIECE:
                    return GameContentManager.PrefabType.REWARDS_GAME_PIECE;

                case RewardType.PORTAL_POINTS:
                    return GameContentManager.PrefabType.REWARDS_PORTAL_POINTS;

                case RewardType.RARE_PORTAL_POINTS:
                    return GameContentManager.PrefabType.REWARDS_RARE_PORTAL_POINTS;

                case RewardType.XP:
                    return GameContentManager.PrefabType.REWARDS_XP;

                case RewardType.PACK_COMPLETE:
                    return GameContentManager.PrefabType.REWARDS_PACK_COMPLETE;

                case RewardType.OPEN_PORTAL:
                    return GameContentManager.PrefabType.REWARDS_OPEN_PORTAL;

                case RewardType.OPEN_RARE_PORTAL:
                    return GameContentManager.PrefabType.REWARDS_OPEN_RARE_PORTAL;

                case RewardType.HINTS:
                    return GameContentManager.PrefabType.REWARDS_HINTS;

                default:
                    return GameContentManager.PrefabType.NONE;
            }
        }

        public static Direction GetDirectionFromLocations(this BoardLocation[] locations)
        {
            if (locations.Length < 2) return Direction.NONE;
            else
            {
                int y = locations[locations.Length - 1].Row - locations[locations.Length - 2].Row;
                int x = locations[locations.Length - 1].Column - locations[locations.Length - 2].Column;

                if (x > 0) return Direction.RIGHT;
                else if (x < 0) return Direction.LEFT;
                else if (y < 0) return Direction.UP;
                else if (y > 0) return Direction.DOWN;
            }

            return Direction.NONE;
        }

        public static int GetLocation(this BoardLocation boardLocation, IClientFourzy model)
        {
            if (boardLocation.Row == 0 || boardLocation.Row == model.Rows - 1)
                return boardLocation.Column;
            else if (boardLocation.Column == 0 || boardLocation.Column == model.Columns - 1)
                return boardLocation.Row;

            return -1;
        }

        public static int GetLocation(this BoardLocation boardLocation, Direction direction)
        {
            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    return boardLocation.Row;

                case Direction.UP:
                case Direction.DOWN:
                    return boardLocation.Column;
            }

            return -1;
        }

        public static Direction GetTurnDirection(this BoardLocation boardLocation, IClientFourzy model)
        {
            if (boardLocation.Row == 0)
                return Direction.DOWN;
            else if (boardLocation.Row == model.Rows - 1)
                return Direction.UP;
            else if (boardLocation.Column == 0)
                return Direction.RIGHT;
            else if (boardLocation.Column == model.Columns - 1)
                return Direction.LEFT;

            return Direction.NONE;
        }

        public static Vector2 GetVector(this Direction direction)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    return Vector2.left;

                case Direction.UP:
                    return Vector2.up;

                case Direction.DOWN:
                    return Vector2.down;

                default:
                    return Vector2.right;
            }
        }

        public static BoardLocation Mirrored(this BoardLocation boardLocation, IClientFourzy model)
        {
            switch (boardLocation.GetTurnDirection(model))
            {
                case Direction.UP:
                    return new BoardLocation(0, boardLocation.Column);

                case Direction.DOWN:
                    return new BoardLocation(model.Rows - 1, boardLocation.Column);

                case Direction.LEFT:
                    return new BoardLocation(boardLocation.Row, 0);

                case Direction.RIGHT:
                    return new BoardLocation(boardLocation.Row, model.Columns - 1);

                default:
                    return boardLocation;
            }
        }

        /// <summary>
        /// Removes last element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T[] RemoveElement<T>(this T[] values)
        {
            T[] result = new T[values.Length - 1];

            Array.Copy(values, result, result.Length);

            return result;
        }

        /// <summary>
        /// List shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static float Vector3ArrayLength(this Vector3[] set)
        {
            float length = 0;

            if (set == null)
                return length;

            for (int i = 1; i < set.Length; i++)
                length += (set[i] - set[i - 1]).magnitude;

            return length;
        }

        public static float Vector2ArrayLength(this Vector2[] set)
        {
            float length = 0;

            if (set == null)
                return length;

            for (int i = 1; i < set.Length; i++)
                length += (set[i] - set[i - 1]).magnitude;

            return length;
        }

        /// <summary>
        /// Get poin on array of Vector3
        /// </summary>
        /// <param name="set">Array</param>
        /// <param name="delta">Delta (0 - 1)</param>
        /// <returns></returns>
        public static Vector3 PointOnVector3Array(this Vector3[] set, float delta)
        {
            if (set == null)
                return Vector3.zero;

            float totalLength = set.Vector3ArrayLength();
            float lengthLeft = totalLength * delta;
            float deltaLeft = Mathf.Clamp01(delta);

            Vector3 result = set[0];

            for (int i = 1; i < set.Length; i++)
            {
                float pieceLength = (set[i] - set[i - 1]).magnitude;

                if (lengthLeft - pieceLength > 0f)
                {
                    lengthLeft -= pieceLength;
                    deltaLeft -= pieceLength / totalLength;
                }
                else
                    return Vector3.Lerp(set[i - 1], set[i], deltaLeft / (pieceLength / totalLength));
            }

            return result;
        }

        public static string SplitText(this string text, int charactersLimit)
        {
            string[] words = text.Split(' ');

            List<string> lines = new List<string>();
            lines.Add("");

            foreach (string word in words)
            {
                if (lines[lines.Count - 1].Length + word.Length + 1 >= charactersLimit)
                    lines.Add("");

                lines[lines.Count - 1] += word + " ";
            }

            return string.Join("\n", lines.ToArray());
        }

        public static string ToJson(this Color c)
        {
            return string.Format("{0},{1},{2},{3}", c.r, c.g, c.b, c.a);
        }

        public static string GetIDFromPuzzleDataFile(this ResourceItem puzzleData)
        {
            string text = puzzleData.Load<TextAsset>().text;
            int from = text.IndexOf('"', text.IndexOf("ID") + 4);
            int to = text.IndexOf('"', from + 1);

            return text.Substring(from + 1, to - from - 1);
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (System.Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static Color ColorFromJson(this string value)
        {
            Color color = new Color();
            var rgba = value.Split(',');
            if (rgba.Length == 4)
            {
                color.r = float.Parse(rgba[0]);
                color.g = float.Parse(rgba[1]);
                color.b = float.Parse(rgba[2]);
                color.a = float.Parse(rgba[3]);
            }
            return color;
        }

        public static void MoveItem<T>(this List<T> collection, int oldIndex, int newIndex)
        {
            T removedItem = collection[oldIndex];

            collection.RemoveAt(oldIndex);
            collection.Insert(newIndex, removedItem);
        }

        public static string GoalTypeToKey(this PuzzleGoalType goalType)
        {
            switch (goalType)
            {
                case PuzzleGoalType.WIN:
                    return "win_goal";

                case PuzzleGoalType.SURVIVE:
                    return "survive_goal";

                case PuzzleGoalType.COVER:
                    return "cover_goal";
            }

            return "";
        }

        public static AnalyticsManager.AnalyticsGameEvents GameModeToAnalyticsEvent(this GameMode mode, bool start)
        {
            switch (mode)
            {
                case GameMode.LOCAL_VERSUS:
                    return start ? AnalyticsManager.AnalyticsGameEvents.VERSUS_GAME_START : AnalyticsManager.AnalyticsGameEvents.VERSUS_GAME_END;

                case GameMode.PUZZLE_FAST:
                    return start ? AnalyticsManager.AnalyticsGameEvents.RANDOM_PUZZLE_START : AnalyticsManager.AnalyticsGameEvents.RANDOM_PUZZLE_END;

                case GameMode.GAUNTLET:
                    return start ? AnalyticsManager.AnalyticsGameEvents.GAUNTLET_LEVEL_START : AnalyticsManager.AnalyticsGameEvents.GAUNTLET_LEVEL_END;

                case GameMode.AI_PACK:
                    return start ? AnalyticsManager.AnalyticsGameEvents.AI_LEVEL_START : AnalyticsManager.AnalyticsGameEvents.AI_LEVEL_END;

                case GameMode.BOSS_AI_PACK:
                    return start ? AnalyticsManager.AnalyticsGameEvents.BOSS_AI_LEVEL_START : AnalyticsManager.AnalyticsGameEvents.BOSS_AI_LEVEL_END;

                case GameMode.PUZZLE_PACK:
                    return start ? AnalyticsManager.AnalyticsGameEvents.PUZZLE_LEVEL_START : AnalyticsManager.AnalyticsGameEvents.PUZZLE_LEVEL_END;
            }

            return AnalyticsManager.AnalyticsGameEvents.NONE;
        }

        public static Vector2 GetViewportPosition(this RectTransform target)
        {
            Canvas canvas = target.GetComponentInParent<Canvas>();
            MenuController menuController = target.GetComponentInParent<MenuController>();

            if (!canvas) return Vector2.zero;

            Vector2 _position = (Vector2)target.position + target.GetCenterOffset() * menuController.transform.localScale.x;

            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    return new Vector2(_position.x / menuController.size.x, _position.y / menuController.size.y);

                default:
                    return Camera.main.WorldToViewportPoint(_position);
            }
        }

        public static string TokenTypeToString(this TokenType type)
        {
            switch (type)
            {
                case TokenType.ARROW:
                    return TokenConstants.Arrow.ToString();

                case TokenType.BLOCKER:
                    return TokenConstants.Blocker.ToString();

                case TokenType.BOUNCE:
                    return TokenConstants.Bounce.ToString();

                case TokenType.CIRCLE_BOMB:
                    return TokenConstants.CircleBomb.ToString();

                case TokenType.COLD:
                    return TokenConstants.Cold.ToString();

                case TokenType.CROSS_BOMB:
                    return TokenConstants.CrossBomb.ToString();

                case TokenType.FIRE:
                    return TokenConstants.Fire.ToString();

                case TokenType.FLOWERS:
                    return TokenConstants.Flowers.ToString();

                case TokenType.FOUNTAIN:
                    return TokenConstants.Fountain.ToString();

                case TokenType.FOURWAY_ARROW:
                    return TokenConstants.FourWayArrow.ToString();

                case TokenType.FRUIT:
                    return TokenConstants.Fruit.ToString();

                case TokenType.FRUIT_TREE:
                    return TokenConstants.FruitTree.ToString();

                case TokenType.GHOST:
                    return TokenConstants.Ghost.ToString();

                case TokenType.GLASS_BLOCK:
                    return TokenConstants.GlassBlock.ToString();

                case TokenType.GOLD:
                    return TokenConstants.Gold.ToString();

                case TokenType.GRASS:
                    return TokenConstants.Grass.ToString();

                case TokenType.GUARD:
                    return TokenConstants.Guard.ToString();

                case TokenType.HEX:
                    return TokenConstants.Hex.ToString();

                case TokenType.ICE:
                    return TokenConstants.Ice.ToString();

                case TokenType.ICE_BLOCK:
                    return TokenConstants.IceBlock.ToString();

                case TokenType.LAVA:
                    return TokenConstants.Lava.ToString();

                case TokenType.LEFT_TURN:
                    return TokenConstants.LeftTurn.ToString();

                case TokenType.LURE:
                    return TokenConstants.Lure.ToString();

                case TokenType.MOVING_CLOUD:
                    return TokenConstants.MovingCloud.ToString();

                case TokenType.MOVING_GHOST:
                    return TokenConstants.MovingGhost.ToString();

                case TokenType.MOVING_SUN:
                    return TokenConstants.MovingSun.ToString();

                case TokenType.MOVE_BLOCKER:
                    return TokenConstants.MoveBlocker.ToString();

                case TokenType.MUD:
                    return TokenConstants.Mud.ToString();

                case TokenType.PIT:
                    return TokenConstants.Pit.ToString();

                case TokenType.PORTAL:
                    return TokenConstants.Portal.ToString();

                case TokenType.PUDDLE:
                    return TokenConstants.Puddle.ToString();

                case TokenType.QUICKSAND:
                    return TokenConstants.Quicksand.ToString();

                case TokenType.RANDOM_FOURWAY:
                    return TokenConstants.RandomFourWay.ToString();

                case TokenType.RANDOM_LEFTRIGHT:
                    return TokenConstants.RandomLeftRight.ToString();

                case TokenType.RANDOM_UPDOWN:
                    return TokenConstants.RandomUpDown.ToString();

                case TokenType.RIGHT_TURN:
                    return TokenConstants.RightTurn.ToString();

                case TokenType.ROTATING_ARROW:
                    return TokenConstants.RotatingArrow.ToString();

                case TokenType.SAND:
                    return TokenConstants.Sand.ToString();

                case TokenType.SHARK:
                    return TokenConstants.Shark.ToString();

                case TokenType.SNOW:
                    return TokenConstants.Snow.ToString();

                case TokenType.SNOWMAN:
                    return TokenConstants.Snowman.ToString();

                case TokenType.SPIDER:
                    return TokenConstants.Spider.ToString();

                case TokenType.STICKY:
                    return TokenConstants.Sticky.ToString();

                case TokenType.SWITCH:
                    return TokenConstants.Switch.ToString();

                case TokenType.TRAPDOOR:
                    return TokenConstants.TrapDoor.ToString();

                case TokenType.VOLCANO:
                    return TokenConstants.Volcano.ToString();

                case TokenType.WALL:
                    return TokenConstants.Wall.ToString();

                case TokenType.WATER:
                    return TokenConstants.Water.ToString();

                case TokenType.WEB:
                    return TokenConstants.SpiderWeb.ToString();

                case TokenType.WIND:
                    return TokenConstants.Wind.ToString();

                case TokenType.WISP:
                    return TokenConstants.Wisp.ToString();

                case TokenType.ZONE:
                    return TokenConstants.Zone.ToString();

                default:
                    return TokenConstants.Blocker.ToString();
            }
        }

        public static TokenType StringToToken(this string value)
        {
            switch (value[0])
            {
                case TokenConstants.Arrow:
                    return TokenType.ARROW;

                case TokenConstants.Blocker:
                    return TokenType.BLOCKER;

                case TokenConstants.Ghost:
                    return TokenType.GHOST;

                case TokenConstants.Sticky:
                    return TokenType.STICKY;

                case TokenConstants.Sand:
                    return TokenType.SAND;

                case TokenConstants.Snow:
                    return TokenType.SNOW;

                case TokenConstants.Grass:
                    return TokenType.GRASS;

                case TokenConstants.Water:
                    return TokenType.WATER;

                case TokenConstants.Puddle:
                    return TokenType.PUDDLE;

                case TokenConstants.Ice:
                    return TokenType.ICE;

                case TokenConstants.Flowers:
                    return TokenType.FLOWERS;

                case TokenConstants.Mud:
                    return TokenType.MUD;

                case TokenConstants.Lava:
                    return TokenType.LAVA;

                case TokenConstants.Volcano:
                    return TokenType.VOLCANO;

                case TokenConstants.Hex:
                    return TokenType.HEX;

                case TokenConstants.Pit:
                    return TokenType.PIT;

                case TokenConstants.Quicksand:
                    return TokenType.QUICKSAND;

                case TokenConstants.Switch:
                    return TokenType.SWITCH;

                case TokenConstants.TrapDoor:
                    return TokenType.TRAPDOOR;

                case TokenConstants.Cold:
                    return TokenType.COLD;

                case TokenConstants.Fire:
                    return TokenType.FIRE;

                case TokenConstants.Wind:
                    return TokenType.WIND;

                case TokenConstants.Zone:
                    return TokenType.ZONE;

                case TokenConstants.Bounce:
                    return TokenType.BOUNCE;

                case TokenConstants.Fountain:
                    return TokenType.FOUNTAIN;

                case TokenConstants.GlassBlock:
                    return TokenType.GLASS_BLOCK;

                case TokenConstants.FourWayArrow:
                    return TokenType.FOURWAY_ARROW;

                case TokenConstants.LeftTurn:
                    return TokenType.LEFT_TURN;

                case TokenConstants.RightTurn:
                    return TokenType.RIGHT_TURN;

                case TokenConstants.Wall:
                    return TokenType.WALL;

                case TokenConstants.Fruit:
                    return TokenType.FRUIT;

                case TokenConstants.FruitTree:
                    return TokenType.FRUIT_TREE;

                case TokenConstants.Guard:
                    return TokenType.GUARD;

                case TokenConstants.Shark:
                    return TokenType.SHARK;

                case TokenConstants.Snowman:
                    return TokenType.SNOWMAN;

                case TokenConstants.Spider:
                    return TokenType.SPIDER;

                case TokenConstants.Wisp:
                    return TokenType.WISP;

                case TokenConstants.MovingGhost:
                    return TokenType.MOVING_GHOST;

                case TokenConstants.MovingCloud:
                    return TokenType.MOVING_CLOUD;

                case TokenConstants.MovingSun:
                    return TokenType.MOVING_SUN;

                case TokenConstants.MoveBlocker:
                    return TokenType.MOVE_BLOCKER;

                case TokenConstants.SpiderWeb:
                    return TokenType.WEB;

                case TokenConstants.CircleBomb:
                    return TokenType.CIRCLE_BOMB;

                case TokenConstants.CrossBomb:
                    return TokenType.CROSS_BOMB;

                case TokenConstants.Gold:
                    return TokenType.GOLD;

                case TokenConstants.Lure:
                    return TokenType.LURE;

                case TokenConstants.Portal:
                    return TokenType.PORTAL;

                case TokenConstants.IceBlock:
                    return TokenType.ICE_BLOCK;

                case TokenConstants.RotatingArrow:
                    return TokenType.ROTATING_ARROW;

                case TokenConstants.RandomUpDown:
                    return TokenType.RANDOM_UPDOWN;

                case TokenConstants.RandomLeftRight:
                    return TokenType.RANDOM_LEFTRIGHT;

                case TokenConstants.RandomFourWay:
                    return TokenType.RANDOM_FOURWAY;

                default:
                    return TokenType.BLOCKER;
            }
        }

        public static bool IsSet<T>(T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

#if UNITY_EDITOR
        public static T[] GetAtPath<T>(string path)
        {
            ArrayList al = new ArrayList();
            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

            foreach (string fileName in fileEntries)
            {
                string temp = fileName.Replace("\\", "/");
                int index = temp.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                    localPath += temp.Substring(index);

                UnityEngine.Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

            T[] result = new T[al.Count];

            for (int i = 0; i < al.Count; i++)
                result[i] = (T)al[i];

            return result;
        }
#endif

        public static GameBoardDefinition LoadEmpty(this GameBoardDefinition board)
        {
            board.Rows = 8;
            board.Columns = 8;

            board.BoardSpaceData = new List<BoardSpaceData>();

            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                    board.BoardSpaceData.Add(new BoardSpaceData());

            return board;
        }

        public static Vector2 GetGridLayoutSize(this GridLayoutGroup grid, int childCount)
        {
            Vector2 result = new Vector2(grid.padding.left + grid.padding.right, grid.padding.top + grid.padding.bottom);

            //calculate rows
            int rows = 0;
            int columns = 0;
            switch (grid.constraint)
            {
                case GridLayoutGroup.Constraint.FixedColumnCount:
                    rows = Mathf.CeilToInt((float)childCount / grid.constraintCount);
                    columns = grid.constraintCount;

                    break;

                case GridLayoutGroup.Constraint.FixedRowCount:
                    rows = grid.constraintCount;
                    columns = Mathf.CeilToInt((float)childCount / grid.constraintCount);

                    break;
            }

            //add spacings + cell size
            if (columns > 0) result.x += (columns - 1) * grid.spacing.x;
            if (rows > 0) result.y += (rows - 1) * grid.spacing.y;

            //add cell size
            result.x += columns * grid.cellSize.x;
            result.y += rows * grid.cellSize.y;

            return result;
        }

        public static Vector2 GetGridLayoutSize(this GridLayoutGroup grid) => GetGridLayoutSize(grid, grid.transform.childCount);

        public static AnimationCurve CreateStraightCurve()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 1));
            curve.AddKey(new Keyframe(1, 1));
            return curve;
        }

        /// <summary>
        /// Not finished
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Vector2 GetSize(this RectTransform transform) => GetBoundaries(transform, transform, new Rect()).size;

        /// <summary>
        /// Not finished
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="root"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static Rect GetBoundaries(RectTransform obj, RectTransform root, Rect current)
        {
            if (!obj) return current;

            Vector3 position = root == obj ? Vector3.zero : GetAnchoredPosition(obj, root, Vector2.zero);

            float left = position.x - obj.pivot.x * obj.rect.width;
            float right = position.x + (1f - obj.pivot.x) * obj.rect.width;
            float top = position.y + (1f - obj.pivot.y) * obj.rect.height;
            float bottom = position.y - obj.pivot.y * obj.rect.height;

            if (left < current.xMin) current.xMin = left;
            if (right > current.xMax) current.xMax = right;
            if (top > current.yMax) current.yMax = top;
            if (bottom < current.yMin) current.yMin = bottom;

            Debug.Log(obj.name + " " + position + " " + left + " " + right);

            for (int childIndex = 0; childIndex < obj.childCount; childIndex++)
                current = GetBoundaries(obj.GetChild(childIndex).GetComponent<RectTransform>(), root, current);

            return current;
        }

        public static Vector3 GetAnchoredPosition(this RectTransform target, RectTransform root, Vector3 current)
        {
            if (target != root)
            {
                //current += target.anchoredPosition + GetAnchoredPosition(target.parent.GetComponent<RectTransform>(), root, current);
                current += target.localPosition + GetAnchoredPosition(target.parent.GetComponent<RectTransform>(), root, current);
                //Debug.Log(target.name + " " + target.localPosition);

            }

            return current;
        }

        public static Vector2 GetCenterOffset(this RectTransform target)
        {
            Vector2 _offset = Vector2.one * .5f - target.pivot;
            return new Vector2(target.rect.size.x * _offset.x, target.rect.size.y * _offset.y);
        }

        public static AnimationCurve CreateLinearCurve()
        {
            float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, tan45, tan45));
            curve.AddKey(new Keyframe(1, 1, tan45, tan45));
            return curve;
        }

        public static AnimationCurve CreateEaseInEaseOutCurve()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, 0, 0));
            curve.AddKey(new Keyframe(1, 1, 0, 0));
            return curve;
        }

        public static AnimationCurve CreateSteepCurve()
        {
            float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, 0, 0));
            curve.AddKey(new Keyframe(1, 1, tan45, tan45));
            return curve;
        }

        public static long EpochMilliseconds() => Convert.ToInt64((DateTime.Now).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);

        private static bool LastBoardLocationCheck(List<BoardLocation> actions, BoardLocation checkTo) => actions.Count == 0 || !actions[actions.Count - 1].Equals(checkTo);
    }

    public class StackModified<T>
    {
        private List<T> items = new List<T>();

        public int Count => items.Count;

        public void Push(T item) => items.Add(item);

        public T Pop()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return temp;
            }
            else
                return default(T);
        }

        public T Peek()
        {
            if (items.Count > 0)
                return items[items.Count - 1];
            else
                return default(T);
        }

        public void RemoveAt(int itemAtPosition) => items.RemoveAt(itemAtPosition);

        public void Remove(T element) => items.Remove(element);

        public bool Contains(T element) => items.Contains(element);
    }
}