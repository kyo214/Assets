using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula;

public class CollaboratingWorkbooksEnvironment
{
	public static readonly CollaboratingWorkbooksEnvironment EMPTY = new CollaboratingWorkbooksEnvironment();

	private Dictionary<string, WorkbookEvaluator> _evaluatorsByName;

	private WorkbookEvaluator[] _evaluators;

	private bool _unhooked;

	private CollaboratingWorkbooksEnvironment()
	{
		_evaluatorsByName = new Dictionary<string, WorkbookEvaluator>();
		_evaluators = new WorkbookEvaluator[0];
	}

	public static void Setup(string[] workbookNames, WorkbookEvaluator[] evaluators)
	{
		int num = workbookNames.Length;
		if (evaluators.Length != num)
		{
			throw new ArgumentException("Number of workbook names is " + num + " but number of evaluators is " + evaluators.Length);
		}
		if (num < 1)
		{
			throw new ArgumentException("Must provide at least one collaborating worbook");
		}
		CollaboratingWorkbooksEnvironment env = new CollaboratingWorkbooksEnvironment(workbookNames, evaluators, num);
		HookNewEnvironment(evaluators, env);
	}

	public static void Setup(Dictionary<string, WorkbookEvaluator> evaluatorsByName)
	{
		if (evaluatorsByName.Count < 1)
		{
			throw new ArgumentException("Must provide at least one collaborating worbook");
		}
		List<WorkbookEvaluator> list = new List<WorkbookEvaluator>(evaluatorsByName.Count);
		list.AddRange(evaluatorsByName.Values);
		WorkbookEvaluator[] evaluators = list.ToArray();
		new CollaboratingWorkbooksEnvironment(evaluatorsByName, evaluators);
	}

	public static void SetupFormulaEvaluator(Dictionary<string, IFormulaEvaluator> evaluators)
	{
		Dictionary<string, WorkbookEvaluator> dictionary = new Dictionary<string, WorkbookEvaluator>(evaluators.Count);
		foreach (KeyValuePair<string, IFormulaEvaluator> evaluator in evaluators)
		{
			string key = evaluator.Key;
			IFormulaEvaluator value = evaluator.Value;
			if (value is IWorkbookEvaluatorProvider)
			{
				dictionary.Add(key, ((IWorkbookEvaluatorProvider)value).GetWorkbookEvaluator());
				continue;
			}
			throw new ArgumentException("Formula Evaluator " + value?.ToString() + " provides no WorkbookEvaluator access");
		}
		Setup(dictionary);
	}

	private CollaboratingWorkbooksEnvironment(string[] workbookNames, WorkbookEvaluator[] evaluators, int nItems)
		: this(ToUniqueMap(workbookNames, evaluators, nItems), evaluators)
	{
	}

	private static Dictionary<string, WorkbookEvaluator> ToUniqueMap(string[] workbookNames, WorkbookEvaluator[] evaluators, int nItems)
	{
		Dictionary<string, WorkbookEvaluator> dictionary = new Dictionary<string, WorkbookEvaluator>(nItems * 3 / 2);
		for (int i = 0; i < nItems; i++)
		{
			string text = workbookNames[i];
			WorkbookEvaluator value = evaluators[i];
			if (dictionary.ContainsKey(text))
			{
				throw new ArgumentException("Duplicate workbook name '" + text + "'");
			}
			dictionary.Add(text, value);
		}
		return dictionary;
	}

	private CollaboratingWorkbooksEnvironment(Dictionary<string, WorkbookEvaluator> evaluatorsByName, WorkbookEvaluator[] evaluators)
	{
		Dictionary<WorkbookEvaluator, string> dictionary = new Dictionary<WorkbookEvaluator, string>(evaluators.Length);
		foreach (KeyValuePair<string, WorkbookEvaluator> item in evaluatorsByName)
		{
			if (dictionary.ContainsKey(item.Value))
			{
				throw new ArgumentException("Attempted to register same workbook under names '" + dictionary[item.Value] + "' and '" + item.Key + "'");
			}
			dictionary.Add(item.Value, item.Key);
		}
		UnhookOldEnvironments(evaluators);
		HookNewEnvironment(evaluators, this);
		_unhooked = false;
		_evaluators = (WorkbookEvaluator[])evaluators.Clone();
		_evaluatorsByName = evaluatorsByName;
	}

	private static void HookNewEnvironment(WorkbookEvaluator[] evaluators, CollaboratingWorkbooksEnvironment env)
	{
		int num = evaluators.Length;
		IEvaluationListener evaluationListener = evaluators[0].GetEvaluationListener();
		for (int i = 0; i < num; i++)
		{
			if (evaluationListener != evaluators[i].GetEvaluationListener())
			{
				throw new Exception("Workbook evaluators must all have the same evaluation listener");
			}
		}
		EvaluationCache cache = new EvaluationCache(evaluationListener);
		for (int j = 0; j < num; j++)
		{
			evaluators[j].AttachToEnvironment(env, cache, j);
		}
	}

	private void UnhookOldEnvironments(WorkbookEvaluator[] evaluators)
	{
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < evaluators.Length; i++)
		{
			arrayList.Add(evaluators[i].GetEnvironment());
		}
		CollaboratingWorkbooksEnvironment[] array = new CollaboratingWorkbooksEnvironment[arrayList.Count];
		array = (CollaboratingWorkbooksEnvironment[])arrayList.ToArray(typeof(CollaboratingWorkbooksEnvironment));
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Unhook();
		}
	}

	private void Unhook()
	{
		if (_evaluators.Length >= 1)
		{
			for (int i = 0; i < _evaluators.Length; i++)
			{
				_evaluators[i].DetachFromEnvironment();
			}
			_unhooked = true;
		}
	}

	public WorkbookEvaluator GetWorkbookEvaluator(string workbookName)
	{
		if (_unhooked)
		{
			throw new InvalidOperationException("This environment Has been unhooked");
		}
		if (_evaluatorsByName.ContainsKey(workbookName))
		{
			return _evaluatorsByName[workbookName];
		}
		StringBuilder stringBuilder = new StringBuilder(256);
		stringBuilder.Append("Could not resolve external workbook name '").Append(workbookName).Append("'.");
		if (_evaluators.Length < 1)
		{
			stringBuilder.Append(" Workbook environment has not been set up.");
		}
		else
		{
			stringBuilder.Append(" The following workbook names are valid: (");
			IEnumerator enumerator = _evaluatorsByName.Keys.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				if (num++ > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("'").Append(enumerator.Current).Append("'");
			}
			stringBuilder.Append(")");
		}
		throw new WorkbookNotFoundException(stringBuilder.ToString());
	}
}
