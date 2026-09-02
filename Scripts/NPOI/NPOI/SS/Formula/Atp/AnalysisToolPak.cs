using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NPOI.SS.Formula.Function;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.UDF;

namespace NPOI.SS.Formula.Atp;

public class AnalysisToolPak : UDFFinder
{
	public static UDFFinder instance = new AnalysisToolPak();

	private static Dictionary<string, FreeRefFunction> _functionsByName = CreateFunctionsMap();

	private AnalysisToolPak()
	{
	}

	public override FreeRefFunction FindFunction(string name)
	{
		if (name.StartsWith("_xlfn."))
		{
			name = name.Substring(6);
		}
		string key = name.ToUpper();
		if (_functionsByName.ContainsKey(key))
		{
			return _functionsByName[key];
		}
		return null;
	}

	private static Dictionary<string, FreeRefFunction> CreateFunctionsMap()
	{
		Dictionary<string, FreeRefFunction> dictionary = new Dictionary<string, FreeRefFunction>(120);
		r(dictionary, "ACCRINT", null);
		r(dictionary, "ACCRINTM", null);
		r(dictionary, "AMORDEGRC", null);
		r(dictionary, "AMORLINC", null);
		r(dictionary, "AVERAGEIF", AverageIf.instance);
		r(dictionary, "AVERAGEIFS", AverageIfs.instance);
		r(dictionary, "BAHTTEXT", null);
		r(dictionary, "BESSELI", null);
		r(dictionary, "BESSELJ", null);
		r(dictionary, "BESSELK", null);
		r(dictionary, "BESSELY", null);
		r(dictionary, "BIN2DEC", Bin2Dec.instance);
		r(dictionary, "BIN2HEX", null);
		r(dictionary, "BIN2OCT", null);
		r(dictionary, "COMPLEX", Complex.Instance);
		r(dictionary, "CONVERT", null);
		r(dictionary, "COUNTIFS", Countifs.instance);
		r(dictionary, "COUPDAYBS", null);
		r(dictionary, "COUPDAYS", null);
		r(dictionary, "COUPDAYSNC", null);
		r(dictionary, "COUPNCD", null);
		r(dictionary, "COUPNUM", null);
		r(dictionary, "COUPPCD", null);
		r(dictionary, "CUBEKPIMEMBER", null);
		r(dictionary, "CUBEMEMBER", null);
		r(dictionary, "CUBEMEMBERPROPERTY", null);
		r(dictionary, "CUBERANKEDMEMBER", null);
		r(dictionary, "CUBESET", null);
		r(dictionary, "CUBESETCOUNT", null);
		r(dictionary, "CUBEVALUE", null);
		r(dictionary, "CUMIPMT", null);
		r(dictionary, "CUMPRINC", null);
		r(dictionary, "DEC2BIN", Dec2Bin.instance);
		r(dictionary, "DEC2HEX", Dec2Hex.instance);
		r(dictionary, "DEC2OCT", null);
		r(dictionary, "DELTA", Delta.instance);
		r(dictionary, "DISC", null);
		r(dictionary, "DOLLARDE", null);
		r(dictionary, "DOLLARFR", null);
		r(dictionary, "DURATION", null);
		r(dictionary, "EDATE", EDate.Instance);
		r(dictionary, "EFFECT", null);
		r(dictionary, "EOMONTH", EOMonth.instance);
		r(dictionary, "ERF", null);
		r(dictionary, "ERFC", null);
		r(dictionary, "FACTDOUBLE", FactDouble.instance);
		r(dictionary, "FVSCHEDULE", null);
		r(dictionary, "GCD", null);
		r(dictionary, "GESTEP", null);
		r(dictionary, "HEX2BIN", null);
		r(dictionary, "HEX2DEC", Hex2Dec.instance);
		r(dictionary, "HEX2OCT", null);
		r(dictionary, "IFERROR", IfError.Instance);
		r(dictionary, "IFNA", IfNa.instance);
		r(dictionary, "IFS", Ifs.Instance);
		r(dictionary, "IMABS", null);
		r(dictionary, "IMAGINARY", Imaginary.instance);
		r(dictionary, "IMARGUMENT", null);
		r(dictionary, "IMCONJUGATE", null);
		r(dictionary, "IMCOS", null);
		r(dictionary, "IMDIV", null);
		r(dictionary, "IMEXP", null);
		r(dictionary, "IMLN", null);
		r(dictionary, "IMLOG10", null);
		r(dictionary, "IMLOG2", null);
		r(dictionary, "IMPOWER", null);
		r(dictionary, "IMPRODUCT", null);
		r(dictionary, "IMREAL", ImReal.instance);
		r(dictionary, "IMSIN", null);
		r(dictionary, "IMSQRT", null);
		r(dictionary, "IMSUB", null);
		r(dictionary, "IMSUM", null);
		r(dictionary, "INTRATE", null);
		r(dictionary, "ISEVEN", ParityFunction.IS_EVEN);
		r(dictionary, "ISODD", ParityFunction.IS_ODD);
		r(dictionary, "JIS", null);
		r(dictionary, "LCM", null);
		r(dictionary, "MAXIFS", Maxifs.instance);
		r(dictionary, "MDURATION", null);
		r(dictionary, "MINIFS", Minifs.instance);
		r(dictionary, "MROUND", MRound.Instance);
		r(dictionary, "MULTINOMIAL", null);
		r(dictionary, "NETWORKDAYS", NetworkdaysFunction.instance);
		r(dictionary, "NOMINAL", null);
		r(dictionary, "OCT2BIN", null);
		r(dictionary, "OCT2DEC", Oct2Dec.instance);
		r(dictionary, "OCT2HEX", null);
		r(dictionary, "ODDFPRICE", null);
		r(dictionary, "ODDFYIELD", null);
		r(dictionary, "ODDLPRICE", null);
		r(dictionary, "ODDLYIELD", null);
		r(dictionary, "PRICE", null);
		r(dictionary, "PRICEDISC", null);
		r(dictionary, "PRICEMAT", null);
		r(dictionary, "QUOTIENT", Quotient.instance);
		r(dictionary, "RANDBETWEEN", RandBetween.Instance);
		r(dictionary, "RECEIVED", null);
		r(dictionary, "RTD", null);
		r(dictionary, "SERIESSUM", null);
		r(dictionary, "SQRTPI", null);
		r(dictionary, "SUMIFS", Sumifs.instance);
		r(dictionary, "SWITCH", Switch.instance);
		r(dictionary, "TBILLEQ", null);
		r(dictionary, "TBILLPRICE", null);
		r(dictionary, "TBILLYIELD", null);
		r(dictionary, "TEXTJOIN", TextJoinFunction.instance);
		r(dictionary, "WEEKNUM", WeekNum.instance);
		r(dictionary, "WORKDAY", WorkdayFunction.instance);
		r(dictionary, "XIRR", null);
		r(dictionary, "XNPV", null);
		r(dictionary, "YEARFRAC", YearFrac.instance);
		r(dictionary, "YIELD", null);
		r(dictionary, "YIELDDISC", null);
		r(dictionary, "YIELDMAT", null);
		return dictionary;
	}

