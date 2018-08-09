using System;
using System.IO;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Commands;

namespace Server
{
    public class Teleporters
    {
        private static bool UseCsvFiles = true;
        private static bool UseCustomFiles = false;

        public static string[] m_Paths;
        public static Dictionary<Point4D, Teleport> m_TeleportersD1 = new Dictionary<Point4D, Teleport>();
        public static Dictionary<Point4D, Teleport> m_TeleportersD2 = new Dictionary<Point4D, Teleport>();

        private static string m_Path = Path.Combine("Data", "teleporters_Old.csv");
        private static char[] m_Sep = { ',' };

        public static void Initialize()
        {
            Console.WriteLine("Loading Fraz's Teleport System");
            LoadPaths();

        //    CommandSystem.Register("TeleMenu", AccessLevel.Administrator, new CommandEventHandler(TeleMenu_OnCommand)); // Next Version - Fraz
            CommandSystem.Register("TeleGlow", AccessLevel.Player, new CommandEventHandler(TeleGlow_OnCommand));

            EventSink.Movement += new MovementEventHandler(EventSink_Movement);
        //  EventSink.OnTeleport += new TeleportEventHandler(EventSink_Teleport); // This EventSink is not yet implemented in ServUO - Fraz
        }

        public static void EventSink_Movement(MovementEventArgs e)
        {
            Mobile from = e.Mobile;
            Direction d = e.Direction;

            if (IsTeleporting(from, d))
            {
                e.Blocked = true;
            }
        }

        /*
        public static void EventSink_Teleport(TeleportEventArgs e)
        {
            Mobile from = e.Mobile;
        }
        */

