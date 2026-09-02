using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMDebugMenuTabManager : MonoBehaviour
{
	public List<MMDebugMenuTab> Tabs;

	public List<MMDebugMenuTabContents> TabsContents;

	public virtual void Select(int selected)
	{
		foreach (MMDebugMenuTab tab in Tabs)
		{
			if (tab.Index != selected)
			{
				tab.Deselect();
			}
		}
		foreach (MMDebugMenuTabContents tabsContent in TabsContents)
		{
			if (tabsContent.Index == selected)
			{
				tabsContent.gameObject.SetActive(value: true);
			}
			else
			{
				tabsContent.gameObject.SetActive(value: false);
			}
		}
	}
}
