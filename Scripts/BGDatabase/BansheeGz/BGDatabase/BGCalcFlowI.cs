namespace BansheeGz.BGDatabase;

public interface BGCalcFlowI : BGCalcVarsOwnerI, BGCalcVarsOwnerBaseI
{
	int Level { get; set; }

	BGCalcFlowI Parent { get; set; }

	BGCalcFlowContext Context { get; }

	object Result { get; set; }

	bool BreakIsRequested { get; set; }

	BGCalcControlOutputI Run(BGCalcControlInputI port);

	T GetValue<T>(BGCalcValueInputI input);

	object GetValue(BGCalcValueInputI input);

	object GetValue(BGCalcValueOutputI output);

	void SetValue(BGCalcPortI port, object value);

	void RunNested(BGCalcControlInputI connectedPort);

	object GetLocalVar(BGCalcPortI port);
}
