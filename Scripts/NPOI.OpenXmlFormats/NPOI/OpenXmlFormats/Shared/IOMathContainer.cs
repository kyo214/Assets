using System.Collections;

namespace NPOI.OpenXmlFormats.Shared;

public interface IOMathContainer
{
	ArrayList Items { get; }

	CT_R AddNewR();

	CT_Acc AddNewAcc();

	CT_Nary AddNewNary();

	CT_SSub AddNewSSub();

	CT_SSup AddNewSSup();

	CT_F AddNewF();

	CT_Rad AddNewRad();
}
