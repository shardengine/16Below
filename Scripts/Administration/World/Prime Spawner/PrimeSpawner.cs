using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Server;
using Server.Commands;
using Server.Items;
using Server.Network;
using CPA = Server.CommandPropertyAttribute;

namespace Server.Mobiles
{
    public class PrimeSpawner : Item
    {
        private int I_Team;
        private int I_HomeRange; // = old SpawnRange
        private int I_WalkingRange = -1; // = old HomeRange
        private int I_SpawnID = 1;
        private int spawnMax;
        private int spawnMaxA;
        private int spawnMaxB;
        private int spawnMaxC;
        private int spawnMaxD;
        private int spawnMaxE;
        private TimeSpan I_MinDelay;
        private TimeSpan I_MaxDelay;
        private List<string> I_CreaturesName; // creatures to be spawned
        private List<IEntity> I_Creatures; // spawned creatures
        private List<string> I_CreaturesNameA;
        private List<IEntity> I_CreaturesA;
        private List<string> I_CreaturesNameB;
        private List<IEntity> I_CreaturesB;
        private List<string> I_CreaturesNameC;
        private List<IEntity> I_CreaturesC;
        private List<string> I_CreaturesNameD;
        private List<IEntity> I_CreaturesD;
        private List<string> I_CreaturesNameE;
        private List<IEntity> I_CreaturesE;
        private DateTime I_End;
        private InternalTimer I_Timer;
        private bool I_Running;
        private bool I_Water;
        private bool I_Group;
        private WayPoint I_WayPoint;

        public bool IsFull { get { return (I_Creatures != null && I_Creatures.Count >= spawnMax); } }
        public bool IsFulla { get { return (I_CreaturesA != null && I_CreaturesA.Count >= spawnMaxA); } }
        public bool IsFullb { get { return (I_CreaturesB != null && I_CreaturesB.Count >= spawnMaxB); } }
        public bool IsFullc { get { return (I_CreaturesC != null && I_CreaturesC.Count >= spawnMaxC); } }
        public bool IsFulld { get { return (I_CreaturesD != null && I_CreaturesD.Count >= spawnMaxD); } }
        public bool IsFulle { get { return (I_CreaturesE != null && I_CreaturesE.Count >= spawnMaxE); } }

