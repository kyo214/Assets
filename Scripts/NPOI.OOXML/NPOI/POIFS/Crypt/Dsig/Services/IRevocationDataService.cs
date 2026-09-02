using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace NPOI.POIFS.Crypt.Dsig.Services;

public interface IRevocationDataService
{
	RevocationData GetRevocationData(List<X509Certificate> certificateChain);
}
