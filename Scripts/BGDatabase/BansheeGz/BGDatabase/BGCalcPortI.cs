using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public interface BGCalcPortI
{
	string Id { get; }

	string Name { get; }

	BGCalcPortTypeEnum PortType { get; }

	bool IsSingle { get; }

	Type Type { get; }

	bool IsInput { get; }

	BGCalcTypeCode TypeCode { get; }

	BGCalcUnitI Unit { get; }

	bool IsConnected { get; }

	List<BGCalcPortI> ConnectedPorts { get; }

	void Connect(BGCalcPortI port, bool connectBoth = true);

	void Disconnect(BGCalcPortI port, bool disconnectBoth = true);

	void DisconnectAll();

	bool CanConnectTo(BGCalcPortI toConnectPort);

	bool IsEqual(BGCalcPortI other);
}