        public List<string> CreaturesName
        {
            get { return I_CreaturesName; }
            set
            {
                I_CreaturesName = value;
                if (I_CreaturesName.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<string> SubSpawnerA
        {
            get { return I_CreaturesNameA; }
            set
            {
                I_CreaturesNameA = value;
                if (I_CreaturesNameA.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<string> SubSpawnerB
        {
            get { return I_CreaturesNameB; }
            set
            {
                I_CreaturesNameB = value;
                if (I_CreaturesNameB.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<string> SubSpawnerC
        {
            get { return I_CreaturesNameC; }
            set
            {
                I_CreaturesNameC = value;
                if (I_CreaturesNameC.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<string> SubSpawnerD
        {
            get { return I_CreaturesNameD; }
            set
            {
                I_CreaturesNameD = value;
                if (I_CreaturesNameD.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<string> SubSpawnerE
        {
            get { return I_CreaturesNameE; }
            set
            {
                I_CreaturesNameE = value;
                if (I_CreaturesNameE.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<IEntity> Creatures
        {
            get { return I_Creatures; }
            set
            {
                I_Creatures = value;
                if (I_Creatures.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<IEntity> CreaturesA
        {
            get { return I_CreaturesA; }
            set
            {
                I_CreaturesA = value;
                if (I_CreaturesA.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<IEntity> CreaturesB
        {
            get { return I_CreaturesB; }
            set
            {
                I_CreaturesB = value;
                if (I_CreaturesB.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<IEntity> CreaturesC
        {
            get { return I_CreaturesC; }
            set
            {
                I_CreaturesC = value;
                if (I_CreaturesC.Count < 1)
                   Stop();
                InvalidateProperties();
            }
        }

        public List<IEntity> CreaturesD
        {
            get { return I_CreaturesD; }
            set
            {
                I_CreaturesD = value;
                if (I_CreaturesD.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public List<IEntity> CreaturesE
        {
            get { return I_CreaturesE; }
            set
            {
                I_CreaturesE = value;
                if (I_CreaturesE.Count < 1)
                    Stop();
                InvalidateProperties();
            }
        }

        public virtual int CreaturesNameCount { get { return I_CreaturesName.Count; } }
        public virtual int CreaturesNameCountA { get { return I_CreaturesNameA.Count; } }
        public virtual int CreaturesNameCountB { get { return I_CreaturesNameB.Count; } }
        public virtual int CreaturesNameCountC { get { return I_CreaturesNameC.Count; } }
        public virtual int CreaturesNameCountD { get { return I_CreaturesNameD.Count; } }
        public virtual int CreaturesNameCountE { get { return I_CreaturesNameE.Count; } }

        public override void OnAfterDuped(Item newItem)
        {
            PrimeSpawner s = newItem as PrimeSpawner;

            if (s == null)
                return;

            s.I_CreaturesName = new List<string>(I_CreaturesName);
            s.I_CreaturesNameA = new List<string>(I_CreaturesNameA);
            s.I_CreaturesNameB = new List<string>(I_CreaturesNameB);
            s.I_CreaturesNameC = new List<string>(I_CreaturesNameC);
            s.I_CreaturesNameD = new List<string>(I_CreaturesNameD);
            s.I_CreaturesNameE = new List<string>(I_CreaturesNameE);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnMax
        {
            get { return spawnMax; }
            set { spawnMax = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnMaxA
        {
            get { return spawnMaxA; }
            set { spawnMaxA = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnMaxB
        {
            get { return spawnMaxB; }
            set { spawnMaxB = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnMaxC
        {
            get { return spawnMaxC; }
            set { spawnMaxC = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnMaxD
        {
            get { return spawnMaxD; }
            set { spawnMaxD = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnMaxE
        {
            get { return spawnMaxE; }
            set { spawnMaxE = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WayPoint WayPoint
        {
            get
            {
                return I_WayPoint;
            }
            set
            {
                I_WayPoint = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Running
        {
            get { return I_Running; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeRange
        {
            get { return I_HomeRange; }
            set { I_HomeRange = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WalkingRange
        {
            get { return I_WalkingRange; }
            set { I_WalkingRange = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnID
        {
            get { return I_SpawnID; }
            set { I_SpawnID = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Team
        {
            get { return I_Team; }
            set { I_Team = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan MinDelay
        {
            get { return I_MinDelay; }
            set { I_MinDelay = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan MaxDelay
        {
            get { return I_MaxDelay; }
            set { I_MaxDelay = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextSpawn
        {
            get
            {
                if (I_Running)
                    return I_End - DateTime.UtcNow;
                else
                    return TimeSpan.FromSeconds(0);
            }
            set
            {
                Start();
                DoTimer(value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Group
        {
            get { return I_Group; }
            set { I_Group = value; InvalidateProperties(); }
        }

        [Constructable]
        public PrimeSpawner(int amount, int subamountA, int subamountB, int subamountC, int subamountD, int subamountE, int spawnid, int minDelay, int maxDelay, int team, int homeRange, int walkingRange, string creatureName, string creatureNameA, string creatureNameB, string creatureNameC, string creatureNameD, string creatureNameE) : base(0x1f13)
        {
            List<string> creaturesName = new List<string>();
            creaturesName.Add(creatureName);

            List<string> creatureNameAA = new List<string>();
            creaturesName.Add(creatureNameA);

            List<string> creatureNameBB = new List<string>();
            creaturesName.Add(creatureNameB);

            List<string> creatureNameCC = new List<string>();
            creaturesName.Add(creatureNameC);

            List<string> creatureNameDD = new List<string>();
            creaturesName.Add(creatureNameD);

            List<string> creatureNameEE = new List<string>();
            creaturesName.Add(creatureNameE);

            InitSpawn(amount, subamountA, subamountB, subamountC, subamountD, subamountE, spawnid, TimeSpan.FromMinutes(minDelay), TimeSpan.FromMinutes(maxDelay), team, homeRange, walkingRange, creaturesName, creatureNameAA, creatureNameBB, creatureNameCC, creatureNameDD, creatureNameEE);
        }

        [Constructable]
        public PrimeSpawner(string creatureName) : base(0x1f13)
        {
            List<string> creaturesName = new List<string>();
            creaturesName.Add(creatureName);

            List<string> creatureNameAA = new List<string>();
            List<string> creatureNameBB = new List<string>();
            List<string> creatureNameCC = new List<string>();
            List<string> creatureNameDD = new List<string>();
            List<string> creatureNameEE = new List<string>();

            InitSpawn(1, 0, 0, 0, 0, 0, 1, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), 0, 4, -1, creaturesName, creatureNameAA, creatureNameBB, creatureNameCC, creatureNameDD, creatureNameEE);
        }

        [Constructable]
        public PrimeSpawner() : base(0x1f13)
        {
            List<string> creaturesName = new List<string>();
            List<string> creatureNameAA = new List<string>();
            List<string> creatureNameBB = new List<string>();
            List<string> creatureNameCC = new List<string>();
            List<string> creatureNameDD = new List<string>();
            List<string> creatureNameEE = new List<string>();

            InitSpawn(1, 0, 0, 0, 0, 0, 1, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), 0, 4, -1, creaturesName, creatureNameAA, creatureNameBB, creatureNameCC, creatureNameDD, creatureNameEE);
        }

        public PrimeSpawner(int amount, int subamountA, int subamountB, int subamountC, int subamountD, int subamountE, int spawnid, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, int walkingRange, List<string> creaturesName, List<string> creatureNameAA, List<string> creatureNameBB, List<string> creatureNameCC, List<string> creatureNameDD, List<string> creatureNameEE) : base(0x1f13)
        {
            InitSpawn(amount, subamountA, subamountB, subamountC, subamountD, subamountE, spawnid, minDelay, maxDelay, team, homeRange, walkingRange, creaturesName, creatureNameAA, creatureNameBB, creatureNameCC, creatureNameDD, creatureNameEE);
        }

        public override string DefaultName
        {
            get { return "Premium Spawner"; }
        }

        public void InitSpawn(int amount, int subamountA, int subamountB, int subamountC, int subamountD, int subamountE, int SpawnID, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, int walkingRange, List<string> creaturesName, List<string> creatureNameAA, List<string> creatureNameBB, List<string> creatureNameCC, List<string> creatureNameDD, List<string> creatureNameEE)
        {
            Name = "Premium Spawner";
            I_SpawnID = SpawnID;
            Visible = false;
            Movable = false;
            I_Running = true;
            I_Water = false;
            I_Group = false;
            I_MinDelay = minDelay;
            I_MaxDelay = maxDelay;
            spawnMax = amount;
            spawnMaxA = subamountA;
            spawnMaxB = subamountB;
            spawnMaxC = subamountC;
            spawnMaxD = subamountD;
            spawnMaxE = subamountE;
            I_Team = team;
            I_HomeRange = homeRange;
            I_WalkingRange = walkingRange;
            I_CreaturesName = creaturesName;
            I_CreaturesNameA = creatureNameAA;
            I_CreaturesNameB = creatureNameBB;
            I_CreaturesNameC = creatureNameCC;
            I_CreaturesNameD = creatureNameDD;
            I_CreaturesNameE = creatureNameEE;
            I_Creatures = new List<IEntity>();
            I_CreaturesA = new List<IEntity>();
            I_CreaturesB = new List<IEntity>();
            I_CreaturesC = new List<IEntity>();
            I_CreaturesD = new List<IEntity>();
            I_CreaturesE = new List<IEntity>();
            DoTimer(TimeSpan.FromSeconds(1));
        }

        public PrimeSpawner(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.GameMaster)
                return;

            PrimeSpawnerGump g = new PrimeSpawnerGump(this);
            from.CloseGump(typeof(PrimeSpawnerGump));
            from.SendGump(g);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (I_Running)
            {
                list.Add(1060742); // active
                list.Add(1060656, spawnMax.ToString());
                list.Add(1061169, I_HomeRange.ToString());
                list.Add(1060658, "walking range\t{0}", I_WalkingRange);
                list.Add(1060663, "SpawnID\t{0}", I_SpawnID.ToString());
                list.Add(1060661, "speed\t{0} to {1}", I_MinDelay, I_MaxDelay);

                for (int i = 0; i < 2 && i < I_CreaturesName.Count; ++i)
                    list.Add(1060662 + i, "{0}\t{1}", I_CreaturesName[i], CountCreatures(Creatures, I_CreaturesName[i]));
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (I_Running)
                LabelTo(from, "[Running]");
            else
                LabelTo(from, "[Off]");
        }

        public void SpawnWater()
        {
        }

        public void Start()
        {
            if (!I_Running)
            {
                if (I_CreaturesName.Count > 0 || I_CreaturesNameA.Count > 0 || I_CreaturesNameB.Count > 0 || I_CreaturesNameC.Count > 0 || I_CreaturesNameD.Count > 0 || I_CreaturesNameE.Count > 0)
                {
                    I_Running = true;
                    DoTimer();
                }
            }
        }

        public void Stop()
        {

            if (I_Running)
            {

                I_Timer.Stop();

                I_Running = false;

                RemoveCreatures(I_Creatures);

                RemoveCreatures(I_CreaturesA);

                RemoveCreatures(I_CreaturesB);

                RemoveCreatures(I_CreaturesC);

                RemoveCreatures(I_CreaturesD);

                RemoveCreatures(I_CreaturesE);

            }

        }



        public static string ParseType(string s)
        {

            return s.Split(null, 2)[0];

        }



        public void Defrag(List<IEntity> I_Beings)
        {

            bool removed = false;



            for (int i = 0; i < I_Beings.Count; ++i)
            {

                IEntity e = I_Beings[i];



                if (e is Item)
                {

                    Item item = (Item)e;



                    if (item.Deleted || item.Parent != null)
                    {

                        I_Beings.RemoveAt(i);

                        --i;

                        removed = true;

                    }

                }

                else if (e is Mobile)
                {

                    Mobile m = (Mobile)e;



                    if (m.Deleted)
                    {

                        I_Beings.RemoveAt(i);

                        --i;

                        removed = true;

                    }

                    else if (m is BaseCreature)
                    {

                        BaseCreature bc = (BaseCreature)m;

                        if (bc.Controlled || bc.IsStabled)
                        {

                            I_Beings.RemoveAt(i);

                            --i;

                            removed = true;

                        }

                    }

                }

                else
                {

                    I_Beings.RemoveAt(i);

                    --i;

                    removed = true;

                }

            }



            if (removed)

                InvalidateProperties();

        }



        public void OnTick()
        {

            DoTimer();



            if (I_Group)
            {

                Defrag(I_Creatures);

                Defrag(I_CreaturesA);

                Defrag(I_CreaturesB);

                Defrag(I_CreaturesC);

                Defrag(I_CreaturesD);

                Defrag(I_CreaturesE);



                if (I_Creatures.Count == 0 || I_CreaturesA.Count == 0 || I_CreaturesB.Count == 0 || I_CreaturesC.Count == 0 || I_CreaturesD.Count == 0 || I_CreaturesE.Count == 0)
                {

                    Respawn();

                }

                else
                {

                    return;

                }

            }

            else
            {

                Spawn(CreaturesNameCount, I_Creatures, spawnMax, I_CreaturesName);

                Spawn(CreaturesNameCountA, I_CreaturesA, spawnMaxA, I_CreaturesNameA);

                Spawn(CreaturesNameCountB, I_CreaturesB, spawnMaxB, I_CreaturesNameB);

                Spawn(CreaturesNameCountC, I_CreaturesC, spawnMaxC, I_CreaturesNameC);

                Spawn(CreaturesNameCountD, I_CreaturesD, spawnMaxD, I_CreaturesNameD);

                Spawn(CreaturesNameCountE, I_CreaturesE, spawnMaxE, I_CreaturesNameE);

            }

        }



        public void Respawn() // remove all creatures and spawn all again
        {

            RemoveCreatures(I_Creatures);

            RemoveCreatures(I_CreaturesA);

            RemoveCreatures(I_CreaturesB);

            RemoveCreatures(I_CreaturesC);

            RemoveCreatures(I_CreaturesD);

            RemoveCreatures(I_CreaturesE);



            for (int i = 0; i < spawnMax; i++)

                Spawn(CreaturesNameCount, I_Creatures, spawnMax, I_CreaturesName);

            for (int i = 0; i < spawnMaxA; i++)

                Spawn(CreaturesNameCountA, I_CreaturesA, spawnMaxA, I_CreaturesNameA);

            for (int i = 0; i < spawnMaxB; i++)

                Spawn(CreaturesNameCountB, I_CreaturesB, spawnMaxB, I_CreaturesNameB);

            for (int i = 0; i < spawnMaxC; i++)

                Spawn(CreaturesNameCountC, I_CreaturesC, spawnMaxC, I_CreaturesNameC);

            for (int i = 0; i < spawnMaxD; i++)

                Spawn(CreaturesNameCountD, I_CreaturesD, spawnMaxD, I_CreaturesNameD);

            for (int i = 0; i < spawnMaxE; i++)

                Spawn(CreaturesNameCountE, I_CreaturesE, spawnMaxE, I_CreaturesNameE);

        }



        public void Spawn(int CreatNameCount, List<IEntity> I_Creat, int I_Countt, List<string> I_CreatName)
        {

            if (CreatNameCount > 0)

                SpawnTwo(Utility.Random(CreatNameCount), CreatNameCount, I_Creat, I_Countt, I_CreatName);



        }



        // Used only by PrimeSpawnerGump(line 415-45)

        // BROKEN

        public void SpawnFromGump(List<string> I_subspawnName, List<IEntity> I_subspawn, int subCount, int subNameCount, string creatureName)
        {

            for (int i = 0; i < I_subspawnName.Count; i++)
            {

                if (I_subspawnName[i] == creatureName)
                {

                    SpawnTwo(i, subNameCount, I_subspawn, subCount, I_subspawnName);

                    break;

                }

            }

        }



        protected virtual IEntity CreateSpawnedObject(int index, List<string> I_CreatName)
        {

            if (index >= I_CreatName.Count)

                return null;



            Type type = ScriptCompiler.FindTypeByName(ParseType(I_CreatName[index]));



            if (type != null)
            {

                try
                {

                    return Build(CommandSystem.Split(I_CreatName[index]));

                }

                catch
                {

                }

            }



            return null;

        }



        public static IEntity Build(string[] args)
        {

            string name = args[0];



            Add.FixArgs(ref args);



            string[,] props = null;



            for (int i = 0; i < args.Length; ++i)
            {

                if (Insensitive.Equals(args[i], "set"))
                {

                    int remains = args.Length - i - 1;



                    if (remains >= 2)
                    {

                        props = new string[remains / 2, 2];



                        remains /= 2;



                        for (int j = 0; j < remains; ++j)
                        {

                            props[j, 0] = args[i + (j * 2) + 1];

                            props[j, 1] = args[i + (j * 2) + 2];

                        }



                        Add.FixSetString(ref args, i);

                    }



                    break;

                }

            }



            Type type = ScriptCompiler.FindTypeByName(name);



            if (!Add.IsEntity(type))
            {

                return null;

            }



            PropertyInfo[] realProps = null;



            if (props != null)
            {

                realProps = new PropertyInfo[props.GetLength(0)];



                PropertyInfo[] allProps = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);



                for (int i = 0; i < realProps.Length; ++i)
                {

                    PropertyInfo thisProp = null;



                    string propName = props[i, 0];



                    for (int j = 0; thisProp == null && j < allProps.Length; ++j)
                    {

                        if (Insensitive.Equals(propName, allProps[j].Name))

                            thisProp = allProps[j];

                    }



                    if (thisProp != null)
                    {

                        CPA attr = Properties.GetCPA(thisProp);



                        if (attr != null && AccessLevel.GameMaster >= attr.WriteLevel && thisProp.CanWrite && !attr.ReadOnly)

                            realProps[i] = thisProp;

                    }

                }

            }



            ConstructorInfo[] ctors = type.GetConstructors();



            for (int i = 0; i < ctors.Length; ++i)
            {

                ConstructorInfo ctor = ctors[i];



                if (!Add.IsConstructable(ctor, AccessLevel.GameMaster))

                    continue;



                ParameterInfo[] paramList = ctor.GetParameters();



                if (args.Length == paramList.Length)
                {

                    object[] paramValues = Add.ParseValues(paramList, args);



                    if (paramValues == null)

                        continue;



                    object built = ctor.Invoke(paramValues);



                    if (built != null && realProps != null)
                    {

                        for (int j = 0; j < realProps.Length; ++j)
                        {

                            if (realProps[j] == null)

                                continue;



                            string result = Properties.InternalSetValue(built, realProps[j], props[j, 1]);

                        }

                    }



                    return (IEntity)built;

                }

            }



            return null;

        }



        public void SpawnTwo(int index, int CreatNameCount, List<IEntity> I_Creat, int I_Countt, List<string> I_CreatName)
        {

            Map map = Map;



            if (map == null || map == Map.Internal || CreatNameCount == 0 || index >= CreatNameCount || Parent != null)

                return;



            Defrag(I_Creatures);

            Defrag(I_CreaturesA);

            Defrag(I_CreaturesB);

            Defrag(I_CreaturesC);

            Defrag(I_CreaturesD);

            Defrag(I_CreaturesE);



            if (I_Creat.Count >= I_Countt)

                return;





            IEntity ent = CreateSpawnedObject(index, I_CreatName);



            if (ent is Mobile)
            {

                Mobile m = (Mobile)ent;



                if (m.CanSwim)
                {

                    I_Water = true;

                }

                else
                {

                    I_Water = false;

                }



                I_Creat.Add(m);





                Point3D loc = (m is BaseVendor ? this.Location : GetSpawnPosition());



                if (m is WanderingHealer || m is EvilWanderingHealer || m is EvilHealer)
                {

                    loc = GetSpawnPosition();

                }



                m.OnBeforeSpawn(loc, map);

                InvalidateProperties();





                m.MoveToWorld(loc, map);



                if (m is BaseCreature)
                {

                    BaseCreature c = (BaseCreature)m;



                    if (I_WalkingRange >= 0)

                        c.RangeHome = I_WalkingRange;

                    else

                        c.RangeHome = I_HomeRange;



                    c.CurrentWayPoint = I_WayPoint;



                    if (I_Team > 0)

                        c.Team = I_Team;



                    c.Home = this.Location;

                }



                m.OnAfterSpawn();

            }

            else if (ent is Item)
            {

                Item item = (Item)ent;



                I_Creat.Add(item);



                Point3D loc = GetSpawnPosition();



                item.OnBeforeSpawn(loc, map);

                InvalidateProperties();



                item.MoveToWorld(loc, map);



                item.OnAfterSpawn();

            }

        }



        public Point3D GetSpawnPosition()
        {

            Map map = Map;



            if (map == null)

                return Location;



            // Try 10 times to find a Spawnable location.

            for (int i = 0; i < 10; i++)
            {

                int x, y;



                if (I_HomeRange > 0)
                {

                    x = Location.X + (Utility.Random((I_HomeRange * 2) + 1) - I_HomeRange);

                    y = Location.Y + (Utility.Random((I_HomeRange * 2) + 1) - I_HomeRange);

                }
                else
                {

                    x = Location.X;

                    y = Location.Y;

                }



                int z = Map.GetAverageZ(x, y);



                if (I_Water)
                {

                    TileMatrix tiles = Map.Tiles;

                    LandTile _tile = tiles.GetLandTile(x, y);

                    int id = _tile.ID;

                    if ((id >= 168 && id <= 171) || id == 100)
                    {

                        return new Point3D(x, y, this.Z);

                    }

                    else
                    {

                        continue;

                    }

                }



                if (Map.CanSpawnMobile(new Point2D(x, y), this.Z))

                    return new Point3D(x, y, this.Z);

                else if (Map.CanSpawnMobile(new Point2D(x, y), z))

                    return new Point3D(x, y, z);

            }



            return this.Location;

        }



        public void DoTimer()
        {

            if (!I_Running)

                return;



            int minSeconds = (int)I_MinDelay.TotalSeconds;

            int maxSeconds = (int)I_MaxDelay.TotalSeconds;



            TimeSpan delay = TimeSpan.FromSeconds(Utility.RandomMinMax(minSeconds, maxSeconds));

            DoTimer(delay);

        }



        public void DoTimer(TimeSpan delay)
        {

            if (!I_Running)

                return;



            I_End = DateTime.UtcNow + delay;



            if (I_Timer != null)

                I_Timer.Stop();



            I_Timer = new InternalTimer(this, delay);

            I_Timer.Start();

        }



        private class InternalTimer : Timer
        {

            private PrimeSpawner I_PrimeSpawner;



            public InternalTimer(PrimeSpawner spawner, TimeSpan delay)
                : base(delay)
            {

                if (spawner.IsFull || spawner.IsFulla || spawner.IsFullb || spawner.IsFullc || spawner.IsFulld || spawner.IsFulle)

                    Priority = TimerPriority.FiveSeconds;

                else

                    Priority = TimerPriority.OneSecond;



                I_PrimeSpawner = spawner;

            }



            protected override void OnTick()
            {

                if (I_PrimeSpawner != null)

                    if (!I_PrimeSpawner.Deleted)

                        I_PrimeSpawner.OnTick();

            }

        }



        // Used only by PrimeSpawnerGump (except 1st, used here too)

        public int CountCreatures(List<IEntity> I_subspawn, string creatureName)
        {

            Defrag(I_subspawn);



            int count = 0;



            for (int i = 0; i < I_subspawn.Count; ++i)

                if (Insensitive.Equals(creatureName, I_subspawn[i].GetType().Name))

                    ++count;



            return count;

        }



        // Used only by PrimeSpawnerGump (lines 446-76)

        // BROKEN

        public void RemoveCreaturesFromGump(List<IEntity> I_subspawn, string creatureName)
        {

            Defrag(I_subspawn);



            for (int i = 0; i < I_subspawn.Count; ++i)
            {

                IEntity e = I_subspawn[i];



                if (Insensitive.Equals(creatureName, e.GetType().Name))

                    e.Delete();

            }



            InvalidateProperties();

        }



        // Used only here

        public void RemoveCreatures(List<IEntity> I_Creatur)
        {

            Defrag(I_Creatur);



            for (int i = 0; i < I_Creatur.Count; ++i)

                I_Creatur[i].Delete();



            InvalidateProperties();

        }



        //Used by PrimeSpawnerGump

        public void BringToHome(List<IEntity> I_Beings)
        {

            for (int i = 0; i < I_Beings.Count; ++i)
            {

                IEntity e = I_Beings[i];



                if (e is Mobile)
                {

                    Mobile m = (Mobile)e;



                    m.MoveToWorld(Location, Map);

                }

                else if (e is Item)
                {

                    Item item = (Item)e;



                    item.MoveToWorld(Location, Map);

                }

            }

        }



        //Used by PrimeSpawnerGump

        public void BringToHome()
        {

            Defrag(I_Creatures);

            Defrag(I_CreaturesA);

            Defrag(I_CreaturesB);

            Defrag(I_CreaturesC);

            Defrag(I_CreaturesD);

            Defrag(I_CreaturesE);



            BringToHome(I_Creatures);

            BringToHome(I_CreaturesA);

            BringToHome(I_CreaturesB);

            BringToHome(I_CreaturesC);

            BringToHome(I_CreaturesD);

            BringToHome(I_CreaturesE);

        }



        public override void OnDelete()
        {

            base.OnDelete();



            RemoveCreatures(I_Creatures);

            RemoveCreatures(I_CreaturesA);

            RemoveCreatures(I_CreaturesB);

            RemoveCreatures(I_CreaturesC);

            RemoveCreatures(I_CreaturesD);

            RemoveCreatures(I_CreaturesE);

            if (I_Timer != null)

                I_Timer.Stop();

        }



        public override void Serialize(GenericWriter writer)
        {

            base.Serialize(writer);



            writer.Write((int)4); // version

            writer.Write(I_WalkingRange);



            writer.Write(I_SpawnID);

            writer.Write(spawnMaxA);

            writer.Write(spawnMaxB);

            writer.Write(spawnMaxC);

            writer.Write(spawnMaxD);

            writer.Write(spawnMaxE);



            writer.Write(I_WayPoint);



            writer.Write(I_Group);



            writer.Write(I_MinDelay);

            writer.Write(I_MaxDelay);

            writer.Write(spawnMax);

            writer.Write(I_Team);

            writer.Write(I_HomeRange);

            writer.Write(I_Running);



            if (I_Running)

                writer.WriteDeltaTime(I_End);



            writer.Write(I_CreaturesName.Count);



            for (int i = 0; i < I_CreaturesName.Count; ++i)

                writer.Write(I_CreaturesName[i]);



            writer.Write(I_CreaturesNameA.Count);



            for (int i = 0; i < I_CreaturesNameA.Count; ++i)

                writer.Write((string)I_CreaturesNameA[i]);



            writer.Write(I_CreaturesNameB.Count);



            for (int i = 0; i < I_CreaturesNameB.Count; ++i)

                writer.Write((string)I_CreaturesNameB[i]);



            writer.Write(I_CreaturesNameC.Count);



            for (int i = 0; i < I_CreaturesNameC.Count; ++i)

                writer.Write((string)I_CreaturesNameC[i]);



            writer.Write(I_CreaturesNameD.Count);



            for (int i = 0; i < I_CreaturesNameD.Count; ++i)

                writer.Write((string)I_CreaturesNameD[i]);



            writer.Write(I_CreaturesNameE.Count);



            for (int i = 0; i < I_CreaturesNameE.Count; ++i)

                writer.Write((string)I_CreaturesNameE[i]);



            writer.Write(I_Creatures.Count);



            for (int i = 0; i < I_Creatures.Count; ++i)
            {

                IEntity e = I_Creatures[i];



                if (e is Item)

                    writer.Write((Item)e);

                else if (e is Mobile)

                    writer.Write((Mobile)e);

                else

                    writer.Write(Serial.MinusOne);

            }



            writer.Write(I_CreaturesA.Count);



            for (int i = 0; i < I_CreaturesA.Count; ++i)
            {

                IEntity e = I_CreaturesA[i];



                if (e is Item)

                    writer.Write((Item)e);

                else if (e is Mobile)

                    writer.Write((Mobile)e);

                else

                    writer.Write(Serial.MinusOne);

            }



            writer.Write(I_CreaturesB.Count);



            for (int i = 0; i < I_CreaturesB.Count; ++i)
            {

                IEntity e = I_CreaturesB[i];



                if (e is Item)

                    writer.Write((Item)e);

                else if (e is Mobile)

                    writer.Write((Mobile)e);

                else

                    writer.Write(Serial.MinusOne);

            }



            writer.Write(I_CreaturesC.Count);



            for (int i = 0; i < I_CreaturesC.Count; ++i)
            {

                IEntity e = I_CreaturesC[i];



                if (e is Item)

                    writer.Write((Item)e);

                else if (e is Mobile)

                    writer.Write((Mobile)e);

                else

                    writer.Write(Serial.MinusOne);

            }



            writer.Write(I_CreaturesD.Count);



            for (int i = 0; i < I_CreaturesD.Count; ++i)
            {

                IEntity e = I_CreaturesD[i];



                if (e is Item)

                    writer.Write((Item)e);

                else if (e is Mobile)

                    writer.Write((Mobile)e);

                else

                    writer.Write(Serial.MinusOne);

            }



            writer.Write(I_CreaturesE.Count);



            for (int i = 0; i < I_CreaturesE.Count; ++i)
            {

                IEntity e = I_CreaturesE[i];



                if (e is Item)

                    writer.Write((Item)e);

                else if (e is Mobile)

                    writer.Write((Mobile)e);

                else

                    writer.Write(Serial.MinusOne);

            }



        }



        private static WarnTimer I_WarnTimer;



        public override void Deserialize(GenericReader reader)
        {

            base.Deserialize(reader);



            int version = reader.ReadInt();



            switch (version)
            {

                case 4:
                    {

                        I_WalkingRange = reader.ReadInt();

                        I_SpawnID = reader.ReadInt();

                        spawnMaxA = reader.ReadInt();

                        spawnMaxB = reader.ReadInt();

                        spawnMaxC = reader.ReadInt();

                        spawnMaxD = reader.ReadInt();

                        spawnMaxE = reader.ReadInt();



                        goto case 3;

                    }

                case 3:

                case 2:
                    {

                        I_WayPoint = reader.ReadItem() as WayPoint;



                        goto case 1;

                    }



                case 1:
                    {

                        I_Group = reader.ReadBool();



                        goto case 0;

                    }



                case 0:
                    {

                        I_MinDelay = reader.ReadTimeSpan();

                        I_MaxDelay = reader.ReadTimeSpan();

                        spawnMax = reader.ReadInt();

                        I_Team = reader.ReadInt();

                        I_HomeRange = reader.ReadInt();

                        I_Running = reader.ReadBool();



                        TimeSpan ts = TimeSpan.Zero;



                        if (I_Running)

                            ts = reader.ReadDeltaTime() - DateTime.UtcNow;



                        int size = reader.ReadInt();

                        I_CreaturesName = new List<string>(size);

                        for (int i = 0; i < size; ++i)
                        {

                            string creatureString = reader.ReadString();



                            I_CreaturesName.Add(creatureString);

                            string typeName = ParseType(creatureString);



                            if (ScriptCompiler.FindTypeByName(typeName) == null)
                            {

                                if (I_WarnTimer == null)

                                    I_WarnTimer = new WarnTimer();



                                I_WarnTimer.Add(Location, Map, typeName);

                            }

                        }



                        int sizeA = reader.ReadInt();

                        I_CreaturesNameA = new List<string>(sizeA);

                        for (int i = 0; i < sizeA; ++i)
                        {

                            string creatureString = reader.ReadString();



                            I_CreaturesNameA.Add(creatureString);

                            string typeName = ParseType(creatureString);



                            if (ScriptCompiler.FindTypeByName(typeName) == null)
                            {

                                if (I_WarnTimer == null)

                                    I_WarnTimer = new WarnTimer();



                                I_WarnTimer.Add(Location, Map, typeName);

                            }

                        }



                        int sizeB = reader.ReadInt();

                        I_CreaturesNameB = new List<string>(sizeB);

                        for (int i = 0; i < sizeB; ++i)
                        {

                            string creatureString = reader.ReadString();



                            I_CreaturesNameB.Add(creatureString);

                            string typeName = ParseType(creatureString);



                            if (ScriptCompiler.FindTypeByName(typeName) == null)
                            {

                                if (I_WarnTimer == null)

                                    I_WarnTimer = new WarnTimer();



                                I_WarnTimer.Add(Location, Map, typeName);

                            }

                        }



                        int sizeC = reader.ReadInt();

                        I_CreaturesNameC = new List<string>(sizeC);

                        for (int i = 0; i < sizeC; ++i)
                        {

                            string creatureString = reader.ReadString();



                            I_CreaturesNameC.Add(creatureString);

                            string typeName = ParseType(creatureString);



                            if (ScriptCompiler.FindTypeByName(typeName) == null)
                            {

                                if (I_WarnTimer == null)

                                    I_WarnTimer = new WarnTimer();



                                I_WarnTimer.Add(Location, Map, typeName);

                            }

                        }



                        int sizeD = reader.ReadInt();

                        I_CreaturesNameD = new List<string>(sizeD);

                        for (int i = 0; i < sizeD; ++i)
                        {

                            string creatureString = reader.ReadString();



                            I_CreaturesNameD.Add(creatureString);

                            string typeName = ParseType(creatureString);



                            if (ScriptCompiler.FindTypeByName(typeName) == null)
                            {

                                if (I_WarnTimer == null)

                                    I_WarnTimer = new WarnTimer();



                                I_WarnTimer.Add(Location, Map, typeName);

                            }

                        }



                        int sizeE = reader.ReadInt();

                        I_CreaturesNameE = new List<string>(sizeE);

                        for (int i = 0; i < sizeE; ++i)
                        {

                            string creatureString = reader.ReadString();



                            I_CreaturesNameE.Add(creatureString);

                            string typeName = ParseType(creatureString);



                            if (ScriptCompiler.FindTypeByName(typeName) == null)
                            {

                                if (I_WarnTimer == null)

                                    I_WarnTimer = new WarnTimer();



                                I_WarnTimer.Add(Location, Map, typeName);

                            }

                        }



                        int count = reader.ReadInt();

                        I_Creatures = new List<IEntity>(count);

                        for (int i = 0; i < count; ++i)
                        {

                            IEntity e = World.FindEntity(reader.ReadInt());



                            if (e != null)

                                I_Creatures.Add(e);

                        }



                        int countA = reader.ReadInt();

                        I_CreaturesA = new List<IEntity>(countA);

                        for (int i = 0; i < countA; ++i)
                        {

                            IEntity e = World.FindEntity(reader.ReadInt());



                            if (e != null)

                                I_CreaturesA.Add(e);

                        }



                        int countB = reader.ReadInt();

                        I_CreaturesB = new List<IEntity>(countB);

                        for (int i = 0; i < countB; ++i)
                        {

                            IEntity e = World.FindEntity(reader.ReadInt());



                            if (e != null)

                                I_CreaturesB.Add(e);

                        }



                        int countC = reader.ReadInt();

                        I_CreaturesC = new List<IEntity>(countC);

                        for (int i = 0; i < countC; ++i)
                        {

                            IEntity e = World.FindEntity(reader.ReadInt());



                            if (e != null)

                                I_CreaturesC.Add(e);

                        }



                        int countD = reader.ReadInt();

                        I_CreaturesD = new List<IEntity>(countD);

                        for (int i = 0; i < countD; ++i)
                        {

                            IEntity e = World.FindEntity(reader.ReadInt());



                            if (e != null)

                                I_CreaturesD.Add(e);

                        }



                        int countE = reader.ReadInt();

                        I_CreaturesE = new List<IEntity>(countE);

                        for (int i = 0; i < countE; ++i)
                        {

                            IEntity e = World.FindEntity(reader.ReadInt());



                            if (e != null)

                                I_CreaturesE.Add(e);

                        }



                        if (I_Running)

                            DoTimer(ts);



                        break;

                    }

            }



            if (version < 3 && Weight == 0)

                Weight = -1;

        }

        private class WarnTimer : Timer
        {
            private List<WarnEntry> I_List;
            private class WarnEntry
            {
                public Point3D I_Point;
                public Map I_Map;
                public string I_Name;

                public WarnEntry(Point3D p, Map map, string name)
                {
                    I_Point = p;
                    I_Map = map;
                    I_Name = name;
                }
            }

            public WarnTimer()
                : base(TimeSpan.FromSeconds(1.0))
            {
                I_List = new List<WarnEntry>();
                Start();
            }
            public void Add(Point3D p, Map map, string name)
            {
                I_List.Add(new WarnEntry(p, map, name));
            }

            protected override void OnTick()
            {
                try
                {
                    Console.WriteLine("Warning: {0} bad spawns detected, logged: 'PremiumBadspawn.log'", I_List.Count);
                    using (StreamWriter op = new StreamWriter("Output/Logs/PremiumBadspawn.log", true))
                    {
                        op.WriteLine("# Bad spawns : {0}", DateTime.UtcNow);
                        op.WriteLine("# Format: X Y Z F Name");
                        op.WriteLine();
                        foreach (WarnEntry e in I_List)
                            op.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", e.I_Point.X, e.I_Point.Y, e.I_Point.Z, e.I_Map, e.I_Name);
                        op.WriteLine();
                        op.WriteLine();
                    }
                }
                catch
                {
                }
            }

        }
    }
}