	private static void r(Dictionary<string, FreeRefFunction> m, string functionName, FreeRefFunction pFunc)
	{
		FreeRefFunction freeRefFunction;
		if (pFunc != null)
		{
			freeRefFunction = pFunc;
		}
		else
		{
			FreeRefFunction freeRefFunction2 = new NotImplemented(functionName);
			freeRefFunction = freeRefFunction2;
		}
		FreeRefFunction value = freeRefFunction;
		m[functionName] = value;
	}

	public static bool IsATPFunction(string name)
	{
		return _functionsByName.ContainsKey(name);
	}

	public static ReadOnlyCollection<string> GetSupportedFunctionNames()
	{
		_ = (AnalysisToolPak)instance;
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, FreeRefFunction> item in _functionsByName)
		{
			FreeRefFunction value = item.Value;
			if (value != null && !(value is NotImplemented))
			{
				list.Add(item.Key);
			}
		}
		return list.AsReadOnly();
	}

	public static ReadOnlyCollection<string> GetNotSupportedFunctionNames()
	{
		_ = (AnalysisToolPak)instance;
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, FreeRefFunction> item in _functionsByName)
		{
			FreeRefFunction value = item.Value;
			if (value != null && value is NotImplemented)
			{
				list.Add(item.Key);
			}
		}
		return list.AsReadOnly();
	}

	public static void RegisterFunction(string name, FreeRefFunction func)
	{
		AnalysisToolPak obj = (AnalysisToolPak)instance;
		if (!IsATPFunction(name))
		{
			if (FunctionMetadataRegistry.GetFunctionByName(name) != null)
			{
				throw new ArgumentException(name + " is a built-in Excel function. Use FunctoinEval.RegisterFunction(String name, Function func) instead.");
			}
			throw new ArgumentException(name + " is not a function from the Excel Analysis Toolpack.");
		}
		FreeRefFunction freeRefFunction = obj.FindFunction(name);
		if (freeRefFunction != null && !(freeRefFunction is NotImplemented))
		{
			throw new ArgumentException("POI already implememts " + name + ". You cannot override POI's implementations of Excel functions");
		}
		if (_functionsByName.ContainsKey(name))
		{
			_functionsByName[name] = func;
		}
		else
		{
			_functionsByName.Add(name, func);
		}
	}
}
