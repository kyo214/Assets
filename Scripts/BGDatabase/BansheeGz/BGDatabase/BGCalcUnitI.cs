using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public interface BGCalcUnitI : BGCalcVarsLiteOwnerI, BGCalcVarsOwnerBaseI
{
	BGCalcGraph Graph { get; set; }

	string Title { get; }

	ushort TypeCode { get; }

	List<BGCalcControlInputI> InControls { get; }

	List<BGCalcControlOutputI> OutControls { get; }

	List<BGCalcValueInputI> InValues { get; }

	List<BGCalcValueOutputI> OutValues { get; }

	List<BGCalcPortI> Ports { get; }

	int PortsCount { get; }

	Vector2 Position { get; set; }

	string GetPublicVarLabel(byte varId);

	void Definition();

	List<BGCalcPortI> FindPorts(Predicate<BGCalcPortI> filter);

	BGCalcPortI FindPort(Predicate<BGCalcPortI> filter);

	BGCalcPortI FindPort(string id);

	void RemovePort(BGCalcPortI port);

	bool IsEqual(BGCalcUnitI other);
}