        private static void LoadPaths()
        {
            if (UseCsvFiles)
            {
                LoadCsvFiles();                
            }

            if (!UseCustomFiles) return;

            List<string> list = new List<string>();

            if (list != null)
            {
                string path = Path.Combine(Core.BaseDirectory, "Data/Teleporters/teleporters.cfg");
                if (File.Exists(path))
                {
                    using (StreamReader streamReader = new StreamReader(path))
                    {
                        string text;
                        while ((text = streamReader.ReadLine()) != null)
                        {
                            if (text.Length != 0 && !text.StartsWith("#"))
                            {
                                try
                                {
                                    string item = Path.Combine(Core.BaseDirectory, string.Format("Data/Teleporters/{0}.cfg", text.Trim()));
                                    list.Add(item);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    m_Paths = list.ToArray();
                }
            }
            LoadLocations();
        }

        private static void LoadLocations()
        {
            int err_count = 0;

            if (m_Paths == null) return;

            string[] paths = m_Paths;

            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == null) continue;

                string text = paths[i];
                if (File.Exists(text))
                {
                    using (StreamReader streamReader = new StreamReader(text))
                    {
                        string text2;
                        while ((text2 = streamReader.ReadLine()) != null)
                        {
                            if (text2.Length != 0 && !text2.StartsWith("#"))
                            {
                                try
                                {
                                    string[] array = text2.Split(new char[]
                                    {
                                        '|'
                                    });
                                    string[] array2 = text2.Split(new char[]
                                    {
                                        '|'
                                    });
                                    int x = Convert.ToInt32(array[0]);
                                    int y = Convert.ToInt32(array[1]);
                                    int z = Convert.ToInt32(array[2]);
                                    int index = Convert.ToInt32(array[3]);

                                    int x2 = Convert.ToInt32(array[4]);
                                    int y2 = Convert.ToInt32(array[5]);
                                    int z2 = Convert.ToInt32(array[6]);
                                    int index2 = Convert.ToInt32(array[7]);

                                    Map mapLoc = Map.AllMaps[index];

                                    Map mapDest = Map.AllMaps[index2];

                                    Point3D pointLoc = new Point3D(x, y, z);
                                    Point3D pointDest = new Point3D(x2, y2, z2);
                                    Teleport teleport = new Teleport(pointLoc, mapLoc, pointDest, mapDest);

                                    if (array2[8] == "true")
                                    {
                                        teleport.Active = true;
                                    }
                                    else
                                    {
                                        teleport.Active = false;
                                    }
                                    if (array2[9] == "true")
                                    {
                                        teleport.Creatures = true;
                                    }
                                    else
                                    {
                                        teleport.Creatures = false;
                                    }
                                    if (array2[10] == "true")
                                    {
                                        teleport.CombatCheck = true;
                                    }
                                    else
                                    {
                                        teleport.CombatCheck = false;
                                    }
                                    if (array2[11] == "true")
                                    {
                                        teleport.CriminalCheck = true;
                                    }
                                    else
                                    {
                                        teleport.CriminalCheck = false;
                                    }
                                    teleport.SourceEffect = Utility.ToInt32(array2[12]);
                                    teleport.DestEffect = Utility.ToInt32(array2[13]);
                                    teleport.SoundID = Utility.ToInt32(array[14]);
                                    teleport.Delay = TimeSpan.FromSeconds((double)Convert.ToInt32(array[15]));
                                    if (array2[16] == "true")
                                    {
                                        teleport.Oneway = true;
                                    }
                                    else
                                    {
                                        teleport.Oneway = false;
                                    }
                                    if (array2[17] == "true")
                                    {
                                        teleport.Bonded = true;
                                    }
                                    else
                                    {
                                        teleport.Bonded = false;
                                    }

                                    Point4D Loc = new Point4D(x, y, z, mapLoc.MapID);
                                    if (m_TeleportersD1.ContainsKey(Loc)) continue;
                                    m_TeleportersD1.Add(Loc, teleport);
                                    if (teleport.Oneway == false)
                                    {
                                        Teleport teleport2 = new Teleport(pointLoc, mapLoc, pointDest, mapDest);
                                        teleport2.Active = teleport.Active;
                                        teleport2.Bonded = teleport.Bonded;
                                        teleport2.CombatCheck = teleport.CombatCheck;
                                        teleport2.Creatures = teleport.Creatures;
                                        teleport2.CriminalCheck = teleport.CriminalCheck;
                                        teleport2.Delay = teleport.Delay;
                                        teleport2.DestEffect = teleport.DestEffect;
                                        teleport2.SoundID = teleport.SoundID;
                                        teleport2.SourceEffect = teleport.SourceEffect;
                                        teleport2.Oneway = teleport.Oneway;
                                        Point4D Dest = new Point4D(x2, y2, z2, mapDest.MapID);
                                        if (m_TeleportersD2.ContainsKey(Dest)) continue;
                                        m_TeleportersD2.Add(Dest, teleport2);
                                    }
                                }
                                catch
                                {
                                    err_count++;
                                    /*
                                    Console.Write("ERROR: Bad teleport location entry in ({0}) ", new object[]
									{
										text
									});
                                    Console.WriteLine(" ERROR COUNT: ({0})", err_count);
                                    */
                                }
                            }
                        }
                    }
                }
            }

        }

        /*
        [Usage("TeleMenu")]
        [Description("Opens the Fraz Teleporters Menu.")]
        public static void TeleMenu_OnCommand(CommandEventArgs e)
        {


        }
        */

        [Usage("TeleGlow")]
        [Description("Glows any teleporters in visual range.")]
        public static void TeleGlow_OnCommand(CommandEventArgs e)
        {    
            Mobile m = e.Mobile;
            m.SendMessage("Teleporters in your area are now highlighted.");

            foreach (KeyValuePair<Point4D, Teleport> tele in m_TeleportersD1)
            {
                if (m.Map.MapIndex != tele.Key.M) continue;
                Point3D loc = new Point3D(tele.Key.X, tele.Key.Y, tele.Key.Z + 1);

                if (m.InRange(loc, Core.GlobalUpdateRange))
                {
                    if (!tele.Value.Active) Effects.SendLocationEffect(loc, Map.Maps[tele.Key.M], 1173, 120, 0, 0x26, 3);
                    else Effects.SendLocationEffect(loc, Map.Maps[tele.Key.M], 1173, 75, 1, 1151, 3);
                }
            }
            foreach (KeyValuePair<Point4D, Teleport> tele2 in m_TeleportersD2)
            {
                if (m.Map.MapIndex != tele2.Key.M) continue;
                Point3D loc2 = new Point3D(tele2.Key.X, tele2.Key.Y, tele2.Key.Z + 1);

                if (m.InRange(loc2, Core.GlobalUpdateRange))
                {
                    if (!tele2.Value.Active) Effects.SendLocationEffect(loc2, Map.Maps[tele2.Key.M], 1173, 120, 0, 0x26, 3);
                    else Effects.SendLocationEffect(loc2, Map.Maps[tele2.Key.M], 1173, 0, 0, 1151, 3);
                }
            }

        }

        private static void LoadCsvFiles()
        {
            StreamReader reader = new StreamReader(m_Path);

            string line;
            int lineNum = 0;
            while ((line = reader.ReadLine()) != null)
            {
                ++lineNum;
                line = line.Trim();
                if (line.StartsWith("#"))
                    continue;
                string[] parts = line.Split(m_Sep);
                if (parts.Length != 9)
                {
                    continue;
                }
                try
                {
                    Point3D pointLoc = new Point3D(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                    Map mapLoc = Map.Parse(parts[3]);
 
                    Point3D pointDest = new Point3D(int.Parse(parts[4]), int.Parse(parts[5]), int.Parse(parts[6]));
                    Map mapDest = Map.Parse(parts[7]);

                    Teleport teleport = new Teleport(pointLoc, mapLoc, pointDest, mapDest);
                    if (bool.Parse(parts[8])) teleport.Oneway = false;

                    Point4D Loc = new Point4D(pointLoc.X, pointLoc.Y, pointLoc.Z, mapLoc.MapID);
                    if (m_TeleportersD1.ContainsKey(Loc)) continue;
                    m_TeleportersD1.Add(Loc, teleport);

                    if (teleport.Oneway == false)
                    {
                        Teleport teleport2 = new Teleport(pointLoc, mapLoc, pointDest, mapDest);
                        teleport2.Oneway = teleport.Oneway;
                        Point4D Dest = new Point4D(pointDest.X, pointDest.Y, pointDest.Z, mapDest.MapID);
                        if (m_TeleportersD2.ContainsKey(Dest)) continue;
                        m_TeleportersD2.Add(Dest, teleport2);
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine(string.Format("Bad number format on line {0}", lineNum));
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(string.Format("Argument Execption {0} on line {1}", ex.Message, lineNum));
                }
            }
            reader.Close();
        }

        public static bool IsTeleporting(Mobile m, Direction d)
        {
            int newZ;
            Point3D oldLocation = m.Location;
            int x = oldLocation.X, y = oldLocation.Y;
            int oldX = x, oldY = y;
            int oldZ = oldLocation.Z;

            if (m.CheckMovement(d, out newZ))
            {
                switch (d & Direction.Mask)
                {
                    case Direction.North:
                        --y;
                        break;
                    case Direction.Right:
                        ++x;
                        --y;
                        break;
                    case Direction.East:
                        ++x;
                        break;
                    case Direction.Down:
                        ++x;
                        ++y;
                        break;
                    case Direction.South:
                        ++y;
                        break;
                    case Direction.Left:
                        --x;
                        ++y;
                        break;
                    case Direction.West:
                        --x;
                        break;
                    case Direction.Up:
                        --x;
                        --y;
                        break;
                }
            }

            Point4D Telespot = new Point4D(x, y, newZ, m.Map.MapID);

            Teleport Teleport1;
            if (m_TeleportersD1.TryGetValue(Telespot, out Teleport1))
            {
                if (Teleport1.OnMoveOver(m))
                {
                    return true;
                }
            }
            else
            {
                Teleport Teleport2;
                if (m_TeleportersD2.TryGetValue(Telespot, out Teleport2))
                {
                    if (Teleport2.OnMoveOverReturn(m))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class Teleport
    {
        private bool m_Active;
        private bool m_Creatures;
        private bool m_CombatCheck;
        private bool m_CriminalCheck;
        private bool m_Oneway;
        private bool m_Bonded;
        private Point3D m_PointLoc;
        private Map m_MapLoc;
        private Point3D m_PointDest;
        private Map m_MapDest;
        private int m_SourceEffect;
        private int m_DestEffect;
        private int m_SoundID;
        private TimeSpan m_Delay;
        private static readonly TimeSpan CombatHeatDelay = TimeSpan.FromSeconds(30.0);

        public int SourceEffect
        {
            get
            {
                return m_SourceEffect;
            }
            set
            {
                m_SourceEffect = value;
            }
        }
        public int DestEffect
        {
            get
            {
                return m_DestEffect;
            }
            set
            {
                m_DestEffect = value;
            }
        }
        public int SoundID
        {
            get
            {
                return m_SoundID;
            }
            set
            {
                m_SoundID = value;
            }
        }
        public TimeSpan Delay
        {
            get
            {
                return m_Delay;
            }
            set
            {
                m_Delay = value;
            }
        }
        public bool Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                m_Active = value;
            }
        }
        public Point3D PointLoc
        {
            get
            {
                return m_PointLoc;
            }
            set
            {
                m_PointLoc = value;
            }
        }
        public Map MapLoc
        {
            get
            {
                return m_MapLoc;
            }
            set
            {
                m_MapLoc = value;
            }
        }
        public Point3D PointDest
        {
            get
            {
                return m_PointDest;
            }
            set
            {
                m_PointDest = value;
            }
        }
        public Map MapDest
        {
            get
            {
                return m_MapDest;
            }
            set
            {
                m_MapDest = value;
            }
        }
        public bool Creatures
        {
            get
            {
                return m_Creatures;
            }
            set
            {
                m_Creatures = value;
            }
        }
        public bool CombatCheck
        {
            get
            {
                return m_CombatCheck;
            }
            set
            {
                m_CombatCheck = value;
            }
        }
        public bool CriminalCheck
        {
            get
            {
                return m_CriminalCheck;
            }
            set
            {
                m_CriminalCheck = value;
            }
        }
        public bool Oneway
        {
            get
            {
                return m_Oneway;
            }
            set
            {
                m_Oneway = value;
            }
        }
        public bool Bonded
        {
            get
            {
                return m_Bonded;
            }
            set
            {
                m_Bonded = value;
            }
        }

        public Teleport(Point3D pointLoc, Map mapLoc, Point3D pointDest, Map mapDest)
        {
            m_Active = true;
            m_PointLoc = pointLoc;
            m_MapLoc = mapLoc;
            m_PointDest = pointDest;
            m_MapDest = mapDest;
            m_Creatures = false;
            m_CombatCheck = false;
            m_CriminalCheck = false;
            m_Oneway = false;
            m_Bonded = false;
        }

        public static bool CheckCombat(Mobile m)
        {
            for (int i = 0; i < m.Aggressed.Count; i++)
            {
                AggressorInfo aggressorInfo = m.Aggressed[i];
                if (aggressorInfo.Defender.Player && DateTime.UtcNow - aggressorInfo.LastCombatTime < CombatHeatDelay)
                {
                    return true;
                }
            }
            if (Core.Expansion == Expansion.AOS)
            {
                for (int j = 0; j < m.Aggressors.Count; j++)
                {
                    AggressorInfo aggressorInfo2 = m.Aggressors[j];
                    if (aggressorInfo2.Attacker.Player && DateTime.UtcNow - aggressorInfo2.LastCombatTime < CombatHeatDelay)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool CanTeleport(Mobile m)
        {
            if (!m_Creatures && !m.Player)
            {
                return false;
            }
            if (m_CriminalCheck && m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 34);
                return false;
            }
            if (m_CombatCheck && CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 34);
                return false;
            }
            return true;
        }

        public virtual void DoTeleport(Mobile m)
        {
            Map map = m_MapLoc;
            if (map == null || map == Map.Internal)
            {
                map = m.Map;
            }
            Map map2 = m_MapDest;
            if (map2 == null || map2 == Map.Internal)
            {
                map2 = m.Map;
            }
            Point3D point3D = m_PointDest;

            if (point3D == Point3D.Zero)
            {
                point3D = m.Location;
            }

            TeleportPets(m, point3D, map2, m_Bonded);
            bool flag = !m.Hidden || m.IsPlayer();
            if (SourceEffect > 0 && flag)
            {
                Effects.SendLocationEffect(m.Location, map, SourceEffect, 35);
            }
            if (SoundID > 0 && flag)
            {
                Effects.PlaySound(point3D, map, SoundID);
            }
            m.MoveToWorld(point3D, map2);
            if (DestEffect > 0 && flag)
            {
                Effects.SendLocationEffect(m.Location, m.Map, DestEffect, 35);
            }
            if (SoundID > 0 && flag)
            {
                Effects.PlaySound(m.Location, m.Map, SoundID);
            }
        }

        public virtual void DoTeleportReturn(Mobile m)
        {
            Map map = m_MapDest;
            if (map == null || map == Map.Internal)
            {
                map = m.Map;
            }
            Map map2 = m_MapLoc;
            if (map2 == null || map2 == Map.Internal)
            {
                map2 = m.Map;
            }
            Point3D point3D = m_PointLoc;
            if (point3D == Point3D.Zero)
            {
                point3D = m.Location;
            }

            TeleportPets(m, point3D, map2, m_Bonded);
            bool flag = !m.Hidden || m.IsPlayer();
            if (SourceEffect > 0 && flag)
            {
                Effects.SendLocationEffect(m.Location, m.Map, SourceEffect, 35);
            }
            if (SoundID > 0 && flag)
            {
                Effects.PlaySound(m.Location, m.Map, SoundID);
            }

            m.MoveToWorld(point3D, map2);

            if (DestEffect > 0 && flag)
            {
                Effects.SendLocationEffect(m.Location, m.Map, DestEffect, 35);
            }
            if (SoundID > 0 && flag)
            {
                Effects.PlaySound(m.Location, m.Map, SoundID);
            }
        }

        public bool OnMoveOver(Mobile m)
        {
            if (m.Holding != null)
            {
                m.SendLocalizedMessage(1071955);
                return false;
            }
            if (!m_Active || !CanTeleport(m))
            {
                return false;
            }
            if (m_Delay == TimeSpan.Zero)
            {
                DoTeleport(m);
            }
            else
            {
                Timer.DelayCall(m_Delay, new TimerStateCallback<Mobile>(DoTeleport), m);
            }
            return true;
        }

        public bool OnMoveOverReturn(Mobile m)
        {
            if (m_Oneway)
            {
                return false;
            }
            if (m.Holding != null)
            {
                m.SendLocalizedMessage(1071955);
                return false;
            }
            if (!m_Active || !CanTeleport(m))
            {
                return false;
            }
            if (m_Delay == TimeSpan.Zero)
            {
                DoTeleportReturn(m);
            }
            else
            {
                Timer.DelayCall(m_Delay, new TimerStateCallback<Mobile>(DoTeleport), m);
            }
            return true;
        }

        public static void TeleportPets(Mobile master, Point3D loc, Map map, bool onlyBonded)
        {
            List<Mobile> list = new List<Mobile>();
            foreach (Mobile current in master.GetMobilesInRange(3))
            {
                if (current is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)current;
                    if ((!onlyBonded || pet.IsBonded) && pet.Controlled && pet.ControlMaster == master && IsControlOrderWithMe(pet))
                    {
                        list.Add(current);
                    }
                }
            }
            foreach (Mobile current2 in list)
            {
                current2.MoveToWorld(loc, map);
            }
        }

        private static bool IsControlOrderWithMe(BaseCreature pet)
        {
            return (pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come);
        }

    }
}
