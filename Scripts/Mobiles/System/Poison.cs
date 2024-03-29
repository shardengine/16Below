#region Header
// **********
// 16Below - Poison.cs
// **********
#endregion

#region References
using System;
using System.Globalization;

using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
#endregion

namespace Server
{
	public class PoisonImpl : Poison
	{
		[CallPriority(10)]
		public static void Configure()
		{
			if (Core.AOS)
			{
				Register(new PoisonImpl("Lesser", 0, 4, 16, 7.5, 3.0, 2.25, 10, 4));
				Register(new PoisonImpl("Regular", 1, 8, 18, 10.0, 3.0, 3.25, 10, 3));
				Register(new PoisonImpl("Greater", 2, 12, 20, 15.0, 3.0, 4.25, 10, 2));
				Register(new PoisonImpl("Deadly", 3, 16, 30, 30.0, 3.0, 5.25, 15, 2));
				Register(new PoisonImpl("Lethal", 4, 20, 50, 35.0, 3.0, 5.25, 20, 2));
			}
			else
			{
				Register(new PoisonImpl("Lesser", 0, 4, 26, 2.500, 3.5, 3.0, 10, 2));
				Register(new PoisonImpl("Regular", 1, 5, 26, 3.125, 3.5, 3.0, 10, 2));
				Register(new PoisonImpl("Greater", 2, 6, 26, 6.250, 3.5, 3.0, 10, 2));
				Register(new PoisonImpl("Deadly", 3, 7, 26, 12.500, 3.5, 4.0, 10, 2));
				Register(new PoisonImpl("Lethal", 4, 9, 26, 25.000, 3.5, 5.0, 10, 2));
			}
		}

		public static Poison IncreaseLevel(Poison oldPoison)
		{
			Poison newPoison = oldPoison == null ? null : GetPoison(oldPoison.Level + 1);

			return newPoison ?? oldPoison;
		}

		// Info
		private readonly string m_Name;
		private readonly int m_Level;

		// Damage
		private readonly int m_Minimum;
		private readonly int m_Maximum;
		private readonly double m_Scalar;

		// Timers
		private readonly TimeSpan m_Delay;
		private readonly TimeSpan m_Interval;
		private readonly int m_Count;

		private readonly int m_MessageInterval;

		public override string Name { get { return m_Name; } }
		public override int Level { get { return m_Level; } }

		public override int RealLevel
		{
			get
			{
				if (m_Level >= 14)
				{
					return m_Level - 14;
				}

				if (m_Level >= 10)
				{
					return m_Level - 10;
				}

				return m_Level;
			}
		}

		public override int LabelNumber
		{
			get
			{
				if (m_Level >= 14)
				{
					return 1072852; // parasitic poison charges: ~1_val~
				}

				if (m_Level >= 10)
				{
					return 1072853; // darkglow poison charges: ~1_val~
				}

				return 1062412 + m_Level; // ~poison~ poison charges: ~1_val~
			}
		}

		public PoisonImpl(
			string name,
			int level,
			int min,
			int max,
			double percent,
			double delay,
			double interval,
			int count,
			int messageInterval)
		{
			m_Name = name;
			m_Level = level;
			m_Minimum = min;
			m_Maximum = max;
			m_Scalar = percent * 0.01;
			m_Delay = TimeSpan.FromSeconds(delay);
			m_Interval = TimeSpan.FromSeconds(interval);
			m_Count = count;
			m_MessageInterval = messageInterval;
		}

		public override Timer ConstructTimer(Mobile m)
		{
			return new PoisonTimer(m, this);
		}

		public class PoisonTimer : Timer
		{
			private readonly PoisonImpl m_Poison;
			private readonly Mobile m_Mobile;
			private Mobile m_From;
			private int m_LastDamage;
			private int m_Index;

			public Mobile From { get { return m_From; } set { m_From = value; } }

			public PoisonTimer(Mobile m, PoisonImpl p)
				: base(p.m_Delay, p.m_Interval)
			{
				m_From = m;
				m_Mobile = m;
				m_Poison = p;
			}

			protected override void OnTick()
			{
				if (m_Index++ == m_Poison.m_Count)
				{
					m_Mobile.SendLocalizedMessage(502136); // The poison seems to have worn off.
					m_Mobile.Poison = null;

					Stop();
					return;
				}

				int damage;

				if (!Core.AOS && m_LastDamage != 0 && Utility.RandomBool())
				{
					damage = m_LastDamage;
				}
				else
				{
					damage = 1 + (int)(m_Mobile.Hits * m_Poison.m_Scalar);

					if (damage < m_Poison.m_Minimum)
					{
						damage = m_Poison.m_Minimum;
					}
					else if (damage > m_Poison.m_Maximum)
					{
						damage = m_Poison.m_Maximum;
					}

					m_LastDamage = damage;
				}

				if (m_From != null)
				{
					m_From.DoHarmful(m_Mobile, true);
				}

				IHonorTarget honorTarget = m_Mobile as IHonorTarget;
				if (honorTarget != null && honorTarget.ReceivedHonorContext != null)
				{
					honorTarget.ReceivedHonorContext.OnTargetPoisoned();
				}

				m_Mobile.Damage(damage, m_From);

				if (0.60 <= Utility.RandomDouble())
				// OSI: randomly revealed between first and third damage tick, guessing 60% chance
				{
					m_Mobile.RevealingAction();
				}

				if ((m_Index % m_Poison.m_MessageInterval) == 0)
				{
					m_Mobile.OnPoisoned(m_From, m_Poison, m_Poison);
				}
			}
		}

