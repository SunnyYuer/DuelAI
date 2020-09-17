﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class DuelEventWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(DuelEvent), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("Initialize", Initialize);
		L.RegFunction("SetThisCard", SetThisCard);
		L.RegFunction("SetThisEffect", SetThisEffect);
		L.RegFunction("CreateEffect", CreateEffect);
		L.RegFunction("SetDuelEffect", SetDuelEffect);
		L.RegFunction("DrawCard", DrawCard);
		L.RegFunction("DisCard", DisCard);
		L.RegFunction("DisCardAll", DisCardAll);
		L.RegFunction("SelectCard", SelectCard);
		L.RegFunction("ShowCard", ShowCard);
		L.RegFunction("NormalSummon", NormalSummon);
		L.RegFunction("SpecialSummon", SpecialSummon);
		L.RegFunction("SetMagicTrap", SetMagicTrap);
		L.RegFunction("ChangeMean", ChangeMean);
		L.RegFunction("AfterThat", AfterThat);
		L.RegFunction("AttackNew", AttackNew);
		L.RegFunction("ActivateInvalid", ActivateInvalid);
		L.RegFunction("InCase", InCase);
		L.RegFunction("InTimePoint", InTimePoint);
		L.RegFunction("ThisCardIsBattle", ThisCardIsBattle);
		L.RegFunction("GetAntiMonster", GetAntiMonster);
		L.RegFunction("GetPlayerOrder", GetPlayerOrder);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("duelData", get_duelData, set_duelData);
		L.RegVar("thiscard", get_thiscard, set_thiscard);
		L.RegVar("thiseffect", get_thiseffect, set_thiseffect);
		L.RegVar("precheck", get_precheck, set_precheck);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Initialize(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			Duel arg0 = (Duel)ToLua.CheckObject<Duel>(L, 2);
			obj.Initialize(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetThisCard(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			DuelCard arg0 = (DuelCard)ToLua.CheckObject<DuelCard>(L, 2);
			obj.SetThisCard(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetThisEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.SetThisEffect(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			CardEffect o = obj.CreateEffect(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDuelEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			obj.SetDuelEffect();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DrawCard(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				obj.DrawCard(arg0);
				return 0;
			}
			else if (count == 3)
			{
				DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				obj.DrawCard(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: DuelEvent.DrawCard");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DisCard(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			System.Collections.Generic.List<DuelCard> arg0 = (System.Collections.Generic.List<DuelCard>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<DuelCard>));
			obj.DisCard(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DisCardAll(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			int o = obj.DisCardAll(arg0);
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SelectCard(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			TargetCard arg0 = (TargetCard)ToLua.CheckObject<TargetCard>(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
			int arg2 = (int)LuaDLL.luaL_checknumber(L, 4);
			obj.SelectCard(arg0, arg1, arg2);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowCard(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			DuelCard arg0 = (DuelCard)ToLua.CheckObject<DuelCard>(L, 2);
			obj.ShowCard(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NormalSummon(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			DuelCard arg0 = (DuelCard)ToLua.CheckObject<DuelCard>(L, 2);
			obj.NormalSummon(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SpecialSummon(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<DuelCard>(L, 2))
			{
				DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
				DuelCard arg0 = (DuelCard)ToLua.ToObject(L, 2);
				obj.SpecialSummon(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<System.Collections.Generic.List<DuelCard>>(L, 2))
			{
				DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
				System.Collections.Generic.List<DuelCard> arg0 = (System.Collections.Generic.List<DuelCard>)ToLua.ToObject(L, 2);
				obj.SpecialSummon(arg0);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: DuelEvent.SpecialSummon");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetMagicTrap(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			DuelCard arg0 = (DuelCard)ToLua.CheckObject<DuelCard>(L, 2);
			obj.SetMagicTrap(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeMean(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
				DuelCard arg0 = (DuelCard)ToLua.CheckObject<DuelCard>(L, 2);
				obj.ChangeMean(arg0);
				return 0;
			}
			else if (count == 3)
			{
				DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
				DuelCard arg0 = (DuelCard)ToLua.CheckObject<DuelCard>(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				obj.ChangeMean(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: DuelEvent.ChangeMean");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AfterThat(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			obj.AfterThat();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AttackNew(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.AttackNew(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ActivateInvalid(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			DuelCard o = obj.ActivateInvalid();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InCase(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			object arg0 = ToLua.ToVarObject(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
			bool o = obj.InCase(arg0, arg1);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InTimePoint(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			object arg0 = ToLua.ToVarObject(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
			bool o = obj.InTimePoint(arg0, arg1);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ThisCardIsBattle(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			bool o = obj.ThisCardIsBattle();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAntiMonster(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			DuelCard o = obj.GetAntiMonster();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPlayerOrder(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			DuelEvent obj = (DuelEvent)ToLua.CheckObject<DuelEvent>(L, 1);
			System.Collections.Generic.List<int> o = obj.GetPlayerOrder();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_duelData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			DuelDataManager ret = obj.duelData;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index duelData on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_thiscard(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			DuelCard ret = obj.thiscard;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index thiscard on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_thiseffect(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			CardEffect ret = obj.thiseffect;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index thiseffect on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_precheck(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			bool ret = obj.precheck;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index precheck on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_duelData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			DuelDataManager arg0 = (DuelDataManager)ToLua.CheckObject<DuelDataManager>(L, 2);
			obj.duelData = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index duelData on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_thiscard(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			DuelCard arg0 = (DuelCard)ToLua.CheckObject<DuelCard>(L, 2);
			obj.thiscard = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index thiscard on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_thiseffect(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			CardEffect arg0 = (CardEffect)ToLua.CheckObject<CardEffect>(L, 2);
			obj.thiseffect = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index thiseffect on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_precheck(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			DuelEvent obj = (DuelEvent)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.precheck = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index precheck on a nil value");
		}
	}
}

