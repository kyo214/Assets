using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dissonance.Networking;

internal interface IClientCollection<TPeer>
{
	[ContractAnnotation("=> true, info:notnull; => false, info:null")]
	bool TryGetClientInfoById(ushort clientId, out ClientInfo<TPeer> info);

	[ContractAnnotation("=> true, info:notnull; => false, info:null")]
	bool TryGetClientInfoByName([NotNull] string clientName, out ClientInfo<TPeer> info);

	bool TryGetClientsInRoom([NotNull] string room, List<ClientInfo<TPeer>> output);

	bool TryGetClientsInRoom(ushort roomId, List<ClientInfo<TPeer>> output);
}