		public class ParasiticTimer : Timer
		{
			public Mobile m_Mobile;
			public Mobile m_From;
			private int m_Damage;
			private readonly int m_MaxCount;
			private int m_Count;
			private readonly PoisonImpl m_Poison;
			private int m_Index;

			public ParasiticTimer(Mobile m, Mobile from, PoisonImpl poison, int Index)
				: base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
			{
				Random rnd = new Random();

				this.m_Mobile = m;
				this.m_From = from;
				this.m_Count = 0;
				this.m_MaxCount = (int)(rnd.Next(70, 75) / 5);
				this.m_Poison = poison;
				this.m_Index = Index;
			}

			protected override void OnTick()
			{
				Random dmg = new Random();
				this.m_Damage = dmg.Next(25, 33);
				this.m_Count++;

				if (this.m_Count > this.m_MaxCount || this.m_Mobile == null)
				{
					m_Mobile.SendLocalizedMessage(502136); // The poison seems to have worn off.
					m_Mobile.Poison = null;
					this.Stop();
					return;
				}

				m_Mobile.LocalOverheadMessage(
							MessageType.Emote, 0x3F, true, "* You feel extremely weak and are in severe pain *");

				m_Mobile.NonlocalOverheadMessage(
					MessageType.Emote, 0x3F, true, String.Format("* {0} is wracked with extreme pain *", m_Mobile.Name));

				int toHeal = Math.Min(m_From.HitsMax - m_From.Hits, m_Damage);

				if (toHeal > 0)
				{
					m_From.SendLocalizedMessage(1060203, toHeal.ToString(CultureInfo.InvariantCulture));
					// You have had ~1_HEALED_AMOUNT~ hit points of damage healed.
					m_From.Heal(toHeal, m_Mobile, false);
				}

				if (m_Mobile != null)
					m_Mobile.Damage(m_Damage, m_From);

				if (0.60 <= Utility.RandomDouble())
				// OSI: randomly revealed between first and third damage tick, guessing 60% chance
				{
					m_Mobile.RevealingAction();
				}

				if ((m_Index % m_Poison.m_MessageInterval) == 0)
				{
					m_Mobile.OnPoisoned(m_From, m_Poison, m_Poison);
				}

			}
		}

		public class DarkglowTimer : Timer
		{
			public Mobile m_Mobile;
			public Mobile m_From;
			private int m_Damage;
			private readonly int m_MaxCount;
			private int m_Count;
			private readonly PoisonImpl m_Poison;
			private int m_Index;

			public DarkglowTimer(Mobile m, Mobile from, PoisonImpl poison, int Index)
				: base(TimeSpan.FromSeconds(4.0), TimeSpan.FromSeconds(4.0))
			{
				Random rnd = new Random();

				this.m_Mobile = m;
				this.m_From = from;
				this.m_Count = 0;
				this.m_MaxCount = (int)(rnd.Next(45, 60) / 4);
				this.m_Poison = poison;
				this.m_Index = Index;
			}

			protected override void OnTick()
			{
				Random dmg = new Random();
				this.m_Damage = dmg.Next(14, 21);
				this.m_Count++;

				if (this.m_Count > this.m_MaxCount || this.m_Mobile == null)
				{
					m_Mobile.SendLocalizedMessage(502136); // The poison seems to have worn off.
					m_Mobile.Poison = null;
					this.Stop();
					return;
				}

				m_Mobile.LocalOverheadMessage(
							MessageType.Emote, 0x3F, true, "* You begin to feel pain throughout your body *");

				m_Mobile.NonlocalOverheadMessage(
					MessageType.Emote, 0x3F, true, String.Format("* {0} stumbles around in confusion and pain *", m_Mobile.Name));

				if (m_Mobile != null)
					m_Mobile.Damage(m_Damage, m_From);

				if (0.60 <= Utility.RandomDouble())
				// OSI: randomly revealed between first and third damage tick, guessing 60% chance
				{
					m_Mobile.RevealingAction();
				}

				if ((m_Index % m_Poison.m_MessageInterval) == 0)
				{
					m_Mobile.OnPoisoned(m_From, m_Poison, m_Poison);
				}
			}
		}
	}
}