﻿//@vadym udod

using Fourzy._Updates.ClientModel;
using FourzyGameModel.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
            if (enumerable == null)
                return default(T);

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[UnityEngine.Random.Range(0, list.Count)];
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

        public static GameBoardData ToGameBoardData(this GameBoardDefinition definition)
        {
            GameBoardData _data = new GameBoardData();

            _data.Rows = definition.Rows;
            _data.Columns = definition.Columns;
            _data.Area = definition.Area;
            _data.BoardSpaceData = definition.BoardSpaceData;

            return _data;
        }

        public static int GetLocation(this BoardLocation boardLocation, IClientFourzy model)
        {
            if (boardLocation.Row == 0 || boardLocation.Row == model.Rows - 1)
                return boardLocation.Column;
            else if (boardLocation.Column == 0 || boardLocation.Column == model.Columns - 1)
                return boardLocation.Row;

            return -1;
        }

        public static Direction GetDirection(this BoardLocation boardLocation, IClientFourzy model)
        {
            if (boardLocation.Row == 0)
                return Direction.UP;
            else if (boardLocation.Row == model.Rows - 1)
                return Direction.DOWN;
            else if (boardLocation.Column == 0)
                return Direction.LEFT;
            else if (boardLocation.Column == model.Columns - 1)
                return Direction.RIGHT;

            return Direction.NONE;
        }

        public static BoardLocation Mirrored(this BoardLocation boardLocation, IClientFourzy model)
        {
            switch (boardLocation.GetDirection(model))
            {
                case Direction.UP:
                    return new BoardLocation(model.Rows - 1, boardLocation.Column);

                case Direction.DOWN:
                    return new BoardLocation(0, boardLocation.Column);

                case Direction.LEFT:
                    return new BoardLocation(boardLocation.Row, model.Columns - 1);

                case Direction.RIGHT:
                    return new BoardLocation(boardLocation.Row, 0);

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

        public static float MoveActionsDistance(this GameActionMove[] actions)
        {
            float distance = 0f;

            for (int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
                distance += actions[actionIndex].Distance;

            return distance;
        }

        public static float GetEloRatingDelta(int myRating, int opponentRating, float gameResult)
        {
            float myChanceToWin = 1 / (1 + Mathf.Pow(10, (opponentRating - myRating) / 400));

            return Mathf.Floor(60 * (gameResult - myChanceToWin));
        }

        public static string ToJson(this Color c)
        {
            return string.Format("{0},{1},{2},{3}", c.r, c.g, c.b, c.a);
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

        public static AnimationCurve CreateStraightCurve()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 1));
            curve.AddKey(new Keyframe(1, 1));
            return curve;
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